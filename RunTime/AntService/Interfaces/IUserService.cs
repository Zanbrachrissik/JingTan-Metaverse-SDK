using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Ant.Metaverse
{
    /// <summary>
    /// 获取用户信息授权相关的接口。
    /// </summary>
    public interface IUserService
    {
        void GetAlipayAuthCode(string scope, Action<Exception, string> callback);

        void GetJingTanAuthCode(JObject args = null, Action<Exception, string> callback = null);

        void GetHealthData(DateTime date, Action<Exception, string> callback);

        void GetFriends(Action<Exception, List<Friend>> callback);
    }
}
