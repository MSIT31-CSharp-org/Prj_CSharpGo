using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Prj_CSharpGo.Models;

namespace Prj_CSharpGo.Controllers
{
    public class RecipeSearchController : Controller
    {
        private WildnessCampingContext _context;
        public RecipeSearchController(WildnessCampingContext context)
        {
            _context = context;
        }
        public IActionResult Index(string searchString)
        {
            
                var recipe = from r in _context.Recipes
                             select r;
                if (!string.IsNullOrEmpty(searchString))
                {
                    recipe = recipe.Where(s => s.RecipeName.Contains(searchString));
                }
                List<Recipe> recipeList = _context.Recipes.ToList();
                return View("Index", recipeList);
            
        }
    }
}
