using Nop.Plugin.Misc.WebApi.Framework.Dto;
using Nop.Plugin.Misc.WebApi.Frontend.Dto.Catalog;
using Nop.Plugin.Misc.WebApi.Frontend.Dto.Product;
using static Nop.Web.Models.ShoppingCart.MiniShoppingCartModel;

namespace Nop.Plugin.Misc.WebApi.Frontend.Dto.ShoppingCart;

public partial class MiniShoppingCartModelDto : ModelDto
{
    public IList<MiniShoppingCartItemModelDto> Items { get; set; }

    public int TotalProducts { get; set; }

    public string SubTotal { get; set; }

    public decimal SubTotalValue { get; set; }

    public bool DisplayShoppingCartButton { get; set; }

    public bool DisplayCheckoutButton { get; set; }

    public bool CurrentCustomerIsGuest { get; set; }

    public bool AnonymousCheckoutAllowed { get; set; }

    public bool ShowProductImages { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal SubTotalValueWithDiscount { get; set; }
    public DiscountBoxModelDto DiscountBox { get; set; }
    public OrderTotalsModelDto OrderTotalsModel { get; set; }
    public List<DiscountInfoDto> AppliedDiscountDetails { get; set; }
    public decimal ShippingAmountInDollars { get; set; }


    #region

    public partial class MiniShoppingCartItemModelDto : ModelWithIdDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        public int Quantity { get; set; }

        public string UnitPrice { get; set; }
        public decimal UnitPriceValue { get; set; }

        public string AttributeInfo { get; set; }

        public PictureModelDto Picture { get; set; }
        public ProductSpecificationModelDto ProductSpecificationModel { get; set; }
    }

    public partial class DiscountInfoDto : ModelWithIdDto
    {
        /// <summary>
        /// Gets or sets the name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the admin comment
        /// </summary>
        public string AdminComment { get; set; }

        /// <summary>
        /// Gets or sets the discount type identifier
        /// </summary>
        public int DiscountTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to use percentage
        /// </summary>
        public bool UsePercentage { get; set; }

        /// <summary>
        /// Gets or sets the discount percentage
        /// </summary>
        public decimal DiscountPercentage { get; set; }

        /// <summary>
        /// Gets or sets the discount amount
        /// </summary>
        public decimal DiscountAmount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether discount requires coupon code
        /// </summary>
        public bool RequiresCouponCode { get; set; }

        /// <summary>
        /// Gets or sets the coupon code
        /// </summary>
        public string CouponCode { get; set; }

        public decimal DiscountAppliedInCart { get; set; }

    }


    #endregion
}