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
            //SearchAllViewModel searchAll = new SearchAllViewModel();

            //string[] productIdArr = searchAll.products.Select(s => s.ProductId).ToArray();
            //searchAll.productImgs = _context.ProductImgs.Where(s=> productIdArr.Contains(s.ProductId)).ToList();


            // 全站搜尋
            IQueryable<string> RgenreQuery = from r in _context.Recipes
                                             orderby r.RecipeName
                                             select r.RecipeName;

            IQueryable<string> PgenreQuery = from r in _context.Products
                                             orderby r.ProductName
                                             select r.ProductName;

            IQueryable<string> CgenreQuery = from r in _context.Camps
                                             orderby r.CampName
                                             select r.CampName;

            var recipes = from r in _context.Recipes
                          select r;

            var products = from p in _context.Products
                           select p;

            var camps = from c in _context.Camps
                        select c;
            
            if (!string.IsNullOrEmpty(searchString))
            {
                // Recipe 食譜
                recipes = recipes.Where(s => s.RecipeName.Contains(searchString));
                // Product 產品
                products = products.Where(s => s.ProductName.Contains(searchString));
                // Camp 營地
                camps = camps.Where(s => s.CampName.Contains(searchString));
            }

            //if (!string.IsNullOrEmpty(Genre))
            //{
            //    recipes = recipes.Where(x => x.RecipeName == Genre);
            //    products = products.Where(x => x.ProductName == Genre);
            //    camps = camps.Where(x => x.CampName == Genre);
            //}

            SearchAllViewModel allGenreVM = new SearchAllViewModel
            {
                RGenres = new SelectList(await RgenreQuery.Distinct().ToListAsync()),
                PGenres = new SelectList(await PgenreQuery.Distinct().ToListAsync()),
                CGenres = new SelectList(await CgenreQuery.Distinct().ToListAsync()),
                //productImgs = _context.ProductImgs,
                Recipes = await recipes.ToListAsync(),
                Products = await products.ToListAsync(),
                Camps = await camps.ToListAsync(),

                products = _context.Products.ToList(),
                productImgs = _context.ProductImgs.ToList(),
                recipes = _context.Recipes.ToList(),
                recipeImgs = _context.RecipeImgs.ToList(),
                camps = _context.Camps.ToList(),
                campImgs = _context.CampImgs.ToList(),
                categories = _context.Categories.ToList(),
                categoriesTypeIs = _context.CategoriesTypeIs.ToList(),
                categoriesTypeIis = _context.CategoriesTypeIis.ToList()
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
