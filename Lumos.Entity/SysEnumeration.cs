﻿
using System;

namespace Lumos.Entity
{

    /// <summary>
    /// 系统的枚举
    /// </summary>
    public partial class Enumeration
    {
        public enum SmsSendResult
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("成功")]
            Success = 1,
            [Remark("失败")]
            Failure = 2,
            [Remark("异常")]
            Exception = 2,
        }

        public enum AppType
        {
            Unknow = 0,
            NativeApp = 1,
            PublicNumber = 2,
            MinProgram = 3
        }

        public enum UserStatus
        {
            Unknow = 0,
            [Remark("正常")]
            Normal = 1,
            [Remark("禁用")]
            Disable = 2
        }


        public enum BelongSite
        {
            Unknow = 0,
            [Remark("管理端")]
            Admin = 1,
            [Remark("商户端")]
            Merchant = 2,
            [Remark("客户端")]
            Client = 3,
        }

        public enum LoginType
        {
            Unknow = 0,
            Website = 1,
            AndroidApp = 2,
            IosApp = 3,
            Wechat = 4
        }

        public enum LoginResult
        {

            Unknow = 0,
            Success = 1,
            Failure = 2
        }

        public enum LoginResultTip
        {
            Unknow = 0,
            VerifyPass = 1,
            UserNotExist = 2,
            UserPasswordIncorrect = 3,
            UserDisabled = 4,
            UserDeleted = 5,
            UserAccessFailedMaxCount = 6
        }

        public enum InputType
        {
            CheckBox = 0,
            Hidden = 1,
            Password = 2,
            Radio = 3,
            Text = 4,
            Select = 5
        }

        public enum OperateType
        {
            /// <summary>
            /// 未知
            /// </summary>
            Unknow = 0,
            /// <summary>
            /// 新增
            /// </summary>
            New = 1,
            /// <summary>
            /// 修改
            /// </summary>
            Update = 2,
            /// <summary>
            /// 删除
            /// </summary>
            Delete = 3,
            /// <summary>
            /// 暂存
            /// </summary>
            Save = 4,
            /// <summary>
            /// 确定
            /// </summary>
            Submit = 5,
            /// <summary>
            /// 通过
            /// </summary>
            Pass = 6,
            /// <summary>
            /// 暂存
            /// </summary>
            Reject = 7,
            /// <summary>
            /// 拒绝
            /// </summary>
            Refuse = 8,
            /// <summary>
            /// 取消
            /// </summary>
            Cancle = 9,

            /// <summary>
            /// 查询
            /// </summary>
            Serach = 101,
            /// <summary>
            /// 导出Excel
            /// </summary>
            ExportExcel = 102
        }

        public enum SysItemCacheType
        {
            Unknow = 0,
            User = 1
        }

        public enum BackgroundJobStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("停止")]
            Stoped = 1,
            [Remark("运行中")]
            Runing = 2,
            [Remark("启动中")]
            Starting = 3,
            [Remark("停止中")]
            Stoping = 4
        }
        public enum SysOrganizationStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("正常")]
            Valid = 1,
            [Remark("停用")]
            Invalid = 2
        }


        //1开头的为管理端系统职位，2开头为商户端系统职位
        public enum SysPositionId
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("管理员")]
            AdminAdministrator = 100,
            [Remark("初级运营")]
            AdminJuniorOperators = 101,


            [Remark("管理员")]
            MerchantAdministrator = 200,
            [Remark("总部负责人")]
            MerchantGM = 201,
            [Remark("场地负责人")]
            MerchantUM = 202,
            [Remark("小组负责人")]
            MerchantTL = 203,
            [Remark("电话外呼人员")]
            MerchantTSR = 204,
            [Remark("后勤负责人")]
            MerchantHL = 205,
            [Remark("后勤人员")]
            MerchantHQR = 206,
            [Remark("数据导入员")]
            MerchantDBA = 207,
            [Remark("普通人员")]
            MerchantStaff = 299
        }

        public enum WorkStatus
        {
            [Remark("未知")]
            Unknow = 0,
            [Remark("工作中")]
            OnLine = 1,
            [Remark("离线")]
            OffLine = 2
        }

        public enum TelePhoneStatus
        {

            [Remark("未知")]
            Unknow = 0,
            [Remark("空闲")]
            IDLE = 1,
            [Remark("呼出接通")]
            CallOut = 2,
            [Remark("呼入接通")]
            CallIn = 3,
            [Remark("振铃")]
            Ringing = 4,
            [Remark("整理中")]
            Process = 5
        }
    }
}
