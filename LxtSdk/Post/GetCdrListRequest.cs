﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LxtSdk
{
    public class GetCdrListRequest
    {
        public GetCdrListRequest(GetCdrListRequestData postdata)
        {
            this.PostData = postdata;
        }


        public Object PostData { get; set; }

        public string ApiUrl
        {
            get
            {
                return "http://39.108.86.40:80/openapi/V2.0.6/GetCdrCall";
            }
        }
    }
}
