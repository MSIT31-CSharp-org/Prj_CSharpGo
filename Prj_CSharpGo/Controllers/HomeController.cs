using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Prj_CSharpGo.Models;
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

        //測試喔~~~
        public HomeController(ILogger<HomeController> logger , WildnessCampingContext dbContext)
        {
            _logger = logger;
            _context = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string userName, string password)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return View();
            }
            // Session 的寫入
            HttpContext.Session.SetString("userName", userName);
            return Redirect("/Home/Index");
        }

        public IActionResult MemberCenter()
        {
            string userName = HttpContext.Session.GetString("userName") ?? "Guest";
            if (userName == "Guest")
            {
                return Redirect("/Home/Index");
            }
            return View();
        }
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("userName");
            return Redirect("/Home/Index");
        }
    }
}
