using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace sp_plugin_dotnet.Models
{
    public class PaymentDetails
    {
        [JsonPropertyName("checkout_url")]
        public string? CheckOutUrl { get; set; }

        [JsonPropertyName("amount")]
        public float? Amount { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("sp_order_id")]
        public string? SpOrderId { get; set; }

        [JsonPropertyName("customer_order_id")]
        public string? CustomerOrderId { get; set; }

        [JsonPropertyName("customer_name")]
        public string? CustomerName { get; set; }

        [JsonPropertyName("customer_address")]
        public string? CustomerAddress { get; set; }

        [JsonPropertyName("customer_city")]
        public string? CustomerCity { get; set; }

        [JsonPropertyName("customer_phone")]
        public string? CustomerPhone { get; set; }

        [JsonPropertyName("customer_email")]
        public string? CustomerEmail { get; set; }

        [JsonPropertyName("client_ip")]
        public string? ClientIp { get; set; }

        [JsonPropertyName("intent")]
        public string? Intent { get; set; }

        [JsonPropertyName("transactionStatus")]
        public string? TransactionStatus { get; set; }
    }
}
