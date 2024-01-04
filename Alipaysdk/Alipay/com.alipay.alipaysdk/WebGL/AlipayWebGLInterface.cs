using System;
using System.Runtime.InteropServices;
using UnityEngine.Scripting;
using UnityEngine;

[assembly: Preserve]

namespace AlipaySdk.Bridge
{
    public class AlipayWebGLInterface
    {
#if UNITY_WEBPLAYER || UNITY_WEBGL
        // 以下接口为 Web 使用，用于调用 JS 代码
        [method: Preserve]
        [DllImport("__Internal")]
        public static extern void unityCallJs(String eventId, String paramJson);
#else
        public static void unityCallJs(String eventId, String paramJson)
        {
            Debug.LogError("message dropped, please check platform");
        }
#endif
    }
}
