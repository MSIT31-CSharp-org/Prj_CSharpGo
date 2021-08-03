using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Prj_CSharpGo.Models;
using Prj_CSharpGo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Prj_CSharpGo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private WildnessCampingContext _context;

        public HomeController(ILogger<HomeController> logger, WildnessCampingContext dbContext)
        {
            _logger = logger;
            _context = dbContext;
        }

        public IActionResult Index()
        {

            ProductHome productHome = new ProductHome
            {
                products = _context.Products.ToList(),
                productImgs = _context.ProductImgs.ToList(),
                categories = _context.Categories.ToList(),
                categoriesTypeIs = _context.CategoriesTypeIs.ToList(),
                categoriesTypeIis = _context.CategoriesTypeIis.ToList()
            };

            // 寫入Session 
            //string userSession = HttpContext.Session.GetString("userName") ?? "Guest";
            //if (userSession == "Guest")
            //{
            //    return View(productHome);
            //}
            //HttpContext.Session.SetString("userToastr", "已登入");

            
            return View(productHome);
        }

        public IActionResult ProductCate()
        {
            return View();
        }

        // 正常來說會收到一段string productid 但是目前頁面還沒處理好 我就直接給 productid = "Aa10CL007"
        public IActionResult Product(string productid, string categoryid)
        {

            productid = "Aa10CL007";
            categoryid = "A";

            // 新創個類別 類別在~/Model/ViewModels/ProductHome.cs 修改類別的話Jane DiDI要討論一下 建議是從下面{}取得的值修改就好
            ProductHome productHome = new ProductHome
            {
                products = from o in _context.Products
                           where o.ProductId == productid 
                           select o,
                productImgs = from o in _context.ProductImgs
                              where o.ProductId == productid
                              select o ,
                categories = from o in _context.Categories
                             where o.CategoryId == categoryid
                             select o,
                categoriesTypeIs = _context.CategoriesTypeIs.ToList(),
                categoriesTypeIis = _context.CategoriesTypeIis.ToList()
            };
            return View(productHome);
        }

        public IActionResult ProductsList(string? CategoryId, string? CategoryTypeId)
        {
            
            ProductHome productHome = new ProductHome
            {
                products = _context.Products.ToList(),
                productImgs = _context.ProductImgs.ToList(),
                categories = _context.Categories.ToList(),
                categoriesTypeIs = _context.CategoriesTypeIs.ToList(),
                categoriesTypeIis = _context.CategoriesTypeIis.ToList()
            };
            Console.WriteLine("CategoryId:" + CategoryId);
            Console.WriteLine("CategoryTypeId:" + CategoryTypeId);

            if (CategoryId != null && CategoryId != "0")
            {
                productHome.products = _context.Products.Where(x => x.CategoryId == CategoryId);
            }

            if (CategoryTypeId != null && CategoryTypeId != "0")
            {
                productHome.products = _context.Products.Where(x => x.CategoryId == CategoryId);
            }
            if(CategoryId !=null && CategoryTypeId != null)
            {
                productHome.products = _context.Products.Where(x => x.CategoryType == CategoryTypeId);
            }
            return View(productHome);
        }

        public IActionResult Talk(string productid)
        {


            return View();
        }


        //內建 無視掉
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
