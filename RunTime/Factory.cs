using System;
using System.Collections.Generic;

namespace Ant.MetaVerse
{
    /// <summary>
    /// 工厂类。
    /// </summary>
    public class Factory
    {
        /// <summary>
        /// 用于获取指定类型的对象。使用后记得及时销毁。
        /// </summary>
        /// <typeparam name="T">要获取服务的类型</typeparam>
        /// <returns>指定服务的类型，哪果返回 `NULL` 则表示该服务暂不支持。</returns>
        public static T GetService<T>() where T : class
        {
            Type type =  typeof (T);
            if(type == typeof(IPaymentService)){
                return new PaymentService() as T;
            }
            else if(type == typeof(IUserService)){
                return new UserService() as T;
            }
            else if(type == typeof(IAvatarService)){
                return AvatarViewObject.Instance as T;
            }
            else{
                return default(T);
            }
        }

    }
}