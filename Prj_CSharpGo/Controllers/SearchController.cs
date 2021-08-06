using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Prj_CSharpGo.Models;
using Prj_CSharpGo.Models.ViewModels;


namespace Prj_CSharpControllers
    {
        public class SearchController : Controller
        {
        //private readonly ILogger<SearchController> _logger;
        private WildnessCampingContext _context;
        

        public SearchController(WildnessCampingContext dbContext)
        {
            //_logger = logger;
            _context = dbContext;
        }


        public async Task<IActionResult> Index(string searchString)
        {
            // 關鍵字搜尋
            IQueryable<string> PgenreQuery = from r in _context.Products
                                             orderby r.ProductName
                                             select r.ProductName;

            //Product pp = new Product();

            var products = from p in _context.Products
                           select p;

            var productImgs = from c in _context.ProductImgs
                              select c;

            var recipeImgs = from c in _context.RecipeImgs
                             select c;

            var campImgs = from c in _context.CampImgs
                           select c;

            var categories = from c in _context.Categories
                             select c;

            var categoriesTypeIs = from c in _context.CategoriesTypeIs
                                   select c;

            var categoriesTypeIis = from c in _context.CategoriesTypeIis
                                    select c;

            // 
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.ProductName.Contains(searchString));
            }
            //if (!string.IsNullOrEmpty(Genre))
            //{
            //    recipes = recipes.Where(x => x.RecipeName == Genre);
            //    products = products.Where(x => x.ProductName == Genre);
            //    camps = camps.Where(x => x.CampName == Genre);
            //}

            SearchAllViewModel allGenreVM = new SearchAllViewModel
            {
                //pGenres = new SelectList(await PgenreQuery.Distinct().ToListAsync()),
                products = await products.ToListAsync(),
                productImgs =await productImgs.ToListAsync(),
                categories = await categories.ToListAsync(),
                categoriesTypeIs =await categoriesTypeIs.ToListAsync(),
                categoriesTypeIis =await categoriesTypeIis.ToListAsync()
            };
            return View(allGenreVM);

        }
      
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        //測試全站搜尋
        //public IActionResult Test(string searchString, object campImg)
        //{

        //}


    }
}
