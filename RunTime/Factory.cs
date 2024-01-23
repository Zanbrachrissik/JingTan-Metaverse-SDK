using System;
using System.Collections.Generic;

namespace Ant.Metaverse
{
    /// <summary>
    /// 工厂类。
    /// </summary>
    public class Factory
    {
        static CommonService commonService;
    #if !JINGTAN_APP
        static UserService userService;
        static PaymentService paymentService;
        static FileService fileService;
    #endif
        /// <summary>
        /// 用于获取指定类型的对象。使用后记得及时销毁。
        /// </summary>
        /// <typeparam name="T">要获取服务的类型</typeparam>
        /// <returns>指定服务的类型，哪果返回 `NULL` 则表示该服务暂不支持。</returns>
        public static T GetService<T>() where T : class
        {
            Type type =  typeof (T);
#if JINGTAN_APP
            if(type == typeof(ICommonService)){
                commonService ??= new CommonService();
                return commonService as T;
            }
#else
            if(type == typeof(IUserService)){
                userService ??= new UserService();
                return userService as T;
            }
            else if(type == typeof(IAvatarService)){
                return AvatarViewObject.Instance as T;
            }
            else if(type == typeof(IPaymentService)){
                paymentService ??= new PaymentService();
                return paymentService as T;
            }
            else if(type == typeof(ICommonService)){
                commonService ??= new CommonService();
                return commonService as T;
            }
            else if(type == typeof(IFileService)){
                fileService ??= new FileService();
                return fileService as T;
            }
#endif
            else{
                return default(T);
            }
        }

    }
}