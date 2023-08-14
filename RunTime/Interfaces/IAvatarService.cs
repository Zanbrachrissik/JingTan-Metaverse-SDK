using System;

namespace Ant.MetaVerse
{

    /// <summary>
    /// 数字人信息。
    /// </summary>
    public class Avatar
    {
        /// <summary>标识。</summary>
        public string ID { get; set; }

        /// <summary>名称。</summary>
        public string Name { get; set; }

        /// <summary>性别，取值为 `male` 或 `female`，不区别分大小写。</summary>
        public string Gender { get; set; }

        /// <summary>当前数字形像的性别是否是真实性别。</summary>
        public bool IsRealGender { get; set; }

        /// <summary>描述数字人外形的 JSON 字符串。</summary>
        public string Profile { get; set; }
    }

    /// <summary>
    /// 数字人服务。
    /// </summary>
    public interface IAvatarService
    {
        /// <summary>
        /// 数字人服务。
        /// </summary>
        /// <param name="sceneId">场景标识</param>
        /// <param name="callback">
        /// </param>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <remarks>
        /// </remarks>
        void GetAvatar(string sceneId, Action<Exception, UnityEngine.Object> callback);
    }
}