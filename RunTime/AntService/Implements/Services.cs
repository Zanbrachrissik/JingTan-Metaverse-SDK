using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
#if !JINGTAN_APP
using AlipaySdk;
#endif
using Newtonsoft.Json.Linq;
using System.Reflection;

namespace Ant.MetaVerse
{
    public class CommonService : ICommonService
    {
        public void HideLoadingView()
        {
            Debug.Log("HideLoadingView");
#if JINGTAN_APP
            Assembly assembly = AntMetaverseUtil.GetLoadedAssembly("Assembly-CSharp");
            assembly.GetType("Platform").GetMethod("HideLoadingView").Invoke(null, null);
#else
            AlipaySDK.API.HideLoadingView();
#endif
        }

        public void GetOrientation(Action<Exception, string> callback)
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

        public void SetOrientation(ScreenOrientation orientation)
        {
#if !JINGTAN_APP
            try{
                AlipaySDK.API.SetOrientation(orientation, null);
            }
            catch(Exception e){
                Debug.Log("SetOrientation Error " + e);
            }
#endif
        }

        public void GetLaunchOptions(string[] query, Action<Exception, string> callback)
        {
            Debug.Log("GetLaunchOptions");
#if !JINGTAN_APP
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
            Debug.Log("NavigateToMiniProgram");
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
        }

        public void StartBizService(JObject param, Action<Exception, string> callback)
        {
            Debug.Log("StartBizService");
            try{
                AlipaySDK.API.StartBizService("bigworld", param, (result) =>
                {
                    Debug.Log("StartBizService result: " + result);
                    callback(null, result);
                });
            }
            catch(Exception e){
                Debug.Log(e);
                callback(e, null);
            }

        }

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

        public void GetAuthCode(AccountType accountType, string scope, Action<Exception, string> callback)
        {
            try{
                if(accountType == AccountType.JINGTAN){
                    Action realCall = () => {
                        JObject param = new JObject();
                        param.Add("chInfo", "小游戏");
                        param.Add("transAnimate", "YES");
                        param.Add("transparent", "YES");
                        param.Add("scene", "auth");
                        JObject param2 = new JObject();
                        param.Add("query", param2);

                        Factory.GetService<ICommonService>().StartBizService(param, (e, result) => {
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
                }
                else if(accountType == AccountType.ALIPAY){
                    string[] scopes = new string[]{scope};
                    AlipaySDK.API.GetAuthCode(scopes, result =>
                    {
                        Debug.Log(string.Format("GetAuthCode scope: {0}, result: {1}", scope, result));
                        AuthResponse response = JsonConvert.DeserializeObject<AuthResponse>(result);
                        Debug.Log(string.Format("GetAuthCode after deserialization: {0}. Authcode: {1}", response, response.authCode));
                        if(response.error != 0){
                            Debug.Log(string.Format("GetAuthCode error: {0}, message: {1}", response.error, response.errorMessage));
                            callback(new Exception(response.error + response.errorMessage), null);
                        }
                        callback(null, response.authCode);
                    });
                }
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
            Factory.GetService<ICommonService>().GetOrientation(
                (e, orientation) => {
                    if(e != null){
                        Debug.LogError("GetOrientation error: " + e);
                        orientation = "landscape";
                    }
                    Debug.Log("xxk###: " + Application.platform.ToString());

                    // ios平台特写
                    if(orientation == "landscape"){
                        Factory.GetService<ICommonService>().SetOrientation(ScreenOrientation.Portrait);
                        Factory.GetService<ICommonService>().AddOnShowListener(iosOnShowBehaviour);
                        action();
                    }
                    else{
                        Debug.Log("xxk### else 逻辑");
                        action();
                    }

                }
            );
        }

        private static void iosOnShowBehaviour(string result)
        {
            Debug.Log("OnShowBehaviour");
            Factory.GetService<ICommonService>().SetOrientation(ScreenOrientation.LandscapeLeft);
            Factory.GetService<ICommonService>().RemoveOnShowListener(iosOnShowBehaviour);
        }
    }
#endif

#if !JINGTAN_APP
    public class PaymentService : IPaymentService
    {
        public void Buy(string productId, string transactionId, Action<Exception, string> callback)
        {
            Debug.Log("Buy");
            try{

                Action realCall = () => {
                    JObject param = new JObject();
                    param.Add("chInfo", "小游戏");
                    param.Add("transAnimate", "YES");
                    param.Add("transparent", "YES");
                    param.Add("scene", "purchase");
                    JObject param2 = new JObject();
                    param2.Add("productId", "xianxiao");
                    param2.Add("transactionId", "xianxiao2");
                    param.Add("query", param2);

                    Factory.GetService<ICommonService>().StartBizService(param, (e, result) => {
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

            }
            catch(Exception e){
                callback(e, null);
            }
        }

    }
#endif

    [Serializable]
    public class AuthResponse
    {
        public string authCode;
        public string authcode;
        public string[] authSuccessScopes;

        public int error;
        public string errorMessage;
    }



}