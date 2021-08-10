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
            var rec = _context.Recipes.FirstOrDefault(m => m.RecipeId == id);
            if (rec == null)
            {
                return NotFound();
            }
            Recipe re = _context.Recipes.Find(id);
            ViewData["Association"] = this._context.Associations.Where(x => x.RecipeId == id).ToList();
            ViewData["Product"] = this._context.Products.ToList();
            ViewData["ProductImg"] = this._context.ProductImgs.ToList();
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
            var rec = _context.Recipes.FirstOrDefault(m => m.RecipeId == id);
            if (rec == null)
            {
                return NotFound();
            }

            //這邊無法用2次Model去foreach，所以用ViewData取代 => 這邊如果用all.Products會抓不到，所以直接用資料庫文法重抓
            ViewData["Products"] = this._context.Products.Where(x => x.CategoryId == "E ").ToList();
            return View(all);
        }
        [HttpPost]
        public IActionResult Edit(Recipe reForm, IFormFile Img, Dictionary<int, string> ProductID, Dictionary<int, string> Description)
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
            var RecipeId = reForm.RecipeId;

            // 刪除之前的Association
            var AssociationList = _context.Associations.Where(x => x.RecipeId == RecipeId).ToList();
            if (AssociationList.Count() > 0)
            {
                foreach (var item in AssociationList)
                {
                    _context.Associations.Remove(item);
                }
            }
            // 再做新增
            foreach (KeyValuePair<int, string> item in ProductID)
            {
                var AssociationObj = new Association();
                AssociationObj.RecipeId = RecipeId;
                AssociationObj.ProductId = "";
                AssociationObj.Description = "";
                if (item.Value != null)
                {
                    AssociationObj.ProductId = item.Value;
                }
                if(Description[item.Key]!=null)
                {
                    AssociationObj.Description = Description[item.Key].ToString();
                }
                _context.Associations.Add(AssociationObj);
            }
            _context.SaveChanges();

            return Redirect($"/Recipe/Detail/{@reForm.RecipeId}");
            //return RedirectToAction("Detail");
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
        public IActionResult Create(Recipe newRecipe, IFormFile Img, Dictionary<int, string> ProductID, Dictionary<int, string> Description)
        {
            // 上傳圖片檔案
            if (Img != null)
            {
                Console.WriteLine(Img.FileName);
                string[] subs = Img.FileName.Split('.');
                String NewImgName = DateTime.Now.ToString("yyyyMMddHHmmss") + "." + subs[1];
                Img.CopyTo(new FileStream("./wwwroot/Didi/img/" + NewImgName, FileMode.Create));
                newRecipe.Img = NewImgName;
            }
            newRecipe.UserId = 1001;
            _context.Add(newRecipe);
            _context.SaveChanges();

            List<Recipe> Recipes = _context.Recipes.ToList();
            var RecipesCount = Recipes.Count();
            var RecipeId = Recipes[RecipesCount - 1].RecipeId;

            foreach (KeyValuePair<int, string> item in ProductID)
            {
                var AssociationObj = new Association();
                AssociationObj.RecipeId = RecipeId;
                AssociationObj.ProductId = "";
                AssociationObj.Description = "";
                if (item.Value != null)
                {
                    AssociationObj.ProductId = item.Value;
                }
                if(Description[item.Key] != null)
                {
                    AssociationObj.Description = Description[item.Key].ToString();
                }
                _context.Associations.Add(AssociationObj);
            }
            _context.SaveChanges();
            return Redirect("/Recipe/index");
        }

        public IActionResult Delete(int? id)
        {
            var de = _context.Recipes.Find(id);
            _context.Recipes.Remove(de);

            // 刪除時一併刪除Association相關資料
            var AssociationList = _context.Associations.Where(x => x.RecipeId == id).ToList();
            if (AssociationList.Count() > 0)
            {
                foreach (var item in AssociationList)
                {
                    _context.Associations.Remove(item);
                }
            }
            this._context.SaveChanges();
            return RedirectToAction("index");
        }

        //會員/陌生登入頁面
        public IActionResult cust_Index()
        {
            List<Recipe> Recipes = _context.Recipes.ToList();
            return View("cust_Index", Recipes);
        }
        public IActionResult cust_Detail(int? id)
        {
            var rec = _context.Recipes.FirstOrDefault(m => m.RecipeId == id);
            if (rec == null)
            {
                return NotFound();
            }
            Recipe re = _context.Recipes.Find(id);
            ViewData["Association"] = this._context.Associations.Where(x => x.RecipeId == id).ToList();
            ViewData["Product"] = this._context.Products.ToList();
            ViewData["ProductImg"] = this._context.ProductImgs.ToList();
            return View(re);
        }
    }
}
