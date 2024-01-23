

namespace Ant.Metaverse
{
    /// <summary>
    /// 单例继承类，需要在Game重新实现Instance
    /// </summary>
    public abstract class BaseManager<T> where T : new()
	{
        protected static T s_Instance;

		#region Properties

		public static T Instance
		{
            get
			{
				if (s_Instance == null)
				{
					s_Instance = new T();
                    // 记录每个Manager的启动
                    AntLog.Info("Base Manager " + string.Format("BaseManager Instance New: {0}", typeof(T).Name));
                }

				return s_Instance;
			}
		}

		#endregion
    }
}