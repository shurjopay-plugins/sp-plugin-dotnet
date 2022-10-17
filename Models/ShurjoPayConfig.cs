using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.shurjopay.plugin.Models
{
    internal class ShurjoPayConfig
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string CallbackUrl { get; set; }
        public string ApiBaseUrl { get; set; }
    }
}
