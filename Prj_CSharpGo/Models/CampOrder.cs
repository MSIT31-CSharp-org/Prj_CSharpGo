using System;
using System.Collections.Generic;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class CampOrder
    {
        public int CampOrderId { get; set; }
        public int UserId { get; set; }
        public int CampId { get; set; }
        public DateTime? OrderDay { get; set; }
        public int? CampQuantity { get; set; }
        public DateTime? StartDay { get; set; }
        public DateTime? EndDay { get; set; }
        public int? WeekdayPrice { get; set; }
        public int? HolidayPrice { get; set; }
        public int? Peoplenumber { get; set; }
        public int? TotalPrice { get; set; }
        public double? Discount { get; set; }
        public string CampStatus { get; set; }
        public string OrderStatus { get; set; }
        public string PayMethod { get; set; }
        public string Approval { get; set; }

        public virtual Camp Camp { get; set; }
        public virtual User User { get; set; }
    }
}
