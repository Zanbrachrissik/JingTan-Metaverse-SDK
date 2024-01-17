using System;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Ant.MetaVerse
{
    /// <summary>
    /// 文件读取服务（支付宝：10.5.70版本以上使用）
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public void IsExist(string filePath, Action<Exception, string> callback);
        public void ReadFile(JObject args, Action<Exception, byte[]> callback);
        public void WriteFile(JObject args, Action<Exception, string> callback);
        public void DeleteFile(string filePath, Action<Exception, string> callback);
    }
}