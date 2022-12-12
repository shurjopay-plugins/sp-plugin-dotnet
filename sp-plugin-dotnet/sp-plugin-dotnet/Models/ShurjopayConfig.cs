using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shurjopay.Plugin.Models
{
    public class ShurjopayConfig
    {
        public string? SP_USERNAME {set;get;}
        public string? SP_PASSWORD { set; get; }
        public string? SP_ENDPOINT { set; get ; }
        public string? SP_CALLBACK { set; get; }
    }
}
