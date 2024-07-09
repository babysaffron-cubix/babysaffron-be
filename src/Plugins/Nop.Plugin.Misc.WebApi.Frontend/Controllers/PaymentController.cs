using Microsoft.AspNetCore.Mvc;
using Nop.Plugin.Misc.WebApi.Framework;
using Nop.Plugin.Misc.WebApi.Framework.Controllers;
using Nop.Plugin.Misc.WebApi.Framework.Helpers;
using Nop.Plugin.Misc.WebApi.Framework.Models;
using Nop.Plugin.Misc.WebApi.Frontend.Dto.Orders;
using Nop.Plugin.Misc.WebApi.Frontend.Models;
using Nop.Plugin.Misc.WebApi.Frontend.Services;
using Nop.Services.Payments;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Nop.Plugin.Misc.WebApi.Frontend.Controllers;


public class PaymentController : BaseNopWebApiFrontendController
{

    #region Fields
    protected readonly IRazorpayPaymentService _razorpayPaymentService;


    #endregion

    #region Ctor

    public PaymentController(IRazorpayPaymentService razorpayPaymentService)
    {
        _razorpayPaymentService = razorpayPaymentService;
    }

    #endregion

    #region methods

    [HttpPost]
    [ProducesResponseType(typeof(String), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> CreateRazorpayOrder(int dbOrderId)
    {
        var response = await _razorpayPaymentService.CreateOrder(dbOrderId);
        return Ok(response);
    }


    [HttpPost]
    [ProducesResponseType(typeof(String), StatusCodes.Status200OK)]
    public virtual async Task<IActionResult> VerifyRazorpayPayment([FromBody] RazorpayPaymentVerificationRequest request)
    {
        var response = await _razorpayPaymentService.VerifyPayment(request);
        return Ok(response);
    }

    #endregion
}

