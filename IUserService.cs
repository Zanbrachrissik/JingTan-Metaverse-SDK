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
        /// void OnButtonClick()
        /// {
        ///     var userService = Factory.GetService&lt;IUserService&gt;();
        ///     if (userService == null)
        ///     {
        ///         Debug.WriteLine("The service is not found.");
        ///         return;
        ///     }
        ///     userService.GetAuthCode(HandleAuthCodeResult);
        /// }
        /// </code>
        /// </example>
        /// <remarks>
        /// 接入流程：
        /// 1. 到开放平台控制台为目标小程序绑定 <a href="https://open.alipay.com/develop/uni/mini/choose-product?bundleId=com.alipay.alipaywallet&productCode=I1080300001000042699">获取会员信息</a> 产品。
        /// 2. 完成产品绑定以后，点击「用户信息申请入口」，按需申请相应的字段。
        /// 3. 在 <a href="https://gw.alipayobjects.com/mdn/rms_390dfd/afts/img/A*LqsXR45_a4IAAAAAAAAAAAAAARQnAQ">用户信息申请</a> 页面，按需申请相应的字段。
        /// 4. 客户端调用 GetAuthCode获取授权码，并上报到自己的服务端。
        /// 5. 服务端使用授权码，调用 <a href="https://opendocs.alipay.com/open/02xtla">alipay.system.oauth.token</a> 取得 user_id 和 token（授权令牌）。
        /// 6. 服务端继续使用所取得的 token 调用 <a href="https://opendocs.alipay.com/open/02xtlb">alipay.user.info.share</a> 最终获得用户信息。
        /// </remarks>
        void GetAuthCode(Action<Exception, string> callback);

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
        ///     var date = new DateOnly(2015, 10, 21);
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
        void GetHealthData(DateOnly date, Action<Exception, string> callback);

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
