using Fawaterak.DTOs;
using Fawaterak.Services;
using Microsoft.AspNetCore.Mvc;

namespace Fawaterak.Controllers;

/// <summary>
/// Order management endpoints demonstrating end-to-end order-to-invoice flow
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Consumes("application/json")]
[Produces("application/json")]
public class OrdersController : ControllerBase
{
    private readonly IFawaterakPaymentService _fawaterakPaymentService;

    public OrdersController(IFawaterakPaymentService fawaterakPaymentService)
    {
        _fawaterakPaymentService = fawaterakPaymentService;
    }

    /// <summary>
    /// Create a new order and initialize payment
    /// </summary>
    /// <param name="createOrderDto">Order creation details including cart ID and payment method</param>
    /// <returns>Order details along with payment invoice result</returns>
    /// <response code="200">Order created and payment initialized successfully</response>
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    public async Task<ActionResult<object>> CreateOrder(CreateOrderDto createOrderDto)
    {
        #region Pseudo Code
        // 1. Validate the ShoppingCartId && Payment Method Id
        // 2. Retrieve the shopping cart items from the database or cache
        // 3. validate the items in the cart 
        // 4. Calculate the total price of the items in the cart
        // 5. create an order record in the database with status "Pending"
        // 6. Create the invoice using FawaterakPaymentService
        // 7. Save the invoice id in the order record & order id in the invoice payload
        // 8. Return the order details along with the invoice link to the client 
        #endregion

        #region Create the invoice
        var invoice = new EInvoiceRequestModel()
        {
            Currency = "EGP",
            PaymentMethodId = createOrderDto.PaymentMethodId,
            Customer = new EInvoiceRequestModel.CustomerModel
            {
                // from current logged-in user
                FirstName = "John",
                LastName = "Doe",
                Email = "user@gmail.com",
                CustomerId = "current-user-id",
                Phone = "01122445555"
            },
            CartItems =
            [
                new EInvoiceRequestModel.CartItemModel
                {
                    Name = "item-1",
                    Quantity = 2,
                    Price = 100.0m
                },
                new EInvoiceRequestModel.CartItemModel
                {
                    Name = "plan-2",
                    Quantity = 1,
                    Price = 50.0m
                }
            ],
            PayLoad = new EInvoiceRequestModel.InvoicePayload
            {
                OrderId = "order-id-001"
            },
            RedirectionUrls = new EInvoiceRequestModel.RedirectionUrlsModel
            {
                OnSuccess = "https://domain-of-my-project.com/success",
                OnFailure = "https://domain-of-my-project.com/failure",
                OnPending = "https://domain-of-my-project.com/pending"
            }
        };
        #endregion

        // Call Fawaterak to create the invoice
        var invoiceResult = await _fawaterakPaymentService.GeneralPay(invoice);

        return Ok(new { Message = "Order created successfully", invoice, result = (object)invoiceResult });
    }
}
