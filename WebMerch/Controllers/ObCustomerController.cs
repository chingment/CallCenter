using Lumos;
using Lumos.BLL.Service.Merch;
using Lumos.Common;
using Lumos.Entity;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;

namespace WebMerch.Controllers
{
    public class ObCustomerController : OwnBaseController
    {
        public ActionResult ListByCommon()
        {
            return View();
        }

        public ActionResult ListByCarIns()
        {
            return View();
        }

        public Expression<Func<ObCustomer, bool>> predicate()
        {

            Expression<Func<ObCustomer, bool>> predicate = null;


            predicate = f => f.BelongerId == "";

            return predicate;
        }

        public CustomJsonResult GetListByCarIns(RupObCustomerGetList rup)
        {
            var accessUserIds = MerchServiceFactory.User.GetCanAccessUserIds(this.CurrentMerchantId, this.CurrentUserId);

            var query = (from u in CurrentDb.ObCustomer
                         where

                         u.MerchantId == this.CurrentMerchantId &&
                           accessUserIds.Contains(u.BelongerId) &&
                         (rup.CarPlateNo == null || u.CarPlateNo.Contains(rup.CarPlateNo)) &&
                         (rup.CarModel == null || u.CarModel.Contains(rup.CarModel)) &&
                         (rup.CarEngineNo == null || u.CarEngineNo.Contains(rup.CarEngineNo)) &&
                         (rup.CsrAddress == null || u.CsrAddress.Contains(rup.CsrAddress)) &&
                         (rup.CsrPhoneNumber == null || u.CsrPhoneNumber.Contains(rup.CsrPhoneNumber)) &&
                         (rup.CsrName == null || u.CsrName.Contains(rup.CsrName)) &&
                         (rup.CsrIdNumber == null || u.CsrIdNumber.Contains(rup.CsrIdNumber)) &&
                         (rup.CarInsLastCompany == null || u.CarInsLastCompany.Contains(rup.CarInsLastCompany)) &&
                         (rup.CarRegisterDateStart == null || u.CarRegisterDate >= rup.CarRegisterDateStart) &&
                         (rup.CarRegisterDateEnd == null || u.CarRegisterDate <= rup.CarRegisterDateEnd) &&
                         (rup.CarInsLastStartTime == null || u.CarInsLastStartTime >= rup.CarInsLastStartTime) &&
                         (rup.CarInsLastEndTime == null || u.CarInsLastEndTime <= rup.CarInsLastEndTime)
                          &&
                          u.RecoveryTime <= DateTime.Now
                         select new { u.Id, u.CsrName, u.CsrPhoneNumber, u.CsrAddress, u.CsrIdNumber, u.CarRegisterDate, u.CarPlateNo, u.CarModel, u.CarEngineNo, u.CarVin, u.CarInsLastQzNo, u.CarInsLastSyNo, u.CarInsLastCompany, u.CarInsLastStartTime, u.CarInsLastEndTime, u.CreateTime });

            // var s = predicate();
            // query = query.Where();

            int total = query.Count();

            int pageIndex = rup.PageIndex;
            int pageSize = rup.PageSize;
            query = query.OrderByDescending(r => r.Id).Skip(pageSize * (pageIndex)).Take(pageSize);


            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {

                olist.Add(new
                {
                    Id = item.Id,
                    CsrName = item.CsrName,
                    CsrPhoneNumber = item.CsrPhoneNumber,
                    CsrAddress = item.CsrAddress,
                    CsrIdNumber = item.CsrIdNumber,
                    CarRegisterDate = item.CarRegisterDate.ToUnifiedFormatDate(),
                    CarPlateNo = item.CarPlateNo,
                    CarModel = item.CarModel,
                    CarEngineNo = item.CarEngineNo,
                    CarVin = item.CarVin,
                    CarInsLastQzNo = item.CarInsLastQzNo,
                    CarInsLastCompany = item.CarInsLastCompany,
                    CarInsLastStartTime = item.CarInsLastStartTime.ToUnifiedFormatDate(),
                    CarInsLastEndTime = item.CarInsLastEndTime.ToUnifiedFormatDate(),
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }

        public CustomJsonResult GetListByCommon(RupObCustomerGetList rup)
        {
            var accessUserIds = MerchServiceFactory.User.GetCanAccessUserIds(this.CurrentMerchantId, this.CurrentUserId);

            var query = (from u in CurrentDb.ObCustomer
                         where

                         u.MerchantId == this.CurrentMerchantId &&
                           accessUserIds.Contains(u.BelongerId) &&

                         (rup.CsrPhoneNumber == null || u.CsrPhoneNumber.Contains(rup.CsrPhoneNumber)) &&
                         (rup.CsrName == null || u.CsrName.Contains(rup.CsrName)) &&
                         (rup.CsrCompany == null || u.CsrCompany.Contains(rup.CsrCompany))
                                                   &&
                          u.RecoveryTime <= DateTime.Now

                         select new { u.Id, u.CsrName, u.CsrPhoneNumber, u.CsrAddress, u.CsrCompany, u.CreateTime });

            // var s = predicate();
            // query = query.Where();

            int total = query.Count();

            int pageIndex = rup.PageIndex;
            int pageSize = rup.PageSize;
            query = query.OrderByDescending(r => r.Id).Skip(pageSize * (pageIndex)).Take(pageSize);


            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {

                olist.Add(new
                {
                    Id = item.Id,
                    CsrName = item.CsrName,
                    CsrPhoneNumber = item.CsrPhoneNumber,
                    CsrAddress = item.CsrAddress,
                    CsrCompany = item.CsrCompany,
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime()
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = olist };

            return Json(ResultType.Success, pageEntity, "");
        }
    }
}