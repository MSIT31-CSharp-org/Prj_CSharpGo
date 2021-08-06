using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Prj_CSharpGo.Models;
using Prj_CSharpGo.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using X.PagedList;

namespace Prj_CSharpGo.Controllers
{
    public class CampProducts : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private WildnessCampingContext _context;

        public CampProducts(ILogger<HomeController> logger, WildnessCampingContext dbContext)
        {
            _logger = logger;
            _context = dbContext;
        }


        //分頁用
        private int pageSize = 9;

        public IActionResult Index(string categoryid="",string categorytype="",int Page=1)
        {
            ProductHome productHome = new ProductHome();

            if (categoryid == "" && categorytype == "")
            {
                productHome.products = _context.Products.ToList();
            }
            else if(categorytype == "")
            {
                productHome.products = _context.Products.Where(s => s.CategoryId == categoryid).ToList();
            }else if (categoryid != "" && categorytype != "")
            {
                productHome.products = _context.Products.Where(s => s.CategoryId == categoryid && s.CategoryType == categorytype).ToList();
            }
            
            string[] productIdArr =  productHome.products.Select(s => s.ProductId).ToArray();
            productHome.productImgs = _context.ProductImgs.Where(s => productIdArr.Contains(s.ProductId)).ToList();
            productHome.categories = _context.Categories.ToList();
            productHome.categoriesTypeIs =  _context.CategoriesTypeIs.ToList();

            productHome.products = productHome.products.OrderBy(o => o.ProductId).ToPagedList(Page, pageSize);
            
            productHome.userModel = new Product()
            {
                CategoryId = categoryid,
                CategoryType = categorytype,
            };

            return View("Index", productHome);

            //return View("Index", productHome);
        }

    

        // 正常來說會收到一段string productid 但是目前頁面還沒處理好 我就直接給 productid = "Aa10CL007"
        public IActionResult ProductDetail(string productid, string categoryid = "", string categorytype = "")
        {
            ProductHome productHome = new ProductHome();

            if (categoryid == "" && categorytype == "")
            {
                productHome.products = _context.Products.Where(s => s.ProductId == productid).ToList();
            }
            else if (categorytype == "")
            {
                productHome.products = _context.Products.Where(s => s.CategoryId == categoryid && s.ProductId== productid).ToList();
            }
            else if (categoryid != "" && categorytype != "")
            {
                productHome.products = _context.Products.Where(s => s.CategoryId == categoryid && s.CategoryType == categorytype && s.ProductId == productid).ToList();
            }
            //productHome.products = categoryid==""? _context.Products.ToList():_context.Products.Where(s => s.CategoryId == categoryid).ToList();
            string[] productIdArr = productHome.products.Select(s => s.ProductId).ToArray();
            productHome.productImgs = _context.ProductImgs.Where(s => productIdArr.Contains(s.ProductId)).ToList();
            productHome.categories = _context.Categories.ToList();
            productHome.categoriesTypeIs = _context.CategoriesTypeIs.ToList();

            //productid = "Aa10CL007";
            //categoryid = "A";

            //// 新創個類別 類別在~/Model/ViewModels/ProductHome.cs 修改類別的話Jane DiDI要討論一下 建議是從下面{}取得的值修改就好
            //ProductHome productHome = new ProductHome
            //{
            //    products = from o in _context.Products
            //               where o.ProductId == productid
            //               select o,
            //    productImgs = from o in _context.ProductImgs
            //                  where o.ProductId == productid
            //                  select o,
            //    categories = from o in _context.Categories
            //                 where o.CategoryId == categoryid
            //                 select o,
            //    categoriesTypeIs = _context.CategoriesTypeIs.ToList(),
            //    categoriesTypeIis = _context.CategoriesTypeIis.ToList()
            //};
            return View("ProductDetail",productHome);
        }

        

        //內建 無視掉
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
