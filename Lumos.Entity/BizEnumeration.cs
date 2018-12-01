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
            Unknow = 0
        }

        public enum BusinessType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("车险")]
            CarIns = 1
        }

        public enum OrganizationStatus
        {

            [Remark("未知")]
            Unknow = 0,
            [Remark("正常")]
            Valid = 1,
            [Remark("停用")]
            Invalid = 2
        }

        public enum DataBatchBizType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("车险")]
            CarIns = 1
        }

        public enum DataBatchSoureType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("文件")]
            File = 1
        }

        public enum DataBatchStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("等待处理")]
            WaitHandle = 1,
            [Remark("数据处理中")]
            Handling = 2,
            [Remark("完成")]
            Complete = 3,
        }


    }
}
