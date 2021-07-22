using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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



        public CampreserveController(ILogger<CampreserveController> logger, WildnessCampingContext dbContext)
        {
            _logger = logger;
            _context = dbContext;

        }

        public IActionResult Index()
        {
            
            ViewBag.startDatevalue = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.startDatemax = DateTime.Now.AddMonths(3).ToString("yyyy-MM-dd");
            ViewBag.EndDatevalue = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            ViewBag.EndDatemax = DateTime.Now.AddMonths(3).AddDays(1).ToString("yyyy-MM-dd");
            ReturnIndexModels returnIndexModels = new ReturnIndexModels();
            returnIndexModels.CampList = _context.Camps.ToList();
            return View("Index",returnIndexModels);
        }
        public IActionResult GetCampList()
        {
            var campList = _context.Camps.Select(s => new { s.CampName, s.CampId, s.CampQuantity }).ToList();
            return Json(campList);
        }


        [HttpPost]
        public IActionResult Index(IFormCollection post)
        {
            int id = Convert.ToInt32(post["CampName"]);
            DateTime startday = Convert.ToDateTime(post["StartDay"]);
            DateTime endday = Convert.ToDateTime(post["EndDay"]);
            int Quantity = Convert.ToInt32(post["CampQuantity"]);

            //判斷 租借日期、退訂日期之間 &  星期二公休
            if (endday <= startday)
            {
                ViewBag.message = "請重新選擇退訂日期！";
            }
            else if (endday == startday)
            {
                ViewBag.message = "請重新選擇退訂日期！";
            }
            else if ((int)startday.DayOfWeek == 2 || (int)endday.DayOfWeek == 2)
            {
                ViewBag.message = "星期二為公休日請重新選擇！";
            }
            //篩選
            ViewBag.startDatevalue = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.startDatemax = DateTime.Now.AddMonths(3).ToString("yyyy-MM-dd");
            ViewBag.EndDatevalue = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd");
            ViewBag.EndDatemax = DateTime.Now.AddMonths(3).AddDays(1).ToString("yyyy-MM-dd");
            ReturnIndexModels returnIndexModels = new ReturnIndexModels();
            returnIndexModels.CampList = _context.Camps.ToList();
            var findCamp = returnIndexModels.CampList.Where(s => s.CampId == id).FirstOrDefault();
            var orderList = _context.CampOrders.Where(s => s.CampId == id && s.StartDay <= startday && s.EndDay <= endday).ToList();

            DateTime dateTime = startday;
            int dayCount = 0;
            int quantityCount = 0;
            while (true)
            {                                 
               if((int)dateTime.DayOfWeek == 2)
                {
                    ViewBag.message = "星期二為公休日請重新選擇！";
                    break;
                }
                quantityCount = orderList.Where(s => s.StartDay <= dateTime && dateTime < s.EndDay).Sum(s => s.CampQuantity).Value;
                if (quantityCount > findCamp.CampQuantity)
                {
                    ViewBag.message = "已額滿無法預約!";
                    break;
                }
                else if (dateTime == endday)
                {
                    break;
                }
                dayCount++;
                dateTime = dateTime.AddDays(1);

            }

            return View("Index", returnIndexModels);
        }


        //取得例假日JSON資料
        private List<HolidayModel> GetHoliday()
        {
            string json = string.Empty;
            string filepath = AppDomain.CurrentDomain.BaseDirectory;
            filepath = filepath + "Holiday.json";
            using (FileStream fs = new FileStream(filepath, FileMode.Open, System.IO.FileAccess.Read, FileShare.ReadWrite))
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
