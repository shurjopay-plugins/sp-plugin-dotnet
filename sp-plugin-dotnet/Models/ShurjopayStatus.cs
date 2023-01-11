using System.Numerics;
using System.Text.Json.Serialization;
namespace Shurjopay.Models
{
    public abstract class ShurjopayStatus
    {
        protected  string SP_SUCCESS = "200";
        protected  string SP_PAYMENT_SUCCESS = "1000";

        [JsonPropertyName("sp_code")]
        public string? SpCode { get; set; }
        [JsonPropertyName("message")]
        public string? SpMessage { get; set; }
        [JsonPropertyName("massage")]
        public string? SpMassage { get; set; }
        abstract public bool IsSuccess();
    }
}
 