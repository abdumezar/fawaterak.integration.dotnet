using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fawaterak.DTOs;

/// <summary>
/// Invoice request model for creating payments with Fawaterak
/// </summary>
public class EInvoiceRequestModel
{
    /// <summary>
    /// Payment method ID from Fawaterak
    /// </summary>
    [JsonProperty("payment_method_id")]
    public int? PaymentMethodId { get; set; }

    /// <summary>
    /// Customer information
    /// </summary>
    [JsonProperty("customer")]
    [Required]
    public required CustomerModel Customer { get; set; }

    /// <summary>
    /// List of items in the cart
    /// </summary>
    [JsonProperty("cartItems")]
    [MinLength(1)]
    [Required]
    public List<CartItemModel> CartItems { get; set; }

    /// <summary>
    /// Total cart amount (calculated automatically)
    /// </summary>
    [JsonProperty("cartTotal")]
    public decimal CartTotal => CartItems.Sum(item => item.Price * item.Quantity);

    /// <summary>
    /// Currency code (e.g., EGP, USD)
    /// </summary>
    [JsonProperty("currency")]
    [Required]
    [StringLength(3, MinimumLength = 3)]
    public string Currency { get; set; } = "EGP";

    /// <summary>
    /// Additional payload data
    /// </summary>
    [JsonProperty("payLoad")]
    public InvoicePayload? PayLoad { get; set; }

    /// <summary>
    /// URLs for payment result redirections
    /// </summary>
    [JsonProperty("redirectionUrls")]
    public RedirectionUrlsModel? RedirectionUrls { get; set; }

    /// <summary>
    /// Additional payload for the invoice
    /// </summary>
    public class InvoicePayload
    {
        /// <summary>
        /// Your internal order ID
        /// </summary>
        public string OrderId { get; set; }
    }

    /// <summary>
    /// Customer information
    /// </summary>
    public class CustomerModel
    {
        /// <summary>
        /// Unique customer identifier in your system
        /// </summary>
        [JsonProperty("customer_unique_id")]
        public string? CustomerId { get; set; }

        /// <summary>
        /// Customer's first name
        /// </summary>
        [JsonProperty("first_name")]
        [Required]
        public required string FirstName { get; set; }

        /// <summary>
        /// Customer's last name
        /// </summary>
        [JsonProperty("last_name")]
        [Required]
        public required string LastName { get; set; }

        /// <summary>
        /// Customer's email address
        /// </summary>
        [JsonProperty("email")]
        [EmailAddress]
        public string? Email { get; set; }

        /// <summary>
        /// Customer's phone number
        /// </summary>
        [JsonProperty("phone")]
        [Phone]
        public string? Phone { get; set; }
    }

    /// <summary>
    /// Cart item details
    /// </summary>
    public class CartItemModel
    {
        /// <summary>
        /// Item name
        /// </summary>
        [JsonProperty("name")]
        [Required]
        public string Name { get; set; }

        /// <summary>
        /// Item price per unit
        /// </summary>
        [JsonProperty("price")]
        [Range(0.01, double.MaxValue)]
        public decimal Price { get; set; }

        /// <summary>
        /// Quantity of this item
        /// </summary>
        [JsonProperty("quantity")]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }
    }

    /// <summary>
    /// Redirection URLs after payment completion
    /// </summary>
    public class RedirectionUrlsModel
    {
        /// <summary>
        /// URL to redirect to on successful payment
        /// </summary>
        [JsonProperty("successUrl")]
        [Url]
        public string? OnSuccess { get; set; }

        /// <summary>
        /// URL to redirect to on failed payment
        /// </summary>
        [JsonProperty("failUrl")]
        [Url]
        public string? OnFailure { get; set; }

        /// <summary>
        /// URL to redirect to on pending payment
        /// </summary>
        [JsonProperty("pendingUrl")]
        [Url]
        public string? OnPending { get; set; }
    }
}

/// <summary>
/// Response from Fawaterak when creating an invoice
/// </summary>
public class EInvoiceResponseModel
{
    /// <summary>
    /// Response status
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// Invoice data
    /// </summary>
    [JsonProperty("data")]
    public EInvoiceResponseDataModel Data { get; set; }

    /// <summary>
    /// Invoice response data
    /// </summary>
    public class EInvoiceResponseDataModel
    {
        /// <summary>
        /// Payment URL for the invoice
        /// </summary>
        [JsonProperty("url")]
        public string Url { get; set; }

        /// <summary>
        /// Unique invoice ID from Fawaterak
        /// </summary>
        [JsonProperty("invoiceId")]
        public string InvoiceId { get; set; }

        /// <summary>
        /// Invoice key for verification
        /// </summary>
        [JsonProperty("invoiceKey")]
        public string InvoiceKey { get; set; }
    }
}