using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text;
using UnityEngine.Networking;
using Unity.EditorCoroutines.Editor;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class JTLogin : EditorWindow
{
    static string phoneNumber;
    static string code;
    

    [MenuItem("Tools/登录测试")]
    public static void ShowWindow()
    {
        GetWindow<JTLogin>("登录测试");

    }

    public void OnGUI()
    {
        phoneNumber = EditorGUILayout.TextField("手机号：", phoneNumber);
        code = EditorGUILayout.TextField("验证码：", code);

        if (GUILayout.Button("获取验证码"))
        {
            EditorCoroutineUtility.StartCoroutineOwnerless(SendGetCode());
        }

        if (GUILayout.Button("登录"))
        {

        }
    }

    static IEnumerator SendGetCode()
    {
        Dictionary<string, string> userInfoToken = new Dictionary<string, string>();
        userInfoToken.Add("bizType", "login");
        userInfoToken.Add("fansId", "");
        userInfoToken.Add("phoneNumber", phoneNumber);
        UnityWebRequest request = UnityWebRequest.Put("https://mgs-mpaas.antfans.com/mgw.htm", "[" + JsonConvert.SerializeObject(userInfoToken) + "]");
        request.method = UnityWebRequest.kHttpVerbPOST;

        request.SetRequestHeader("WorkspaceId", "pre");
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("AppId", "ALIPUB059F038311550");
        request.SetRequestHeader("Operation-Type", "com.antgroup.antchain.mymobileprod.service.user.requestSmsCodeWithoutLogin");
        request.SetRequestHeader("x-source", "fans");

        yield return request.SendWebRequest();


    }
}
