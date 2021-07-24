﻿using System;
using System.Collections.Generic;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class Recipe
    {
        public int RecipeId { get; set; }
        public int UserId { get; set; }
        public string RecipeName { get; set; }
        public DateTime? CreateTime { get; set; }
        public DateTime? ModifiedTime { get; set; }
        public DateTime? PublishTime { get; set; }
        public string Preparation { get; set; }
        public string Step { get; set; }
        public string Img { get; set; }
        public string Status { get; set; }
        public string PreparationTime { get; set; }
        public string CookingTime { get; set; }
        public string Yield { get; set; }
    }
}
