using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Collections;

namespace Ant.Metaverse
{
    public interface ICommonService
    {
        /// <summary>
        /// 隐藏默认的 loading 界面。
        /// </summary>
        public void HideLoadingView();

        /// <summary>
        /// 获取环境检查信息。
        /// </summary>
        /// <param name="callback"></param>
        public void GetSystemInfo(Action<Exception, string> callback);

        public void GetScreenOrientation(Action<Exception, string> callback);

        public void SetScreenOrientation(ScreenOrientation orientation);

        public void GetLaunchOptions(string[] query, Action<Exception, string> callback);
        public void NavigateToMiniProgram(JObject param, Action<Exception, string> callback);
        public void StartBizService(JObject param, Action<Exception, string> callback);

#if !JINGTAN_APP
        public void AddOnShowListener(Action<string> callback);
        public void RemoveOnShowListener(Action<string> callback);
        public void AddOnHideListener(Action callback);
        public void RemoOnHideListener(Action callback);
        public void NavigateToJTSpringBoard(JObject param, Action<Exception, string> callback);
#endif
        /// <summary>
        /// 埋点
        /// (支付宝：10.5.70版本以上使用）
        /// </summary>
        public void ReportLog(string JTEventId, string AlipayEventId, JObject extParam, Action<Exception, string> callback);
        /// <summary>
        /// 分享
        /// </summary>
        /// <param name="title">分享标题</param>
        /// <param name="desc">分享描述</param>
        /// <param name="imgUrl">分享图片Url，必须是https链接</param>
        /// <param name="callback">回调</param>
        /// <returns></returns>
        public void Share(string title, string desc, string link, string imgUrl, Action<Exception, string> callback);
        /// <summary>
        /// 设置透明导航栏
        /// (支付宝：10.5.70版本以上使用）
        /// </summary>
        public void SetTransparentTitle(string title, Action<Exception, string> callback);
        /// <summary>
        /// StartAPP
        /// (支付宝：10.5.70版本以上使用）
        /// </summary>
        public void StartAPP(string appId, string scheme, Action<Exception, string> callback);
        /// <summary>
        /// 根据原始头像Url生成压缩后的头像Url
        /// </summary>
        /// <param name="url">原始头像Url</param>
        /// <param name="size">压缩后的头像尺寸</param>
        /// <returns>
        /// 压缩后的头像Url
        /// </returns>
        public string GetCompressedAvatarUrl(string url, int size);

#if JINGTAN_APP
        public void QuitGame();
#endif
    }
}