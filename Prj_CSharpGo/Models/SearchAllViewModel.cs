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

        // 食譜 Recipe
        public List<Recipe> Recipes { get; set; }
        public SelectList RGenres { get; set; }
        public string RecipeGenre { get; set; }

        // 產品 Product
        public List<Product> Products { get; set; }
        public SelectList PGenres { get; set; }
        public string ProductGenre { get; set; }

        // 營地 Camp
        public List<Camp> Camps { get; set; }
        public SelectList CGenres { get; set; }
        public string CampGenre { get; set; }


        //private Hashtable _AllGenre;

        //public Hashtable AllGenre
        //{
        //    get { return this._AllGenre; }
        //}

        //private void Init(List<Recipe> recipe, List<Product> product, List<Camp> camp)      // 功能 : 轉接
        //{
        //    this._AllGenre = new Hashtable();
        //    // ======================
        //    this.Products = product;
        //    this.Recipes = recipe;
        //    this.Camps = camp;
        //}

        //public All(List<Recipe> recipe, List<Product> product, List<Camp> camp)
        //{
        //    get { return this.Init(recipe)}
        //    set;
        //}
    }
}
