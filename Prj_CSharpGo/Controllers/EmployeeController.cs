using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Prj_CSharpGo.Models;
using Prj_CSharpGo.Models.ViewModels;
using X.PagedList;
using System.IO;
using OfficeOpenXml;
using Microsoft.AspNetCore.Hosting;

namespace Prj_CSharpGo.Controllers
{
    public class EmployeeController : Controller
    {
        private WildnessCampingContext _context;
        private IWebHostEnvironment _hostEnvironment;
        public EmployeeController(WildnessCampingContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        //分頁用
        private int pageSize = 12;

        public IWebHostEnvironment HostEnvironment { get; }

        public IActionResult Test()
        {
            var data = _context.Users.ToList();
            var stream = new MemoryStream();
            using (var xlpackage = new ExcelPackage(stream))
            {
                var sheet = xlpackage.Workbook.Worksheets.Add("Loai");
                sheet.Cells.LoadFromCollection(data, true);
                xlpackage.Save();
            }
            stream.Position = 0;

            var fileName = $"Loai_{DateTime.Now.ToString("yyyy/MM/dd/hh/mm/ss")}.xlsx";
            return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
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
            Employee query = await _context.Employees.FirstOrDefaultAsync(m => m.EmployeeEmail == employee.EmployeeEmail);
            if (query != null && query.EmployeePassword == employee.EmployeePassword)
            {
                if (query.EmployeeStatus == null)
                {
                    HttpContext.Session.SetString("employeeToastr", "帳號已註銷");
                    return Redirect("/Employee/Login");
                }
                HttpContext.Session.SetString("employeeId", query.EmployeeId.ToString());
                HttpContext.Session.SetString("employeeName", query.EmployeeName.ToString());
                HttpContext.Session.SetString("employeeStatus", query.EmployeeStatus.ToString());
                HttpContext.Session.SetString("employeeToastr", "登入成功");
                return Redirect("/Employee/Index");
            }
            HttpContext.Session.SetString("employeeToastr", "帳號或密碼錯誤");
            return Redirect("/Employee/Login");
        }
        // 登出
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("employeeId");
            HttpContext.Session.SetString("employeeToastr", "登出成功");
            return Redirect("/Employee/Login");
        }

        // 員工資料頁面
        public async Task<IActionResult> Index(int Page = 1)
        {
            // 登入驗證
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }
            //return View(await _context.Employees.OrderBy(p => p.EmployeeId).ToPagedListAsync(Page, pageSize));
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
        public async Task<IActionResult> Create(Employee employee)
        {
            var queryemp = from o in _context.Employees
                           where o.EmployeeEmail == employee.EmployeeEmail
                           select o;
            if (queryemp.Count() != 0)
            {
                HttpContext.Session.SetString("employeeToastr", "帳號已存在");
                return View();
            }

            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("employeeToastr", "新增成功");
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

            var empId = _context.Employees.FirstOrDefault(m => m.EmployeeId == id);
            if (empId == null)
            {
                return NotFound();
            }

            Employee employee = await _context.Employees.FindAsync(id);
            return View("Edit", employee);
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
                HttpContext.Session.SetString("employeeToastr", "修改成功");
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

            var empId = _context.Employees.FirstOrDefault(m => m.EmployeeId == id);
            if (empId == null)
            {
                return NotFound();
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
            HttpContext.Session.SetString("employeeToastr", "刪除成功");
            return Redirect("/Employee/Index");
        }

        // 會員資料頁面
        public async Task<IActionResult> Member(int Page = 1)
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }
            return View(await _context.Users.OrderBy(p => p.UserId).ToPagedListAsync(Page, pageSize));
            //return View(await _context.Users.ToListAsync());
        }

        // 會員編輯頁面
        public async Task<IActionResult> MemberEdit(int? id)
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }

            var UsersId = _context.Users.FirstOrDefault(m => m.UserId == id);
            if (UsersId == null)
            {
                return NotFound();
            }

            var member = await _context.Users.FindAsync(id);
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MemberEdit([Bind("UserId,UserAccount,UserPassword,UserName,Birthday,Region,Address,Phone,Email,Img,DiscountCode,UpdateDate,UserStatus,IsSuccess,ConfirmPassword,Vip")] User member)
        {
            if (ModelState.IsValid)
            {
                _context.Update(member);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("employeeToastr", "修改成功");
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

            var OrderId = _context.OrderDetails.FirstOrDefault(m => m.OrderId == id);
            if (OrderId == null)
            {
                return NotFound();
            }

            EmployeeOrder emporder = new EmployeeOrder()
            {
                _order = await _context.Orders.FindAsync(id),
                OrderDetails = from o in _context.OrderDetails
                               where o.OrderId == id
                               select o,
                Products = await _context.Products.ToListAsync()
            };
            return View(emporder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderEdit([Bind("OrderId,OrderDate,PayMethod,TotalPrice,UserId,Approval,UserName,Address")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Update(order);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("employeeToastr", "修改成功");
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

        // 商品編輯頁面
        public async Task<IActionResult> ProductEdit(string id)
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }

            var ProductsId = _context.Products.FirstOrDefault(m => m.ProductId == id);
            if (ProductsId == null)
            {
                return NotFound();
            }
            ViewData["Img"] = from o in _context.ProductImgs
                              where o.ProductId == id
                              select o;

            var member = await _context.Products.FindAsync(id);
            var upimg = new UpImg
            {
                ProductId = member.ProductId,
                CategoryId = member.CategoryId,
                ProductName = member.ProductName,
                ProductDescription = member.ProductDescription,
                Specification = member.Specification,
                Cost = member.Cost,
                UnitPrice = member.UnitPrice,
                UnitInStock = member.UnitInStock,
                Status = member.Status,
                Approval = member.Approval,
                CategoryType = member.CategoryType
            };
            return View(upimg);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProductEdit(UpImg productImg, [Bind("ProductId,CategoryId,ProductName,ProductDescription,Specification,UnitPrice,UnitInStock,Status,CategoryType")] Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.CategoryType.Substring(0, 1) == "a")
                {
                    product.CategoryId = "A ";
                }
                else if (product.CategoryType.Substring(0, 1) == "b")
                {
                    product.CategoryId = "B ";
                }
                else if (product.CategoryType.Substring(0, 1) == "c")
                {
                    product.CategoryId = "C ";
                }
                else if (product.CategoryType.Substring(0, 1) == "d")
                {
                    product.CategoryId = "D ";
                }
                else
                {
                    product.CategoryId = "E ";
                };

                if (productImg.ImageFile != null)
                {
                    string[] subs = productImg.ImageFile.FileName.Split('.');
                    string NewImgName = product.ProductId + "." + subs[1];

                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(productImg.ImageFile.FileName);
                    string extension = Path.GetExtension(productImg.Img);
                    productImg.Img = fileName = fileName + product.ProductId + extension;
                    string path = Path.Combine(wwwRootPath + "/Img/ProductsImg", NewImgName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await productImg.ImageFile.CopyToAsync(fileStream);
                    }
                    ProductImg temp = new ProductImg();
                    temp.Img = NewImgName;
                    temp.ProductId = product.ProductId;
                    _context.Add(temp);
                }

                _context.Update(product);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("employeeToastr", "修改成功");
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
        public async Task<IActionResult> ProductCreate(UpImg productImg, [Bind("ProductId,CategoryId,ProductName,ProductDescription,Specification,UnitPrice,UnitInStock,Status,CategoryType")] Product product)
        {
            if (await _context.Products.FindAsync(product.ProductId) != null)
            {
                HttpContext.Session.SetString("employeeToastr", "商品已存在");
                return View();
            }
            if (ModelState.IsValid)
            {
                if (product.CategoryType.Substring(0, 1) == "a")
                {
                    product.CategoryId = "A ";
                }
                else if (product.CategoryType.Substring(0, 1) == "b")
                {
                    product.CategoryId = "B ";
                }
                else if (product.CategoryType.Substring(0, 1) == "c")
                {
                    product.CategoryId = "C ";
                }
                else if (product.CategoryType.Substring(0, 1) == "d")
                {
                    product.CategoryId = "D ";
                }
                else
                {
                    product.CategoryId = "E ";
                };
                if (productImg != null)
                {
                    string[] subs = productImg.ImageFile.FileName.Split('.');
                    string NewImgName = product.ProductId + "." + subs[1];

                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(productImg.ImageFile.FileName);
                    string extension = Path.GetExtension(productImg.Img);
                    productImg.Img = fileName = fileName + product.ProductId + extension;
                    string path = Path.Combine(wwwRootPath + "/Img/ProductsImg", NewImgName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await productImg.ImageFile.CopyToAsync(fileStream);
                    }
                    ProductImg temp = new ProductImg();
                    temp.Img = NewImgName;
                    temp.ProductId = product.ProductId;
                    _context.Add(temp);
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("employeeToastr", "新增商品成功");
                return Redirect("/Employee/Product");
            }
            return View(product);
        }



        public async Task<IActionResult> Camp()
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }
            return View(await _context.Camps.ToListAsync());
        }

        // 會員編輯頁面
        public async Task<IActionResult> CampEdit(int? id)
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }

            var UsersId = _context.Camps.FirstOrDefault(m => m.CampId == id);
            if (UsersId == null)
            {
                return NotFound();
            }

            var camp = await _context.Camps.FindAsync(id);
            return View(camp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CampEdit(IFormFile ImgName, [Bind("CampId,CampName,CampSize,CampQuantity,WeekdayPrice,HolidayPrice,LimitPeople,Description,Approval,PlusPrice,Ahref,Img")] Camp camp)
        {

            if (ModelState.IsValid)
            {
                if (ImgName != null)
                {
                    string[] subs = ImgName.FileName.Split('.');
                    string[] subs2 = camp.Ahref.Split("/");
                    string NewImgName = subs2[1] + "." + subs[1];

                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Path.GetFileNameWithoutExtension(ImgName.FileName);
                    string extension = Path.GetExtension(camp.Img);

                    string path = Path.Combine(wwwRootPath + "/Img", NewImgName);
                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await ImgName.CopyToAsync(fileStream);
                    }
                    camp.Img = NewImgName;
                }

                _context.Update(camp);
                await _context.SaveChangesAsync();
                HttpContext.Session.SetString("employeeToastr", "修改成功");
                return Redirect("/Employee/Camp");
            }
            return View(camp);
        }


        //預約
        public async Task<IActionResult> CampOrder()
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }

            return View(await _context.CampOrders.ToListAsync());
        }

        [HttpPost]
        // 會員中心 => 預約變更
        public IActionResult CampOrder(int id,string Approval)
        {
            string empSession = HttpContext.Session.GetString("employeeId") ?? "Guest";
            if (empSession == "Guest")
            {
                return Redirect("/Employee/Login");
            }


            var orderDetail1 = _context.CampOrders.Where(o => o.CampOrderId == id).ToList();


            // 藉由找出訂單ID 列出詳細訂單資訊
            if (orderDetail1 == null)
            {
                HttpContext.Session.SetString("campdel", " SoS！顯示異常");
                return View();
            }
            var orderDetail = orderDetail1[0];
            orderDetail.Approval = Approval;
            _context.Update(orderDetail);
            _context.SaveChanges();
            HttpContext.Session.SetString("campdel", "變更成功!");


            return View(_context.CampOrders.ToList());

        }
    }
}
