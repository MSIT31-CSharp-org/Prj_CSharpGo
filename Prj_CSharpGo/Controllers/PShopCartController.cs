using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Prj_CSharpGo.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using Prj_CSharpGo.Models.ShopCartViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

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

        public IActionResult Index()//檢視購物車訂單頁面-一開始推進去的數值
        {
            returnshCartIndexVM returnshCartIndexVM = new returnshCartIndexVM();
            returnshCartIndexVM.ShoppingCarts = _context.ShoppingCarts.ToList();
            returnshCartIndexVM.Products = _context.Products.ToList();
            returnshCartIndexVM.Users = _context.Users.ToList();

            ViewData["total"] = _context.ShoppingCarts;
            return View("Index", returnshCartIndexVM);          
        }

        [HttpPost]
        public IActionResult Index(ShoppingCart temp)//檢視購物車訂單頁面-一開始推進去的數值
        {
            //var query = from o in 

            return View();
        }

        public ActionResult SHOPQ()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SHOPQ(int UserId, int ProductId, int ProductName, int Quantity, int UnitPrice, int SMTotal)
        {
            // 利用 RedirectToAction 導至其他 Action 
            return RedirectToAction("ShopList", new { UserId = UserId, ProductId = ProductId, ProductName = ProductName, Quantity = Quantity, UnitPrice = UnitPrice, SMTotal = SMTotal });
        }
    }
}

/*
  public IActionResult PSHOPIndex(IFormCollection post)//第2次運算後推進去的數值
{

    returnshCartIndexVM returnshCartIndexVM = new returnshCartIndexVM();
    int id = Convert.ToInt32(post["UserId"]);//會員號轉id為了等一下可以有專門的購物車號           
    int USHOPENDQ = Convert.ToInt32(post["Quantity"]);//客戶最終決定下單的數量
    string errorMsg = "";//判斷有沒有會員用的

    //會員可選擇的數值
    returnshCartIndexVM.PuserFilterModel = new ShoppingCartUserFilterModel()//這歌會員的購物車裡的內容清單
    {                                      
       // Quantity = (short?)USHOPENDQ,
        UserId = id//id當購物車編號
    };

    //租借日期、退租日期和帳數判斷
    returnshCartIndexVM.Users = _context.Users.ToList();
    if (id == null)
    {
        errorMsg = "請登入會員！";
    }
    else
    {
        var findPSHOPP = returnshCartIndexVM.PuserFilterModel;
        var temp = _context.CampOrders.ToList();
        var UnitPrice = returnshCartIndexVM.PuserFilterModel.UnitPrice;
        int dayCount = 0;
        int quantityCount = 0;             
        int Totalprice = 0;


            ShoppingCartUserFilterModel PSHOPCorderModel = new ShoppingCartUserFilterModel()
            {
                UserId = 1001,
                ProductId = findPSHOPP.ProductId,
                ProductName = findPSHOPP.ProductName,
                UnitPrice = findPSHOPP.UnitPrice,
                Quantity = findPSHOPP.Quantity,

                SMTotal = USHOPENDQ * UnitPrice,
                //BigTotal = SMTotal


            };
            return RedirectToAction("PSHOPIndex");

    }

    ViewBag.message = errorMsg;
    return View("PSHOPIndex", returnshCartIndexVM);
}
從購物車資料庫抓出所有東西
public async Task<IActionResult> IndexAsync()//從商品庫抓出所有商品
{
    //List<ShoppingCart> ShopProductsList = _context.ShoppingCarts.ToList();

    //return View(ShopProductsList);

    var products = from p in _context.Products
                   select p;

    var shoppingCart = from c in _context.ShoppingCarts
                select c;


    var allshCartVM = new returnshCartIndexVM()
    {
        Products = await products.ToListAsync(),
        ShoppingCarts = await shoppingCart.ToListAsync()
    };

    return View(allshCartVM);
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
// foreach (var item in viewdata["products"]) { item.ProductName};
//viewdata["products"] = _context.products
//  ViewDara["Product"] = _context.Products
*/