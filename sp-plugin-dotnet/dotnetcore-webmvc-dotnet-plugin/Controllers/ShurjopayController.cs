using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using sp_plugin_dotnet.Models;
using sp_plugin_dotnet;
using Microsoft.Extensions.Options;

namespace dotnetcore_webmvc_dotnet_plugin.Controllers
{
    public class ShurjopayController : Controller
    {

        public Shurjopay Shurjopay;
        public ShurjopayController(IOptions<ShurjopayConfig> options)
        {
            Shurjopay = new Shurjopay(options.Value);
        }


        // GET: ShurjopayController
        public IActionResult Index()
        {

            return View();
        }

        // GET: ShurjopayController/Details/5
        public ActionResult Details(int id)
        {
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

        // GET: ShurjopayController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ShurjopayController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ShurjopayController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ShurjopayController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
