using UnityEngine;

namespace Ant.MetaVerse
{

    ///TODO: 1. 支持日志等级，比如只打印warning以上的日志,2. 设置宏

    /// <summary>
    /// AntLog is a wrapper for Unity's Debug.Log. 如果不想打日志，设置宏定义ANT_DISABLE_LOG
    /// </summary>
    public class AntLog
    {
        public static void Info(string message)
        {
            Debug.Log(message);
        }

        public static void Error(string message)
        {
            Debug.LogError(message);
        }
    }
}
