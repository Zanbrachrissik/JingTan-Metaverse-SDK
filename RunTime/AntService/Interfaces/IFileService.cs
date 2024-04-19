using System;
using UnityEngine;
using Newtonsoft.Json.Linq;

namespace Ant.Metaverse
{
    /// <summary>
    /// 文件读取服务（支付宝：10.5.70版本以上使用）
    /// </summary>
    public interface IFileService
    {
        
#if !JINGTAN_APP
        /// <summary>
        /// 判断文件是否存在
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public void IsExist(string filePath, Action<Exception, string> callback);
        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="args">必须包含filePath和encoding字段</param>
        /// <param name="callback"></param>
        public void ReadFile(JObject args, Action<Exception, byte[]> callback);
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="callback"></param>
        public void DeleteFile(string filePath, Action<Exception, string> callback);
#endif
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="args">支付宝小程序平台必须包含filePath和encoding字段；鲸探APP必须包含fileUrl字段，返回localPath</param>
        /// <param name="callback"></param>
        public void WriteFile(JObject args, Action<Exception, string> callback);
    }
}