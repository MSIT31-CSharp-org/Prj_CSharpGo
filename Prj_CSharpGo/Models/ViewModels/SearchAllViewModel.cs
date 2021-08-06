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
        // 顯示要搜尋的字串 SearchString
        public string SearchString { get; set; }
        public string productGenre { get; set; }
        public SelectList pGenres { get; set; }
        public IEnumerable<Product> products { get; set; }
        public IEnumerable<ProductImg> productImgs { get; set; }
        public IEnumerable<Category> categories { get; set; }
        public IEnumerable<CategoriesTypeI> categoriesTypeIs { get; set; }
        public IEnumerable<CategoriesTypeIi> categoriesTypeIis { get; set; }
    }


}
