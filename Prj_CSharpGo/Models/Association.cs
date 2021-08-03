using System;
using System.Collections.Generic;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class Association
    {
        public int RecipeId { get; set; }
        public string ProductId { get; set; }
        public string Description { get; set; }
        public int Sequence { get; set; }

        public virtual Product Product { get; set; }
        public virtual Recipe Recipe { get; set; }
    }
}
