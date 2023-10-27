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
            Debug.Log("GetOrientation");
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

        public void SetOrientation(ScreenOrientation orientation, Action<Exception, string> callback)
        {
#if !JINGTAN_APP
            try{
                AlipaySDK.API.SetOrientation(orientation, result => {
                    Debug.Log(result);
                    callback(null, result);
                });
            }
            catch(Exception e){
                Debug.Log(e);
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

        public void GetAuthCode(string scope, Action<Exception, string> callback)
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
                        callback(new Exception(response.error + response.errorMessage), null);
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