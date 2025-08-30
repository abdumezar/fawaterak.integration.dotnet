using Fawaterak.DTOs;
using static Fawaterak.DTOs.EInvoiceResponseModel;
using static Fawaterak.DTOs.PaymentMethodsResponse;

namespace Fawaterak.Services;

public interface IFawaterakPaymentService
{
    // Create EInvoice Link
    Task<EInvoiceResponseDataModel?> CreateEInvoiceAsync(EInvoiceRequestModel eInvoice);

    // Payment Integration
    Task<IList<PaymentMethod>?> GetPaymentMethods();
    Task<BasePaymentDataResponse?> GeneralPay(EInvoiceRequestModel invoice);

    // WebHook Verification
    bool VerifyWebhook(WebHookModel webHook);
    bool VerifyCancelTransaction(CancelTransactionModel cancelTransaction);
    bool VerifyApiKeyTransaction(string apiKey);

    // HashKey
    string GenerateHashKeyForIFrame(string domain);
}
