using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Prj_CSharpGo.Models;

namespace Prj_CSharpGo.Controllers
{
    public class SearchController : Controller
    {
        private readonly WildnessCampingContext _context;

        public SearchController(WildnessCampingContext context)
        {
            _context = context;
        }

        // GET: ProductsSearch
        public async Task<IActionResult> Index(string sortorder, string searchString)
        {
            var wildnessCampingContext = _context.Products.Include(p => p.Category);
            return View(await wildnessCampingContext.ToListAsync());

        }
     
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
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

}

