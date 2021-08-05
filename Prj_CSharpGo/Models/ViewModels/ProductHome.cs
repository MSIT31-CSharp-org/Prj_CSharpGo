using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prj_CSharpGo.Models.ViewModels
{
    public class ProductHome
    {
        public IEnumerable<Product> products { get; set; }
        public IEnumerable<ProductImg> productImgs { get; set; }
        public IEnumerable<Category> categories { get; set; }
        public IEnumerable<CategoriesTypeI> categoriesTypeIs { get; set; }
        public IEnumerable<CategoriesTypeIi> categoriesTypeIis { get; set; }

        public Product userModel { get; set; }

    }


}
