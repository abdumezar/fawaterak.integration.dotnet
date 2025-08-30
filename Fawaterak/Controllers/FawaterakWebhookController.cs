using Fawaterak.DTOs;
using Fawaterak.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fawaterak.Controllers;

/// <summary>
/// Webhook endpoints for Fawaterak payment notifications
/// </summary>
[AllowAnonymous]
[ApiController]
[Route("api/fawaterak/webhooks")]
[Consumes("application/json")]
[Produces("application/json")]
public class FawaterakWebhooksController : ControllerBase
{
    private readonly IFawaterakPaymentService _payments;

    public FawaterakWebhooksController(IFawaterakPaymentService payments)
    {
        _payments = payments;
    }

    /// <summary>
    /// Handle successful payment notification from Fawaterak
    /// </summary>
    /// <param name="model">Payment webhook data with invoice details and verification hash</param>
    /// <returns>Confirmation message</returns>
    /// <response code="200">Webhook processed successfully</response>
    /// <response code="401">Invalid webhook signature</response>
    [HttpPost("paid_json")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<string> WebhookPaid([FromBody] WebHookModel model)
    {
        var valid = _payments.VerifyWebhook(model);
        if (!valid) return Unauthorized();

        // Handle the payment logic here

        return Ok("got it!");
    }

    /// <summary>
    /// Handle payment cancellation notification from Fawaterak
    /// </summary>
    /// <param name="model">Cancellation webhook data with reference ID and verification hash</param>
    /// <returns>Acknowledgment of cancellation</returns>
    /// <response code="200">Cancellation webhook processed successfully</response>
    /// <response code="401">Invalid webhook signature</response>
    [HttpPost("cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult WebhookCancel([FromBody] CancelTransactionModel model)
    {
        var valid = _payments.VerifyCancelTransaction(model);
        if (!valid) return Unauthorized();

        // Handle the cancellation logic here

        return Ok();
    }

    /// <summary>
    /// Handle failed payment notification from Fawaterak
    /// </summary>
    /// <param name="model">Failed payment webhook data with reference ID and verification hash</param>
    /// <returns>Acknowledgment of failure</returns>
    /// <response code="200">Failure webhook processed successfully</response>
    /// <response code="401">Invalid webhook signature</response>
    [HttpPost("failed")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public IActionResult WebhookFaild([FromBody] CancelTransactionModel model)
    {
        var valid = _payments.VerifyCancelTransaction(model);
        if (!valid) return Unauthorized();

        // Handle the failed logic here

        return Ok();
    }
}