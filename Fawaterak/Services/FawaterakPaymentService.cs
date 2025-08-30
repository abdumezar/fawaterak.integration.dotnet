using Fawaterak.DTOs;
using Fawaterak.Options;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using static Fawaterak.DTOs.EInvoiceResponseModel;
using static Fawaterak.DTOs.PaymentMethodsResponse;

namespace Fawaterak.Services;

public class FawaterakPaymentService : IFawaterakPaymentService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string ApiKey;
    private readonly string BaseUrl;
    private readonly string ProviderKey;

    public FawaterakPaymentService(IHttpClientFactory httpClientFactory, IOptions<FawaterakOptions> options)
    {
        _httpClientFactory = httpClientFactory;
        var cfg = options.Value;
        ApiKey = cfg.ApiKey;
        BaseUrl = cfg.BaseUrl;
        ProviderKey = cfg.ProviderKey;
    }

    #region Create EInvoice Link
    public async Task<EInvoiceResponseDataModel?> CreateEInvoiceAsync(EInvoiceRequestModel eInvoice)
    {
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/createInvoiceLink");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
        request.Content = new StringContent(JsonConvert.SerializeObject(eInvoice), Encoding.UTF8, "application/json");

        var response = await client.SendAsync(request);
        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var _response = JsonConvert.DeserializeObject<EInvoiceResponseModel>(responseContent);
            return _response!.Data;
        }

        return null;
    }
    #endregion

    #region Payment Integration

    #region Payment Methods
    public async Task<IList<PaymentMethod>?> GetPaymentMethods()
    {
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Get, $"{BaseUrl}/getPaymentmethods");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
        request.Content = new StringContent(string.Empty, Encoding.UTF8, "application/json");
        var result = await client.SendAsync(request);

        if (result.IsSuccessStatusCode)
        {
            var responseContent = await result.Content.ReadAsStringAsync();
            var _response = JsonConvert.DeserializeObject<PaymentMethodsResponse>(responseContent);

            foreach (var item in _response.Data)
            {
                item.Id = (int)(await GetPaymentMethod(item.PaymentId, _response.Data));
            }
            return _response!.Data!;
        }

        return null;
    }

    public async Task<PaymentMethods> GetPaymentMethod(int paymentMethodId, IList<PaymentMethod>? paymentMethods = null)
    {
        var methods = paymentMethods ?? await GetPaymentMethods();

        var method = methods?.FirstOrDefault(x => x.PaymentId == paymentMethodId);
        if (string.IsNullOrWhiteSpace(method?.NameEn))
            return PaymentMethods.Card;

        var name = method.NameEn;

        if (name.Contains("Fawry", StringComparison.OrdinalIgnoreCase))
            return PaymentMethods.Fawry;

        if (name.Contains("Meeza", StringComparison.OrdinalIgnoreCase) ||
            name.Contains("Wallet", StringComparison.OrdinalIgnoreCase))
            return PaymentMethods.EWallet;

        return PaymentMethods.Card;
    }

    #endregion

    public async Task<BasePaymentDataResponse?> GeneralPay(EInvoiceRequestModel invoice)
    {
        var client = _httpClientFactory.CreateClient();
        var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}/invoiceInitPay");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
        request.Content = new StringContent(JsonConvert.SerializeObject(invoice), Encoding.UTF8, "application/json");
        invoice.Customer.Email = NormalizeEmailDots(invoice.Customer.Email);
        var response = await client.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync();
            var method = await GetPaymentMethod(invoice.PaymentMethodId.Value);

            switch (method)
            {
                case PaymentMethods.Fawry:
                    var _fawryResponse = JsonConvert.DeserializeObject<FawryPaymentResponse>(responseContent);
                    return _fawryResponse!.Data;
                case PaymentMethods.EWallet:
                    var _meezaResponse = JsonConvert.DeserializeObject<MeezaPaymentResponse>(responseContent);
                    return _meezaResponse!.Data;
                case PaymentMethods.Card:
                    var _cardResponse = JsonConvert.DeserializeObject<CardPaymentResponse>(responseContent);
                    return _cardResponse!.Data;
                default:
                    return null;
            }
        }

        var errorResponseContent = await response.Content.ReadAsStringAsync();
        return null;
    }
    #endregion

    #region WebHook Verification
    public bool VerifyWebhook(WebHookModel webHook)
    {
        var generatedHashKey =
            GenerateHashKeyForWebhookVerification(webHook.InvoiceId, webHook.InvoiceKey, webHook.PaymentMethod);
        return generatedHashKey == webHook.HashKey;
    }

    public bool VerifyCancelTransaction(CancelTransactionModel cancelTransaction)
    {
        var generatedHashKey = GenerateHashKeyForCancelTransaction(cancelTransaction.ReferenceId, cancelTransaction.PaymentMethod);
        return generatedHashKey == cancelTransaction.HashKey;
    }

    public bool VerifyApiKeyTransaction(string apiKey)
    {
        return apiKey == ApiKey;
    }
    #endregion

    #region Generate HashKey
    public string GenerateHashKeyForIFrame(string domain)
    {
        var queryParam = $"Domain={domain}&ProviderKey={ProviderKey}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(ApiKey));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryParam));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    private string GenerateHashKeyForWebhookVerification(long invoiceId, string invoiceKey, string paymentMethod)
    {
        var queryParam = $"InvoiceId={invoiceId}&InvoiceKey={invoiceKey}&PaymentMethod={paymentMethod}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(ApiKey));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryParam));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    private string GenerateHashKeyForCancelTransaction(string referenceId, string paymentMethod)
    {
        var queryParam = $"referenceId={referenceId}&PaymentMethod={paymentMethod}";
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(ApiKey));
        var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(queryParam));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }

    private static string NormalizeEmailDots(string email)
    {
        if (string.IsNullOrEmpty(email)) return email;

        int atIndex = email.IndexOf('@');
        if (atIndex < 0) return email;

        string localPart = email.Substring(0, atIndex);
        string domainPart = email.Substring(atIndex);

        localPart = System.Text.RegularExpressions.Regex.Replace(localPart, @"\.+", ".");

        return localPart + domainPart;
    }
    #endregion
}