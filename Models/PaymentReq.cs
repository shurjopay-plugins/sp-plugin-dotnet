using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.shurjopay.plugin.Models
{
    internal class PaymenReq
    {
        [JsonPropertyName("token")]
        public string? AuthToken { get; set; }

        [JsonPropertyName("return_url")]
        public string? ReturnUrl { get; set; }

        [JsonPropertyName("cancel_url")]
        public string? CancelUrl { get; set; }

        [JsonPropertyName("store_id")]
        public int? StoreId { get; set; }

        public double? Amount { get; set; }

        [JsonPropertyName("order_id")]
        public string? OrderId { get; set; }
        public string? Currency { get; set; }

        [JsonPropertyName("customer_name")]
        public string? CustomerName { get; set; }

        [JsonPropertyName("customer_address")]
        public string? CustomerAddress { get; set; }

        [JsonPropertyName("customer_phone")]
        public string? CustomerPhone { get; set; }

        [JsonPropertyName("customer_city")]
        public string? CustomerCity { get; set; }

        [JsonPropertyName("customer_post_code")]
        public string? CustomerPostCode { get; set; }

        [JsonPropertyName("customer_email")]
        public string? CustomerEmail { get; set; }

        /** Shipping related fields are used to get information of Ecommerce's transactions */

        [JsonPropertyName("shipping_address")]
        public string? ShippingAddress { get; set; }

        [JsonPropertyName("shipping_city")]
        public string? ShippingCity { get; set; }

        [JsonPropertyName("shipping_country")]
        public string? ShippingCountry { get; set; }

        [JsonPropertyName("received_person_name")]
        public string? ShippingReceiverName { get; set; }

        [JsonPropertyName("shipping_phone_number")]
        public string? ShippingPhone { get; set; }

        [JsonPropertyName("client_ip")]
        public string? ClientIp { get; set; }
    }
}