using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Prj_CSharpGo.Models;
using X.PagedList;
using X.PagedList.Mvc.Core;
using X.PagedList.Mvc;

namespace Prj_CSharpGo.Controllers
{
    public class SearchController : Controller
    {
        private readonly WildnessCampingContext _context;

        public SearchController(WildnessCampingContext context)
        {
            _context = context;
        }
       

        public List<Product> Products;
        public SelectList Publishs;
        private Category category;
        public string Category { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Description { get; set; }
        public string ProductNameSort { get; set; }
        public string CategorySort { get; set; }
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }

        public X.PagedList.IPagedList<Product> Search { get; set; }

        public async Task OnGetAsync(string Category, string searchString, string sortOrder, int? pageIndex)
        {

            ViewData["CurrentSort"] = sortOrder;
            this.Category = Category;
            int pageSize = 5;
            ViewData["SearchString"] = searchString;
            var page = pageIndex ?? 1;

            //查詢
            IQueryable<string> PublishingQuery = (IQueryable<string>)(from m in _context.Products
                                                                      orderby m.Category
                                                                      select m.Category);

            var products = from m in _context.Products
                           select m;


            if (!String.IsNullOrEmpty(searchString))
            {
                products = products.Where(s => s.Products.Contains(searchString));
            }

            if (!String.IsNullOrEmpty(Category))
            {
                products = products.Where(x => x.Category == category);
            }
            Publishs = new SelectList(await PublishingQuery.Distinct().ToListAsync());

            //排序
            ProductNameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            CategorySort = sortOrder == "Category" ? "category_desc" : "Category";

            ViewData["ProductNameSortParm"] = ProductNameSort;
            ViewData["CategorySortParm"] = CategorySort;


            switch (sortOrder)
            {

                case "name_desc":
                    products = products.OrderByDescending(s => s.Products);
                    break;

                case "Category":
                    products = products.OrderBy(s => s.Category);
                    break;

                case "category_desc":
                    products = products.OrderByDescending(s => s.Category);

                    break;

                default:
                    products = products.OrderBy(s => s.Products);
                    break;

            }

            products = (IQueryable<Product>)await products.AsNoTracking().ToPagedListAsync(page, pageSize);
            //將分頁結果放入ViewData供View使用
            ViewData[" products "] = products;

        }
    }
        //    // GET: ProductsSearch/Create
        //    public IActionResult Create()
        //    {
        //        ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId");
        //        return View();
        //    }

        //    // POST: ProductsSearch/Create
        //    // To protect from overposting attacks, enable the specific properties you want to bind to.
        //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Create([Bind("ProductId,CategoryId,ProductName,ProductDescription,Specification,Cost,UnitPrice,UnitInStock,Status,Approval")] Product product)
        //    {
        //        if (ModelState.IsValid)
        //        {
        //            _context.Add(product);
        //            await _context.SaveChangesAsync();
        //            return RedirectToAction(nameof(Index));
        //        }
        //        ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
        //        return View(product);
        //    }

        //    // GET: ProductsSearch/Edit/5
        //    public async Task<IActionResult> Edit(string id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var product = await _context.Products.FindAsync(id);
        //        if (product == null)
        //        {
        //            return NotFound();
        //        }
        //        ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
        //        return View(product);
        //    }

        //    // POST: ProductsSearch/Edit/5
        //    // To protect from overposting attacks, enable the specific properties you want to bind to.
        //    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //    [HttpPost]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> Edit(string id, [Bind("ProductId,CategoryId,ProductName,ProductDescription,Specification,Cost,UnitPrice,UnitInStock,Status,Approval")] Product product)
        //    {
        //        if (id != product.ProductId)
        //        {
        //            return NotFound();
        //        }

        //        if (ModelState.IsValid)
        //        {
        //            try
        //            {
        //                _context.Update(product);
        //                await _context.SaveChangesAsync();
        //            }
        //            catch (DbUpdateConcurrencyException)
        //            {
        //                if (!ProductExists(product.ProductId))
        //                {
        //                    return NotFound();
        //                }
        //                else
        //                {
        //                    throw;
        //                }
        //            }
        //            return RedirectToAction(nameof(Index));
        //        }
        //        ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryId", product.CategoryId);
        //        return View(product);
        //    }

        //    // GET: ProductsSearch/Delete/5
        //    public async Task<IActionResult> Delete(string id)
        //    {
        //        if (id == null)
        //        {
        //            return NotFound();
        //        }

        //        var product = await _context.Products
        //            .Include(p => p.Category)
        //            .FirstOrDefaultAsync(m => m.ProductId == id);
        //        if (product == null)
        //        {
        //            return NotFound();
        //        }

        //        return View(product);
        //    }

        //    // POST: ProductsSearch/Delete/5
        //    [HttpPost, ActionName("Delete")]
        //    [ValidateAntiForgeryToken]
        //    public async Task<IActionResult> DeleteConfirmed(string id)
        //    {
        //        var product = await _context.Products.FindAsync(id);
        //        _context.Products.Remove(product);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }

        //    private bool ProductExists(string id)
        //    {
        //        return _context.Products.Any(e => e.ProductId == id);
        //    }
    }






