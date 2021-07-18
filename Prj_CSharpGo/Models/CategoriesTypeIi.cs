using System;
using System.Collections.Generic;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class CategoriesTypeIi
    {
        public string CategoryTypeI { get; set; }
        public int? CategoryType2 { get; set; }

        public virtual Category CategoryTypeINavigation { get; set; }
    }
}
