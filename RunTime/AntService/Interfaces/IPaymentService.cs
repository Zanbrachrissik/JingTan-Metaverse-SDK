using System;
using System.Collections.Generic;

namespace Ant.Metaverse
{
    /// <summary>
    /// 支付相关接口。
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// 购买接口
        /// </summary>
        /// <param name="itemId">商品详情ID</param>
        /// <param name="externalOrderId">外部生成的交易单号ID</param>
        /// <param name="callback">回调函数，第一个参数为异常信息，如果异常信息为 `NULL` 则表示接口调用成功返回，
        public void Buy(string itemId, string externalOrderId = "", Action<Exception, string> callback = null);
    }
}