﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Admin
{
    public class AdminServiceFactory : BaseFactory
    {
        public static SysOrganizationProvider SysOrganization
        {
            get
            {
                return new SysOrganizationProvider();
            }
        }

        public static SysPositionProvider SysPosition
        {
            get
            {
                return new SysPositionProvider();
            }
        }

        public static MerchantProvider Merchant
        {
            get
            {
                return new MerchantProvider();
            }
        }

        public static ObBatchProvider ObBatch
        {
            get
            {
                return new ObBatchProvider();
            }
        }

        public static SysUserProvider SysUser
        {
            get
            {
                return new SysUserProvider();
            }
        }

        public static SysAdminUserProvider SysAdminUser
        {
            get
            {
                return new SysAdminUserProvider();
            }
        }

        public static SysRoleProvider SysRole
        {
            get
            {
                return new SysRoleProvider();
            }
        }

        public static SysMenuProvider SysMenu
        {
            get
            {
                return new SysMenuProvider();
            }
        }

        public static SysItemCacheUpdateTimeProvider SysItemCacheUpdateTime
        {
            get
            {
                return new SysItemCacheUpdateTimeProvider();
            }
        }

        public static AuthorizeRelayProvider AuthorizeRelay
        {
            get
            {
                return new AuthorizeRelayProvider();
            }
        }

        public static BackgroundJobProvider BackgroundJob
        {
            get
            {
                return new BackgroundJobProvider();
            }
        }
    }
}
