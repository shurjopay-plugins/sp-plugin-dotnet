using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace com.shurjopay.plugin.Models
{
    internal class ShurjoPayToken
    {
        public string Token { get; set; }

        [JsonPropertyName("execute_url")]
        public string SpPaymentApi { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("sp_code")]
        public string SpStatusCode { get; set; }

        [JsonPropertyName("token_create_time")]
        public string TokenCreatedTime { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiredTimeInSecond { get; set; }

        [JsonPropertyName("store_id")]
        public int StoreId { get; set; }

        public string Message { get; set; }
    }
}
