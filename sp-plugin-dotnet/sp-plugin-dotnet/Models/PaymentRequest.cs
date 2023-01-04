using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shurjopay.Plugin.Models
{
 
    public class PaymentRequest
    {

        [JsonPropertyName("prefix")]
        public string Prefix { get; set; }

        [JsonPropertyName("amount")]
        public double Amount { get; set; }

        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }

        [JsonPropertyName("currency")]
        public string Currency { get; set; }

        [JsonPropertyName("customer_name")]
        public string CustomerName { get; set; }

        [JsonPropertyName("customer_address")]
        public string CustomerAddress { get; set; }

        [JsonPropertyName("customer_phone")]
        public string CustomerPhone { get; set; }

        [JsonPropertyName("customer_city")]
        public string CustomerCity { get; set; }

        [JsonPropertyName("customer_post_code")]
        public string CustomerPostCode { get; set; }
        
    }
}
