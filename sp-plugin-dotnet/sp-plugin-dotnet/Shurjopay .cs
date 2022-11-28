using sp_plugin_dotnet.Models;
using System;
using System.Net.Http;
using System.Text.Json.Nodes;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using System.Collections;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace sp_plugin_dotnet
{
    
    public class Shurjopay
    {

        //TODO get the configuration from example app
        const string SP_USERNAME = "sp_sandbox";
        const string SP_PASSWORD = "pyyk97hu&6u6";
        const string SHURJOPAY_API = "https://sandbox.shurjopayment.com/api/";
        const string SP_CALLBACK = "https://www.sandbox.shurjopayment.com/response";

        // Http client 
        static readonly HttpClient httpclient = new HttpClient();
        ShurjopayToken? AuthToken { get; set; } = null;
   

        async Task<ShurjopayToken?> Authenticate()
        {
            string tokenUrl = SHURJOPAY_API + Endpoints.TOKEN;
            Hashtable payloads = new Hashtable();
            payloads.Add("username", SP_USERNAME);
            payloads.Add("password", SP_PASSWORD);
            var json = JsonConvert.SerializeObject(payloads);
            // Call asynchronous network methods in a try/catch block to handle exceptions while authenticating marchent
            try
            {
                var data = new StringContent(json, Encoding.UTF8, "application/json");
                using HttpResponseMessage response = await httpclient.PostAsync(tokenUrl, data);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                return JsonHelper.ToClass<ShurjopayToken>(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return null;
        }

        public async Task<PaymentDetails?> MakePayment(PaymentRequest request)
        {
            try
            {
                /* Authenticate Marchent if AuthToken is null */
                if(this.AuthToken == null)
                {
                    this.AuthToken = await Authenticate();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Shurjopay Authentication Exception Caught");
                Console.WriteLine("Message :{0} ", e.Message);
                throw;
            }

            // Create Make Payment URL 
            string makePayemntUrl = SHURJOPAY_API + Endpoints.TOKEN;
            var paymentRequest = this.MapPaymentRequest(request);
            var json = JsonConvert.SerializeObject(paymentRequest);
            // Call asynchronous network methods in a try/catch block to handle exceptions while making payment
            try
            {
                var data = new StringContent(json, Encoding.UTF8, "application/json");
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                //  httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(this.AuthToken.TokenType,this.AuthToken.Token);
                httpclient.DefaultRequestHeaders.Add("Authorization", "Bearer " + this.AuthToken.Token);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                using HttpResponseMessage response = await httpclient.PostAsync(makePayemntUrl, data);
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Payment Request");
                Console.WriteLine(responseBody);
                return JsonHelper.ToClass<PaymentDetails>(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return null;
        }

        public Hashtable MapPaymentRequest(PaymentRequest request)
        {
            Hashtable paymentRequest = new Hashtable();

#pragma warning disable CS8602 // Dereference of a possibly null reference.
            paymentRequest.Add("token", this.AuthToken.Token);
            paymentRequest.Add("store_id", this.AuthToken.StoreId);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            paymentRequest.Add("return_url", SP_CALLBACK);
            paymentRequest.Add("cancel_url", SP_CALLBACK);
            paymentRequest.Add("prefix", request.Prefix);
            paymentRequest.Add("amount", request.Amount);
            paymentRequest.Add("order_id", request.OrderId);
            paymentRequest.Add("currency", request.Currency);
            paymentRequest.Add("customer_name", request.CustomerName);
            paymentRequest.Add("customer_address", request.CustomerAddress);
            paymentRequest.Add("customer_phone", request.CustomerPhone);
            paymentRequest.Add("customer_city", request.CustomerCity);
            paymentRequest.Add("customer_post_code",request.CustomerPostCode);
            paymentRequest.Add("client_ip","0.0.0.0");
            return paymentRequest;
        }

    }
}