using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class MerchServiceFactory : BaseFactory
    {
        public static CustomerProvider Customer
        {
            get
            {
                return new CustomerProvider();
            }
        }

        public static ObBatchProvider ObBatch
        {
            get
            {
                return new ObBatchProvider();
            }
        }

        public static ObBatchAllocateProvider ObBatchAllocate
        {
            get
            {
                return new ObBatchAllocateProvider();
            }
        }


        public static OrganizationProvider Organization
        {
            get
            {
                return new OrganizationProvider();
            }
        }


        public static UserProvider User
        {
            get
            {
                return new UserProvider();
            }
        }

        public static ObCalloutProvider ObCallout
        {
            get
            {
                return new ObCalloutProvider();
            }
        }

        public static CarInsProvider CarIns
        {
            get
            {
                return new CarInsProvider();
            }
        }

    }
}
