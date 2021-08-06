using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prj_CSharpGo.Models.ViewModels
{
    public class UpImg
    {
        public string ProductId { get; set; }
        public string Img { get; set; }
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

        public IFormFile ImageFile { get; set; }
    }
}
