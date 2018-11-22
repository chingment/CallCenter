using System;


namespace Lumos.Entity
{
    /// <summary>
    /// 业务的枚举
    /// </summary>
    public partial class Enumeration
    {
        public enum BizSnType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("订单号")]
            Order = 1,
            [Remark("入库单号")]
            Order2StockIn = 2,
            [Remark("出库单号")]
            Order2StockOut = 3
        }
        public enum WxUserInfoFrom
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("授权")]
            Authorize = 1,
            [Remark("订阅")]
            Subscribe = 2
        }
        public enum WxAutoReplyType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("订阅")]
            Subscribe = 1,
            [Remark("关键字")]
            Keyword = 2,
            [Remark("菜单点击")]
            MenuClick = 3,
            [Remark("不是关键字")]
            NotKeyword = 4
        }

        public enum BizProcessesAuditType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("车险订单")]
            OrderToCarInsure = 1,
            [Remark("车险理赔")]
            OrderToCarClaim = 2,
            [Remark("商户资料审核")]
            MerchantAudit = 3,
            [Remark("人才输送")]
            OrderToTalentDemand = 4,
            [Remark("定损点申请")]
            OrderToApplyLossAssess = 5,
            [Remark("违章处理")]
            OrderToLllegalDealt = 6,
            [Remark("贷款")]
            OrderToCredit = 7,
            [Remark("保险产品订单")]
            OrderToInsurance = 8
        }
    }
}
