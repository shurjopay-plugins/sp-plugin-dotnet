using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sp_plugin_dotnet.Models
{
    internal class ShurjopayConfig
    {
        public string? username { get; set; }
        public string? password { get; set; }
        public string? callbackUrl { get; set; }
        public string? apiBaseUrl { get; set; }
    }
}
