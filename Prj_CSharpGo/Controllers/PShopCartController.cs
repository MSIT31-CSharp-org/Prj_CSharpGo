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
using Microsoft.AspNetCore.Hosting;

namespace Prj_CSharpGo.Controllers
{
    public class PShopCartController : Controller
    {
        private readonly ILogger<PShopCartController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly WildnessCampingContext _context;
        public PShopCartController(WildnessCampingContext context, ILogger<PShopCartController> logger, Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        public ActionResult Home()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ShoppingCart>>> myCart()
        {
            string userId = HttpContext.Session.GetString("userId") ?? "Guest";
            if (userId == "Guest")
            {
                return Redirect("/Auth/Login");
            }

            var shopcart = _context.ShoppingCarts.Where(x => x.UserId.ToString() == userId);
            return await shopcart.ToListAsync();
        }


        [HttpPost]
        public IActionResult dash(string ProductId)
        {
            string userId = HttpContext.Session.GetString("userId") ?? "Guest";
            if (userId == "Guest")
            {
                return Redirect("/Auth/Login");
            }

            var dash0 = _context.ShoppingCarts.Where(x => x.UserId.ToString() == userId);

            var _dash = dash0.Where(x => x.ProductId == ProductId).ToList()[0];
            if (_dash == null)
            {
                return NotFound();
            }
            if (_dash.Quantity == null)
            {
                return NotFound();
            }

            if (_dash.Quantity > 1)
            {
                _dash.Quantity -= 1;
                _context.SaveChanges();
            }

            return Redirect("/PShopCart/Index");
        }
        [HttpPost]
        public IActionResult plus(string ProductId)
        {
            string userId = HttpContext.Session.GetString("userId") ?? "Guest";
            if (userId == "Guest")
            {
                return Redirect("/Auth/Login");
            }
            var dash0 = _context.ShoppingCarts.Where(x => x.UserId.ToString() == userId);

            var _dash = dash0.Where(x => x.ProductId == ProductId).ToList()[0];
            if (_dash == null)
            {
                return NotFound();
            }
            if (_dash.Quantity == null)
            {
                return NotFound();
            }

            if (_dash.Quantity < _context.Products.Find(ProductId).UnitInStock)
            {
                _dash.Quantity += 1;
                _context.SaveChanges();
            }


            ViewBag.messagea = "已完成訂單";
            return Redirect("/PShopCart/Index");
        }
        [HttpPost]
        public IActionResult delete(string ProductId)
        {
            string userId = HttpContext.Session.GetString("userId") ?? "Guest";
            if (userId == "Guest")
            {
                return Redirect("/Auth/Login");
            }
            var dash0 = _context.ShoppingCarts.Where(x => x.UserId.ToString() == userId);

            var _dash = dash0.Where(x => x.ProductId == ProductId).ToList()[0];
            if (_dash == null)
            {
                return NotFound();
            }
            if (_dash.Quantity == null)
            {
                return NotFound();
            }

            _context.ShoppingCarts.Remove(_dash);
            _context.SaveChanges();

            return Redirect("/PShopCart/Index");
        }
        [HttpPost]
        public IActionResult Bill([Bind("OrderID,UserID,TotalPrice,PayMethod,OrderDate,Approval,Address,UserName")] Order order)
        {
            string userId = HttpContext.Session.GetString("userId") ?? "Guest";
            if (userId == "Guest")
            {
                return Redirect("/Auth/Login");
            }
            var dash0 = _context.ShoppingCarts.Where(x => x.UserId.ToString() == userId).ToList();


            if (dash0 == null)
            {
                return NotFound();
            }

            order.UserId = Convert.ToInt32(userId);
            order.OrderDate = DateTime.Now;
            order.TotalPrice = 0;
            foreach (var item in dash0)
            {
                order.TotalPrice += item.Quantity*item.UnitPrice;

            }
            
            
            if (order.TotalPrice == 0)
            {
                HttpContext.Session.SetString("shopcart", "購物車沒有東西");
                return Redirect("/PShopCart/Index");
            }
            order.Approval = "SP";
            _context.Orders.Add(order);
            _context.SaveChanges();

            var query = (from o in _context.Orders
                        where o.UserId.ToString() == userId
                        orderby o.OrderDate descending
                        select o).FirstOrDefault();
            
            foreach (var item in dash0)
            {
                OrderDetail orderDetail=new();
                orderDetail.OrderId = query.OrderId;
                orderDetail.ProductId = item.ProductId;
                orderDetail.UnitPrice = item.UnitPrice;
                orderDetail.Quantity = item.Quantity;
                _context.OrderDetails.Add(orderDetail);
                _context.SaveChanges();
            }


            foreach (var item in dash0)
            {
                _context.ShoppingCarts.Remove(item);
            }
            _context.SaveChanges();

            HttpContext.Session.SetString("shopcart", "訂單已產生");

            return Redirect($"/Auth/MemberOrderEdit/{query.OrderId}");

        }

        //以下是購物車相關------------------------------------------------------------------------------------
        [HttpPost]
        public IActionResult AddCart(ShoppingCart ShoppingCarts,string notgocart)//檢視：無-接收商品頁面的資料 要傳進購物車資料庫PShopCartViewMolds中的ShoppingCarts的動作
        {
            string userId = HttpContext.Session.GetString("userId") ?? "Guest";
            if (userId == "Guest")
            {
                return Redirect("/Auth/Login");
            }

            if (_context.ShoppingCarts.FirstOrDefault(x => x.ProductId == ShoppingCarts.ProductId) != null)
            {
                _context.ShoppingCarts.FirstOrDefault(x => x.ProductId == ShoppingCarts.ProductId).Quantity += ShoppingCarts.Quantity;
                if (_context.ShoppingCarts.FirstOrDefault(x => x.ProductId == ShoppingCarts.ProductId).Quantity > _context.Products.FirstOrDefault(x => x.ProductId == ShoppingCarts.ProductId).UnitInStock)
                { _context.ShoppingCarts.FirstOrDefault(x => x.ProductId == ShoppingCarts.ProductId).Quantity = _context.Products.FirstOrDefault(x => x.ProductId == ShoppingCarts.ProductId).UnitInStock; }
            }
            else
            {
                if(ShoppingCarts.Quantity > _context.Products.FirstOrDefault(x=>x.ProductId== ShoppingCarts.ProductId).UnitInStock) 
                { ShoppingCarts.Quantity = _context.Products.FirstOrDefault(x => x.ProductId == ShoppingCarts.ProductId).UnitInStock; }
                ShoppingCart PTOSCOrder = new ShoppingCart()
                {
                    UserId = ShoppingCarts.UserId,
                    ProductId = ShoppingCarts.ProductId,
                    Quantity = ShoppingCarts.Quantity,
                    Status = ShoppingCarts.Status,
                    ProductName = ShoppingCarts.ProductName,
                    UnitPrice = ShoppingCarts.UnitPrice,
                };
                _context.ShoppingCarts.Add(PTOSCOrder);
            }
            _context.SaveChanges();
            if (notgocart== "notgocart") {
                HttpContext.Session.SetString("notgocart", "已加入購物車!");
                return Redirect($"/CampProducts/ProductDetail?productid={ShoppingCarts.ProductId}"); 
            }
            return Redirect("/PShopCart/Index");//接收的直要呈現在VIEW的檢視頁面/PShopCart/Index
        }
        public IActionResult Index()//檢視：購物車訂單頁面-一開始推進去購物車資料的動作  從購物車資料庫裡抓資料                                
        {

            string userId = HttpContext.Session.GetString("userId") ?? "Guest";
            if (userId == "Guest")
            {
                return Redirect("/Auth/Login");
            }

            CartView returnshCartIndexVM = new CartView()
            {

                ShoppingCarts = _context.ShoppingCarts.Where(x => x.UserId.ToString() == userId),
                Products = _context.Products.ToList(),
                ProductImgs = _context.ProductImgs.ToList(),
                UserName = _context.Users.Find(Convert.ToInt32(userId)).UserName,
                Address = _context.Users.Find(Convert.ToInt32(userId)).Address

            };

            ViewBag.Address = _context.Users.Find(Convert.ToInt32(userId)).Address;
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
        public IActionResult OrderIndex(POrderAllModel POrderAllModel)//檢視：改成AddOrder?無-接收購物車清單要傳進訂單資料庫的資料
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


        //public IActionResult Addressjson(int? id)
        //{

        //    return _context.Users.Find(id);
        //}



        // public IActionResult OrderIndex(IFormCollection post)//檢視：確定的訂單頁面-購物車要將清單推進訂單資料庫與頁面的動作 
        //{

        //int UId = Convert.ToInt32(post["UserId"]);
        // int ProductId = Convert.ToInt32(post["ProductId"]);
        // string ProductName = post["ProductName"];
        // int UnitPrice = Convert.ToInt32(post["UnitPrice"]);
        // int Quantity = Convert.ToInt32(post["Quantity"]);
        // int SMTotal  = Convert.ToInt32(post["UnitPrice"])* Convert.ToInt32(post["Quantity"]);
        // POrderAllModel POrderAllModel = new POrderAllModel(UId, ProductId, ProductName, UnitPrice, Quantity, SMTotal);                      
        // return View("OrderIndex", POrderAllModel);//推進資料庫POrderAllModel  給訂單頁面OrderIndex用
        //}
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