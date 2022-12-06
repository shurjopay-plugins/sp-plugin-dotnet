using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sp_plugin_dotnet.Models;
using sp_plugin_dotnet;
using Microsoft.Extensions.Options;

namespace dotnetcore_webmvc_dotnet_plugin.Controllers
{
    public class ShurjopayController : Controller
    {
        private readonly ILogger<Shurjopay> _logger;
        public Shurjopay Shurjopay;
        public ShurjopayController(IOptions<ShurjopayConfig> options, ILogger<Shurjopay> logger)
        {
            Shurjopay = new Shurjopay(options.Value,logger);
            _logger = logger;
        }


        // GET: ShurjopayController
        public IActionResult Index()
        {
            Console.WriteLine(Shurjopay.GetLocalIPAddress());
            return View();
        }

        // GET: ShurjopayController/Details/5
        public ActionResult Details(string id)
        {
            Task<VerifiedPayment?> verifiedPayment = Shurjopay.CheckPayment(id);
            ViewBag.VerifiedPayment = verifiedPayment;
            return View();
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
                Task<PaymentDetails?> paymentDetailsTask = Shurjopay.MakePayment(paymentRequest);
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
