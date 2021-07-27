
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

    {
        public class SearchController : Controller
        {

        {
        }

        {

            IQueryable<string> RgenreQuery = from r in _context.Recipes
                                             orderby r.RecipeName
                                             select r.RecipeName;

            IQueryable<string> PgenreQuery = from r in _context.Products
                                             orderby r.ProductName
                                             select r.ProductName;



            var products = from p in _context.Products
                           select p;


            {
            }
            Publishs = new SelectList(await PublishingQuery.Distinct().ToListAsync());


            {



        }

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

        }



