using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Prj_CSharpGo.Models;
using Prj_CSharpGo.Models.CampreserveModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Prj_CSharpGo.Controllers
{
    public class CampreserveController : Controller
    {

        private readonly ILogger<CampreserveController> _logger;
        private WildnessCampingContext _context;

        private readonly Microsoft.AspNetCore.Hosting.IWebHostEnvironment _hostingEnvironment;


        public CampreserveController(ILogger<CampreserveController> logger, WildnessCampingContext dbContext, Microsoft.AspNetCore.Hosting.IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            _context = dbContext;
            _hostingEnvironment = hostingEnvironment;

        }


        public IActionResult Index()
        {

            ViewBag.startDateMin = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.startDateMax = DateTime.Now.AddMonths(3).ToString("yyyy-MM-dd");
            ViewBag.EndDateMin = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            ViewBag.EndDateMax = DateTime.Now.AddMonths(3).AddDays(1).ToString("yyyy-MM-dd");
            ReturnIndexModels returnIndexModels = new ReturnIndexModels();
            returnIndexModels.CampList = _context.Camps.ToList();

            return View("Index", returnIndexModels);
        }

        //取得CampQuantity
        public IActionResult GetCampList()
        {
            var campList = _context.Camps.Select(s => new { s.CampName, s.CampId, s.CampQuantity, s.Img}).ToList();
            return Json(campList);
        }


        [HttpPost]
        public IActionResult Index(IFormCollection post)
        {
            string userId = HttpContext.Session.GetString("userId") ?? "Guest";
            if (userId == "Guest")
            {
                return Redirect("/Auth/Login");
            }

            ViewBag.startDateMin = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.startDateMax = DateTime.Now.AddMonths(3).ToString("yyyy-MM-dd");
            ViewBag.EndDateMin = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            ViewBag.EndDateMax = DateTime.Now.AddMonths(3).AddDays(1).ToString("yyyy-MM-dd");

            
            ReturnIndexModels returnIndexModels = new ReturnIndexModels();        
            int id = Convert.ToInt32(post["CampName"]);
            DateTime? startday = post["StartDay"].ToString()==""?null:Convert.ToDateTime(post["StartDay"]);
            DateTime? endday = post["EndDay"].ToString() == "" ? null : Convert.ToDateTime(post["EndDay"]);
            int Quantity = Convert.ToInt32(post["CampQuantity"]);
            string errorMsg = "";
            //選擇值
            returnIndexModels.userFilterModel = new CampreserveUserFilterModel()
            {
                CampId = id,
                StartDate = post["StartDay"],
                EndDate = post["EndDay"],
                CampQuantity = Quantity
            };

            //租借日期、退租日期和帳數判斷
            returnIndexModels.CampList = _context.Camps.ToList();
            if (endday == null || startday == null)
            {
                errorMsg = "請選擇日期！";
            }
            else
            {
                var findCamp = returnIndexModels.CampList.Where(s => s.CampId == id).FirstOrDefault();
                var temp = _context.CampOrders.ToList();
                var orderList = _context.CampOrders.Where(s => s.CampId == id && s.StartDay <= startday && s.EndDay >= endday).ToList();

                DateTime dateTime = startday.Value;
                int dayCount = 0;
                int quantityCount = 0;
                var HolidayCount = 0;
                var WeekdayCount = 0;
                int Totalprice = 0;
                List<HolidayModel> holidays = GetHoliday();
                while (true)
                {
                    if ((int)dateTime.DayOfWeek == 2)
                    {
                        errorMsg = "星期二為公休日，請重新選擇日期！";
                        break;
                    }
                    else if (endday <= startday)
                    {
                        errorMsg = "請重新選擇退訂日期！";
                        break;
                    }
                    else if (endday == null || startday == null)
                    {
                        errorMsg = "請選擇日期！";
                        break;
                    }

                    quantityCount = orderList.Where(s => s.StartDay <= dateTime && dateTime <= s.EndDay).Sum(s => s.CampQuantity).Value;
                    var quantityCounta = quantityCount + Quantity;

                    if(quantityCounta > findCamp.CampQuantity)
                    {

                        if ((findCamp.CampQuantity - quantityCount) == 1)
                        {
                            errorMsg = "剩餘1個帳數可預約!";
                            break;

                        }
                        if ((findCamp.CampQuantity - quantityCount) == 2)
                        {
                            errorMsg = "剩餘2個帳數可預約!";
                            break;

                        }
                        if ((findCamp.CampQuantity - quantityCount) == 3)
                        {
                            errorMsg = "剩餘3個帳數可預約!";
                            break;
                        }
                        if ((findCamp.CampQuantity - quantityCount) == 4)
                        {
                            errorMsg = "剩餘4個帳數可預約!";
                            break;
                        }
                        else if (quantityCount >= findCamp.CampQuantity)
                        {
                            errorMsg = "已額滿無法預約!";
                            break;
                        }
                        else if (dateTime == endday)
                        {
                            break;
                        }

                    }             
                    else if (dateTime == endday)
                    {
                        break;
                    }
                    dayCount++;

                    //平日假日
                    if (holidays.Where(s => s.date == dateTime.ToString("yyyy/M/d")).Any())
                    {
                        HolidayCount += 1;
                        Totalprice += (int)findCamp.HolidayPrice;
                    }
                    else
                    {
                        WeekdayCount += 1;
                        Totalprice += (int)findCamp.WeekdayPrice;
                    }

                    dateTime = dateTime.AddDays(1);
                }


                if (errorMsg + "" == "")
                {
                    CampreserveOrderModel orderModel = new CampreserveOrderModel()
                    {
                        UserId = Convert.ToInt32(userId),
                        CampId = findCamp.CampId,
                        CampName = findCamp.CampName,
                        CampQuantity = Quantity,
                        StartDay = startday.Value,
                        EndDay = endday.Value,
                        TotalPricesmall = Totalprice,
                        TotalPricebig = Totalprice * Quantity,
                        HolidayPrice = (int)findCamp.HolidayPrice,
                        WeekdayPrice = (int)findCamp.WeekdayPrice,
                        Daycount = dayCount,
                        OrderDay = DateTime.Now,
                        PlusPrice = (int)findCamp.PlusPrice,
                    };
                    return RedirectToAction("ComfirmResult", "Campreserve", orderModel);
                }
            }
            
            ViewBag.message = errorMsg;
            return View("Index", returnIndexModels);
        }


        public IActionResult ComfirmResult(CampreserveOrderModel orderModel)
        {
            var order = orderModel.CampId;
            var Camp = _context.Camps.Find(order).Img;
            ViewBag.Img = Camp;


            return View(orderModel);
        }


        [HttpPost]
        public IActionResult ComfirmResult(SaveCampreserveOrderModel orderModel)
        {
            CampOrder campOrder = new CampOrder()
            {
                Approval = "SP",
                UserId = orderModel.UserId,
                CampId = orderModel.CampId,
                OrderDay = DateTime.Now,
                CampQuantity = orderModel.CampQuantity,
                StartDay = orderModel.StartDay,
                EndDay = orderModel.EndDay,
                WeekdayPrice = orderModel.WeekdayPrice,
                HolidayPrice = orderModel.HolidayPrice,
                Peoplenumber = orderModel.Peoplenumber,
                TotalPrice =  orderModel.TotalPricebig+ orderModel.PlusPrice * orderModel.Peoplenumber,

            };
            _context.CampOrders.Add(campOrder);
            _context.SaveChanges();

            HttpContext.Session.SetString("campmessage", "已完成預約!");
            return Redirect("/Campreserve/MemberOrder");
        }

        //取得例假日JSON資料
        private List<HolidayModel> GetHoliday()
        {
            string json = string.Empty;
            string webRootPath = _hostingEnvironment.WebRootPath;
            webRootPath = webRootPath + "\\" + "Holiday.json";
            using (FileStream fs = new FileStream(webRootPath, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.GetEncoding("utf-8")))
                {
                    json = sr.ReadToEnd().ToString();
                }
            }
            return JsonConvert.DeserializeObject<List<HolidayModel>>(json);
        }





        // 預約查詢

        public IActionResult MemberOrder()
        {
            string userId = HttpContext.Session.GetString("userId") ?? "Guest";

            // 找出目前已登入的使用者 id
            var userID = (from u in _context.Users
                          where u.UserId.ToString() == userId
                          select u.UserId).FirstOrDefault();

            if (userId == "Guest" || userID.ToString() != userId || userId == null)
            {
                HttpContext.Session.SetString("userToastr", "目前您的身分為訪客，請重新登入");
                return Redirect("/Auth/Login");
            }

            // 找出此會員的所有訂單
            var f_UserID = _context.Users.Find(userID);

            // ·························································································
            // ························ 兩種方法 : 找到此 UserId 的所有訂單 ······························
            // ····················· 10 ································· 10 ···························
            var MemberOrderView = _context.CampOrders.Where(o => o.UserId == f_UserID.UserId).ToList();
            // ····················· 20 ································· 20 ···························
            //var MOrder = (from u in _context.Orders
            //              where u.UserId == f_UserID.UserId
            //              select u).ToList();
            // ·························································································

            if (MemberOrderView == null)
            {
                HttpContext.Session.SetString("userToastr", "已為您查詢訂單，但未能成功");
                return View();
            }
            return View(MemberOrderView);
        }

        [HttpPost]
        // 會員中心 => 預約變更
        public IActionResult MemberOrder(int id)
        {
            string userId = HttpContext.Session.GetString("userId") ?? "Guest";

            // 找出目前已登入的使用者 id
            var userID = _context.Users.Where(u => u.UserId.ToString() == userId).FirstOrDefault();

            // 判斷 Session 傳入的 userId 身分是否為訪客
            if (userId == "Guest" || userID.UserId.ToString() != userId || userId == null)
            {
                HttpContext.Session.SetString("userToastr", "目前您的身分為訪客，請重新登入");
                return Redirect("/Auth/Login");
            }

            var orderDetail = _context.CampOrders.Where(o => o.CampOrderId == id).ToList()[0];



            // 藉由找出訂單ID 列出詳細訂單資訊
            if (orderDetail == null)
            {
                HttpContext.Session.SetString("userToastr", " SoS！顯示異常");
                return View();
            }

            orderDetail.Approval = "WL";
            _context.Update(orderDetail);
            _context.SaveChanges();

            HttpContext.Session.SetString("campdel", "取消成功!");
            //MemberOrder User_order = new MemberOrder()
            //{
            //    _order = await _context.Orders.FindAsync(id),
            //    OrderDetails = _context.OrderDetails.Where(o => o.OrderId == id),
            //    Products = await _context.Products.ToListAsync(),
            //    //Users = _context.Users.Where(u => u.UserId == f_order_user)
            //};

            //if (User_order == null)
            //{
            //    return NotFound();
            //}

            string userId1 = HttpContext.Session.GetString("userId") ?? "Guest";

            // 找出目前已登入的使用者 id
            var userID1 = (from u in _context.Users
                          where u.UserId.ToString() == userId1
                          select u.UserId).FirstOrDefault();

            if (userId1 == "Guest" || userID1.ToString() != userId1 || userId1 == null)
            {
                HttpContext.Session.SetString("userToastr", "目前您的身分為訪客，請重新登入");
                return Redirect("/Auth/Login");
            }

            // 找出此會員的所有訂單
            var f_UserID = _context.Users.Find(userID1);

            // ·························································································
            // ························ 兩種方法 : 找到此 UserId 的所有訂單 ······························
            // ····················· 10 ································· 10 ···························
            var MemberOrderView = _context.CampOrders.Where(o => o.UserId == f_UserID.UserId).ToList();
            // ····················· 20 ································· 20 ···························
            //var MOrder = (from u in _context.Orders
            //              where u.UserId == f_UserID.UserId
            //              select u).ToList();
            // ·························································································

            if (MemberOrderView == null)
            {
                HttpContext.Session.SetString("userToastr", "已為您查詢訂單，但未能成功");
                return View();
            }
            return View(MemberOrderView);

        }
    }
}
