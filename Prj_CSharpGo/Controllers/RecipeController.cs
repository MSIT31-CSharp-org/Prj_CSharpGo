using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Mvc;
using Prj_CSharpGo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using Prj_CSharpGo.Models.ViewModels;

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
        public IActionResult Edit(int id)
        {
            RecipeProduct all = new RecipeProduct()
            {
                Recipe = this._context.Recipes.Find(id),
                Associations = from o in _context.Associations
                               where o.RecipeId == id
                               select o,
                Products = this._context.Products.Where(x => x.CategoryId == "E ")
            };
            return View(all);
        }
        [HttpPost]
        public IActionResult Edit(Recipe reForm, IFormFile Img)
        {
            Recipe re = this._context.Recipes.Find(reForm.RecipeId);
            // 上傳檔案
            if (Img != null)
            {
                string[] subs = Img.FileName.Split('.');
                String NewImgName = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + subs[1];
                re.Img = NewImgName;
                Img.CopyTo(new FileStream("./wwwroot/Didi/img/" + NewImgName, FileMode.Create));
            }
            re.RecipeName = reForm.RecipeName;
            re.CookingTime = reForm.CookingTime;
            re.Preparation = reForm.Preparation;
            re.Step = reForm.Step;
            this._context.SaveChanges();
            return Redirect($"/Recipe/Detail/{@reForm.RecipeId}");
        }


        public IActionResult Create(int id)
        {
            RecipeProduct all = new RecipeProduct()
            {
                Products = this._context.Products.Where(x => x.CategoryId == "E ")
            };
            return View(all);
        }

        [HttpPost]
        public IActionResult Create(Recipe newRecipe, IFormFile Img)
        {
            // 上傳圖片檔案
            if (Img != null)
            {
                string[] subs = Img.FileName.Split('.');
                String NewImgName = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + subs[1];
                Img.CopyTo(new FileStream("./wwwroot/img/" + NewImgName, FileMode.Create));
                newRecipe.Img = NewImgName;
            }
            newRecipe.UserId = 2;
            _context.Add(newRecipe);
            _context.SaveChanges();
            return Redirect("/Recipe/Recipe");
        }

        public IActionResult Delete(int? id)
        {
            var de = _context.Recipes.Find(id);
            _context.Recipes.Remove(de);
            this._context.SaveChanges();
            return RedirectToAction("Recipe");
        }
    }
}
