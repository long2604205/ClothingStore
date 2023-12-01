using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using asm.Areas.Identity.Data;
using asm.Models;

namespace asm.Controllers.Admin
{
    public class _ProductSizeController : Controller
    {
        private readonly asmIdentityDbContext _context;

        public _ProductSizeController(asmIdentityDbContext context)
        {
            _context = context;
        }

        // GET: _ProductSize
        [Route("/administrator/product/productsize")]
        [HttpGet("/administrator/product/productsize")]
        public async Task<IActionResult> Index()
        {
            var asmIdentityDbContext = _context.ProductSizes.Include(p => p.Product).Include(p => p.Size);
            return View(await asmIdentityDbContext.ToListAsync());
        }

        // GET: _ProductSize/Details/5
        [Route("/administrator/product/productsize")]
        [HttpGet("/administrator/product/productsize/details")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.ProductSizes == null)
            {
                return NotFound();
            }

            var productSize = await _context.ProductSizes
                .Include(p => p.Product)
                .Include(p => p.Size)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (productSize == null)
            {
                return NotFound();
            }

            return View(productSize);
        }

        // GET: _ProductSize/Create
        [Route("/administrator/product/productsize")]
        [HttpGet("/administrator/product/productsize/create")]
        public IActionResult Create(int ProductID)
        {
            ViewData["ProductID"] = ProductID;
            //ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Alias");
            ViewData["SizeID"] = new SelectList(_context.Sizes, "ID", "Name");
            return View();
        }

        // POST: _ProductSize/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Route("/administrator/product/productsize")]
        [HttpPost("/administrator/product/productsize/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,ProductID,SizeID,StockQuantity")] ProductSize productSize)
        {
            if (ModelState.IsValid)
            {

                // Kiểm tra xem có ProductSize nào đã tồn tại với ProductID và SizeID đã cho chưa
                var existingProductSize = _context.ProductSizes
                    .FirstOrDefault(ps => ps.ProductID == productSize.ProductID && ps.SizeID == productSize.SizeID);

                if (existingProductSize != null)
                {
                    // Nếu tồn tại, cộng thêm số lượng vào StockQuantity
                    existingProductSize.StockQuantity += productSize.StockQuantity;
                }
                else
                {
                    // Nếu không tồn tại, thêm mới ProductSize
                    _context.ProductSizes.Add(productSize);
                }

                // _context.Add(productSize);
                await _context.SaveChangesAsync();
                // return RedirectToAction(nameof(Index));
                return RedirectToAction("details", "_product", new { id = productSize.ProductID });
            }
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Alias", productSize.ProductID);
            ViewData["SizeID"] = new SelectList(_context.Sizes, "ID", "Name", productSize.SizeID);
            return View(productSize);
        }

        // GET: _ProductSize/Edit/5
        [Route("/administrator/product/productsize")]
        [HttpGet("/administrator/product/productsize/edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.ProductSizes == null)
            {
                return NotFound();
            }

            var productSize = await _context.ProductSizes.FindAsync(id);
            if (productSize == null)
            {
                return NotFound();
            }

            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Alias", productSize.ProductID);
            ViewData["SizeID"] = new SelectList(_context.Sizes, "ID", "Name", productSize.SizeID);
            return View(productSize);
        }

        // POST: _ProductSize/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("/administrator/product/productsize")]
        [HttpPost("/administrator/product/productsize/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,ProductID,SizeID,StockQuantity")] ProductSize productSize)
        {
            if (id != productSize.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(productSize);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductSizeExists(productSize.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                // return RedirectToAction(nameof(Index));
                return RedirectToAction("details", "_product", new { id = productSize.ProductID });
            }
            ViewData["ProductID"] = new SelectList(_context.Products, "ID", "Alias", productSize.ProductID);
            ViewData["SizeID"] = new SelectList(_context.Sizes, "ID", "Name", productSize.SizeID);
            return View(productSize);
        }

        // GET: _ProductSize/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.ProductSizes == null)
            {
                return NotFound();
            }

            var productSize = await _context.ProductSizes
                .Include(p => p.Product)
                .Include(p => p.Size)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (productSize == null)
            {
                return NotFound();
            }

            return View(productSize);
        }

        // POST: _ProductSize/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.ProductSizes == null)
            {
                return Problem("Entity set 'asmIdentityDbContext.ProductSizes'  is null.");
            }
            var productSize = await _context.ProductSizes.FindAsync(id);
            if (productSize != null)
            {
                _context.ProductSizes.Remove(productSize);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductSizeExists(int id)
        {
            return (_context.ProductSizes?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
