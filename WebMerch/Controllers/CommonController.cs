﻿using Lumos.Common;
using System;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Linq;
using System.Text;
using Lumos;
using Lumos.BLL;
using System.Collections.Generic;
using Lumos.Entity;
using Lumos.BLL.Service.Merch;

namespace WebMerch.Controllers
{

    public class CommonController : OwnBaseController
    {
        [HttpPost]
        public ActionResult CkEditorUpLoadFile()
        {
            LogUtil.Info("调用上传图片接口(CkEditorUpLoadFile)");
            string CKEditorFuncNum = Request.QueryString["CKEditorFuncNum"];
            try
            {
                HttpPostedFileBase file_upload = Request.Files["upload"];
                string type = Request.QueryString["type"].ToString();
                string fileName = file_upload.FileName;
                string fileExtension = System.IO.Path.GetExtension(fileName).ToLower();
                StringBuilder sb = new StringBuilder();
                if (fileExtension != ".jpg" && fileExtension != ".gif" && fileExtension != ".png" && fileExtension != ".bmp")
                {
                    return Content(CkEditorUpLoadCallFunction(CKEditorFuncNum, "上传的文件格式不正确,请重新选择！"));
                }

                string strUrl = System.Configuration.ConfigurationManager.AppSettings["custom:UploadServerUrl"];

                LogUtil.Info("调用上传图片接口" + strUrl);

                byte[] bytes = null;
                using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                {
                    bytes = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                }
                string fileExt = Path.GetExtension(Request.Files[0].FileName).ToLower();
                UploadFileEntity entity = new UploadFileEntity();
                entity.FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + fileExt;//自定义文件名称，这里以当前时间为例
                entity.FileData = bytes;
                entity.UploadFolder = "CkEditorFile";
                entity.GenerateSize = false;

                CustomJsonResult rm = HttpClientOperate.Post<CustomJsonResult>("", strUrl, entity);//封装的POST提交方
                if (rm.Result == ResultType.Exception || rm.Result == ResultType.Unknown)
                {
                    rm.Message = "上传图片发生异常";
                    LogUtil.Error("调用api上传图片失败");

                    return Content(CkEditorUpLoadCallFunction(CKEditorFuncNum, "远程上传图片发生异常"));
                }

                ImageUpload imageUpload = rm.Data.ToJsonObject<ImageUpload>();

                return Content(CkEditorUpLoadCallFunction(CKEditorFuncNum, "", imageUpload.OriginalPath));
            }
            catch (Exception ex)
            {
                LogUtil.Error("调用上传图片接口(CkEditorUpLoadFile)", ex);
                return Content(CkEditorUpLoadCallFunction(CKEditorFuncNum, ex.Message));
            }
        }

        private string CkEditorUpLoadCallFunction(string CKEditorFuncNum, string message, string imageurl = null)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("<script type=\"text/javascript\">");
            sb.Append("window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ",'" + imageurl + "','" + message + "')");
            sb.Append("</script>");
            return sb.ToString();
        }

        [HttpPost]
        public CustomJsonResult UploadImage(string fileinputname, string path, string oldfilename, bool generateSize)
        {

            CustomJsonResult rm = new CustomJsonResult();
            //rm.ContentType = "text/html";
            try
            {
                LogUtil.Info("调用上传图片接口");

                HttpPostedFileBase file_upload = Request.Files[fileinputname];

                if (file_upload == null)
                    return Json("text/html", ResultType.Failure, "找不到上传的对象");

                if (file_upload.ContentLength == 0)
                    return Json("text/html", ResultType.Failure, "文件内容为空,请重新选择");

                if (file_upload.ContentLength > (10 * 1024 * 1024))
                {
                    return Json("text/html", ResultType.Failure, "图片大小不能超过10M,请重新选择");
                }


                System.IO.FileInfo file = new System.IO.FileInfo(file_upload.FileName);
                string ext = file.Extension.ToLower();
                if (ext != ".jpg" && ext != ".jpeg" && ext != ".png" && ext != ".gif" && ext != ".bmp")
                {
                    return Json("text/html", ResultType.Failure, "上传的文件不是图片格式(jpg,png,gif,bmp)");
                }


                string strUrl = System.Configuration.ConfigurationManager.AppSettings["custom:UploadServerUrl"];
                LogUtil.Info("调用上传图片接口" + strUrl);

                byte[] bytes = null;
                using (var binaryReader = new BinaryReader(Request.Files[0].InputStream))
                {
                    bytes = binaryReader.ReadBytes(Request.Files[0].ContentLength);
                }
                string fileExt = Path.GetExtension(Request.Files[0].FileName).ToLower();
                UploadFileEntity entity = new UploadFileEntity();
                entity.FileName = DateTime.Now.ToString("yyyyMMddHHmmss") + fileExt;//自定义文件名称，这里以当前时间为例
                entity.FileData = bytes;
                entity.UploadFolder = path;
                entity.GenerateSize = generateSize;
                rm = HttpClientOperate.Post<CustomJsonResult>(path, strUrl, entity);//封装的POST提交方
                //rm.ContentType = "text/html";
                if (rm.Result == ResultType.Exception || rm.Result == ResultType.Unknown)
                {
                    rm.Message = "上传图片发生异常.";
                    LogUtil.Error("调用api上传图片失败");
                }
            }
            catch (Exception ex)
            {
                rm.Result = ResultType.Exception;
                rm.Message = "上传图片发生异常..";
                LogUtil.Error("", ex);

            }
            return rm;
        }

        /// <summary>
        /// 获取验证码的图片 使用方式 请求url:/Common/GetImgVerifyCode?name=sessionname
        /// </summary>
        /// <param name="name">代表后台session的名称</param>
        /// <returns>返回一张带数字的图片</returns>
        [AllowAnonymous]
        public ActionResult GetImgVerifyCode(string name)
        {
            VerifyCodeHelper v = new VerifyCodeHelper();
            v.CodeSerial = "0,1,2,3,4,5,6,7,8,9";
            string code = v.CreateVerifyCode(); //取随机码 
            v.CreateImageOnPage(code, ControllerContext.HttpContext);   //输出图片
            Session[name] = code;   //Session 取出验证码
            Response.End();
            return null;
        }

        public CustomJsonResult GetSelectFields(string type)
        {
            if (type == null)
                return new CustomJsonResult(ResultType.Failure, ResultCode.Failure, "类型为空");

            var result = new CustomJsonResult();



            //var sysPositions = CurrentDb.SysPosition.Where(m => m.BelongSite == Enumeration.BelongSite.Admin).ToList();

            //foreach (var item in sysPositions)
            //{
            //    ret.ConfigPositions.Add(new FieldModel(item.Name, ((int)item.Id).ToString()));
            //}

            var fields = new List<FieldModel>();

            type = type.ToLower();

            switch (type)
            {
                case "sysposition":
                    #region sysposition
                    string organizationId = Request.QueryString["organizationId"] ?? "";
                    var organization = CurrentDb.Organization.Where(m => m.Id == organizationId).FirstOrDefault();
                    string dept = organization == null ? "-1" : organization.Dept.ToString();
                    var sysPositions = CurrentDb.SysPosition.Where(m => m.BelongSite == Enumeration.BelongSite.Merchant && m.BelongOrganizationDepts.Contains(dept)).ToList();
                    foreach (var item in sysPositions)
                    {
                        fields.Add(new FieldModel(item.Name, ((int)item.Id).ToString()));
                    }
                    #endregion
                    break;
                case "sysorganization":
                    #region sysorganization
                    var sysOrganizations = MerchServiceFactory.Organization.GetSons(this.CurrentMerchantId, GuidUtil.Empty());

                    foreach (var item in sysOrganizations)
                    {
                        var field = new FieldModel();
                        field.Value = item.Id;
                        field.PValue = item.PId;
                        field.Name = item.Name;
                        field.Dept = item.Dept;
                        fields.Add(field);
                    }
                    #endregion 
                    break;
                case "callresultcode":
                    #region callresultcode
                    Enumeration.BusinessType businessType = (Enumeration.BusinessType)int.Parse(Request.QueryString["businessType"]);

                    var merchant = CurrentDb.Merchant.Where(m => m.Id == this.CurrentMerchantId).FirstOrDefault();

                    var callResultCodes = CurrentDb.CallResultCode.Where(m => m.BusinessType == merchant.BusinessType && m.Code.Length == 4).OrderBy(m => m.Priority).ToList();

                    foreach (var item in callResultCodes)
                    {
                        var field = new FieldModel();
                        field.Value = item.Code;
                        field.Name = item.Name;
                        field.Ext = new { isAllowNextCall = item.IsAllowNextCall, isInValidData = item.IsInValidData };
                        fields.Add(field);
                    }
                    #endregion 
                    break;
                case "teleseat":
                    #region TeleSeatAccounts

                    var userId = Request.QueryString["userId"];
                    List<TeleSeat> teleSeats = new List<TeleSeat>();
                    var teleSeatIds = CurrentDb.SysMerchantUser.Where(m => m.MerchantId == this.CurrentMerchantId && m.TeleSeatId != null && m.Id != userId).Select(m => m.TeleSeatId).ToArray();

                    teleSeats = CurrentDb.TeleSeat.Where(m => m.MerchantId == this.CurrentMerchantId && !teleSeatIds.Contains(m.Id)).ToList();

                    foreach (var item in teleSeats)
                    {
                        var field = new FieldModel();
                        field.Value = item.Id;
                        field.Name = item.Account;
                        fields.Add(field);
                    }
                    #endregion 
                    break;
            }



            return new CustomJsonResult(ResultType.Success, ResultCode.Success, "获取成功", fields);

        }
    }
}