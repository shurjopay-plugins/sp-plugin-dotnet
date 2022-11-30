using sp_plugin_dotnet.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net.Http.Headers;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace sp_plugin_dotnet
{
    //Todo Logging
    public class Shurjopay
    {
        //TODO get the configuration from example app
        const string SP_USERNAME = "sp_sandbox";
        const string SP_PASSWORD = "pyyk97hu&6u6";
        const string SHURJOPAY_API = "https://sandbox.shurjopayment.com/api/";
        const string SP_CALLBACK = "https://www.sandbox.shurjopayment.com/response";

        // static http client for http request handling
        static readonly HttpClient httpclient = new HttpClient();
        ShurjopayToken? AuthToken { get; set; } = null;


        //TODO doc-string
        public async Task<ShurjopayToken?> Authenticate()
        {
            // Create Token URI
            string tokenUrl = SHURJOPAY_API + Endpoints.TOKEN;
            // Create payload content
            Hashtable payload = new Hashtable();
            payload.Add("username", SP_USERNAME);
            payload.Add("password", SP_PASSWORD);
            // Serialize the Hashtable to Json
            var jsonContent = JsonHelper.FromClass(payload);
            // Create Http Request Message
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json"),
                RequestUri = new Uri(tokenUrl)
            };
            // Call asynchronous network methods in a try/catch block to handle exceptions while authenticating marchent
            try
            {
                // Send asyncronus post reques & await for the response
                var response = await httpclient.SendAsync(requestMessage);
                // Ensure the Http request is successful
                response.EnsureSuccessStatusCode();
                // Await for the response content
                string responseBody = await response.Content.ReadAsStringAsync();
                // Return Shurjopay Token Model after Deserialization  
                return JsonHelper.ToClass<ShurjopayToken>(responseBody);
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return null;
        }

        //Todo doc-string
        public async Task<PaymentDetails?> MakePayment(PaymentRequest request)
        {
            try
            {
                /* Authenticate Marchent if AuthToken is null */
                //TODO check expiration of auth token
                if(this.AuthToken == null)
                {
                    this.AuthToken = await Authenticate();
                }
            }
            //Todo Shurjopay Custom Exception
            catch (Exception e)
            {
                Console.WriteLine("Shurjopay Authentication Exception Caught");
                Console.WriteLine("Message :{0} ", e.Message);
                throw;
            }

            // Create Make Payment URI
            string makePayemntUrl = SHURJOPAY_API + Endpoints.MAKE_PAYMENT;
            // Create Shurjopay Payment Request
            var paymentRequest = this.MapPaymentRequest(request);
            // Serialize the Payment Request HashTable to json
            var jsonContent = JsonHelper.FromClass(paymentRequest);
            // Create the Http Request Message 
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json"),
                RequestUri = new Uri(makePayemntUrl)
            };
            // Call asynchronous network methods in a try/catch block to handle exceptions while making payment with Shurjopay
            try
            {
                httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(this.AuthToken.TokenType,this.AuthToken.Token);
                // Send asyncronus post reques to get the payment request details
                var response = await httpclient.SendAsync(requestMessage);
                // Ensure the Http Success status
                response.EnsureSuccessStatusCode();
                // Get the payment request details as json object from response
                string responseBody = await response.Content.ReadAsStringAsync();
                // Return Payment Details as thread task
                return JsonHelper.ToClass<PaymentDetails>(responseBody);
            }
            catch (HttpRequestException e)
            {
                //Todo Log eroor & throw exception
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            catch(IOException e)
            {
            
                //Todo Log eroor & throw exception
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return null;
        }
        

        //Todo doc-string
        public async Task<VerifiedPayment?> VerifyPayment(string OrderId)
        {
            try
            {
                /* Authenticate Marchent if AuthToken is null */
                //TODO check expiration of auth token
                if (this.AuthToken == null)
                {
                    this.AuthToken = await Authenticate();
                }
            }
            //Todo Shurjopay Custom Exception
            catch (Exception e)
            {
                //Todo Log eroor & throw exception
                Console.WriteLine("Shurjopay Authentication Exception Caught");
                Console.WriteLine("Message :{0} ", e.Message);
                throw;
            }
            // Create Make Payment URL 
            string verifyPayemntUrl = SHURJOPAY_API + Endpoints.VERIFY_PAYMENT;
            Hashtable payload = new Hashtable();
            // Add order id to the payload
            payload.Add("order_id", OrderId);
            var jsonContent = JsonHelper.FromClass(payload);
            // Create HttpRequest Message 
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json"),
                RequestUri = new Uri(verifyPayemntUrl)
            };
            // Call asynchronous network methods in a try/catch block to handle exceptions while making payment
            try
            {
                httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(this.AuthToken.TokenType, this.AuthToken.Token);
                // Send asyncronus post reques to get the payment request details
                var response = await httpclient.SendAsync(requestMessage);
                // Ensure the Http Success status
                response.EnsureSuccessStatusCode();
                // Get the payment request details as json object from response
                string responseBody = await response.Content.ReadAsStringAsync();


                //Todo throw Shurjopay exception if sp_code != 1000 

                // Return Verified Payment details as a thread task
                return JsonHelper.ToClass<List<VerifiedPayment?>>(responseBody)[0];
             
            }
            catch (HttpRequestException e)
            {
                //Todo Log eroor & throw exception
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            catch (IOException e)
            {
                //Todo Log eroor & throw exception
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return null;
        }



        //Todo doc-string
        public async Task<VerifiedPayment?> CheckPayment(string OrderId)
        {
            try
            {
                /* Authenticate Marchent if AuthToken is null */
                //TODO check expiration of auth token
                if (this.AuthToken == null)
                {
                    this.AuthToken = await Authenticate();
                }
            }
            //Todo Shurjopay Custom Exception
            catch (Exception e)
            {
                Console.WriteLine("Shurjopay Authentication Exception Caught");
                Console.WriteLine("Message :{0} ", e.Message);
                throw;
            }
            // Create Payment Status URL 
            string verifyPayemntUrl = SHURJOPAY_API + Endpoints.PAYMENT_STATUS;
            Hashtable payload = new Hashtable();
            payload.Add("order_id", OrderId);
            var jsonContent = JsonHelper.FromClass(payload);
            // Create HttpRequest Message 
            var requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json"),
                RequestUri = new Uri(verifyPayemntUrl)
            };
            // Call asynchronous network methods in a try/catch block to handle exceptions while making payment
            try
            {
                httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(this.AuthToken.TokenType, this.AuthToken.Token);
                // Send asyncronus post reques to get the payment details
                var response = await httpclient.SendAsync(requestMessage);
                // Ensure the Http Success status
                response.EnsureSuccessStatusCode();
                // Get the payment request details as json object from response
                string responseBody = await response.Content.ReadAsStringAsync();
                // Return Verified Payment details as a thread task
                return JsonHelper.ToClass<List<VerifiedPayment?>>(responseBody)[0];

            }
            catch (HttpRequestException e)
            {
                //Todo Log eroor & throw exception
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            catch (IOException e)
            {
                //Todo Log eroor & throw exception
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }
            return null;
        }

        //TODO doc-string
        public bool IsTokenExpired()
        {
            DateTime parsedDate;
            // Parse token creation time to default format
            if (!DateTime.TryParse(this.AuthToken.TokenCreatedTime, out parsedDate))
            {
                //TODO throw Shurjopay Exception
            }
            // Return false if token is expired otherwise true
            return (parsedDate.AddSeconds((double)this.AuthToken.ExpiredTimeInSecond) > DateTime.Now) ? false : true;
        }

        //TODO doc-string
        public Hashtable MapPaymentRequest(PaymentRequest request)
        {
            // Store payment request as key value pair
            Hashtable paymentRequest = new Hashtable();
            paymentRequest.Add("token", this.AuthToken.Token);
            paymentRequest.Add("store_id", this.AuthToken.StoreId);
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
            paymentRequest.Add("client_ip","0.0.0.0"); //TODO get the ip address of host machine
            return paymentRequest;
        }
    }
}