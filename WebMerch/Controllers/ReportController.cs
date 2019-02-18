﻿using Lumos;
using Lumos.DAL;
using Lumos.Entity;
using Lumos.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
namespace WebMerch.Controllers
{
    public class ReportModel
    {
        public Enumeration.OperateType Operate { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public string TableHtml { get; set; }
    }

    public class ReportTable
    {
        public ReportTable()
        {

        }

        public ReportTable(string html)
        {
            this.Html = html;
        }

        public string Html
        {
            get;
            set;
        }
    }

    public class ReportController : OwnBaseController
    {
        public ActionResult TakeData(ReportModel model)
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.Append("<table class='list-tb' cellspacing='0' cellpadding='0'>");
            sbTable.Append("<thead>");
            sbTable.Append("<tr>");
            sbTable.Append("<th>序号</th>");
            sbTable.Append("<th>机构</th>");
            sbTable.Append("<th>账号</th>");
            sbTable.Append("<th>姓名</th>");
            sbTable.Append("<th>任务量</th>");
            sbTable.Append("<th>已取</th>");
            sbTable.Append("<th>未取</th>");
            sbTable.Append("</tr>");
            sbTable.Append("</thead>");
            sbTable.Append("<tbody>");
            sbTable.Append("{content}");
            sbTable.Append("</tbody>");
            sbTable.Append("</table>");

            if (Request.HttpMethod == "GET")
            {
                #region GET
                sbTable.Replace("{content}", "<tr><td colspan=\"7\"></td></tr>");

                model.TableHtml = sbTable.ToString();
                return View(model);

                #endregion
            }
            else
            {
                #region POST


                return Json(ResultType.Success, "");
                #endregion
            }
        }


        public ActionResult TeleSeatAccount(ReportModel model)
        {
            StringBuilder sbTable = new StringBuilder();
            sbTable.Append("<table class='list-tb' cellspacing='0' cellpadding='0'>");
            sbTable.Append("<thead>");
            sbTable.Append("<tr>");
            sbTable.Append("<th>序号</th>");
            sbTable.Append("<th>话机账号</th>");
            sbTable.Append("<th>话机口令</th>");
            sbTable.Append("<th>域名</th>");
            sbTable.Append("<th>电话号码</th>");
            sbTable.Append("</tr>");
            sbTable.Append("</thead>");
            sbTable.Append("<tbody>");
            sbTable.Append("{content}");
            sbTable.Append("</tbody>");
            sbTable.Append("</table>");

            if (Request.HttpMethod == "GET")
            {
                #region GET
                sbTable.Replace("{content}", "<tr><td colspan=\"5\"></td></tr>");

                model.TableHtml = sbTable.ToString();
                return View(model);

                #endregion
            }
            else
            {
                #region POST
                StringBuilder sql = new StringBuilder(" select  Account,[Password], Domain,PhoneNumber from [dbo].[TeleSeat] ");

                sql.Append(" where 1=1 and MerchantId='" + this.CurrentMerchantId + "' ");


                sql.Append(" order by [Account] asc  ");


                DataTable dtData = DatabaseFactory.GetIDBOptionBySql().GetDataSet(sql.ToString()).Tables[0].ToStringDataTable();
                StringBuilder sbTableContent = new StringBuilder();
                for (int r = 0; r < dtData.Rows.Count; r++)
                {
                    sbTableContent.Append("<tr>");

                    sbTableContent.Append("<td>" + (r + 1) + "</td>");

                    for (int c = 0; c < dtData.Columns.Count; c++)
                    {
                        string td_value = "";

                        switch (c)
                        {
                            default:
                                td_value = dtData.Rows[r][c].ToString().Trim();
                                break;
                        }

                        sbTableContent.Append("<td>" + td_value + "</td>");

                    }

                    sbTableContent.Append("</tr>");
                }

                sbTable.Replace("{content}", sbTableContent.ToString());

                ReportTable reportTable = new ReportTable(sbTable.ToString());

                if (model.Operate == Enumeration.OperateType.Serach)
                {
                    return Json(ResultType.Success, reportTable, "");
                }
                else
                {
                    NPOIExcelHelper.HtmlTable2Excel(reportTable.Html, "话机号码");

                    return Json(ResultType.Success, "");
                }
                #endregion
            }
        }
    }
}