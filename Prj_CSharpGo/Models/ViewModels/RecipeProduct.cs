using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prj_CSharpGo.Models.ViewModels
{
    public class RecipeProduct
    {
        public IEnumerable<Recipe> Recipes { get; set; }
        public IEnumerable<Association> Associations { get; set; }
        public IEnumerable<Product> Products { get; set; }

        public Association Association { get; set; }
        public Recipe Recipe { get; set; }
        public Product Product { get; set; }
    }
}
