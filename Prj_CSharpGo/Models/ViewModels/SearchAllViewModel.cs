using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;


namespace Prj_CSharpGo.Models
{
    // 全站搜尋
    public class SearchAllViewModel
    {
        // 關鍵字 SearchString
        public string SearchString { get; set; }

        public IEnumerable<Recipe> Recipes { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<Camp> Camps { get; set; }
        public IEnumerable<ProductImg> productImgs { get; set; }


        public SelectList RGenres { get; set; }
        public SelectList PGenres { get; set; }
        public SelectList CGenres { get; set; }


        // 食譜 Recipe
        public string RecipeGenre { get; set; }
        // 產品 Product
        public string ProductGenre { get; set; }
        // 營地 Camp
        public string CampGenre { get; set; }

        public IEnumerable<Product> products { get; set; }
        public IEnumerable<ProductImg> productImgs { get; set; }
        public IEnumerable<Recipe> recipes { get; set; }
        public IEnumerable<RecipeImg> recipeImgs { get; set; }
        public IEnumerable<Camp> camps { get; set; }
        public IEnumerable<CampImg> campImgs { get; set; }
        public IEnumerable<Category> categories { get; set; }
        public IEnumerable<CategoriesTypeI> categoriesTypeIs { get; set; }
        public IEnumerable<CategoriesTypeIi> categoriesTypeIis { get; set; }



      
        public class SearchOwnProduct
        {
            public IEnumerable<Product> products { get; set; }
            public IEnumerable<ProductImg> productImgs { get; set; }
            public IEnumerable<Category> categories { get; set; }
            public IEnumerable<CategoriesTypeI> categoriesTypeIs { get; set; }
            public IEnumerable<CategoriesTypeIi> categoriesTypeIis { get; set; }
        }
    }


}
