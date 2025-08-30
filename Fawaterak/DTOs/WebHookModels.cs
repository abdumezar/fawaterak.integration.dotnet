using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Fawaterak.DTOs;

/// <summary>
/// Webhook payload data
/// </summary>
public class WebhookPayload
{
    /// <summary>
    /// Order ID from your system
    /// </summary>
    [JsonPropertyName("OrderId")]
    public string? OrderId { get; set; }
}

/// <summary>
/// Webhook model for successful payment notifications
/// </summary>
public class WebHookModel
{
    /// <summary>
    /// Invoice ID from Fawaterak
    /// </summary>
    [JsonPropertyName("invoice_id")]
    [Required]
    public long InvoiceId { get; set; }

    /// <summary>
    /// Invoice key from Fawaterak
    /// </summary>
    [JsonPropertyName("invoice_key")]
    [Required]
    public string InvoiceKey { get; set; }

    /// <summary>
    /// Verification hash key
    /// </summary>
    [JsonPropertyName("hashKey")]
    [Required]
    public string HashKey { get; set; }

    /// <summary>
    /// Payment method used for the transaction
    /// </summary>
    [JsonPropertyName("payment_method")]
    [Required]
    public string PaymentMethod { get; set; }

    /// <summary>
    /// Current status of the invoice
    /// </summary>
    [JsonPropertyName("invoice_status")]
    [Required]
    public string InvoiceStatus { get; set; }

    /// <summary>
    /// Payload as JSON string
    /// </summary>
    [JsonPropertyName("pay_load")]
    public string? PayloadString { get; set; }

    /// <summary>
    /// Parsed payload data
    /// </summary>
    //[JsonPropertyName("pay_load")]
    public WebhookPayload? Payload { get; set; }
}

/// <summary>
/// Webhook model for cancelled or failed transactions
/// </summary>
public class CancelTransactionModel
{
    /// <summary>
    /// Verification hash key
    /// </summary>
    [JsonPropertyName("hashKey")]
    [Required]
    public string HashKey { get; set; }

    /// <summary>
    /// Transaction reference ID
    /// </summary>
    [JsonPropertyName("referenceId")]
    [Required]
    public string ReferenceId { get; set; }

    /// <summary>
    /// Transaction status
    /// </summary>
    [JsonPropertyName("status")]
    [Required]
    public string Status { get; set; }

    /// <summary>
    /// Payment method used for the transaction
    /// </summary>
    [JsonPropertyName("paymentMethod")]
    [Required]
    public string PaymentMethod { get; set; }

    /// <summary>
    /// Additional payload data
    /// </summary>
    [JsonPropertyName("pay_load")]
    public object? PayLoad { get; set; }
}

/// <summary>
/// Failed webhook model (form-encoded)
/// </summary>
public class FaliledWebHook
{
    /// <summary>
    /// Invoice ID from failed transaction
    /// </summary>
    [FromForm(Name = "invoice_id")]
    [Required]
    public long InvoiceId { get; set; }

    /// <summary>
    /// Invoice key from failed transaction
    /// </summary>
    [FromForm(Name = "invoice_key")]
    [Required]
    public string InvoiceKey { get; set; }

    /// <summary>
    /// Error message describing the failure
    /// </summary>
    [FromForm(Name = "errorMessage")]
    [Required]
    public string ErrorMessage { get; set; }
}