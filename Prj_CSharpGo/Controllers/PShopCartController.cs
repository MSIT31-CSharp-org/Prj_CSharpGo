using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prj_CSharpGo.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Prj_CSharpGo.Models.ShopCartViewModels;


namespace Prj_CSharpGo.Controllers
{
    public class PShopCartController : Controller
    {
        private readonly ILogger<CampreserveController> _logger;
        private readonly WildnessCampingContext _context;
        public PShopCartController(WildnessCampingContext context, ILogger<CampreserveController> logger)
        {
            _context = context;
            _logger = logger;
        }



        //從購物車資料庫抓出所有東西
        public IActionResult Index()//從商品庫抓出所有商品
        {
            List<ShoppingCart> ShopProductsList = _context.ShoppingCarts.ToList();

            return View(ShopProductsList);
        }

        //在購物車頁面點那個商品的圖片會回到商品詳細介紹那一頁
        // public IActionResult Detail(int? ProductId)
        // {

        // Product reProduct = _context.ShoppingCarts.Find(ProductId) ;

        // ReturnIndexModels returnIndexModels = new ReturnIndexModels();
        // returnIndexModels.userFilterModel = new CampreserveUserFilterModel()
        // {
        //  CampId = id,
        // StartDate = post["StartDay"],
        // EndDate = post["EndDay"],
        //  CampQuantity = Quantity
        // };

        //reProduct
        // return View(reProduct);
    }

    //[HttpGet]
    // public async Task<ActionResult<IEnumerable<Product>>> Detail()
    // {
    //  var shquery = from o in _context.Products
    //  orderby o.CategoryId ascending, o.Price descending
    //select o;
    //return await shquery.ToListAsync();
    // }
}
