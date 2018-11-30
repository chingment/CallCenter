﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class MerchServiceFactory : BaseFactory
    {

        public static DataBatchProvider DataBatch
        {
            get
            {
                return new DataBatchProvider();
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


    }
}
