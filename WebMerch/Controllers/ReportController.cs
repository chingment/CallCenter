using Lumos;
using Lumos.Entity;
using System;
using System.Collections.Generic;
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

    public class ReportController : Controller
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
    }
}