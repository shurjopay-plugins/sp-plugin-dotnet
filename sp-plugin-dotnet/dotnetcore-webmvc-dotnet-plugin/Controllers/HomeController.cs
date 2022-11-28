using dotnetcore_webmvc_dotnet_plugin.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using sp_plugin_dotnet;
using sp_plugin_dotnet.Models;

namespace dotnetcore_webmvc_dotnet_plugin.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            Shurjopay shurjopay= new Shurjopay();
            //Task<ShurjopayToken?> shurjopayToken = shurjopay.Authenticate();
           // shurjopay.WriteConsole();
            PaymentRequest paymentRequest = new PaymentRequest();
            paymentRequest.Amount = 10;
            paymentRequest.Prefix = "sp-dotnet";
            paymentRequest.Currency = "BDT";
            paymentRequest.OrderId = "aps-5221";
            paymentRequest.CustomerName = "Mahabubul";
            paymentRequest.CustomerAddress = "APS";
            paymentRequest.CustomerCity = "Dhaka";
            paymentRequest.CustomerPhone = "01311310975";
            paymentRequest.CustomerPostCode = "1229";

            Task<PaymentDetails?> paymentDetails =  shurjopay.MakePayment(paymentRequest);

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}