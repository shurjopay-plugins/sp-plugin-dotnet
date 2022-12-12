﻿/// Licensed to the .NET Foundation under one or more agreements.
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
        string? SP_USERNAME {set;get;}
        string? SP_PASSWORD { set; get; }
        string? SP_ENDPOINT { set; get; }
        string? SP_CALLBACK { set; get; }
        ShurjopayToken? AuthToken { get; set; } = null;
        // Shurjopay Status Codes
        const string SP_AUTH_SUCCESS = "200"; // get-token api returns sp_code in string
        const int SP_PAYMENT_SUCCESS = 1000;  // make-payment api returns sp code in integer 
        // static http client for http request handling
        static readonly HttpClient httpclient = new HttpClient();
        /// <summary>
        /// Constructor to instantiate Shurjopay with Shurjopay Configurations.
        /// </summary>
        /// <typeparam name="ShurjopayConfig">DTO Model for Dependency Injection.</typeparam>
        /// <param name="shurjopayConfig">Shurjopay Configuration.</param>
        public ShurjopayPlugin(ShurjopayConfig shurjopayConfig, ILogger<ShurjopayPlugin> logger )
        {
            this.SP_USERNAME = shurjopayConfig.SP_USERNAME;
            this.SP_PASSWORD = shurjopayConfig.SP_PASSWORD;
            this.SP_CALLBACK = shurjopayConfig.SP_CALLBACK;
            this.SP_ENDPOINT = shurjopayConfig.SP_ENDPOINT;
            _logger = logger;
        }
        /// <summary>
        /// Authenticate Marchent with Shurjopay Gateway.
        /// </summary>
        /// <returns>A <typeparamref name="ShurjopayToken"/> representation of the Authentication Token.</returns>
        public async Task<ShurjopayToken?> Authenticate()
        {
            // Create Token URI
            string tokenUrl = SP_ENDPOINT + Endpoints.TOKEN;
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
                ShurjopayToken? spAuthToken = JsonHelper.ToClass<ShurjopayToken>(responseBody);
                if(spAuthToken.SpStatusCode == SP_AUTH_SUCCESS)
                    // Return Shurjopay Token Model after Deserialization  
                    return spAuthToken;
                _logger.LogError($"Shurjopay Code: {spAuthToken.SpStatusCode}, Shurjopay Message:{spAuthToken.Message}");
                throw new ShurjopayException("Shurjopay Authentication Faield, Check your credentials");
            }
            catch (HttpRequestException e)
            {   
                _logger.LogError("Shurjopay Http Exception Caught", e.Message);
                throw;
            }
            catch(ShurjopayException e)
            {
               _logger.LogError("Shurjopay Authentication Faield", e.Message);
                throw;
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
                PaymentDetails? paymentDetails = JsonHelper.ToClass<PaymentDetails>(responseBody);
                // Check if payment request is successful
                if(paymentDetails.CheckOutUrl.Equals(null))
                {
                    var ex = new ShurjopayException($"Shurjopay Payment Request Failed");
                    _logger.LogError(ex, $"Shurjopay Payment Request Failed");
                    throw ex;
                }
                return paymentDetails;
            }
            catch(ShurjopayException e)
            {
                _logger.LogError("Shurjopay Payment Request Failed", e.Message);
                throw;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Shurjopay Http Exception Caught", e.Message);
                throw;
            }
            catch(IOException e)
            {

                _logger.LogError("Shurjopay IO Exception Caught", e.Message);
                throw;
            }
        }
        /// <summary>
        /// Verify payment Request to Shurjopay Gateway with order-id.
        /// </summary>
        /// <typeparam name="string">The type of the parameter</typeparam>
        /// <returns>A <typeparamref name="VerifiedPayment"/> Representation Model of the Verified Payment Details.</returns>
        /// <param name="OrderId">Payment Request object.</param>
        public async Task<VerifiedPayment?> VerifyPayment(string OrderId)
        {
            try
            {
                // Authenticate Marchent if AuthToken is null or expired
                if (this.AuthToken == null || IsTokenExpired())
                {
                    this.AuthToken = await Authenticate();
                }
            }
            catch (ShurjopayException e)
            {
                _logger.LogError("Authentication Faield", e.Message);
                throw;
            }
            // Create Make Payment URL 
            string verifyPayemntUrl = SP_ENDPOINT + Endpoints.VERIFY_PAYMENT;
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
                VerifiedPayment? verifiedPayment = JsonHelper.ToClass<List<VerifiedPayment?>>(responseBody)[0];
                // Check if payment is successful
                if (verifiedPayment.SpStatusCode != SP_PAYMENT_SUCCESS)
                {
                    var ex = new ShurjopayException($"Code: {verifiedPayment.SpStatusCode} Message: {verifiedPayment.SpStatusMsg}");
                    _logger.LogError(ex, $"Code: {verifiedPayment.SpStatusCode} Message: {verifiedPayment.SpStatusMsg}");
                    throw ex;
                }
                return verifiedPayment;
             
            }
            catch(ShurjopayException e)
            {
                _logger.LogError("Shurjopay Payment Verification Faield", e.Message);
                throw;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Shurjopay Http Exception Caught", e.Message);
                throw;
            }
            catch (IOException e)
            {
                _logger.LogError("Shurjopay IO Exception Caught", e.Message);
                throw;
            }   
        }
        /// <summary>
        /// Check Payment Details request to Shurjopay Gateway.
        /// </summary>
        /// <typeparam name="string">The type of the parameter</typeparam>
        /// <returns>A <typeparamref name="VerifiedPayment"/> Representation of the Verified Payment Details.</returns>
        /// <param name="OrderId">Payment Request object.</param>
        public async Task<VerifiedPayment?> CheckPayment(string orderId)
        {
            try
            {
                // Authenticate Marchent if AuthToken is null or expired
                if (this.AuthToken == null || IsTokenExpired())
                {
                    this.AuthToken = await Authenticate();
                }
            }
            catch (ShurjopayException e)
            {
                _logger.LogError("Authentication Faield", e.Message);
                throw;
            }
            // Create Payment Status URL 
            string verifyPayemntUrl = SP_ENDPOINT + Endpoints.PAYMENT_STATUS;
            Hashtable payload = new Hashtable();
            payload.Add("order_id", orderId);
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
            catch (ShurjopayException e)
            {
                _logger.LogError("Shurjopay Exception Caught While Checking Payment", e.Message);
                throw;
            }
            catch (HttpRequestException e)
            {
                _logger.LogError("Shurjopay Http Exception Caught", e.Message);
                throw;
            }
            catch (IOException e)
            {
                _logger.LogError("Shurjopay IO Exception Caught", e.Message);
                throw;
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
            paymentRequest.Add("client_ip", GetLocalIPAddress());
            return paymentRequest;
        }
        /// <summary>
        /// Get the local ip address of Marchent
        /// </summary>
        /// <returns>A <typeparamref name="string"/>A string consisting marchent's ip address</returns>
        public string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
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