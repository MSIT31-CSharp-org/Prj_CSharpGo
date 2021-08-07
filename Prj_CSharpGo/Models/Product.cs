using System;
using System.Collections.Generic;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class Product
    {
        public Product()
        {
            Associations = new HashSet<Association>();
            OrderDetails = new HashSet<OrderDetail>();
            ProductImgs = new HashSet<ProductImg>();
            ShoppingCarts = new HashSet<ShoppingCart>();
        }

        public string ProductId { get; set; }
        public string CategoryId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string Specification { get; set; }
        public int? Cost { get; set; }
        public int? UnitPrice { get; set; }
        public short? UnitInStock { get; set; }
        public string Status { get; set; }
        public string Approval { get; set; }
        public string CategoryType { get; set; }

        public virtual Category Category { get; set; }
        public virtual ICollection<Association> Associations { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<ProductImg> ProductImgs { get; set; }
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
    }
}
