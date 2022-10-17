using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.shurjopay.plugin.Models
{
    internal class PaymentRes
    {
        [JsonPropertyName("checkout_url")]
        public string? PaymentUrl { get; set; }

      
        public string? Amount { get; set; }
        public string? Currency { get; set; }

        [JsonPropertyName("sp_order_id")]
        public string? SpOrderId { get; set; }

        [JsonPropertyName("customer_order_id")]
        public string? CustomerOrderId { get; set; }

        [JsonPropertyName("customer_name")]
        public string? CustomerName { get; set; }

        [JsonPropertyName("customer_address")]

        public string? CustomerAddress { get; set; }

        [JsonPropertyName("customer_phone")]
        public string? CustomerPhone { get; set; }

        [JsonPropertyName("customer_city")]
        public string? CustomerCity { get; set; }

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

        /** This field is used for presenting payment category. e.g. sale */

        [JsonPropertyName("intent")]
        public string? PaymentCategory { get; set; }

        /** Transaction status of shurjoPay. e.g. Initiated, Failed, Canceled */
        public string? TransactionStatus { get; set; }

        [JsonPropertyName("sp_code")]
        public int? SpCode { get; set; }
        public string? Message { get; set; }
    }
}
