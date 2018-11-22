using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class MerchServiceFactory : BaseFactory
    {

        public static OrganizationProvider Organization
        {
            get
            {
                return new OrganizationProvider();
            }
        }
    

    }
}
