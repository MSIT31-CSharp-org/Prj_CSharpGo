using System;
using System.Collections.Generic;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class RecipeImg
    {
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public string Message { get; set; }
        public string Img { get; set; }

        public virtual Recipe Recipe { get; set; }
        public virtual User User { get; set; }
    }
}
