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

        public IFormFile ImageFile { get; set; }
    }
}
