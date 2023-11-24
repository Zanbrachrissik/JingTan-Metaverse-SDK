using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Ant.MetaVerse
{
    /// <summary>
    /// 获取用户信息相关的接口。
    /// </summary>
    public interface IUserService
    {
        /// <summary>
        /// 请求获取用户信息的授权码。
        /// </summary>
        /// <param name="accountType">请求授权的主体，目前支持 鲸探 和 支付宝 的授权。</param>
        /// <param name="scope"> acctountType为 AccountType.ALIPAY 时才生效。scope 为 `auth_user` 时，授权获取支付宝会员信息。为 `auth_base` 时，表示授权获取支付宝会员唯一标识（user_id）。此方式为静默授权，不会弹出授权浮窗。</param>
        /// <param name="callback">
        /// 回调函数，第一个参数为异常信息，如果异常信息为 `NULL` 则表示接口调用成功返回，
        /// 返回结果存储在第二个参数中。如果异常信息不为 `NULL` 则表示接口调用失败，第二个参数为 `NULL`。
        /// </param>
        /// <example>
        /// <code>
        /// static void HandleAuthCodeResult(Exception e, string authCode)
        /// {
        ///     if (e != null)
        ///     {
        ///         HandleException(e);
        ///         return;
        ///     }
        ///     reportAuthCodeToServer(code);
        /// }
        ///
        /// void AskForAlipayAuthCode()
        /// {
        ///    var userService = Factory.GetService<IUserService>();
        ///    if (userService == null)
        ///    {
        ///        Debug.WriteLine("The service is not found.");
        ///        return;
        ///    }
        ///    userService.GetAuthCode(AccountType.ALIPAY, "auth_user", HandleAuthCodeResult);
        /// }
        /// 
        /// void AskForJINGTANAuthCode()
        /// {
        ///    var userService = Factory.GetService<IUserService>();
        ///    if (userService == null)
        ///    {
        ///        Debug.WriteLine("The service is not found.");
        ///        return;
        ///    }
        ///    userService.GetAuthCode(AccountType.JINGTAN, "", HandleAuthCodeResult);
        /// }
        /// </code>
        /// </example>
        void GetAlipayAuthCode(string scope, Action<Exception, string> callback);

        void GetJingTanAuthCode(JObject args = null, Action<Exception, string> callback = null);

        /// <summary>
        /// 获取运动健康相关数据。
        /// </summary>
        /// <param name="date">要获取的数据日期。</param>
        /// <param name="callback">
        /// 回调函数，第一个参数为异常信息，如果异常信息为 `NULL` 则表示接口调用成功返回，
        /// 返回结果存储在第二个参数中。如果异常信息不为 `NULL` 则表示接口调用失败，第二个参数为 `NULL`。
        /// </param>
        /// <example>
        /// <code>
        /// static void HandleFitnessInfoResult(Exception e, string data)
        /// {
        ///     if (e != null)
        ///     {
        ///         HandleException(e);
        ///         return;
        ///     }
        /// 
        ///     // TODO:
        /// }
        /// 
        /// void OnButtonClick()
        /// {
        ///     var userService = Factory.GetService&lt;IUserService&gt;();
        ///     if (userService == null)
        ///     {
        ///         Debug.WriteLine("The service is not found.");
        ///         return;
        ///     }
        ///     var date = new DateTime(2015, 10, 21);
        ///     userService.GetHealthData(date, HandleFitnessInfoResult);
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// 目前只支持查询最近 30 天内的运动步数，若超过 30 天，则返回的步数为 0。
        /// 接入流程：
        /// 1. <a href="https://open.alipay.com/develop/mini/sub/dev-setting?bundleId=com.alipay.alipaywallet">开放平台-开发设置</a> 设置 接口内容加密方式（可参考文档 <a href="https://opendocs.alipay.com/common/02mse3">接口内容加密方式</a>）。
        /// 2. 在开放平台控制台为当前小程序绑定 <a href="https://open.alipay.com/develop/uni/mini/choose-product?bundleId=com.alipay.alipaywallet&amp;productCode=I1080300001000042966">运动数据</a> 产品。
        /// 3. 点击 `申请用户信息` 按钮，进入用户信息申请页面 （需登录主账号或管理员账号进行操作），申请 my.queryStepDailyCount 权限。
        /// 4. 调用 `GetHealthData()` 获取获取加密后的报文数据。
        /// 5. 客户端把上一步获取到的密文发送给服务端，参考 <a href="https://opendocs.alipay.com/common/02mse3">接口内容加密方式</a> 进行验签解密。
        /// </remarks>
        void GetHealthData(DateTime date, Action<Exception, string> callback);

        /// <summary>
        /// 获取支付宝好友列表。
        /// </summary>
        /// <param name="callback">
        /// 回调函数，第一个参数为异常信息，如果异常信息为 `NULL` 则表示接口调用成功返回，
        /// 返回结果存储在第二个参数中。如果异常信息不为 `NULL` 则表示接口调用失败，第二个参数为 `NULL`。
        /// </param>
        /// <example>
        /// <code>
        /// static void HandleFriendsResult(Exception e, List&lt;Friend&gt; friends)
        /// {
        ///     if (e != null)
        ///     {
        ///         HandleException(e);
        ///         return;
        ///     }
        ///     showFriends(friends);
        /// }
        ///
        /// void OnButtonClick()
        /// {
        ///     var userService = Factory.GetService&lt;IUserService&gt;();
        ///     if (userService == null)
        ///     {
        ///         Debug.WriteLine("The service is not found.");
        ///         return;
        ///     }
        ///     userService.GetFriends(HandleFriendsResult);
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// </remarks>
        void GetFriends(Action<Exception, List<Friend>> callback);
    }
}
