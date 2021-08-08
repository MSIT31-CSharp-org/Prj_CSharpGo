using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prj_CSharpGo.Models.ViewModels
{
    public class VCampImg
    {
        
        public int CampId { get; set; }
        public string CampName { get; set; }
        public string CampSize { get; set; }
        public int? CampQuantity { get; set; }
        public int? WeekdayPrice { get; set; }
        public int? HolidayPrice { get; set; }
        public int? LimitPeople { get; set; }
        public string Description { get; set; }
        public string Approval { get; set; }
        public int? PlusPrice { get; set; }
        public string Ahref { get; set; }
        public string Img { get; set; }
        public IFormFile ImgName { get; set; }
    }
}
