using Microsoft.AspNetCore.Mvc;
using Shurjopay.Plugin.Models;
using Shurjopay.Plugin;
using Microsoft.Extensions.Options;

namespace dotnetcore_webmvc_dotnet_plugin.Controllers
{
    public class ShurjopayController : Controller
    {
        private readonly ILogger<ShurjopayPlugin> _logger;
        public ShurjopayPlugin _ShurjopayPlugin;
        public ShurjopayController(IOptions<ShurjopayConfig> options, ILogger<ShurjopayPlugin> logger)
        {
            _ShurjopayPlugin = new ShurjopayPlugin(options.Value,logger);
            _logger = logger;
        }

        // GET: ShurjopayController
        public IActionResult Index()
        {
            return View();
        }


        /*

        [Route("/shurjopay/ipn")]
        public ActionResult Ipn(int orderId)
        {
            Task<VerifiedPayment?> TVerfiedPayment = Shurjopay.CheckPayment(orderid);
            VerifiedPayment? verifiedPayment= TVerfiedPayment.Result;
            return Details(verifiedPayment.OrderId);
        }
        */

        // GET: ShurjopayController/Details/5
        public ActionResult Details(string orderId)
        {
            Task<VerifiedPayment?> TVerfiedPayment = _ShurjopayPlugin.CheckPayment("sp-dotnet639585995258e");
            return View(TVerfiedPayment.Result);
        }

        // GET: ShurjopayController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ShurjopayController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PaymentRequest paymentRequest)
        {
            try
            {
                Task<PaymentDetails?> paymentDetailsTask = _ShurjopayPlugin.MakePayment(paymentRequest);
                PaymentDetails? paymentDetails = paymentDetailsTask.Result;
                return Redirect(paymentDetails.CheckOutUrl);
            }
            catch
            {
                return View();
            }
        }

    }
}
