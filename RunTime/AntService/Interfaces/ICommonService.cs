using System;
using UnityEngine;

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

        public void SetOrientation(ScreenOrientation orientation, Action<Exception, string> callback);

        public void GetLaunchOptions(string[] query, Action<Exception, string> callback);
    }
}