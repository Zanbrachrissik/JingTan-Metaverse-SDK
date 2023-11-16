using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using System.Text;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;
using System;
using System.Linq;
using Object = UnityEngine.Object;

namespace Ant.MetaVerse.Editor
{
    public class ResponseData
    {
        public int code;
        public string message;
        public bool success;
        public string traceId;
        public bool data;
    }

    public class JingZaoEditor : EditorWindow
    {
        string ROOT_PATH = "";
        Object source;
        string modelId = "";
        List<string> assetPaths = new List<string>();


        private void OnEnable()
        {
            ROOT_PATH = Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets"));
        }

        [MenuItem("Tools/鲸造打包工具")]
        public static void Init()
        {
            JingZaoEditor window = (JingZaoEditor)EditorWindow.GetWindow(typeof(JingZaoEditor), false, "鲸造资源打包上传工具");
            window.Show();
        }

        private void OnGUI() {
            modelId = EditorGUILayout.TextField("模型id(网页复制)", modelId, "TextField");
            
            EditorGUILayout.BeginHorizontal();
            source = EditorGUILayout.ObjectField(source, typeof(Object), true);
            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("开始上传") && source != null)
            {
                if(!CheckIsAsset()){
                    EditorUtility.DisplayDialog("错误", "请选择资源文件", "好的");
                    return;
                }
                PackAssetBundle();
                Upload();
            }
        }

        private bool CheckIsAsset()
        {
            string assetPath = AssetDatabase.GetAssetPath(source);
            return !string.IsNullOrEmpty(assetPath);
        }


        private void PackAssetBundle()
        {
            assetPaths.Clear();
            string assetPath = AssetDatabase.GetAssetPath(source);

            List<AssetBundleBuild> list = new List<AssetBundleBuild>();
            var assetName = source.name;
            var buildInfo = new AssetBundleBuild();
            buildInfo.assetBundleName = assetName;
            buildInfo.assetNames = new string[] { assetPath };
            list.Add(buildInfo);

            // string filePath = ROOT_PATH + "JingZaoAssetBundle";
            // assetPaths.Add(filePath + "/Android/" + assetName);
            // assetPaths.Add(filePath + "/iOS/" + assetName);
            // assetPaths.Add(filePath + "/WebGL/" + assetName);
            Build(list);
        }

        private void Build(List<AssetBundleBuild> buildList)
        {
            var assetName = buildList[0].assetBundleName;
            string filePath = ROOT_PATH + "JingZaoAssetBundle";

            if (Directory.Exists(filePath))
            {
                try
                {
                    Directory.Delete(filePath, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Exception: {ex}");
                }
            }
            assetPaths.Add(filePath + "/Android/" + assetName);
            assetPaths.Add(filePath + "/iOS/" + assetName);
            assetPaths.Add(filePath + "/WebGL/" + assetName);

            Directory.CreateDirectory(filePath + "/Android");
            BuildPipeline.BuildAssetBundles("JingZaoAssetBundle/Android", buildList.ToArray(), BuildAssetBundleOptions.StrictMode, BuildTarget.Android);

            Directory.CreateDirectory(filePath + "/iOS");
            BuildPipeline.BuildAssetBundles("JingZaoAssetBundle/iOS", buildList.ToArray(), BuildAssetBundleOptions.StrictMode, BuildTarget.iOS);

            Directory.CreateDirectory(filePath + "/WebGL");
            BuildPipeline.BuildAssetBundles("JingZaoAssetBundle/WebGL", buildList.ToArray(), BuildAssetBundleOptions.StrictMode, BuildTarget.WebGL);
        }

        public void Upload()
        {
            EditorCoroutineUtility.StartCoroutine(UploadMultipleFiles(), this);
        }

        IEnumerator UploadMultipleFiles()
        {
            UnityWebRequest[] files = new UnityWebRequest[assetPaths.Count];
            WWWForm form = new WWWForm();
            form.AddField("modelId", modelId);

            // 加1是因为还有上传过程
            int totalProcess = assetPaths.Count + 1;

            for (int i = 0; i < assetPaths.Count; i++)
            {
                var path = Path.Combine(ROOT_PATH, assetPaths[i]);
                files[i] = UnityWebRequest.Get("file://" + path);
                EditorUtility.DisplayProgressBar("请稍候", string.Format("正在读取{0}", assetPaths[i]), (float)i / totalProcess);
                yield return files[i].SendWebRequest();
                if (assetPaths[i].Contains("iOS"))
                    form.AddBinaryData("iosFile", files[i].downloadHandler.data, Path.GetFileName(path));
                else if (assetPaths[i].Contains("Android"))
                    form.AddBinaryData("androidFile", files[i].downloadHandler.data, Path.GetFileName(path));
                else if (assetPaths[i].Contains("WebGL"))
                    form.AddBinaryData("webglFile", files[i].downloadHandler.data, Path.GetFileName(path));
            }

            string postUrl = "http://zkmynftmerchant-238.gzz8c.dev.alipay.net/jingzao/project/model/supplyABResource";
            UnityWebRequest req = UnityWebRequest.Post(postUrl, form);

            req.uploadHandler.contentType = "multipart/form-data";
            EditorUtility.DisplayProgressBar("请稍候", "正在上传", 1f);

            yield return req.SendWebRequest();
            EditorUtility.ClearProgressBar();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string response = req.downloadHandler.text;
                ResponseData msg = JsonUtility.FromJson<ResponseData>(response);
                Debug.Log(string.Format("traceId: {0}, success: {1}, message: {2}, code: {3}, data: {4}", msg.traceId, msg.success, msg.message, msg.code, msg.data));
                if (msg.data)
                {
                    EditorUtility.DisplayDialog("提示", "上传成功！", "好的");
                }
                else
                {
                    EditorUtility.DisplayDialog("错误", "上传失败！\n 失败信息：" + msg.message, "好的");
                }
            }
            else
            {
                Debug.Log("failed" + req.error);
                EditorUtility.DisplayDialog("错误", "上传失败！\n 失败信息：" + req.error, "好的");
            }
        }
    }
}