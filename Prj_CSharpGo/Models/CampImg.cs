using System;
using System.Collections.Generic;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class CampImg
    {
        public int? CampId { get; set; }
        public string Img { get; set; }

        public virtual Camp Camp { get; set; }
    }
}
