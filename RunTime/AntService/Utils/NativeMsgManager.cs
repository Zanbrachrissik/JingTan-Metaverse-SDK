using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ant.Metaverse
{
    public class NativeMsgManager
    {
        private static int _msgId = 0;
        static Dictionary<int, Action<Exception, string>> _callbackDic = new Dictionary<int, Action<Exception, string>>();
        public static void DownloadFile(string url, Action<Exception, string> callback)
        {
            var info = new{
                uri = url,
                extra = GetUniqueMsgId()
            };
            _callbackDic.Add(info.extra, callback);
            AntMetaverseUtil.GetMethodInfo("Assembly-CSharp", "Platform", "InvokeNativeMethod")?.Invoke(null, new string[]{"download", JsonConvert.SerializeObject(info), ""});
        }
        
        private static int GetUniqueMsgId()
        {
            return _msgId++;
        }
        
        public static void ReceiveMsgFromJT(string msg)
        {
            Debug.Log("ReceiveMsgFromJT" + msg);
            if (string.IsNullOrEmpty(msg))
            {
                return;
            }

            try{
                JObject jObject = JObject.Parse(msg);
                // 下面这样写的话就要补充元数据了 Newtonsoft.Json
                // int extra = jObject.Value<int>("extra");
                string dataStr = jObject["data"]?.ToString();
                if (string.IsNullOrEmpty(dataStr))
                {
                    return;
                }
                JObject data = JObject.Parse(dataStr);
                JObject result = (JObject)data["result"];
                string localPath = (string)AntMetaverseUtil.GetJtokenByKey(result, "localPath");
                int extra = (int)AntMetaverseUtil.GetJtokenByKey(result, "extra");
                string uri = (string)AntMetaverseUtil.GetJtokenByKey(result, "uri");
                int code = (int)AntMetaverseUtil.GetJtokenByKey(result, "code");
                Debug.Log("ReceiveMsgFromJT" + uri + " " + extra + " " + localPath + " " + code);
                if (_callbackDic.TryGetValue(extra, out Action<Exception, string> callback))
                {
                    _callbackDic.Remove(extra);
                    
                    if (string.IsNullOrEmpty(localPath))
                    {
                        callback(new Exception("Download error! localPath is null"), null);
                        return;
                    }
                    callback(null, localPath);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("ReceiveMsgFromJT error: " + e);
            }
        }

        public static void Dispose()
        {
            _callbackDic.Clear();
        }
    }
}