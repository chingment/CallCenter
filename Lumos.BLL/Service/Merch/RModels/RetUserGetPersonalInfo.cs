using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lumos.BLL.Service.Merch
{
    public class RetUserGetPersonalInfo
    {

        private string userName = "";
        private string fullName = "";
        private string email = "";
        private string phoneNumber = "";
        private string organizationName = "";
        private string positionName = "";
        private string teleSeatAccount = "";
        private string teleSeatPassword = "";

        public string UserName
        {
            get
            {
                return userName;
            }
            set
            {
                userName = value;
            }
        }

        public string FullName
        {
            get
            {
                return fullName;
            }
            set
            {
                fullName = value;
            }
        }
        public string Email
        {
            get
            {
                return email;
            }
            set
            {
                email = value;
            }
        }
        public string PhoneNumber
        {
            get
            {
                return phoneNumber;
            }
            set
            {
                phoneNumber = value;
            }
        }
        public string OrganizationName
        {
            get
            {
                return organizationName;
            }
            set
            {
                organizationName = value;
            }
        }
        public string PositionName
        {
            get
            {
                return positionName;
            }
            set
            {
                positionName = value;
            }
        }
        public string TeleSeatAccount
        {
            get
            {
                return teleSeatAccount;
            }
            set
            {
                teleSeatAccount = value;
            }
        }
        public string TeleSeatPassword
        {
            get
            {
                return teleSeatPassword;
            }
            set
            {
                teleSeatPassword = value;
            }
        }
    }
}
