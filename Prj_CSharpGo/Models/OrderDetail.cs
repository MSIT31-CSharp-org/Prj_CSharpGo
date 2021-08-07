using System;
using System.Collections.Generic;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class OrderDetail
    {
        public int OrderId { get; set; }
        public string ProductId { get; set; }
        public int? UnitPrice { get; set; }
        public short? Quantity { get; set; }
        public double? Discount { get; set; }
        public string Commets { get; set; }
        public string Approval { get; set; }
        public int Odpk { get; set; }

        public virtual Order Order { get; set; }
        public virtual Product Product { get; set; }
    }
}
