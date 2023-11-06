using System;
using System.Collections.Generic;

namespace Ant.MetaVerse
{
    /// <summary>
    /// 支付相关接口。
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// 购买接口
        /// </summary>
        /// <param name="productId">商品详情ID</param>
        /// <param name="transactionId">交易ID</param>
        /// <param name="callback">回调函数，第一个参数为异常信息，如果异常信息为 `NULL` 则表示接口调用成功返回，
        public void Buy(string productId, string transactionId, Action<Exception, string> callback);
    }
}