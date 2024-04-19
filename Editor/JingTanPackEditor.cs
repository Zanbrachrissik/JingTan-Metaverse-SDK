using UnityEngine;
using UnityEditor;
using HybridCLR.Editor;
using Mono.Cecil;
using UnityEditorInternal;
using HybridCLR.Editor.Commands;
using System.IO;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEditor.PackageManager;
using UnityEditor.Compilation;
using UnityEditor.PackageManager.Requests;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Linq;

using System.Threading.Tasks;
using AntChain.SDK.NFTC.Models;
using UnityEngine.Networking;
using System.IO.Compression;

namespace Ant.Metaverse.Editor
{
    [Serializable]
    public class DllConfig
    {
        public List<string> MetaAssemblyFiles = new List<string>();
        public List<string> HotUpdateAssemblyFiles = new List<string>();
        public string MainAssemblyFile;
        public string MetaSDKVersion;
    }

// 在打ig包的版本上会报ziparchive的错，需要在unity版本上做一下判断
#if UNITY_2021_3_OR_NEWER

    public class JingTanPackEditor : EditorWindow
    {
        //Dev
        private static string accessKeyId = "ACagbaD61VFsHJbw";
        private static string accessKeySecret = "413478n1sSq4w9A2BxibUyv6eqZ3T1fM";

        private int version = 1;
        private AssemblyDefinitionAsset assemblyDefinitionAsset;
        private string assemblyDefinitionAssetPath;
        private string iosAssetBundleFolder;
        private string androidAssetBundleFolder;

        private static string RootPath => Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets"));
        private static string JingTanAppOutputDir => RootPath + "JingTanAppOutput";
        private static string IOSFolder => JingTanAppOutputDir + "/iOS";
        private static string AndroidFolder => JingTanAppOutputDir + "/Android";
        private const string METASDKDLLNAME = "AntMetaverseRunTime";
        private string iosResouceId;
        private string androidResouceId;

        [MenuItem("鲸探APP打包/Dll打包工具")]
        public static void ShowWindow()
        {
            GetWindow<JingTanPackEditor>("Dll打包工具（产物运行于鲸探端内）");
        }

        private void OnEnable()
        {
            ReadFromJson();
            CheckVersion(true);
        }

        private void OnDisable() {
            SaveToJson();
        }

        private void SaveToJson()
        {
            JObject jObject = new JObject();
            jObject["iosAssetBundleFolder"] = iosAssetBundleFolder;
            jObject["androidAssetBundleFolder"] = androidAssetBundleFolder;
            jObject["iosResouceId"] = iosResouceId;
            jObject["androidResouceId"] = androidResouceId;
            jObject["assemblyDefinitionAssetPath"] = AssetDatabase.GetAssetPath(assemblyDefinitionAsset);
            File.WriteAllText(Application.persistentDataPath + "/JingTanPackConfig.json", jObject.ToString());
        }

        private void ReadFromJson()
        {
            string configPath = Application.persistentDataPath + "/JingTanPackConfig.json";
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                JObject jObject = JObject.Parse(json);
                iosAssetBundleFolder = jObject["iosAssetBundleFolder"].ToString();
                androidAssetBundleFolder = jObject["androidAssetBundleFolder"].ToString();
                iosResouceId = jObject["iosResouceId"].ToString();
                androidResouceId = jObject["androidResouceId"].ToString();
                assemblyDefinitionAssetPath = jObject["assemblyDefinitionAssetPath"].ToString();
                assemblyDefinitionAsset = AssetDatabase.LoadAssetAtPath<AssemblyDefinitionAsset>(assemblyDefinitionAssetPath);
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("资源ID（固定值，由鲸探告知）");
            iosResouceId = EditorGUILayout.TextField("IOS资源ID", iosResouceId);
            androidResouceId = EditorGUILayout.TextField("Android资源ID", androidResouceId);
            EditorGUILayout.Space();
            version = EditorGUILayout.IntField("版本号", version);
            assemblyDefinitionAsset = EditorGUILayout.ObjectField("业务主逻辑程序集", assemblyDefinitionAsset, typeof(AssemblyDefinitionAsset), false) as AssemblyDefinitionAsset;
            
            EditorGUILayout.BeginHorizontal();
            iosAssetBundleFolder = EditorGUILayout.TextField("IOS ab包文件夹", iosAssetBundleFolder);
            if (GUILayout.Button("选择"))
            {
                iosAssetBundleFolder = EditorUtility.OpenFolderPanel("选择AssetBundle文件夹", "", "");
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            androidAssetBundleFolder = EditorGUILayout.TextField("Android ab包文件夹", androidAssetBundleFolder);
            if (GUILayout.Button("选择"))
            {
                androidAssetBundleFolder = EditorUtility.OpenFolderPanel("选择AssetBundle文件夹", "", "");
            }
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("打包双端dll") && CheckAsmdef() && CheckVersion() && CheckAssetBundleFolder())
            {
                PackIosAndroidDllForJingTanApp();
                WriteConfig();
                WriteHotUpdateConfig();
                GenerateZips();

                if (EditorUtility.DisplayDialog("提示", "打包成功，是否继续上传资源到鲸探？", "是", "否"))
                {
                    UploadFileToRes();
                }
            }
        }

        public async void UploadFileToRes()
        {
            var jtInfo = JingTanNFTCUpload.GetAppMsg(accessKeyId, accessKeySecret);
            if (jtInfo == null)
            {
                return;
            }
            if (jtInfo.bizStatusMessage != "成功")
            {
                Debug.LogError("信息获取失败，上传失败 ！！！");
                return;
            }
            _ = await JingTanNFTCUpload.UploadToJT(IOSFolder + ".zip", jtInfo.url, jtInfo.maasToken, jtInfo.appId, iosResouceId, version.ToString());
            _ = await JingTanNFTCUpload.UploadToJT(AndroidFolder + ".zip", jtInfo.url, jtInfo.maasToken, jtInfo.appId, androidResouceId, version.ToString());
        }

        private void GenerateZips()
        {
            GenerateSingleZip(IOSFolder);
            GenerateSingleZip(AndroidFolder);
        }

        private void GenerateSingleZip(string folderPath)
        {
            string sourceDir = folderPath;
            string zipFilePath = folderPath + ".zip";
            string[] filesToCompress = Directory.GetFiles(sourceDir, "*", SearchOption.AllDirectories);
            using (FileStream zipToCreate = new FileStream(zipFilePath, FileMode.Create))
            {
                using (ZipArchive archive = new ZipArchive(zipToCreate, ZipArchiveMode.Create))
                {
                    foreach (string fileInDir in filesToCompress)
                    {
                        string entryName = fileInDir.Replace(sourceDir, "").TrimStart('\\', '/');
                        archive.CreateEntryFromFile(fileInDir, entryName);
                    }
                }
            }
        }

        private bool CheckAssetBundleFolder()
        {
            if(string.IsNullOrEmpty(iosAssetBundleFolder) || string.IsNullOrEmpty(androidAssetBundleFolder) || !Directory.Exists(iosAssetBundleFolder) || Directory.GetFiles(iosAssetBundleFolder).Length == 0 || !Directory.Exists(androidAssetBundleFolder) || Directory.GetFiles(androidAssetBundleFolder).Length == 0)
            {
                if(!EditorUtility.DisplayDialog("错误", "所选目录下无ab。是否继续？", "取消", "继续")){
                    Debug.Log("继续打包");
                    return true;
                };
                return false;
            }
            return true;
        }

        string GetPackageVersion()
        {
            string packageName = "com.antgroup.antmetaverse"; // 替换为您想要获取版本信息的包名称
            ListRequest listRequest = Client.List(); //获取所有已安装的包列表

            while (!listRequest.IsCompleted) //等待请求完成
            {
                System.Threading.Tasks.Task.Delay(100); // 等待100ms
            }
            
            if (listRequest.Status == StatusCode.Success) //请求成功
            {
                foreach (var package in listRequest.Result)
                {
                    if (package.name == packageName)
                    {
                        Debug.Log("Package name: " + package.name);
                        Debug.Log("Package version: " + package.version); //输出版本信息
                        return package.version;
                    }
                }
            }
            else
            {
                EditorUtility.DisplayDialog("错误", "打包失败！Failed to retrieve package list: " + listRequest.Error.message, "确定");
                Debug.Log("打包失败！Failed to retrieve package list: " + listRequest.Error.message);
            }
            return "";
        }

        private void WriteConfig()
        {
            JObject jObject = new JObject();
            jObject["version"] = version.ToString();
            string configPath = IOSFolder + "/config.json";
            File.WriteAllText(configPath, jObject.ToString());
            configPath = AndroidFolder + "/config.json";
            File.WriteAllText(configPath, jObject.ToString());
        }

        private void WriteHotUpdateConfig()
        {
            string metaSDKVersion = GetPackageVersion();
            DllConfig dllConfig = new DllConfig
            {
                MainAssemblyFile = assemblyDefinitionAsset.name,
                MetaAssemblyFiles = SettingsUtil.AOTAssemblyNames,
                HotUpdateAssemblyFiles = SettingsUtil.HotUpdateAssemblyNamesExcludePreserved.OrderBy(x => x != METASDKDLLNAME).ToList(),
                MetaSDKVersion = metaSDKVersion
            };
            string hotUpdateConfigPath = IOSFolder + "/HotUpdateConfig.json";
            File.WriteAllText(hotUpdateConfigPath, JsonConvert.SerializeObject(dllConfig));
            hotUpdateConfigPath = AndroidFolder + "/HotUpdateConfig.json";
            File.WriteAllText(hotUpdateConfigPath, JsonConvert.SerializeObject(dllConfig));
        }

        private bool CheckVersion(bool isInit = false)
        {
            string configPath = IOSFolder + "/config.json";
            if (File.Exists(configPath))
            {
                string json = File.ReadAllText(configPath);
                JObject jObject = JObject.Parse(json);
                int lastVersion = int.Parse(jObject["version"].ToString());
                if(isInit || version == lastVersion)
                {
                    version = lastVersion + 1;
                    Repaint();
                    return true;
                }
                return true;
            }
            version = 1;
            return true;
        }

        private bool CheckAsmdef()
        {
            if (assemblyDefinitionAsset == null)
            {
                EditorUtility.DisplayDialog("错误", "业务主逻辑程序集不能为空", "确定");
                return false;
            }
            return true;
        }


        private void PackIosAndroidDllForJingTanApp()
        {
            if (!Directory.Exists(IOSFolder))
            {
                Directory.CreateDirectory(IOSFolder);
            }
            if (!Directory.Exists(AndroidFolder))
            {
                Directory.CreateDirectory(AndroidFolder);
            }
            BuildAndCopyABAOTHotUpdateDlls(AndroidFolder, BuildTarget.Android);
            BuildAndCopyABAOTHotUpdateDlls(IOSFolder, BuildTarget.iOS);
        }

        private void BuildAndCopyABAOTHotUpdateDlls(string targetFolder, BuildTarget target)
        {
            string dllFolder = Path.Combine(targetFolder, "Dlls");
            if (!Directory.Exists(dllFolder))
            {
                Directory.CreateDirectory(dllFolder);
            }
            string resouceFolder = Path.Combine(targetFolder, "Resources");
            if (!Directory.Exists(resouceFolder))
            {
                Directory.CreateDirectory(resouceFolder);
            }
            if(target == BuildTarget.iOS)
            {
                CopyFolderToFolder(iosAssetBundleFolder, resouceFolder);
            }
            else if(target == BuildTarget.Android)
            {
                CopyFolderToFolder(androidAssetBundleFolder, resouceFolder);
            }
            CompileDllCommand.CompileDll(target);
            CopyABAOTHotUpdateDlls(dllFolder, target);
            AssetDatabase.Refresh();
        }

        private void CopyFolderToFolder(string srcFolder, string dstFolder)
        {
            if(!Directory.Exists(srcFolder))
            {
                Debug.LogError($"[CopyFolderToFolder] srcFolder not exist: {srcFolder}");
                return;
            }
            if (Directory.Exists(dstFolder))
            {
                Directory.Delete(dstFolder, true);
            }
            Directory.CreateDirectory(dstFolder);
            foreach (var file in Directory.GetFiles(srcFolder))
            {
                File.Copy(file, Path.Combine(dstFolder, Path.GetFileName(file)));
            }
        }

        private void CopyABAOTHotUpdateDlls(string dllTargetFolder, BuildTarget target)
        {
            CopyAOTAssembliesToStreamingAssets(dllTargetFolder, target);
            CopyHotUpdateAssembliesToStreamingAssets(dllTargetFolder, target);
        }


        private void CopyAOTAssembliesToStreamingAssets(string dllTargetFolder, BuildTarget target)
        {
            string aotAssembliesSrcDir = SettingsUtil.GetAssembliesPostIl2CppStripDir(target);
            string aotAssembliesDstDir = dllTargetFolder;

            foreach (var dll in SettingsUtil.AOTAssemblyNames)
            {
                string srcDllPath = $"{aotAssembliesSrcDir}/{dll}.dll";
                if (!File.Exists(srcDllPath))
                {
                    Debug.LogError($"ab中添加AOT补充元数据dll:{srcDllPath} 时发生错误,文件不存在。裁剪后的AOT dll在BuildPlayer时才能生成，因此需要你先构建一次游戏App后再打包。");
                    continue;
                }
                string dllBytesPath = $"{aotAssembliesDstDir}/{dll}.dll.bytes";
                File.Copy(srcDllPath, dllBytesPath, true);
                Debug.Log($"[CopyAOTAssembliesToStreamingAssets] copy AOT dll {srcDllPath} -> {dllBytesPath}");
            }
        }

        private void CopyHotUpdateAssembliesToStreamingAssets(string dllTargetFolder, BuildTarget target)
        {
            string hotfixDllSrcDir = SettingsUtil.GetHotUpdateDllsOutputDirByTarget(target);
            string hotfixAssembliesDstDir = dllTargetFolder;
            foreach (var dll in SettingsUtil.HotUpdateAssemblyFilesExcludePreserved)
            {
                string dllPath = $"{hotfixDllSrcDir}/{dll}";
                string dllBytesPath = $"{hotfixAssembliesDstDir}/{dll}.bytes";
                File.Copy(dllPath, dllBytesPath, true);
                Debug.Log($"[CopyHotUpdateAssembliesToStreamingAssets] copy hotfix dll {dllPath} -> {dllBytesPath}");
            }
        }
    }
#endif
}