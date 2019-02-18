using System.Web.Mvc;
using Lumos.Web.Mvc;
using Lumos.DAL;

namespace WebSSO
{

    /// <summary>
    /// BaseController用来扩展Controller,凡是在都该继承BaseController
    /// </summary>
    //[OwnException]
    [ValidateInput(false)]
    public abstract class OwnBaseController : BaseController
    {

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }


        private LumosDbContext _currentDb;

        public LumosDbContext CurrentDb
        {
            get
            {
                if (_currentDb == null)
                {
                    _currentDb = new LumosDbContext();
                }

                return _currentDb;
            }
        }
    }
}