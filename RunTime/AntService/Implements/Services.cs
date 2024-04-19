using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
#if !JINGTAN_APP
using AlipaySdk;
#endif
using Newtonsoft.Json.Linq;
using System.Reflection;
using UnityEngine.Networking;
using System.Collections;

namespace Ant.Metaverse
{
    public class CommonService : ICommonService
    {

#if JINGTAN_APP
        public void QuitGame()
        {
            Debug.Log("QuitGame");
            // 后面挪到UnitySDK里
            AntMetaverseUtil.GetMethodInfo("Assembly-CSharp", "Platform", "InvokeNativeMethod")?.Invoke(null, new string[]{"quit", "", ""});
            NativeMsgManager.Dispose();
            AntMetaverseUtil.Dispose();

        }
#endif

        public void HideLoadingView()
        {
            Debug.Log("HideLoadingView");
#if JINGTAN_APP
            AntMetaverseUtil.GetMethodInfo("Assembly-CSharp", "Platform", "InvokeNativeMethod")?.Invoke(null, new string[]{"SceneManager", "onReady", ""});

            // AntMetaverseUtil.GetMethodInfo("Assembly-CSharp", "Platform", "HideLoadingView")?.Invoke(null, null);
            // assembly.GetType("Platform").GetMethod("HideLoadingView").Invoke(null, null);
#else
            AlipaySDK.API.HideLoadingView();
#endif
        }

        public void GetScreenOrientation(Action<Exception, string> callback)
        {
            try{
#if JINGTAN_APP
                callback(null, Screen.orientation.ToString());
#else
                AlipaySDK.API.GetOrientation(jsonStr => {
                    JObject jObject = JObject.Parse(jsonStr);
                    string orientation = jObject["orientation"].ToString();
                    Debug.Log(orientation);
                    callback(null, orientation);
                });
#endif
            }
            catch(Exception e){
                callback(e, null);
            }
        }

        public void Share(string title, string desc, string link, string imageUrl, Action<Exception, string> callback)
        {
            Debug.Log("Share");
#if JINGTAN_APP
            try{
                var content = new{
                    title = title,
                    description = desc,
                    link = link,
                    imageUrl = imageUrl,
                    // UEFA等定制企业空间屏蔽“分享到动态”按钮
                    channelDisabled = new string[]{"jtmoment"}
                };
                AntMetaverseUtil.GetMethodInfo("Assembly-CSharp", "Platform", "InvokeNativeMethod")?.Invoke(null, new string[]{"share", JsonConvert.SerializeObject(content), ""});
            }
            catch(Exception e){
                callback(e, null);
            }
#else
            try{

                JObject param = new JObject();
                param.Add("chInfo", "小游戏");
                param.Add("transAnimate", "YES");
                param.Add("transparent", "YES");
                param.Add("scene", "share");

                Debug.Log("Share title: " + title + " desc: " + desc + " link: " + link + " imageUrl: " + imageUrl);
                JObject shareConfig = new JObject();
                shareConfig.Add("bizType", "ztokenV0_wVgyZkRS");
                shareConfig.Add("url", link);
                shareConfig.Add("title", title);
                shareConfig.Add("desc", desc);
                param.Add("shareConfig", shareConfig);

                Factory.GetService<ICommonService>().StartBizService(param, (e, result) => {
                    if (e != null)
                    {
                        callback(e, null);
                        return;
                    }
                    callback(null, result);
                });
            }
            catch(Exception e){
                callback(e, null);
            }
#endif

        }

        public void SetScreenOrientation(ScreenOrientation orien)
        {
#if JINGTAN_APP
            try{
                var content = new{
                    orientation = orien == ScreenOrientation.LandscapeLeft ? "landscape" : "portrait"
                };
                AntMetaverseUtil.GetMethodInfo("Assembly-CSharp", "Platform", "InvokeNativeMethod")?.Invoke(null, new string[]{"changeOrientation", JsonConvert.SerializeObject(content), ""});
            }
            catch(Exception e){
                Debug.Log("SetScreenOrientation Error " + e);
            }
#else
            try{
                Factory.GetService<ICommonService>().GetSystemInfo((e, result) => {
                    if (e != null)
                    {
                        Debug.LogError("GetSystemInfo error: " + e);
                        return;
                    }
                    JObject jObject = JObject.Parse(result);
                    string platform = jObject["platform"].ToString();
                    Debug.Log("platform: " + platform);
                    if (platform == "iOS")
                    {
                        AlipaySDK.API.SetOrientation(orien, null);
                    }
                    else
                    {
                        Screen.orientation = orien;
                    }
                });
            }
            catch(Exception e){
                Debug.Log("SetScreenOrientation Error " + e);
            }
#endif
        }

        public void GetLaunchOptions(string[] query, Action<Exception, string> callback)
        {
            Debug.Log("GetLaunchOptions");
#if JINGTAN_APP
            try{
                object result = AntMetaverseUtil.GetMethodInfo("Assembly-CSharp", "LuaManager", "GetLaunchOptionsString")?.Invoke(null, null);
                if(result == null){
                    throw new Exception("GetLaunchOptionsString is null");
                }
                callback(null, result.ToString());
            }
            catch(Exception e){
                callback(e, null);
            
            }
#else
            try{
                AlipaySDK.API.GetLaunchOptions(query, jsonStr => {
                    JObject jObject = JObject.Parse(jsonStr);
                    string result = jObject["query"].ToString();
                    callback(null, result);
                });
            }
            catch(Exception e){
                callback(e, null);
            }
#endif
        }

        public void NavigateToMiniProgram(JObject param, Action<Exception, string> callback)
        {
#if !JINGTAN_APP
            try{
                string appId = param["appId"].ToString();
                AlipaySDK.API.NavigateToMiniProgram(appId, param, (result) =>
                {
                    Debug.Log("NavigateToMiniProgram result: " + result);
                    callback(null, result);
                });
            }
            catch(Exception e){
                callback(e, null);
            }
#endif
        }

        public void StartBizService(JObject param, Action<Exception, string> callback)
        {
            Debug.Log("StartBizService");

#if !JINGTAN_APP
            try{
                AlipaySDK.InternalAPI.StartBizService("bigworld", param, (result) =>
                {
                    Debug.Log("StartBizService result: " + result);
                    callback(null, result);
                });
            }
            catch(Exception e){
                Debug.Log(e);
                callback(e, null);
            }
#endif
        }

#if !JINGTAN_APP
        public void AddOnShowListener(Action<string> callback)
        {
            AlipaySDK.onShow += callback;
        }

        public void RemoveOnShowListener(Action<string> callback)
        {
            AlipaySDK.onShow -= callback;
        }

        public void AddOnHideListener(Action callback)
        {
            AlipaySDK.onHide += callback;
        }

        public void RemoOnHideListener(Action callback)
        {
            AlipaySDK.onHide -= callback;
        }

        public void NavigateToJTSpringBoard(JObject param, Action<Exception, string> callback)
        {
            try{
                param.Add("appId", MetaSDK.JINGTANAPPID);
                Factory.GetService<ICommonService>().NavigateToMiniProgram(param, (exception, result) =>
                {
                    Debug.Log("NavigateToJTSpringBoard result: " + result);
                    callback(null, result);
                });
            }
            catch(Exception e){
                callback(e, null);
            }
        }
#endif
        public void GetSystemInfo(Action<Exception, string> callback)
        {
#if !JINGTAN_APP
            try{
                AlipaySDK.API.GetSystemInfo(result =>
                {
                    Debug.Log("GetSystemInfo result: " + result);
                    callback(null, result);
                });
            }
            catch(Exception e){
                callback(e, null);
            }
#endif
        }

        public void ReportLog(string JTEventId, string AlipayEventId, JObject extParam, Action<Exception, string> callback)
        {
#if JINGTAN_APP
            try{
                var player = new
                {
                    JTEventId = JTEventId,
                    extParam = extParam
                };

                // 将匿名对象转换为JSON字符串
                string json = JsonConvert.SerializeObject(player);
                AntMetaverseUtil.GetMethodInfo("Assembly-CSharp", "Platform", "InvokeNativeMethod")?.Invoke(null, new string[]{"spm", json, ""});
            }
            catch(Exception e){
                callback(e, null);
            
            }
#else
            try{
                AlipaySDK.InternalAPI.EventMonitor(AlipayEventId, extParam, (result) =>
                {
                    Debug.Log("Spm result: " + result);
                    callback(null, result);
                });
            }
            catch(Exception e){
                callback(e, null);
            }
#endif
        }

        public void SetTransparentTitle(string title, Action<Exception, string> callback)
        {
#if !JINGTAN_APP
            try{
                AlipaySDK.InternalAPI.SetTransparentTitle(title, (result) =>
                {
                    Debug.Log("SetTransparentTitle result: " + result);
                    callback(null, result);
                });
            }
            catch(Exception e){
                callback(e, null);
            }
#endif
        }

        public void StartAPP(string appId, string scheme, Action<Exception, string> callback)
        {
#if !JINGTAN_APP
            try{
                AlipaySDK.InternalAPI.StartAPP(appId, scheme, (result) =>
                {
                    Debug.Log("StartApp result: " + result);
                    callback(null, result);
                });
            }
            catch(Exception e){
                callback(e, null);
            }
#endif
        }

        public string GetCompressedAvatarUrl(string url, int size = 256)
        {
            try{
                string targetSize = string.Format("/{0}w", size);
                
                Uri uri = new Uri(url);
                string path = uri.AbsolutePath;
                bool isOrigEnd = path.EndsWith("/original");
                string newPath = "";
                if(isOrigEnd){
                    newPath = path.Replace("/original", targetSize);
                }
                else{
                    newPath = string.Format("{0}{1}", path, targetSize);
                }
                return new UriBuilder(uri.Scheme, uri.Host, uri.Port, newPath, uri.Query).ToString();
            }
            catch(Exception e){
                Debug.Log("GetCompressedAvatarUrl Error " + e);
                return url;
            }
        }
    }


#if !JINGTAN_APP
    //分享、会员信息、广告
    public class UserService : IUserService
    {
        public void GetHealthData(DateTime date, Action<Exception, string> callback)
        {
            try{
                string countDate = date.ToString("yyyy-MM-dd");
                AlipaySDK.API.GetRunData(countDate,(result) =>
                {
                    Debug.Log(result);
                    JObject paramJson = JObject.Parse(result);
                    if(paramJson["error"] != null){
                        callback(new Exception(paramJson["error"].ToString() + paramJson["errorMessage"].ToString()), null);
                        return;
                    }
                    callback(null, result);
                });
            }
            catch(Exception e){
                callback(e, null);
            }
        }

        public void GetFriends(Action<Exception, List<Friend>> callback)
        {
            try{
                Debug.Log("GetFriends has not been implemented");
                callback(null, new List<Friend>(){new Friend()});
            }
            catch(Exception e){
                callback(e, null);
            }
        }

        public void OpenTaskPage(string userInfoToken, string channelType, Action<Exception, string> callback)
        {
            try{
                JObject param = new JObject();
                param.Add("chInfo", "小游戏");
                param.Add("transAnimate", "YES");
                param.Add("transparent", "YES");
                param.Add("scene", "task");
                // param.Add("env", "dev");
                param.Add("channelType", channelType);
                param.Add("tenantId", MetaSDK.GetTenantId());
                param.Add("userInfoToken", userInfoToken);
                Factory.GetService<ICommonService>().StartBizService(param, (e, result) => {
                    if (e != null)
                    {
                        callback(e, null);
                        return;
                    }
                    callback(null, result);
                });
            }
            catch(Exception e){
                callback(e, null);
            }
        }

        public void GetJingTanAuthCode(JObject args = null, Action<Exception, string> callback = null)
        {
            try{
                Action realCall = () => {
                    JObject param = new JObject();
                    param.Add("chInfo", "小游戏");
                    param.Add("transAnimate", "YES");
                    param.Add("transparent", "YES");
                    param.Add("scene", "auth");
                    param.Merge(args);

                    Factory.GetService<ICommonService>().StartBizService(param, (e, result) => {
                        if (e != null)
                        {
                            callback(e, null);
                            return;
                        }
                        callback(null, result);
                    });
                };

                Factory.GetService<ICommonService>().GetSystemInfo((e, result) => {
                    if (e != null)
                    {
                        Debug.LogError("GetSystemInfo error: " + e);
                        realCall();
                        return;
                    }
                    JObject jObject = JObject.Parse(result);
                    string platform = jObject["platform"].ToString();
                    Debug.Log("platform: " + platform);
                    if (platform == "iOS")
                    {
                        iOSGeneralBehaviour.iOSKeepOrien(realCall);
                    }
                    else
                    {
                        realCall();
                    }
                });
            }
            catch(Exception e){
                callback(e, null);
            }
        }


        public void GetAlipayAuthCode(string scope, Action<Exception, string> callback)
        {
            try{
                string[] scopes = new string[]{scope};
                AlipaySDK.API.GetAuthCode(scopes, result =>
                {
                    Debug.Log(string.Format("GetAuthCode scope: {0}, result: {1}", scope, result));
                    AuthResponse response = JsonConvert.DeserializeObject<AuthResponse>(result);
                    Debug.Log(string.Format("GetAuthCode after deserialization: {0}. Authcode: {1}", response, response.authCode));
                    if(response.error != 0){
                        Debug.Log(string.Format("GetAuthCode error: {0}, message: {1}", response.error, response.errorMessage));
                        throw new Exception(response.error + response.errorMessage);
                    }
                    callback(null, response.authCode);
                });
                }
            catch(Exception e){
                callback(e, null);
            }
        }
    }
#endif

#if !JINGTAN_APP
    public class iOSGeneralBehaviour{
        public static void iOSKeepOrien(Action action)
        {
            Factory.GetService<ICommonService>().GetScreenOrientation(
                (e, orientation) => {
                    if(e != null){
                        Debug.LogError("GetScreenOrientation error: " + e);
                        orientation = "landscape";
                    }

                    // ios平台特写
                    if(orientation == "landscape"){
                        Factory.GetService<ICommonService>().SetScreenOrientation(ScreenOrientation.Portrait);
                        Factory.GetService<ICommonService>().AddOnShowListener(iosOnShowBehaviour);
                        action();
                    }
                    else{
                        action();
                    }

                }
            );
        }

        private static void iosOnShowBehaviour(string result)
        {
            Debug.Log("OnShowBehaviour");
            Factory.GetService<ICommonService>().SetScreenOrientation(ScreenOrientation.LandscapeLeft);
            Factory.GetService<ICommonService>().RemoveOnShowListener(iosOnShowBehaviour);
        }
    }
#endif

    public class PaymentService : IPaymentService
    {
        public void Buy(string itemId, string externalOrderId, Action<Exception, string> callback)
        {
            Debug.Log("Buy");
            try{
#if JINGTAN_APP
                string prefixUrl = "antfans://web?appid=68687967&url=";
                string urlParam = UnityWebRequest.EscapeURL(string.Format("/www/trade_commodity.html?itemId={0}&externalOrderId={1}&tenantId={2}&reJumpUrl=&__webview_options__=transparentTitle%3Dalways%26allowsBounceVertical%3DNO", itemId, externalOrderId, MetaSDK.GetTenantId()));
                string url = prefixUrl + urlParam;
                Debug.Log("Buy url: " + url);
                AntMetaverseUtil.OpenUrl(url, (e, result) => {
                    if(e != null){
                        callback(e, null);
                        return;
                    }
                    callback(null, result);
                });
#else
                Action realCall = () => {
                    JObject param = new JObject();
                    param.Add("appId", MetaSDK.JINGTANAPPID);
                    param.Add("page", string.Format("pages/trade/commodity/index?itemId={0}&externalOrderId={1}&tenantId={2}&chInfo=", itemId, externalOrderId, MetaSDK.GetTenantId()));
                    Factory.GetService<ICommonService>().NavigateToMiniProgram(param, (e, result) => {
                        if(e != null){
                            callback(e, null);
                            return;
                        }
                        callback(null, result);
                    });
                };

                Factory.GetService<ICommonService>().GetSystemInfo((e, result) => {
                    if(e != null){
                        Debug.LogError("GetSystemInfo error: " + e);
                        realCall();
                        return;
                    }
                    JObject jObject = JObject.Parse(result);
                    string platform = jObject["platform"].ToString();
                    Debug.Log("platform: " + platform);
                    if(platform == "iOS"){
                        iOSGeneralBehaviour.iOSKeepOrien(realCall);
                    }
                    else{
                        realCall();
                    }
                });

#endif
            }
            catch(Exception e){
                callback(e, null);
            }
        }

    }

    public class FileService : IFileService
    {
        
#if !JINGTAN_APP
        public void IsExist(string filePath, Action<Exception, string> callback)
        {
            try{
                Debug.Log("FileServices.IsExist");
                JObject fsParam = new JObject();
                fsParam.Add("path", filePath);
                AlipaySDK.API.FSManage("access", fsParam, (result) =>
                {
                    FileOperateResponse response = JsonConvert.DeserializeObject<FileOperateResponse>(result);
                    Debug.Log("IsExist result: " + response.success + response.error + response.errorMessage);
                    if(response.error != null){
                        throw new Exception(response.error + response.errorMessage);
                    }
                    callback(null, response.success);
                });
            }
            catch(Exception e){
                callback(e, null);
            }
        }

        public void ReadFile(JObject args, Action<Exception, byte[]> callback)
        {
            try{
                // fsParam.Add("filePath", "share.png");
                // fsParam.Add("encoding", "utf8");
                AlipaySDK.API.FSManage("readFile", args, (result) =>
                {
                    JObject jsonObj = JObject.Parse(result);
                    if (jsonObj.ContainsKey("data"))
                    {
                        byte[] temp = (byte[])jsonObj["data"];
                        callback(null, temp);    
                    }
                });
            }
            catch(Exception e){
                callback(e, null);
            }
        }
#endif

        public void WriteFile(JObject args, Action<Exception, string> callback)
        {
            try{
#if JINGTAN_APP
                // 鲸探APP内处理下载，并返回文件本地路径
                if(!args.ContainsKey("fileUrl")){
                    throw new Exception("fileUrl is required");
                }
                NativeMsgManager.DownloadFile(args["fileUrl"].ToString(), (e, filePath) => {
                    if(e != null){
                        callback(e, null);
                        return;
                    }
                    Debug.Log("WriteFile From NativeMsgManager: " + filePath);
                    callback(null, filePath);
                });
#else
                // JObject fsParam = new JObject();
                // fsParam.Add("filePath", "share.png");
                // fsParam.Add("data，", tempSaveImg);
                // fsParam.Add("encoding", "utf8");
                Debug.Log("WriteFile From SDK");
                AlipaySDK.API.FSManage("writeFile", args, (result) =>
                {
                    Debug.Log(result);
                    callback(null, result);
                });
#endif
            }
            catch(Exception e){
                callback(e, null);
            }
        }


#if !JINGTAN_APP
        public void DeleteFile(string filePath, Action<Exception, string> callback)
        {
            try{
                JObject args = new JObject();
                args.Add("filePath", filePath);
                AlipaySDK.API.FSManage("unlink", args, (result) =>
                {
                    Debug.Log(result);
                    callback(null, result);
                });
            }
            catch(Exception e){
                callback(e, null);
            }
        }

#endif
    }

    [Serializable]
    public class AuthResponse
    {
        public string authCode;
        public string authcode;
        public string[] authSuccessScopes;

        public int error;
        public string errorMessage;
    }

    [Serializable]
    public class FileOperateResponse
    {
        public string success;
        public string error;
        public string errorMessage;
    }

}