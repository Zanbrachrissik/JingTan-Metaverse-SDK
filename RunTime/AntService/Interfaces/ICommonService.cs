using System;

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
    }
}