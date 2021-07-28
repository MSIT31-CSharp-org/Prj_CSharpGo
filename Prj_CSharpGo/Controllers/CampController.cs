using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Prj_CSharpGo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prj_CSharpGo.Controllers
{
    public class CampController : Controller
    {

        private readonly ILogger<CampController> _logger;
        private WildnessCampingContext _context;

        public CampController(ILogger<CampController> logger, WildnessCampingContext dbContext)
        {
            _logger = logger;
            _context = dbContext;
        }
        public IActionResult Index()
        {
            List<Camp> campList = _context.Camps.ToList();

            return View(campList);
        }

        public IActionResult grass()
        {
            return View();
        }

        public IActionResult wooden()
        {
            return View();
        }

        public IActionResult stone()
        {
            return View();
        }



    }
}
