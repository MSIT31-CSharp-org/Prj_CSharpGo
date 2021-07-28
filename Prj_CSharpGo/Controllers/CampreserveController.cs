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
                        errorMsg = "星期二為公休日請重新選擇！";
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
                            errorMsg = "剩餘1帳可預約!";
                            break;

                        }
                        if ((findCamp.CampQuantity - quantityCount) == 2)
                        {
                            errorMsg = "剩餘2帳可預約!";
                            break;

                        }
                        if ((findCamp.CampQuantity - quantityCount) == 3)
                        {
                            errorMsg = "剩餘3帳可預約!";
                            break;
                        }
                        if ((findCamp.CampQuantity - quantityCount) == 4)
                        {
                            errorMsg = "剩餘4帳可預約!";
                            break;
                        }
                        else if (quantityCount > findCamp.CampQuantity)
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
                        UserId = 1001,
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
                        PeoplePrice = (int)findCamp.PlusPrice,
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

            ViewBag.messagea = "已完成預約!";
            return View(orderModel);
        }


        [HttpPost]
        public IActionResult ComfirmResult(SaveCampreserveOrderModel orderModel)
        {
            CampOrder campOrder = new CampOrder()
            {
              
                UserId = orderModel.UserId,
                CampId = orderModel.CampId,
                OrderDay = DateTime.Now,
                CampQuantity = orderModel.CampQuantity,
                StartDay = orderModel.StartDay,
                EndDay = orderModel.EndDay,
                WeekdayPrice = orderModel.WeekdayPrice,
                HolidayPrice = orderModel.HolidayPrice,
                Peoplenumber = orderModel.Peoplenumber,
                TotalPrice =  orderModel.TotalPricebig+orderModel.PeoplePrice * orderModel.Peoplenumber,

            };
            _context.CampOrders.Add(campOrder);
            _context.SaveChanges();
            return Redirect("/Campreserve/Index");
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



    }
}
