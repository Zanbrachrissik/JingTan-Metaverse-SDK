using System;

namespace Ant.MetaVerse
{

    /// <summary>支付服务</summary>
    public interface IPaymentService
    {
        /// <summary>支付</summary>
        /// <param name="transactionId">支付宝交易号，注意 参数有大小写区分。</param>
        /// <param name="action">支付结果回调</param>
        /// <example>
        /// <code>
        /// </code>
        /// </example>
        /// <remarks>
        /// 接入流程：
        /// 1. 需要在 <a href="https://open.alipay.com/develop/manage">开放平台</a> 绑定产品列表中，为当前小程序绑定 JSAPI 支付 产品。具体可参考文档 <a href="https://opendocs.alipay.com/mini/05xmim?pathHash=e9ca29c2#%E4%BA%A7%E5%93%81%E7%BB%91%E5%AE%9A">产品绑定</a>。
        /// 2. 需要将应用和支付宝商家平台账号绑定，应用才可调用需要商家开通的产品，具体步骤可查看 <a href="https://opendocs.alipay.com/open/0128wr">绑定应用</a>。
        /// 3. 获取交易号。
        /// 4. 调用 Pay接口进入支付。
        /// 5. 可以通过在服务端接收异步通知或者接口查询订单状态判断支付是否成功，具体如何操作如下：
        /// ○ 接收异步通知：商家服务端在调用 alipay.trade.create 接口时设置异步通知（notify_url）来获得支付宝服务端返回的支付结果，当异步通知中返回的 trade_status（交易状态）为 TRADE_SUCCESS 时，表示支付成功，可参考 此文档。
        /// ○ 接口查询订单状态：可调用 <a href="https://opendocs.alipay.com/mini/05xsky?scene=common&amp;pathHash=354e8be3">alipay.trade.query</a>（统一收单交易查询接口），通过商家网站唯一订单号 out_trade_no 或支付宝交易号 trade_no 查询商户订单列表中订单的支付状态。
        /// 注意：my.tradePay 回调中的结果码 9000 不作为支付成功的可靠判断依据。
        /// 
        /// 获取交易号：
        /// 1. 下载服务端 SDK，请下载对应语言版本的最新版 <a href="https://opendocs.alipay.com/common/02n6z6">服务端 SDK</a> 并引入开发工程。
        /// 2. 在 SDK 调用具体的接口前需要进行 alipayClient 对象初始化。alipayClient 对象只需要初始化一次，后续调用不同的接口都可以使用同一个 alipayClient 对象。具体可参考文档 <a href="https://opendocs.alipay.com/mini/05xmim?pathHash=e9ca29c2#%E6%8E%A5%E5%8F%A3%E8%B0%83%E7%94%A8%E9%85%8D%E7%BD%AE">接口调用配置</a>。
        /// 3. 通过服务端调用 统一收单交易创建接口（alipay.trade.create）获取 trade_no，此值即为 支付宝交易号，具体可参考文档 <a href="https://opendocs.alipay.com/mini/05x9ku?pathHash=a7b61cca#%E5%88%9B%E5%BB%BA%E4%BA%A4%E6%98%93%E8%AE%A2%E5%8D%95">创建交易订单</a>。
        /// </remarks>
        public void Pay(string transactionId, Action<Exception> action);
    }

}