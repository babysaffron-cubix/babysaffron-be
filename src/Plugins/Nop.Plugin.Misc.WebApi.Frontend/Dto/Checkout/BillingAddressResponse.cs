namespace Nop.Plugin.Misc.WebApi.Frontend.Dto.Checkout;

public partial class BillingAddressResponse : CheckoutRedirectResponse
{
    public CheckoutBillingAddressModelDto Model { get; set; }
}