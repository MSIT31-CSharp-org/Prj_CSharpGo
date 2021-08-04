using System;
using System.Collections.Generic;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class ShoppingCart
    {
        public int ShCartId { get; set; }
        public int UserId { get; set; }
        public string ProductId { get; set; }
        public short? Quantity { get; set; }
        public string Status { get; set; }
        public string ProductName { get; set; }
        public int? UnitPrice { get; set; }

        public virtual Product Product { get; set; }
        public virtual User User { get; set; }
    }
}
