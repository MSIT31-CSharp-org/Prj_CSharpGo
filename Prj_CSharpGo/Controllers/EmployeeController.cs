using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Prj_CSharpGo.Models;


namespace Prj_CSharpGo.Controllers
{
    public class EmployeeController : Controller
    {
        private WildnessCampingContext _context;
        public EmployeeController(WildnessCampingContext context)
        {
            _context = context;
        }
        // 登入頁面
        public IActionResult Login()
        {
            
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(Employee employee)
        {
            // 驗證帳密
            Employee query = await _context.Employees.FirstOrDefaultAsync(m => m.EmployeeId == employee.EmployeeId);
            if (query != null && query.EmployeePassword == employee.EmployeePassword)
            {
                HttpContext.Session.SetString("employeeId", query.EmployeeId.ToString());
                HttpContext.Session.SetString("employeeName", query.EmployeeName.ToString());
                HttpContext.Session.SetString("employeeStatus", query.EmployeeStatus.ToString());
                return Redirect("/Employee/Index");
            }
            return Redirect("/Employee/Login");
        }
        // 登出
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("employeeId");
            return Redirect("/Employee/Login");
        }

        // 員工資料頁面
        public async Task<IActionResult> Index()
        {
            // 登入驗證
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }
            return View(await _context.Employees.ToListAsync());
        }

        // 新增帳號頁面
        public IActionResult Create()
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeId,EmployeePassword,EmployeeName,EmployeeEmail,EmployeeStatus")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                return Redirect("/Employee/Index");
            }
            return View(employee);
        }

        // 修改帳號頁面
        public async Task<IActionResult> Edit(int? id)
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }
            var employee = await _context.Employees.FindAsync(id);
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("EmployeeId,EmployeePassword,EmployeeName,EmployeeEmail,EmployeeStatus")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Update(employee);
                await _context.SaveChangesAsync();
                Employee query = await _context.Employees.FirstOrDefaultAsync(m => m.EmployeeId.ToString() == HttpContext.Session.GetString("employeeId"));
                HttpContext.Session.Remove("employeeStatus");
                HttpContext.Session.SetString("employeeStatus", query.EmployeeStatus.ToString());
                return Redirect("/Employee/Index");
            }
            return View(employee);
        }

        // 刪除帳號頁面
        public async Task<IActionResult> Delete(int? id)
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }
            var employee = await _context.Employees.FirstOrDefaultAsync(m => m.EmployeeId == id);
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();
            return Redirect("/Employee/Index");
        }

        // 會員資料頁面
        public async Task<IActionResult> Member()
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }
            return View(await _context.Users.ToListAsync());
        }

        // 會員編輯頁面
        public async Task<IActionResult> MemberEdit(int? id)
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }
            var member = await _context.Users.FindAsync(id);
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MemberEdit([Bind("UserId,UserAccount,UserPassword,UserName,Birthday,Region,Address,Phone,Email,Img,DiscountCode,UpdateDate,UserStatus,Vip")] User member)
        {
            if (ModelState.IsValid)
            {
                _context.Update(member);
                await _context.SaveChangesAsync();
                return Redirect("/Employee/Member");
            }
            return View(member);
        }

        // 訂單資料頁面
        public async Task<IActionResult> Order()
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }
            return View(await _context.Orders.ToListAsync());
        }

        // 訂單編輯頁面
        public async Task<IActionResult> OrderEdit(int? id)
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }

            var query = from o in _context.OrderDetails
                        where o.OrderId == id
                        select o;

            return View(await query.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderEdit([Bind("UserId,UserAccount,UserPassword,UserName,Birthday,Region,Address,Phone,Email,Img,DiscountCode,UpdateDate,UserStatus,Vip")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
            }
            return Redirect("/Employee/Order");
        }



        // 商品資料頁面
        public async Task<IActionResult> Product()
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }
            return View(await _context.Products.ToListAsync());
        }

        // 訂單編輯頁面
        public async Task<IActionResult> ProductEdit(string id)
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }

            var member = await _context.Products.FindAsync(id);
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit([Bind("ProductId,CategoryId,ProductName,ProductDescription,Specification,UnitPrice,UnitInStock,Status")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Update(product);
                await _context.SaveChangesAsync();
            }
                return Redirect("/Employee/Product");
        }

        // 新增商品頁面
        public IActionResult ProductCreate()
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductCreate([Bind("ProductId,CategoryId,ProductName,ProductDescription,Specification,UnitPrice,UnitInStock,Status")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return Redirect("/Employee/Product");
            }
            return View(product);
        }




    }
}
