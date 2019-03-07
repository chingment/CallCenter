using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LxtSdk
{
    public interface IApiPostRequest<T>
    {
        string ApiUrl { get; }

        Object PostData { get; set; }

        string Customer { get; set; }
        string Password { get; set; }
    }
}
