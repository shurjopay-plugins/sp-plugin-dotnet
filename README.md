# ![ShurjoPay](shurjopay.png) .NET Package (plugin)
![.Net](https://img.shields.io/badge/.NET-5C2D91?style=for-the-badge&logo=.net&logoColor=white)
[![.Nuget](https://badge.fury.io/nu/ShurjopayPlugin.svg)](https://www.nuget.org/packages/ShurjopayPlugin)
![downloads](https://img.shields.io/nuget/dt/ShurjopayPlugin)
[![license](https://badgen.net/pypi/license/pip/)](LICENSE)

Official shurjoPay .net package (plugin) for merchants or service providers to connect with shurjoPay Payment Gateway v2.1 developed and maintained by shurjoMukhi Limited.

This plugin package can be used with .NET Standert 2.1 applications (e.g. .NET Core - MVC, Web API etc.).

This plugin package makes it easy for you to integrate with shurjoPay v2.1 with just three method calls:

- MakePayment()
- VerifyPayment()
- CheckPayemnt()

Also reduces many of the things that you had to do manually

- Handles http request and errors
- JSON serialization and deserialization
- Authentication during checkout and verification of payments

## Audience

This document is intended for the developers and technical personnel of merchants and service providers who want to integrate the shurjoPay online payment gateway using .net framework.

## How to use this shurjoPay Plugin


 > Use `Package Manager` to install this plugin inside your project environment.

```
Install-Package ShurjopayPlugin
```

> Add Shurjopay Configuration to your project's user secrets Here is a sample user-secrects configuration.

```json
"Shurjopay": {
    "SP_USERNAME": "sp_sandbox",
    "SP_PASSWORD": "pyyk97hu&6u6",
    "SP_CALLBACK": "https://www.sandbox.shurjopayment.com/response",
    "SP_ENDPOINT": "https://sandbox.shurjopayment.com/api/"
    "SP_PREFIX": "sp-dotnet"
  }
```

> Add the Configuration in Program.cs file.
```
// Shurjopay Secrets
builder.Services.Configure<ShurjopayConfig>(builder.Configuration.GetSection("Shurjopay"));
```


> Instantiate ShurjopayPlugin with Shurjopay Configuration and Logger

```c#
 private readonly ILogger<ShurjopayPlugin> _logger;
 public ShurjopayPlugin _ShurjopayPlugin;
 public ShurjopayController(IOptions<ShurjopayConfig> options, ILogger<ShurjopayPlugin> logger)
 {
    _ShurjopayPlugin = new ShurjopayPlugin(options.Value,logger);
    _logger = logger;
 }
``` 

>Sample Payment Request Object
```c#
PaymentRequest paymentRequest = new PaymentRequest();
paymentRequest.Amount = 10;
paymentRequest.Prefix = "sp-dotnet";
paymentRequest.OrderID= "sp-dotnet-6.00";
paymentRequest.Currency= "BDT";
paymentRequest.CustomerName= "Mahabubul";
paymentRequest.CustomerAddress= "Haque Tower,Mohakhali";
paymentRequest.CustomerCity= "Dhaka";
paymentRequest.CustomerPhone = "01311310975";
paymentRequest.CustomerPostCode = "1229";
```

> Make Payment Request
```c#
Task<PaymentDetails?> paymentDetailsTask = _ShurjopayPlugin.MakePayment(paymentRequest);
PaymentDetails? paymentDetails = paymentDetailsTask.Result;
return Redirect(paymentDetails.CheckOutUrl);
```


> Payment verification can be done after each transaction with shurjopay order id.

```c#
  Task<VerifiedPayment?> TVerfiedPayment = _ShurjopayPlugin.VerifyPayment(order_id);
  VerifiedPayment? verifiedPayment = TVerfiedPayment.Result;
```

#### That's all! Now you are ready to use the python plugin to seamlessly integrate with shurjoPay to make your payment system easy and smooth.

## References
1. [.Net Core Web MVC example application](https://github.com/shurjopay-plugins/sp-plugin-usage-examples/tree/main/dotnetcore-webmvc-app-dotnet-plugin) showing usage of the .Net plugin.
2. [Sample applications and projects](https://github.com/shurjopay-plugins/sp-plugin-usage-examples) in many different languages and frameworks showing shurjopay integration.
3. [shurjoPay Postman site](https://documenter.getpostman.com/view/6335853/U16dS8ig) illustrating the request and response flow using the sandbox system.
4. [shurjopay Plugins](https://github.com/shurjopay-plugins) home page on github

## License
This code is under the [MIT open source License](LICENSE).
#### Please [contact](https://shurjopay.com.bd/#contacts) with shurjoPay team for more detail.
### Copyright ©️2022 [ShurjoMukhi Limited](https://shurjopay.com.bd/)
