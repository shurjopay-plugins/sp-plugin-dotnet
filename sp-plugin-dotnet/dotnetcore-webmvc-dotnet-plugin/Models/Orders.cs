namespace dotnetcore_webmvc_dotnet_plugin.Models
{
    public class Orders
    {
        public int? Id { get; set; }
        public string? OrderId { get; set; }
        public string? Currency { get; set; }
        public double? Amount { get; set; }
        public double? PayableAmount { get; set; }
        public double? DiscountAmount { get; set; }
        public double? Discpercent { get; set; }
        public double? UsdAmt { get; set; }
        public double? UsdRate { get; set; }
        public double? ReceivedAmt { get; set; }
        public string? CardHolder { get; set; }
        public string? CardNumber { get; set; }
        public string? PhoneNo { get; set; }
        public string? BankTxnId { get; set; }
        public string? InvoiceNo { get; set; }
        public string? BankStatus { get; set; }
        public string? CustomerOrderId { get; set; }
        public int? SpStatusCode { get; set; }
        public string? SpStatusMsg { get; set; }
        public string? CustomerName { get; set; }

        public string? CustomerEmail { get; set; }

        public string? CustomerAddress { get; set; }
        public string? CustomerCity { get; set; }

        public string? Value1 { get; set; }

        public string? Value2 { get; set; }

        public string? Value3 { get; set; }

        public string? Value4 { get; set; }
        public string? TxnStatus { get; set; }

        public string? PaymentMethod { get; set; }

        public string? TxnTime { get; set; }
    }
}
