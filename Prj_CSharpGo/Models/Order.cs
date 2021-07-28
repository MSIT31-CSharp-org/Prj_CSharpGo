using System;
using System.Collections.Generic;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class Order
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int? TotalPrice { get; set; }
        public string PayMethod { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Approval { get; set; }

        public virtual User User { get; set; }

        public static implicit operator Order(List<Order> v)
        {
            throw new NotImplementedException();
        }
    }
}
