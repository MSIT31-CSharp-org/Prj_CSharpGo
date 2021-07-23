using Microsoft.AspNetCore.Mvc;
using Prj_CSharpGo.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prj_CSharpGo.Controllers
{
    public class CampSearchController : Controller
    {
        private WildnessCampingContext _context;
        public CampSearchController(WildnessCampingContext context)
        {
            _context = context;
        }
        public IActionResult Index(string searchString)
        {
            var camp = from c in _context.Camps
                       select c;
            if (!string.IsNullOrEmpty(searchString))
            {
                camp = camp.Where(g => g.CampName.Contains(searchString));
             
            }
            List<Camp>campList = _context.Camps.ToList();
            return View("index",campList);
        }
    }
}
