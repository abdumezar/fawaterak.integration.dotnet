# Fawaterak .NET Payment Integration

A minimal, production-ready ASP.NET Core (.NET 9) Web API that integrates with the Fawaterak payment gateway. It exposes endpoints to:

- Create invoice links
- Initialize payments for cards, wallets, and Fawry
- Retrieve available payment methods
- Generate iframe hash keys

The solution uses IHttpClientFactory, options binding, and Newtonsoft.Json.

## Contents
- Requirements
- Getting started
- Configuration
- Run
- API reference
- Request examples
- Notes and recommendations

## Requirements
- .NET SDK 9.0+
- A Fawaterak account and API credentials (ApiKey, BaseUrl, ProviderKey)

## Getting started
1) Clone the repo

2) Restore and build
- dotnet restore
- dotnet build -c Release

## Configuration
The service reads configuration from the Fawaterak section. Add these settings either in appsettings.json, environment variables, or user-secrets for local development.

Example appsettings.json snippet:
```json
{
  "Fawaterak": {
    "ApiKey": "YOUR_API_KEY",
    "BaseUrl": "https://staging.fawaterak.com/vX",
    "ProviderKey": "YOUR_PROVIDER_KEY"
  }
}
```

Recommended for development: use the Secret Manager to avoid committing secrets.
- dotnet user-secrets init
- dotnet user-secrets set "Fawaterak:ApiKey" "YOUR_API_KEY"
- dotnet user-secrets set "Fawaterak:BaseUrl" "https://api.fawaterak.com/vX"
- dotnet user-secrets set "Fawaterak:ProviderKey" "YOUR_PROVIDER_KEY"

## Run
From the solution root:
- dotnet run --project Fawaterak/Fawaterak.csproj

OpenAPI document is mapped in Development only at:
- GET /openapi/v1.json

If you need Swagger UI, add Swashbuckle or your preferred UI package and map it accordingly.

## Architecture overview
- Controllers
  - OrdersController: Demonstrates an end-to-end order-to-invoice flow.
  - FawaterakPaymentsController: Thin HTTP layer over IFawaterakPaymentService.
- Services
  - IFawaterakPaymentService: Abstraction for payment operations.
  - FawaterakPaymentService: Implements API calls to Fawaterak using HttpClientFactory.
- Options
  - FawaterakOptions: Binds ApiKey, BaseUrl, ProviderKey from configuration.

## API reference
Base route: /api

- **POST** `/api/fawaterak/invoices`
  - Creates a Fawaterak invoice link and returns URL, invoiceId, and invoiceKey.

- **GET** `/api/fawaterak/payment-methods`
  - Returns the list of available payment methods as reported by Fawaterak.

- **POST** `/api/fawaterak/pay`
  - Initializes a payment for the provided invoice payload. The response model depends on the payment method.

- **GET** `/api/fawaterak/iframe-hash?domain={your-domain}`
  - Generates an HMAC-SHA256 hash for iframe embedding using your ProviderKey and ApiKey.

- **POST** `/api/orders`
  - Example endpoint that builds a realistic invoice payload and calls GeneralPay.

## Request examples
Note: property names below match Newtonsoft.Json annotations used by the service.

Create invoice or pay request body (`EInvoiceRequestModel`):
```json
{
  "payment_method_id": 1,
  "currency": "EGP",
  "customer": {
    "first_name": "John",
    "last_name": "Doe",
    "email": "user@gmail.com",
    "customer_unique_id": "current-user-id",
    "phone": "01122445555"
  },
  "cartItems": [
    { "name": "item-1", "quantity": 2, "price": 100.0 },
    { "name": "plan-2", "quantity": 1, "price": 50.0 }
  ],
  "payLoad": {
    "orderId": "order-id-001"
  },
  "redirectionUrls": {
    "successUrl": "https://example.com/success",
    "failUrl": "https://example.com/failure",
    "pendingUrl": "https://example.com/pending"
  }
}
```

Call examples:
- Initialize payment
  ```bash
  curl -X POST "http://localhost:{PORT}/api/fawaterak/pay" -H "Content-Type: application/json" -d @invoice.json
  ```
- Create invoice link
  ```bash
  curl -X POST "http://localhost:{PORT}/api/fawaterak/invoices" -H "Content-Type: application/json" -d @invoice.json
  ```
- Get payment methods
  ```bash
  curl "http://localhost:{PORT}/api/fawaterak/payment-methods"
  ```
- Generate iframe hash
  ```bash
  curl "http://localhost:{PORT}/api/fawaterak/iframe-hash?domain=yourdomain.com"
  ```

## Notes and recommendations
- Environments: OpenAPI is enabled in Development only (see Program.cs). Set `ASPNETCORE_ENVIRONMENT=Development` when needed.
- Email normalization: The service normalizes dots in the local-part of emails before sending to Fawaterak.
- Payment methods mapping: The service attempts to map the returned payment methods to Card, EWallet, or Fawry based on name matching. Prefer calling `GET /payment-methods` first and choose the id returned by Fawaterak.
- HttpClientFactory: All HTTP calls are made via `IHttpClientFactory`. For enterprise scenarios, register named/typed clients, retry policies, and logging as needed.
- Security: Never commit real keys. Use environment variables or Secret Manager in development and a secure store in production.

## License
MIT or per your organization’s policy.
