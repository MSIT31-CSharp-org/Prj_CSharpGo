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

        // 全站搜尋
        public async Task<IActionResult> Index(string searchString)
        {

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

            var allGenreVM = new SearchAllViewModel
            {
                RGenres = new SelectList(await RgenreQuery.Distinct().ToListAsync()),
                PGenres = new SelectList(await PgenreQuery.Distinct().ToListAsync()),
                CGenres = new SelectList(await CgenreQuery.Distinct().ToListAsync()),

                Recipes = await recipes.ToListAsync(),
                Products = await products.ToListAsync(),
                Camps = await camps.ToListAsync(),

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



    }
    }
