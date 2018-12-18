using Lumos;
using Lumos.BLL.Service.Merch;
using Lumos.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebMerch.Controllers
{
    public class CarInsController : OwnBaseController
    {
        public ActionResult ListByDealtUnderwritingOrder()
        {
            return View();
        }

        public ActionResult DealtUnderwritingOrder()
        {
            return View();
        }


        public CustomJsonResult GetListByDealtUnderwritingOrder(RupGetListByHandleUnderwritingOrder rup)
        {

            var accessUserIds = MerchServiceFactory.User.GetCanAccessUserIds(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId);

            var waitCount = (from h in CurrentDb.Order2CarIns where h.MerchantId == this.CurrentMerchantId && h.FollowStatus == Lumos.Entity.Enumeration.OrderFollowStatus.CarInsWtUnderwrie && accessUserIds.Contains(h.SalesmanId) select h.Id).Count();
            var inCount = (from h in CurrentDb.Order2CarIns where h.MerchantId == this.CurrentMerchantId && h.FollowStatus == Lumos.Entity.Enumeration.OrderFollowStatus.CarInsInUnderwrie && accessUserIds.Contains(h.SalesmanId) select h.Id).Count();

            var query = (from u in CurrentDb.Order2CarIns

                         where u.MerchantId == this.CurrentMerchantId
                       && accessUserIds.Contains(u.SalesmanId)

                         select new { u.Id, u.Sn, u.CarOwner, u.CarPlateNo, u.CarOwnerIdNumber, u.CarOwnerAddress, u.CarOwnerPhoneNumber, u.CreateTime, u.FollowStatus, u.SalesmanName, u.SalesmanId, u.SubmitTime });


            if (rup.FollowStatus != Enumeration.OrderFollowStatus.Unknow)
            {
                query = query.Where(m => m.FollowStatus == rup.FollowStatus);
            }

            int total = query.Count();

            int pageIndex = rup.PageIndex;
            int pageSize = 10;
            query = query.OrderByDescending(r => r.CreateTime).Skip(pageSize * (pageIndex)).Take(pageSize);

            var list = query.ToList();

            List<object> olist = new List<object>();

            foreach (var item in list)
            {
                olist.Add(new
                {
                    Id = item.Id,
                    Sn = item.Sn,
                    CarOwner = item.CarOwner,
                    CarPlateNo = item.CarPlateNo,
                    CarOwnerIdNumber = item.CarOwnerIdNumber,
                    CarOwnerAddress = item.CarOwnerAddress,
                    CarOwnerPhoneNumber = item.CarOwnerPhoneNumber,
                    SalerName = "",
                    FollowStatus = item.FollowStatus,
                    SubmitTime = item.SubmitTime.ToUnifiedFormatDateTime(),
                    CreateTime = item.CreateTime.ToUnifiedFormatDateTime(),
                    SalesmanName = item.SalesmanName,
                    SalesmanId = item.SalesmanId
                });
            }


            PageEntity pageEntity = new PageEntity { PageSize = pageSize, TotalRecord = total, Rows = list, Status = new { waitCount = waitCount, inCount = inCount } };


            return Json(ResultType.Success, pageEntity, "");
        }

        public CustomJsonResult GetDealtUnderwritingOrderDetails(string orderId)
        {
            return MerchServiceFactory.CarIns.GetDealtUnderwritingOrderDetails(this.CurrentUserId, this.CurrentMerchantId,this.CurrentUserId, orderId);
        }

        [HttpPost]
        public CustomJsonResult DealtUnderwritingOrder(RopCarInsDealtUnderwritingOrder rop)
        {
            return MerchServiceFactory.CarIns.DealtUnderwritingOrder(this.CurrentUserId, this.CurrentMerchantId, this.CurrentUserId, rop);
        }

    }
}