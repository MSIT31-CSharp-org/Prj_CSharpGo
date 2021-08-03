using System;
using System.Collections.Generic;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class ProductImg
    {
        public string ProductId { get; set; }
        public string Img { get; set; }

        public virtual Product Product { get; set; }
    }
}
