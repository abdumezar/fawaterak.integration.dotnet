using System.ComponentModel.DataAnnotations;

namespace Fawaterak.DTOs
{
    /// <summary>
    /// Request model for creating a new order
    /// </summary>
    public class CreateOrderDto
    {
        /// <summary>
        /// Shopping cart identifier
        /// </summary>
        [Required]
        public string CartId { get; set; }

        /// <summary>
        /// Payment method ID from Fawaterak
        /// </summary>
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Payment method ID must be a positive integer")]
        public int PaymentMethodId { get; set; }
    }
}
