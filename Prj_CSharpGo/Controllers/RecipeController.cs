using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Prj_CSharpGo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prj_CSharpGo.Controllers
{
    public class RecipeController : Controller
    {
        private readonly WildnessCampingContext _context;

        public RecipeController(WildnessCampingContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Recipe> Recipes = _context.Recipes.ToList();
            return View("Index", Recipes);
        }
        public IActionResult Detail(int? id)
        {
            Recipe re = _context.Recipes.Find(id);
            return View(re);
        }
        public IActionResult Edit(int? id)
        {
            Recipe re = _context.Recipes.Find(id);
            return View(re);
        }
    }
}
