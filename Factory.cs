namespace Ant.MetaVerse
{
    /// <summary>
    /// 工厂类。
    /// </summary>
    public class Factory
    {
        /// <summary>
        /// 用于获取指定类型的对象。
        /// </summary>
        /// <typeparam name="T">要获取服务的类型</typeparam>
        /// <returns>指定服务的类型，哪果返回 `NULL` 则表示该服务暂不支持。</returns>
        public static T GetService<T>()
        {
            return default(T);
        }
    }
}