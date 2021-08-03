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
using Prj_CSharpGo.Models.POrderViewModel;

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

        //以下是購物車相關------------------------------------------------------------------------------------
        [HttpPost]
        public IActionResult AddCart(ShoppingCart ShoppingCarts)//檢視：無-接收商品頁面的資料 要傳進購物車資料庫PShopCartViewMolds中的ShoppingCarts的動作

        {
            ShoppingCart PTOSCOrder = new ShoppingCart()
            {
                UserId = ShoppingCarts.UserId,
                ProductId = ShoppingCarts.ProductId,
                Quantity = ShoppingCarts.Quantity,
                UnitPrice = ShoppingCarts.UnitPrice,
                Status = ShoppingCarts.Status,
                ProductName = ShoppingCarts.ProductName,
            };
            return Redirect("/PShopCart/Index");//接收的直要呈現在VIEW的檢視頁面/PShopCart/Index
        }
        public IActionResult Index()//檢視：購物車訂單頁面-一開始推進去購物車資料的動作  從購物車資料庫裡抓資料                                
        {
            returnshCartIndexVM returnshCartIndexVM = new returnshCartIndexVM();
            returnshCartIndexVM.ShoppingCarts = _context.ShoppingCarts.ToList();
            returnshCartIndexVM.Products = _context.Products.ToList();
            returnshCartIndexVM.Users = _context.Users.ToList();

            ViewData["total"] = _context.ShoppingCarts;
            return View("Index", returnshCartIndexVM);
        }
        [HttpPost]
        public IActionResult Index(ShoppingCart temp)//檢視：購物車訂單頁面-購物車商品數量更改後再次推進的動作    
        {
            return View();
        }

        //以下是訂單相關---------------------------------------------------------------------------------------
        [HttpPost]
        public IActionResult OrderIndex(POrderAllModel POrderAllModel)//檢視：無-接收購物車清單要傳進訂單資料庫的資料
                                                                      //AddOrder
        {
            Order SCtoOrder = new Order()//推進資料庫Order
            {

                OrderId = POrderAllModel.OrderId,
                UserId = POrderAllModel.UserId,
                OrderDate = DateTime.Now,
                TotalPrice = POrderAllModel.TotalPrice,
                PayMethod = POrderAllModel.PayMethod,
                Approval = POrderAllModel.Approval,

                //WeekdayPrice = SCPtoOrderModel.WeekdayPrice,
                // HolidayPrice = SCPtoOrderModel.HolidayPrice,
                //Peoplenumber = SCPtoOrderModel.Peoplenumber,
                //TotalPrice = SCPtoOrderModel.TotalPricebig + SCPtoOrderModel.PeoplePrice * SCPtoOrderModel.Peoplenumber,

            };
            _context.Orders.Add(SCtoOrder);
            _context.SaveChanges();
            // return Redirect("/PShopCart/OrderIndex");


            OrderDetail SCtoOrderDetail = new OrderDetail()//推進資料庫OrderDetail
            {

                OrderId = POrderAllModel.OrderId,
                ProductId = POrderAllModel.ProductId,
                // OrderDate = DateTime.Now,
                UnitPrice = POrderAllModel.UnitPrice,
                Quantity = POrderAllModel.Quantity,
                Discount = POrderAllModel.Discount,
                Commets = POrderAllModel.Commets,
                Approval = POrderAllModel.Approval,

                //Peoplenumber = SCPtoOrderModel.Peoplenumber,
                //TotalPrice = SCPtoOrderModel.TotalPricebig + SCPtoOrderModel.PeoplePrice * SCPtoOrderModel.Peoplenumber,

            };
            _context.OrderDetails.Add(SCtoOrderDetail);
            _context.SaveChanges();
            return Redirect("/PShopCart/OrderIndex");
        }
        public IActionResult OrderIndex(IFormCollection post)//檢視：確定的訂單頁面-購物車要將清單推進訂單資料庫與頁面的動作 
        {
          
            int UId = Convert.ToInt32(post["UserId"]);
            int ProductId = Convert.ToInt32(post["ProductId"]);
            string ProductName = post["ProductName"];
            int UnitPrice = Convert.ToInt32(post["UnitPrice"]);
            int Quantity = Convert.ToInt32(post["Quantity"]);
            int SMTotal  = Convert.ToInt32(post["UnitPrice"])* Convert.ToInt32(post["Quantity"]);
            POrderAllModel POrderAllModel = new POrderAllModel(UId, ProductId, ProductName, UnitPrice, Quantity, SMTotal);                      
            return View("OrderIndex", POrderAllModel);//推進資料庫POrderAllModel  給訂單頁面OrderIndex用
        }
        //public IActionResult OrderIndex(PShopCartController orderModel)
        //{
           // ViewBag.messagea = "已完成訂單";
            //return View(orderModel);
       // }
        

        //推進去訂單的動作

        //public IActionResult OrderIndex(int UserId, int ProductId, string ProductName, int Quantity, int UnitPrice, int SMTotal)
        // {
        // 利用 RedirectToAction 導至其他 Action 
        //return RedirectToAction("ShopList", new { UserId = UserId, ProductId = ProductId, ProductName = ProductName, Quantity = Quantity, UnitPrice = UnitPrice, SMTotal = SMTotal });
        //}
    }
}