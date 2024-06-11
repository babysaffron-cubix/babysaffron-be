namespace Nop.Plugin.Misc.WebApi.Frontend.Dto.Checkout;

public partial class PaymentInfoResponse : CheckoutRedirectResponse
{
    public CheckoutPaymentInfoModelDto CheckoutPaymentInfoModel { get; set; }
}