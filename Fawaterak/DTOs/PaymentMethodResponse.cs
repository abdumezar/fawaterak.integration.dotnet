using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Fawaterak.DTOs;

/// <summary>
/// Payment methods response from Fawaterak API
/// </summary>
public class PaymentMethodsResponse
{
    /// <summary>
    /// Response status
    /// </summary>
    [JsonProperty("status")]
    public string Status { get; set; }

    /// <summary>
    /// List of available payment methods
    /// </summary>
    [JsonProperty("data")]
    public List<PaymentMethod> Data { get; set; }

    /// <summary>
    /// Payment method details
    /// </summary>
    public class PaymentMethod
    {
        /// <summary>
        /// Internal payment method ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Fawaterak payment method ID
        /// </summary>
        [JsonProperty("paymentId")]
        public int PaymentId { get; set; }

        /// <summary>
        /// Payment method name in English
        /// </summary>
        [JsonProperty("name_en")]
        public string NameEn { get; set; }

        /// <summary>
        /// Payment method name in Arabic
        /// </summary>
        [JsonProperty("name_ar")]
        public string NameAr { get; set; }

        /// <summary>
        /// Redirect URL for this payment method
        /// </summary>
        [JsonProperty("redirect")]
        public string Redirect { get; set; }

        /// <summary>
        /// Logo URL for this payment method
        /// </summary>
        [JsonProperty("logo")]
        public string Logo { get; set; }
    }
}