
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Prj_CSharpGo.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Prj_CSharpControllers
    {
        public class SearchController : Controller
        {
            private WildnessCampingContext _context;
            public SearchController(WildnessCampingContext context)
            {
                _context = context;
            }
            public IActionResult index(string Category, string searchString,string ImgString)
            {
                var CategoryIdLst = new List<string>()
 ;
                var CategoryIdQry = from p in _context.Products
                                    orderby p.CategoryId
                                    select p.CategoryId;

                CategoryIdLst.AddRange(CategoryIdQry.Distinct());


            var products = from p in _context.Products
                           select p;
            var camp = from c in _context.Camps
                       select c;
          
            var recipe = from r in _context.Recipes
                         select r;
           
            if (!string.IsNullOrEmpty(searchString))
            {
                recipe = recipe.Where(r => r.RecipeName.Contains(searchString));
            }
            if (!string.IsNullOrEmpty(searchString))
            {
                camp = camp.Where(c => c.CampName.Contains(searchString));

            }
            if (!string.IsNullOrEmpty(searchString))
                {
                    products = products.Where(s => s.ProductName.Contains(searchString));
                }

                if (!string.IsNullOrEmpty(Category))
                {
                    products = products.Where(c => c.CategoryId == Category);
                }
                List<Product> productList = _context.Products.ToList();
                return View("index", productList);
            }



        }
    }
