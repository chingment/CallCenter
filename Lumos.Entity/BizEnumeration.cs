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
            [Remark("车险订单")]
            Order2CarIns = 2
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

        public enum DataBizType
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

        public enum ObBatchAllocateMode
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("随机分配")]
            Random = 1,
            [Remark("筛选数据")]
            Filter = 2,
        }

        public enum CarKindInputType
        {
            Unknow = 0,
            None = 1,
            Text = 2,
            DropDownList = 3
        }
        public enum CarKindType
        {
            Unknow = 0,
            Compulsory = 1,
            Commercial = 2,
            AdditionalRisk = 3
        }


        public enum DataRightType
        {
            Unknow = 0,
            Self = 1,
            Organization = 2
        }

        public enum OrderType
        {
            Unknow = 0,
            [Remark("普通商品")]
            Goods = 1,
            [Remark("保险产品")]
            Insure = 2,
            [Remark("业务")]
            Biz = 3,
            [Remark("车险订单")]
            CarIns = 300001,
        }

        public enum OrderPayWay
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("POS")]
            POS = 1,
            [Remark("微信")]
            Wechat = 2,
            [Remark("支付宝")]
            Alipay = 3,
            [Remark("现金")]
            Cash = 4,
        }

        public enum OrderStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("已提交")]
            Submitted = 1,
            [Remark("跟进中")]
            Follow = 2,
            [Remark("待支付")]
            WaitPay = 3,
            [Remark("已完成")]
            Completed = 4,
            [Remark("已取消")]
            Cancled = 5
        }

        public enum OrderFollowStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("待核保")]
            CarInsWtUnderwrie = 6201,
            [Remark("核保中")]
            CarInsInUnderwrie = 6202,
            [Remark("已核保")]
            CarInsAlUnderwrie = 6203
        }

        public enum OrganizationType
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("呼叫中心")]
            Center = 97,
            [Remark("营业处")]
            CenterBizAddress = 98,
            [Remark("营业组")]
            CenterBizGroup = 99
        }

    }
}
