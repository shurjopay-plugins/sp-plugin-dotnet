namespace Shurjopay.Plugin.Models
{
    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public string OrderId { get; set; }
        public string Currency { get; set; }
        public string CustomerName { get; set; }
        public string CustomerAddress { get; set; }
        public string CustomerPhone { get; set; }
        public string CustomerCity { get; set; }
        public string CustomerPostCode { get; set; }

    }
}
