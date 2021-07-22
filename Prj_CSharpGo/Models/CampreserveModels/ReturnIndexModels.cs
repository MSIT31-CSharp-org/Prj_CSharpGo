using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prj_CSharpGo.Models.CampreserveModels
{
    public class ReturnIndexModels
    {
        public int CampId { get; set; }
        public string Img { get; set; }
        public string CampName { get; set; }
        public int CampQuantity { get; set; }
        public int? WeekdayPrice { get; set; }
        public int? HolidayPrice { get; set; }

        public List<Camp> CampList { get; set; }
    }
}
