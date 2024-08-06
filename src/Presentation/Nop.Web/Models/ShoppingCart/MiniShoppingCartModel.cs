using Nop.Core.Domain.Discounts;
using Nop.Web.Framework.Models;
using Nop.Web.Models.Catalog;
using Nop.Web.Models.Media;
using static Nop.Web.Models.ShoppingCart.ShoppingCartModel;

namespace Nop.Web.Models.ShoppingCart;

public partial record MiniShoppingCartModel : BaseNopModel
{
    public MiniShoppingCartModel()
    {
        Items = new List<ShoppingCartItemModel>();
    }

    public IList<ShoppingCartItemModel> Items { get; set; }
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
    public DiscountBoxModel DiscountBox { get; set; }

    public OrderTotalsModel OrderTotalsModel { get; set; }
    public List<DiscountInfo> AppliedDiscountDetails { get; set; }
    public decimal ShippingAmountInDollars { get; set; }

    #region Nested Classes

    public partial record ShoppingCartItemModel : BaseNopEntityModel
    {
        public ShoppingCartItemModel()
        {
            Picture = new PictureModel();
            ProductSpecificationModel = new ProductSpecificationModel();
        }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductSeName { get; set; }

        public int Quantity { get; set; }

        public string UnitPrice { get; set; }
        public decimal UnitPriceValue { get; set; }

        public string AttributeInfo { get; set; }

        public PictureModel Picture { get; set; }

        //specification attributes
        public ProductSpecificationModel ProductSpecificationModel { get; set; }
    }

    public partial record DiscountInfo : BaseNopEntityModel
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

        public string DiscountAppliedInCart { get; set; }

    }



    #endregion
}