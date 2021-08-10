using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prj_CSharpGo.Models.ShopCartViewModels
{
    public class CartView
    {
        public int ShCartId { get; set; }
        public int UserId { get; set; }
        public string ProductId { get; set; }
        public short? Quantity { get; set; }
        public string Status { get; set; }
        public string ProductName { get; set; }
        public int? UnitPrice { get; set; }

        public string CategoryId { get; set; }
        public string ProductDescription { get; set; }
        public string Specification { get; set; }
        public int? Cost { get; set; }
        public short? UnitInStock { get; set; }
        public string Approval { get; set; }
        public string CategoryType { get; set; }

        public string Img { get; set; }

        public int OrderId { get; set; }
        public int? TotalPrice { get; set; }
        public string PayMethod { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }

        public double? Discount { get; set; }
        public string Commets { get; set; }

        public virtual IEnumerable<Product> Products { get; set; }
        public virtual IEnumerable<ProductImg> ProductImgs { get; set; }

        public virtual IEnumerable<OrderDetail> OrderDetails { get; set; }

        public virtual IEnumerable<ShoppingCart> ShoppingCarts { get; set; }

    }
}
