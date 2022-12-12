﻿![alt text](https://shurjopay.com.bd/dev/images/shurjoPay.png)

# ShurjoPay Online Payment API Integration

This document has been prepared by Shurjomukhi Limited to enable the online merchants to integrate shurjoPay payment gateway. The information contained in this document is proprietary and confidential to Shurjomukhi Limited, for the product Shurjopay.

## Audience

This document is intended for the technical personnel of merchants and service providers that want to integrate a new online payment gateway using python plugin provided by shurjoPay.

## Integration

ShurjoPay Online payment gateway has several API's which need to be integrated by merchants for accessing different services. The available services are:

- Authenticate users
- Making payment
- Verifying payment order
- Checking verified payment order status

## shurjoPay dotnet plugin for dotnet standerd 2.1

**Example Applications**

### [ASP DOT NET CORE WEB MVC 6.00 ](https://github.com/shurjopay-plugins/sp-plugin-usage-examples/tree/dev/django-app-python-plugin)

## Installation

> 📝 **NOTE** Install the package inside your project environment

> Use `nuget` to install shuroPay python plugin

```
dotnet install shurjopay
```

> Or `clone` the repository and add the plugin inside your project

```
git clone https://github.com/shurjopay-plugins/sp-plugin-dotnet

```

Then add the sp-plugin-dotnet reference inside your project


## Initialize the shurjoPay dotnet plugin with shurjoPay credentials & api-url and marchent's callback url 

> Here is a sample user-secrects configuration for dot net project

```
"Shurjopay": {
    "SP_USERNAME": "sp_sandbox",
    "SP_PASSWORD": "pyyk97hu&6u6",
    "SP_CALLBACK": "https://www.sandbox.shurjopayment.com/response",
    "SP_ENDPOINT": "https://sandbox.shurjopayment.com/api/"
  }
```

> Now configure the shurjopay secrets inside Program.cs file.
```
// Shurjopay Secrets
builder.Services.Configure<ShurjopayConfig>(builder.Configuration.GetSection("Shurjopay"));
```

## Use Case
> Instantiate ShurjopayPlugin with Shurjopay Configuration and Logger

```
 private readonly ILogger<ShurjopayPlugin> _logger;
 public ShurjopayPlugin _ShurjopayPlugin;
 public ShurjopayController(IOptions<ShurjopayConfig> options, ILogger<ShurjopayPlugin> logger)
 {
    _ShurjopayPlugin = new ShurjopayPlugin(options.Value,logger);
    _logger = logger;
 }
``` 
> Make Payment Request
```
Task<PaymentDetails?> paymentDetailsTask = _ShurjopayPlugin.MakePayment(paymentRequest);
PaymentDetails? paymentDetails = paymentDetailsTask.Result;
return Redirect(paymentDetails.CheckOutUrl);
```
>Sample Payment Request Object
```
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

## Documentation

### [Developer-Guideline](doc/sp_plugin_developer_guideline.md)

### [Github](https://github.com/shurjopay-plugins)

## Contacts

[Shurjopay](https://shurjopay.com.bd/#contacts)

## License

[MIT](LICENSE)