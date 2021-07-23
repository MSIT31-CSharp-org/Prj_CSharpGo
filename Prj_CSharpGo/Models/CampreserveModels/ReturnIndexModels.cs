using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prj_CSharpGo.Models.CampreserveModels
{
    public class ReturnIndexModels
    {
   
        public CampreserveUserFilterModel userFilterModel { get; set; }
        public List<Camp> CampList { get; set; }
    }
    public class CampreserveUserFilterModel
    {
        public int CampId { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int CampQuantity { get; set; }
    }
    public class CampreserveOrderModel
    {
        public int CampId { get; set; }
        public string CampName { get; set; }
        public int CampQuantity { get; set; }
        public DateTime StartDay { get; set; }
        public DateTime EndDay { get; set; }
        public int? TotalPricesmall { get; set; }
        public int HolidayPrice { get; set; }
        public int WeekdayPrice { get; set; }
        public int? PlusPrice { get; set; }
        public string CampStatus { get; set; }
        public string OrderStatus { get; set; }
        public string PayMethod { get; set; }
        public string Approval { get; set; }
        public int? TotalPricebig { get; set; }
    }
}
