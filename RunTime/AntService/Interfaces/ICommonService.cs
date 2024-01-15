using System;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Ant.MetaVerse
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

        public void GetOrientation(Action<Exception, string> callback);

        public void SetOrientation(ScreenOrientation orientation);

        public void GetLaunchOptions(string[] query, Action<Exception, string> callback);
        public void NavigateToMiniProgram(JObject param, Action<Exception, string> callback);
        public void StartBizService(JObject param, Action<Exception, string> callback);
        public void AddOnShowListener(Action<string> callback);
        public void RemoveOnShowListener(Action<string> callback);
        public void AddOnHideListener(Action callback);
        public void RemoOnHideListener(Action callback);

        /// <summary>
        /// 埋点
        /// (支付宝：10.5.70版本以上使用）
        /// </summary>
        public void RegisterEventMonitor(string eventId, JObject extParam, Action<Exception, string> callback);
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
    }
}