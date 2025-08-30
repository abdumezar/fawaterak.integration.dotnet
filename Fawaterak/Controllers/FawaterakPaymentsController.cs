using Fawaterak.DTOs;
using Fawaterak.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fawaterak.Controllers;

/// <summary>
/// Fawaterak payment integration endpoints
/// </summary>
[ApiController]
[Route("api/fawaterak")]
[Consumes("application/json")]
[Produces("application/json")]
public class FawaterakPaymentsController : ControllerBase
{
    private readonly IFawaterakPaymentService _payments;

    public FawaterakPaymentsController(IFawaterakPaymentService payments)
    {
        _payments = payments;
    }

    /// <summary>
    /// Create a Fawaterak invoice link
    /// </summary>
    /// <param name="request">Invoice request details including customer, cart items, and redirection URLs</param>
    /// <returns>Invoice URL, ID, and key for payment processing</returns>
    /// <response code="200">Invoice created successfully</response>
    /// <response code="400">Invalid request data</response>
    [HttpPost("invoices")]
    [ProducesResponseType(typeof(EInvoiceResponseModel.EInvoiceResponseDataModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EInvoiceResponseModel.EInvoiceResponseDataModel>> CreateInvoice([FromBody] EInvoiceRequestModel request)
    {
        var data = await _payments.CreateEInvoiceAsync(request);
        if (data is null) return BadRequest();
        return Ok(data);
    }

    /// <summary>
    /// Get available payment methods from Fawaterak
    /// </summary>
    /// <returns>List of available payment methods with their IDs, names, and logos</returns>
    /// <response code="200">Payment methods retrieved successfully</response>
    /// <response code="204">No payment methods available</response>
    [HttpGet("payment-methods")]
    [ProducesResponseType(typeof(IList<PaymentMethodsResponse.PaymentMethod>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult<IList<PaymentMethodsResponse.PaymentMethod>>> GetPaymentMethods()
    {
        var result = await _payments.GetPaymentMethods();
        if (result is null || result.Count == 0) return NoContent();
        return Ok(result);
    }

    /// <summary>
    /// Initialize payment for cards, wallets, or Fawry
    /// </summary>
    /// <param name="invoice">Invoice details with selected payment method</param>
    /// <returns>Payment response data depending on the payment method (card redirect URL, Fawry code, or wallet QR code)</returns>
    /// <response code="200">Payment initialized successfully</response>
    /// <response code="400">Invalid payment request</response>
    [HttpPost("pay")]
    [ProducesResponseType(typeof(BasePaymentDataResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<BasePaymentDataResponse>> Pay([FromBody] EInvoiceRequestModel invoice)
    {
        var result = await _payments.GeneralPay(invoice);
        if (result is null) return BadRequest();
        return Ok(result);
    }

    /// <summary>
    /// Generate HMAC-SHA256 hash for iframe embedding
    /// </summary>
    /// <param name="domain">Your domain for iframe integration</param>
    /// <returns>Generated hash key for secure iframe embedding</returns>
    /// <response code="200">Hash key generated successfully</response>
    [HttpGet("iframe-hash")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public ActionResult<string> IFrameHash([FromQuery] string domain)
    {
        var result = _payments.GenerateHashKeyForIFrame(domain);
        return Ok(result);
    }
}

