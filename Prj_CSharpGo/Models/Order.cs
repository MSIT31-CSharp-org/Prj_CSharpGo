using System;
using System.Collections.Generic;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int? TotalPrice { get; set; }
        public string PayMethod { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Approval { get; set; }
        public string Address { get; set; }
        public string UserName { get; set; }

        public virtual User User { get; set; }
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
