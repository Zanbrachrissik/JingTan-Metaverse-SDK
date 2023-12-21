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
using Newtonsoft.Json;


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

    [Serializable]
    public class ABData
    {
        public string comment;
        public string assetPath;
        public string modelId;
        [NonSerialized]
        public Object asset;
    }

    public class JingZaoEditor : EditorWindow
    {
        string ROOT_PATH = "";
        List<string> assetPaths = new List<string>();

        [SerializeField]
        public List<ABData> jingzaoABData = new List<ABData>();

        int totalCount = 0;

        bool packAndroid = false;
        bool packIOS = false;
        bool packWebGL = false;


        private void OnEnable()
        {
            ROOT_PATH = Application.dataPath.Substring(0, Application.dataPath.IndexOf("Assets"));
            ImportFromJson();
        }

        private void ImportFromJson()
        {
            string path = ROOT_PATH + "JingZaoAssetBundle/ABData.json";
            if (File.Exists(path))
            {
                string json = File.ReadAllText(path);
                jingzaoABData = JsonConvert.DeserializeObject<List<ABData>>(json);
                totalCount = jingzaoABData.Count;
                jingzaoABData.ForEach(abData =>
                {
                    if (abData.assetPath != null)
                    {
                        abData.asset = AssetDatabase.LoadAssetAtPath<Object>(abData.assetPath);
                    }
                });
            }
        }

        [MenuItem("Tools/鲸造打包工具")]
        public static void Init()
        {
            JingZaoEditor window = (JingZaoEditor)EditorWindow.GetWindow(typeof(JingZaoEditor), false, "鲸造资源打包上传工具");
            window.Show();
        }

        private void OnGUI() {
            EditorGUILayout.LabelField("打包平台", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical();
            packAndroid = EditorGUILayout.Toggle("Android", packAndroid);
            packIOS = EditorGUILayout.Toggle("iOS", packIOS);
            packWebGL = EditorGUILayout.Toggle("WebGL", packWebGL);
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
            totalCount = EditorGUILayout.DelayedIntField("资源总数", totalCount);

            if(totalCount != jingzaoABData.Count){
                if(totalCount > jingzaoABData.Count){
                    for(int i = 0; i < totalCount - jingzaoABData.Count; i++){
                        jingzaoABData.Add(new ABData());
                    }
                }else{
                    for(int i = 0; i < jingzaoABData.Count - totalCount; i++){
                        jingzaoABData.RemoveAt(jingzaoABData.Count - 1);
                    }
                }
            }
            
            EditorGUILayout.Space();
            for(int i = 0; i < totalCount; i++){
                EditorGUILayout.LabelField($"第{i + 1}个资源", EditorStyles.boldLabel);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("模型id", EditorStyles.boldLabel);
                jingzaoABData[i].modelId = EditorGUILayout.TextField(jingzaoABData[i].modelId);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("资源", EditorStyles.boldLabel);
                jingzaoABData[i].asset = EditorGUILayout.ObjectField(jingzaoABData[i].asset, typeof(Object), true);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("注释", EditorStyles.boldLabel);
                jingzaoABData[i].comment = EditorGUILayout.TextField(jingzaoABData[i].comment);
                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            if(GUILayout.Button("新增一个")){
                totalCount++;
            }

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            if (GUILayout.Button("开始上传"))
            {
                if(!CheckIsAsset(out int index)){
                    EditorUtility.DisplayDialog("错误", $"第{index + 1}个选择的资源文件不正确，请重新选择", "好的");
                    return;
                }
                PackAssetBundle();
                EditorCoroutineUtility.StartCoroutine(Upload(), this);
            }
        }

        private bool CheckIsAsset(out int index)
        {
            index = -1;
            for(int i = 0; i < jingzaoABData.Count; i++){
                string assetPath = AssetDatabase.GetAssetPath(jingzaoABData[i].asset);
                if(string.IsNullOrEmpty(assetPath)){
                    index = i;
                    return false;
                }
            }
            return false;
        }


        private void PackAssetBundle()
        {
            assetPaths.Clear();
            List<AssetBundleBuild> list = new List<AssetBundleBuild>();
            for(int i = 0; i < jingzaoABData.Count; i++){
                if(string.IsNullOrEmpty(jingzaoABData[i].modelId)){
                    continue;
                }
                Object source = jingzaoABData[i].asset;
                string assetPath = AssetDatabase.GetAssetPath(source);

                var assetName = i + "_" + jingzaoABData[i].modelId;
                var buildInfo = new AssetBundleBuild();
                buildInfo.assetBundleName = assetName;
                buildInfo.assetNames = new string[] { assetPath };
                list.Add(buildInfo);
            }


            // string filePath = ROOT_PATH + "JingZaoAssetBundle";
            // assetPaths.Add(filePath + "/Android/" + assetName);
            // assetPaths.Add(filePath + "/iOS/" + assetName);
            // assetPaths.Add(filePath + "/WebGL/" + assetName);
            Build(list);
        }

        private void Build(List<AssetBundleBuild> buildList)
        {
            var assetName = buildList[0].assetBundleName;
            string filePath = ROOT_PATH + "JingZaoAssetBundle/Output";

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
            if (packAndroid)
            {
                assetPaths.Add(filePath + "/Android/" + assetName);
                Directory.CreateDirectory(filePath + "/Android");
                BuildPipeline.BuildAssetBundles("JingZaoAssetBundle/Android", buildList.ToArray(), BuildAssetBundleOptions.StrictMode, BuildTarget.Android);
            }
            if (packIOS)
            {
                assetPaths.Add(filePath + "/iOS/" + assetName);
                Directory.CreateDirectory(filePath + "/iOS");
                BuildPipeline.BuildAssetBundles("JingZaoAssetBundle/iOS", buildList.ToArray(), BuildAssetBundleOptions.StrictMode, BuildTarget.iOS);
            }
            if (packWebGL)
            {
                assetPaths.Add(filePath + "/WebGL/" + assetName);
                Directory.CreateDirectory(filePath + "/WebGL");
                BuildPipeline.BuildAssetBundles("JingZaoAssetBundle/WebGL", buildList.ToArray(), BuildAssetBundleOptions.StrictMode, BuildTarget.WebGL);
            }
        }

        IEnumerator Upload()
        {
            for (int i = 0; i < jingzaoABData.Count; i++)
            {
                if (string.IsNullOrEmpty(jingzaoABData[i].modelId))
                {
                    continue;
                }
                yield return EditorCoroutineUtility.StartCoroutine(UploadSingleItem(i), this);
            }
            yield return null;
            EditorUtility.DisplayDialog("提示", "上传完成！详细信息请查看控制台日志", "好的");
        }

        IEnumerator UploadSingleItem(int index)
        {
            string modelId = jingzaoABData[index].modelId;
            string abName = index + "_" + modelId;
            string parentFolder = ROOT_PATH + "JingZaoAssetBundle/Output";


            UnityWebRequest[] files = new UnityWebRequest[3];
            WWWForm form = new WWWForm();
            form.AddField("modelId", modelId);

            // 加1是因为还有上传过程
            int totalProcess = assetPaths.Count + 1;

            if(packAndroid){
                string fullPath = Path.Combine(parentFolder, "Android", abName);
                var file = UnityWebRequest.Get("file://" + fullPath);
                EditorUtility.DisplayProgressBar("请稍候", string.Format("正在读取{0}", fullPath), 0f);
                yield return file.SendWebRequest();
                form.AddBinaryData("androidFile", file.downloadHandler.data, Path.GetFileName(fullPath));
            }
            if(packIOS){
                string fullPath = Path.Combine(parentFolder, "iOS", abName);
                var file = UnityWebRequest.Get("file://" + fullPath);
                EditorUtility.DisplayProgressBar("请稍候", string.Format("正在读取{0}", fullPath), 0f);
                yield return file.SendWebRequest();
                form.AddBinaryData("iosFile", file.downloadHandler.data, Path.GetFileName(fullPath));
            }
            if(packWebGL){
                string fullPath = Path.Combine(parentFolder, "WebGL", abName);
                var file = UnityWebRequest.Get("file://" + fullPath);
                EditorUtility.DisplayProgressBar("请稍候", string.Format("正在读取{0}", fullPath), 0f);
                yield return file.SendWebRequest();
                form.AddBinaryData("webglFile", file.downloadHandler.data, Path.GetFileName(fullPath));
            }

            // dev用http，pre及prod用https
            //prod: https://mynftmerchant.antgroup.com/
            //pre: https://mynftmerchant-pre.antgroup.com/
            string postUrl = "https://mynftmerchant.antgroup.com/jingzao/project/model/supplyABResource";
            UnityWebRequest req = UnityWebRequest.Post(postUrl, form);

            req.uploadHandler.contentType = "multipart/form-data";
            EditorUtility.DisplayProgressBar("请稍候", "正在上传", 1f);

            yield return req.SendWebRequest();
            EditorUtility.ClearProgressBar();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string response = req.downloadHandler.text;
                ResponseData msg = JsonUtility.FromJson<ResponseData>(response);
                Debug.Log(string.Format("第{5}个item上传信息：traceId: {0}, success: {1}, message: {2}, code: {3}, data: {4}", msg.traceId, msg.success, msg.message, msg.code, msg.data, index + 1));
                // if (msg.data)
                // {
                //     EditorUtility.DisplayDialog("提示", "上传成功！", "好的");
                // }
                // else
                // {
                //     EditorUtility.DisplayDialog("错误", "上传失败！\n 失败信息：" + msg.message, "好的");
                // }
            }
            else
            {
                Debug.Log($"第{index + 1}个item上传失败：{req.error}" + req.error);
                // EditorUtility.DisplayDialog("错误", "上传失败！\n 失败信息：" + req.error, "好的");
            }
        }

        private void OnDisable() {
            EditorUtility.ClearProgressBar();
            ExportToJson();
        }

        private void ExportToJson()
        {
            for(int i = 0; i < jingzaoABData.Count; i++){
                jingzaoABData[i].assetPath = AssetDatabase.GetAssetPath(jingzaoABData[i].asset);
            }
            string path = ROOT_PATH + "JingZaoAssetBundle/ABData.json";
            string json = JsonConvert.SerializeObject(jingzaoABData, Formatting.Indented);
            File.WriteAllText(path, json);
        }
    }
}