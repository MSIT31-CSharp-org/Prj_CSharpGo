using System;
using System.Collections.Generic;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class CategoriesTypeI
    {
        public string CategoryId { get; set; }
        public int? CategoryTypeI { get; set; }

        public virtual Category Category { get; set; }
    }
}
