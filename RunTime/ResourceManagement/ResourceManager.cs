using UnityEngine;
using System;
using System.Collections;
using UnityEngine.Networking;

namespace Ant.Metaverse
{
    public class ResourceManager : BaseManager<ResourceManager>
    {
        public AssetBundle LoadAssetBundle(string pathOrUrl)
        {
            return null;
        }

        /// <summary>
        /// 异步加载AssetBundle，如果是本地路径，会先检查本地是否存在，如果不存在，会先下载
        /// </summary>
        /// <param name="pathOrUrl">路径或者URL</param>
        /// <param name="callback">回调</param>
        /// <param name="needDownload">是否需要下载到本地。默认为false，即不会发生磁盘读写，只有内存读写。不会下载到本地</param>
        public IEnumerator LoadAssetBundleAsync(string pathOrUrl, Action<AssetBundle> callback = null, bool needDownload = false)
        {
            if (string.IsNullOrEmpty(pathOrUrl))
            {
                Debug.LogError("pathOrUrl is null");
                yield break;
            }

            if(needDownload)
            {
                // LoadAssetBundleAsync(pathOrUrl, callback);
                yield break;
            }

            UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(pathOrUrl);

            // 发送请求并等待响应
            yield return webRequest.SendWebRequest();

            // 检查是否有错误发生
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error loading AssetBundle: " + webRequest.error);
                yield break;
            }

            // 从响应中获取AssetBundle
            AssetBundle ab = DownloadHandlerAssetBundle.GetContent(webRequest);

            // 释放UnityWebRequest对象
            webRequest.Dispose();
            callback?.Invoke(ab);
        }

    }
}