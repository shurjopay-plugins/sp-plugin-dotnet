using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Shurjopay.Plugin.Models
{
    public class VerifiedPayment
    {
        [JsonPropertyName("id")]
        public int? Id { get; set; }

        [JsonPropertyName("order_id")]
        public string? OrderId { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("amount")]
        public double? Amount { get; set; }

        [JsonPropertyName("payable_amount")]
        public double? PayableAmount { get; set; }

        [JsonPropertyName("discsount_amount")]
        public double? DiscountAmount { get; set; }

        [JsonPropertyName("disc_percent")]
        public double? Discpercent { get; set; }

        [JsonPropertyName("usd_amt")]
        public double? UsdAmt { get; set; }

        [JsonPropertyName("usd_rate")]
        public double? UsdRate { get; set; }

        [JsonPropertyName("recived_amount")]
        public double? ReceivedAmt { get; set; }

        [JsonPropertyName("card_holder_name")]
        public string? CardHolder { get; set; }

        [JsonPropertyName("card_number")]
        public string? CardNumber { get; set; }

        [JsonPropertyName("phone_no")]
        public string? PhoneNo { get; set; }

        [JsonPropertyName("bank_trx_id")]
        public string? BankTxnId { get; set; }

        [JsonPropertyName("invoice_no")]
        public string? InvoiceNo { get; set; }

        [JsonPropertyName("bank_status")]
        public string? BankStatus { get; set; }

        [JsonPropertyName("customer_order_id")]
        public string? CustomerOrderId { get; set; }

        [JsonPropertyName("sp_code")]
        public int? SpStatusCode { get; set; }

        [JsonPropertyName("sp_massage")]
        public string? SpStatusMsg { get; set; }

        [JsonPropertyName("name")]
        public string? CustomerName { get; set; }

        [JsonPropertyName("email")]
        public string? CustomerEmail { get; set; }

        [JsonPropertyName("address")]
        public string? CustomerAddress { get; set; }

        [JsonPropertyName("city")]
        public string? CustomerCity { get; set; }

        /** Sometime customer have to send additional data like studentId 
	    * or any other information which have not any field given by shurjoPay.
	    * value1, value2, value3, value4 is used for customer's additional info if needed
	    */

        [JsonPropertyName("value1")]
        public string? Value1 { get; set; }

        [JsonPropertyName("value2")]
        public string? Value2 { get; set; }

        [JsonPropertyName("value3")]
        public string? Value3 { get; set; }

        [JsonPropertyName("value4")]
        public string? Value4 { get; set; }

        [JsonPropertyName("transaction_status")]
        public string? TxnStatus { get; set; }

        [JsonPropertyName("method")]
        public string? PaymentMethod { get; set; }

        [JsonPropertyName("date_time")]
        public string? TxnTime { get; set; }
    }
}
