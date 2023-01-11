/// Licensed to the .NET Foundation under one or more agreements.
/// The .NET Foundation licenses this file to you under the MIT license.
/// Author Md Mahabubul Hasan, Since 27/11/2022
using Shurjopay.Plugin.Models;
using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Net.Http.Headers;
using System.IO;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using System.Net;

namespace Shurjopay.Plugin
{ 
    public class ShurjopayPlugin
    {
        //Shurjopay Logger
        private readonly ILogger<ShurjopayPlugin> _logger;
        // Shurjopay Configurations
        string SP_USERNAME {set; get; }
        string SP_PASSWORD {set; get; }
        string SP_ENDPOINT {set; get;}
        string SP_CALLBACK {set; get;}
        string SP_PREFIX { set; get; }
        ShurjopayToken? AuthToken { get; set; } = null;
        
        /// <summary>
        /// Constructor to instantiate Shurjopay with Configurations.
        /// </summary>
        /// <typeparam name="ShurjopayConfig">Configuration model used in dependency injection.</typeparam>
        /// <param name="shurjopayConfig">Shurjopay Configuration.</param>
        public ShurjopayPlugin(ShurjopayConfig shurjopayConfig, ILogger<ShurjopayPlugin> logger )
        {
            this.SP_USERNAME = shurjopayConfig.SP_USERNAME;
            this.SP_PASSWORD = shurjopayConfig.SP_PASSWORD;
            this.SP_CALLBACK = shurjopayConfig.SP_CALLBACK;
            this.SP_ENDPOINT = shurjopayConfig.SP_ENDPOINT;
            this.SP_PREFIX = shurjopayConfig.SP_PREFIX;
            _logger = logger;
        }
        /// <summary>
        /// Authenticate Marchent with Shurjopay Gateway.
        /// </summary>
        /// <returns>A <typeparamref name="ShurjopayToken"/> representation of the Authentication Token.</returns>
        public async Task<ShurjopayToken?> Authenticate()
        {
            // Create token uri
            string tokenUrl = SP_ENDPOINT + Endpoints.TOKEN;
            // Create payload content
            Hashtable payload = new Hashtable();
            payload.Add("username", SP_USERNAME);
            payload.Add("password", SP_PASSWORD);
            // Serialize the hashtable to json
            string jsonContent = JsonHelper.FromClass(payload);
            // Create http request message with uri and payload
            HttpRequestMessage httpRequestMessage = GetHttpRequestMessage(jsonContent,tokenUrl);
            // Call asynchronous network methods in a try/catch block to handle exceptions while authenticating marchent
            try
            {
                using (HttpClient httpclient = new HttpClient())
                {
                    // Send asyncronus post reques & await for the response
                    var response = await httpclient.SendAsync(httpRequestMessage);
                    // Ensure the Http request is successful
                    response.EnsureSuccessStatusCode();
                    // Await for the response content
                    string responseBody = await response.Content.ReadAsStringAsync();
                    ShurjopayToken? spAuthToken = JsonHelper.ToClass<ShurjopayToken>(responseBody);
                    if (spAuthToken.IsSuccess())
                    {
                        // Return shurjopay token object if authenticated with shurjopay 
                        _logger.LogInformation("Authencticated with Shurjopay");
                        return spAuthToken;
                    }
                    _logger.LogError($"Shurjopay Code: {spAuthToken.SpCode}, Shurjopay Message:{spAuthToken.SpMessage}");
                    throw new ShurjopayException("Shurjopay Authentication Faield, Check your credentials");
                }
               
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Http Exception Caught During Authentication", ex.Message);
                throw new ShurjopayException("Http Exception Caught During Authentication", ex);
            }
            catch (IOException ex)
            {
                _logger.LogError("IO Exception Caught During Authentication", ex.Message);
                throw new ShurjopayException("IO Exception Caught During Authentication", ex);
            }
        }
        /// <summary>
        /// Check if token is expired or not by comparing current time with token creattion time and expiration time
        /// </summary>
        /// <returns>A <typeparamref name="bool"/>A boolean value depending the validity of token</returns>
        public bool IsTokenExpired()
        {
            DateTime parsedDate;
            // Parse token creation time to default format
            if (!DateTime.TryParse(this.AuthToken.TokenCreatedTime, out parsedDate))
            {
                throw new ShurjopayException("Authentication Token Expired");
            }
            // Return false if token is expired otherwise true
            return (parsedDate.AddSeconds((double)this.AuthToken.ExpiredTimeInSecond) > DateTime.Now) ? false : true;
        }

        /// <summary>
        /// Make payment Request to Shurjopay Gateway.
        /// </summary>
        /// <typeparam name="PaymentRequest">The type to deserialize the JSON response from Shurjopay API.</typeparam>
        /// <returns>A <typeparamref name="PaymentDetails"/> Representation Model Containing Requested Payment Details.</returns>
        /// <param name="request">Payment Request object.</param> 
        public async Task<PaymentDetails?> MakePayment(PaymentRequest request)
        {
            try
            {
                // Authenticate Marchent if AuthToken is null or expired
                if(this.AuthToken == null || IsTokenExpired())
                {
                    this.AuthToken = await Authenticate();
                }
            }
            catch (ShurjopayException e)
            {
                _logger.LogError("Authentication Faield", e.Message);
                throw;
            }
            // Create Make Payment URI
            string makePayemntUrl = SP_ENDPOINT + Endpoints.MAKE_PAYMENT;
            // Create Shurjopay Payment Request
            Hashtable paymentRequest = this.MapPaymentRequest(request);
            // Serialize the Payment Request HashTable to json
            string jsonContent = JsonHelper.FromClass(paymentRequest);
            // Create http request message with uri and payload
            HttpRequestMessage httpRequestMessage = GetHttpRequestMessage(jsonContent,makePayemntUrl);
            // Call asynchronous network methods in a try/catch block to handle exceptions while making payment with Shurjopay
            try
            {
                using (HttpClient httpclient = new HttpClient())
                {
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(this.AuthToken.TokenType, this.AuthToken.Token);
                    // Send asyncronus post request to get the payment request details
                    HttpResponseMessage response = await httpclient.SendAsync(httpRequestMessage);
                    // Ensure the Http Success status
                    response.EnsureSuccessStatusCode();
                    // Get the payment request details as json object from response
                    string responseBody = await response.Content.ReadAsStringAsync();
                    // Return Payment Details as thread task
                    PaymentDetails? paymentDetails = JsonHelper.ToClass<PaymentDetails>(responseBody);
                    // Check if payment request is successful
                    if (paymentDetails.IsSuccess())
                    {
                        // Return if payment request is successful
                        _logger.LogInformation("Shurjopay Payment Request Initiated");
                         return paymentDetails;
                    }
                    Exception ex = new ShurjopayException("Shurjopay Payment Request Failed");
                    _logger.LogError(ex, "Shurjopay Payment Request Failed");
                    throw ex;
                }
            }
            catch(ShurjopayException ex)
            {
                _logger.LogError("Shurjopay Payment Request Failed", ex.Message);
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Http Exception Caught During Payment Request", ex.Message);
                throw new ShurjopayException("Http Exception Caught During Payment Request", ex);
            }
            catch (IOException ex)
            {
                _logger.LogError("IO Exception Caught During Payment Request", ex.Message);
                throw new ShurjopayException("IO Exception Caught During Payment Request", ex);
            }
        }
        /// <summary>
        /// Verify payment with shurjoPay order-id.
        /// </summary>
        /// <typeparam name="string">The type of the parameter</typeparam>
        /// <returns>A <typeparamref name="VerifiedPayment"/> Representation Model of the verified payment details response from shurjoPay.</returns>
        /// <param name="OrderId">Payment Request object.</param>
        /// <exception cref="ShurjopayException">Throws exception if order-id invalid or payment is not successful.</exception>
        public async Task<VerifiedPayment?> VerifyPayment(string OrderId)
        {
            // Create Make Payment URL 
            string verifyPayemntUrl = SP_ENDPOINT + Endpoints.VERIFY_PAYMENT;
            Hashtable payload = new Hashtable();
            // Add order id to the payload
            payload.Add("order_id", OrderId);
            string jsonContent = JsonHelper.FromClass(payload);
            // Create http request message with uri and payload
            HttpRequestMessage httpRequestMessage = GetHttpRequestMessage(jsonContent,verifyPayemntUrl);
            // Call asynchronous network methods in a try/catch block to handle exceptions while making payment
            VerifiedPayment? verifiedPayment = null;
            try
            {
                // Authenticate Marchent if AuthToken is null or expired
                if (this.AuthToken == null || IsTokenExpired())
                {
                    this.AuthToken = await Authenticate();
                }
            }
            catch (ShurjopayException ex)
            {
                _logger.LogError("Authentication Faield", ex.Message);
                throw;
            }
            try
            {
                using (HttpClient httpclient = new HttpClient())
                {
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(this.AuthToken.TokenType, this.AuthToken.Token);
                    // Send asyncronus post reques to get the payment request details
                    HttpResponseMessage response = await httpclient.SendAsync(httpRequestMessage);
                    // Ensure the Http Success status
                    response.EnsureSuccessStatusCode();
                    // Get the payment request details as json object from response
                    string responseBody = await response.Content.ReadAsStringAsync();
                    try
                    {
                        verifiedPayment = JsonHelper.ToClass<List<VerifiedPayment?>>(responseBody)[0];
                        // Check if payment is successful
                        if (verifiedPayment.IsSuccess())
                        {
                            // Return verified payment object as a thread task
                            _logger.LogInformation("Shurjopay Payment Verified");
                            return verifiedPayment;
                        }
                        ShurjopayException ex = new ShurjopayException($"Code: {verifiedPayment.SpCode} Message: {verifiedPayment.SpMessage}");
                        _logger.LogError(ex, $"Code: {verifiedPayment.SpCode} Message: {verifiedPayment.SpMessage}");
                        throw ex;

                    }
                    catch(ShurjopayException)
                    {
                        // Reurn null if invalid order id provied / response is not serializable
                        return verifiedPayment;
                    }
                }
             
            }
            catch(ShurjopayException ex)
            {
                _logger.LogError("Shurjopay Payment Verification Faield", ex.Message);
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Http Exception Caught During Payment Verification", ex.Message);
                throw new ShurjopayException("Http Exception Caught During Payment Verification", ex);
            }
            catch (IOException ex)
            {
                _logger.LogError("IO Exception Caught During Payment Verification", ex.Message);
                throw new ShurjopayException("IO Exception Caught During Payment Verification", ex);
            }
        }
        /// <summary>
        /// Check Payment Details request to Shurjopay Gateway.
        /// </summary>
        /// <typeparam name="string">The type of the parameter</typeparam>
        /// <returns>A <typeparamref name="VerifiedPayment"/> Representation of the Payment Details else null if invalid order id provided.</returns>
        /// <param name="OrderId">Payment Request object.</param>
        public async Task<VerifiedPayment?> CheckPayment(string orderId)
        {
            VerifiedPayment? verifiedPayment=null;
            // Create Payment Status URL 
            string checkPayemntUrl = SP_ENDPOINT + Endpoints.PAYMENT_STATUS;
            Hashtable payload = new Hashtable();
            payload.Add("order_id", orderId);
            string jsonContent = JsonHelper.FromClass(payload);
            // Create HttpRequest Message 
            HttpRequestMessage httpRequestMessage = GetHttpRequestMessage(jsonContent, checkPayemntUrl);
            // Call asynchronous network methods in a try/catch block to handle exceptions while making payment
            try
            {
                // Authenticate Marchent if AuthToken is null or expired
                if (this.AuthToken == null || IsTokenExpired())
                {
                    this.AuthToken = await Authenticate();
                }
            }
            catch (ShurjopayException ex)
            {
                _logger.LogError("Authentication Faield", ex.Message);
                throw;
            }
            try
            {
                using (HttpClient httpclient = new HttpClient())
                {
                    httpclient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(this.AuthToken.TokenType, this.AuthToken.Token);
                    // Send asyncronus post reques to get the payment details
                    HttpResponseMessage response = await httpclient.SendAsync(GetHttpRequestMessage(jsonContent, checkPayemntUrl));
                    // Ensure the Http Success status
                    response.EnsureSuccessStatusCode();
                    // Get the payment request details as json object from response
                    string responseBody = await response.Content.ReadAsStringAsync();
                    try
                    {
                        verifiedPayment = JsonHelper.ToClass<List<VerifiedPayment?>>(responseBody)[0];
                        // Check if payment is successful
                        if (verifiedPayment.IsSuccess())
                        {
                            // Return verified payment object
                            _logger.LogInformation("Shurjopay Payment Details Recived");
                            return verifiedPayment;
                        }
                        ShurjopayException ex = new ShurjopayException($"Code: {verifiedPayment.SpCode} Message: {verifiedPayment.SpMessage}");
                        _logger.LogError(ex, $"Code: {verifiedPayment.SpCode} Message: {verifiedPayment.SpMessage}");
                        throw ex;
                    }
                    catch (ShurjopayException)
                    {
                        // Reurn null if invalid order id provied / response is not serializable
                        return verifiedPayment;
                    }
                } 

            }
            catch (ShurjopayException ex)
            {
                _logger.LogError("Shurjopay Exception Caught While Check Payment", ex.Message);
                throw;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError("Http Exception Caught During Payment Check", ex.Message);
                throw new ShurjopayException("Http Exception Caught During Payment Check",ex);
            }
            catch (IOException ex)
            {
                _logger.LogError("IO Exception Caught During Payment Check", ex.Message);
                throw new ShurjopayException("IO Exception Caught During Payment Check", ex);
            }
        }

        /// <summary>
        /// Add Token details with payment request object
        /// </summary>
        /// <returns>A <typeparamref name="Hashtable"/>A Hashtable Consisting Payment Request Details</returns>
        private Hashtable MapPaymentRequest(PaymentRequest request)
        {
            // Store payment request as key value pair
            Hashtable paymentRequest = new Hashtable();
            paymentRequest.Add("token", this.AuthToken.Token);
            paymentRequest.Add("store_id", this.AuthToken.StoreId);
            paymentRequest.Add("return_url", SP_CALLBACK+Endpoints.RETURN);
            paymentRequest.Add("cancel_url", SP_CALLBACK+Endpoints.CANCEL);
            paymentRequest.Add("prefix", this.SP_PREFIX);
            paymentRequest.Add("amount", request.Amount);
            paymentRequest.Add("order_id", request.OrderId);
            paymentRequest.Add("currency", request.Currency);
            paymentRequest.Add("customer_name", request.CustomerName);
            paymentRequest.Add("customer_address", request.CustomerAddress);
            paymentRequest.Add("customer_phone", request.CustomerPhone);
            paymentRequest.Add("customer_city", request.CustomerCity);
            paymentRequest.Add("customer_post_code",request.CustomerPostCode);
            paymentRequest.Add("client_ip", GetLocalIPAddress());
            return paymentRequest;
        }

        /// <summary>
        /// Get the HttpRequest message of the given content and uri
        /// </summary>
        /// <returns>A <typeparamref name="HttpRequestMessage"/>A HttpRequestMessage object</returns>
        private static HttpRequestMessage GetHttpRequestMessage(string jsonContent,string uri)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                Content = new StringContent(jsonContent, Encoding.UTF8, "application/json"),
                RequestUri = new Uri(uri)
            };
            return requestMessage;
        }

        /// <summary>
        /// Get the local ip address of Marchent
        /// </summary>
        /// <returns>A <typeparamref name="string"/>A string consisting marchent's ip address</returns>
        public static string GetLocalIPAddress()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new ShurjopayException("No network adapters with an IPv4 address in the system!");
        }
    }

    [Serializable]
    public class ShurjopayException : Exception
    {
        public ShurjopayException() : base() { }
        public ShurjopayException(string message) : base(message) { }
        public ShurjopayException(string message, Exception inner) : base(message, inner) { }
    }
}