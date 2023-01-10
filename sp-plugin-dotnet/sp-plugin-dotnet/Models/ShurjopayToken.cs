using Shurjopay.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Transactions;

namespace Shurjopay.Plugin.Models
{
    public class ShurjopayToken:ShurjopayStatus
    {
        [JsonPropertyName("token")]
        public string? Token { get; set; }
        [JsonPropertyName("store_id")]
        public int? StoreId { get; set; }
        [JsonPropertyName("execute_url")] 
        public string? SpPaymentApi { get; set; }
        [JsonPropertyName("token_type")]
        public string? TokenType { get; set; }
        [JsonPropertyName("token_create_time")]
        public string? TokenCreatedTime { get; set; }
        [JsonPropertyName("expires_in")]
        public int? ExpiredTimeInSecond { get; set; }
        /// <summary>
        /// Check if the Shurjopay token is valid not not
        /// </summary>
        /// <returns>true if the token is valid else false</returns>
        public override bool IsSuccess()
        {
            return !string.IsNullOrEmpty(SpCode) && SpCode == SP_SUCCESS;
        }
    }
}