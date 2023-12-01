using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using asm.Areas.Identity.Data;
using asm.Models;
using Microsoft.AspNetCore.Authorization;

namespace asm.Controllers.Admin
{
    // [Authorize(Roles = "Admin")]
    public class _CategoryController : Controller
    {
        private readonly asmIdentityDbContext _context;

        public _CategoryController(asmIdentityDbContext context)
        {
            _context = context;
        }

        [Route("/administrator/category")]     
        [HttpGet("/administrator/category")]
        // GET: _Category
        public async Task<IActionResult> Index()
        {
            return _context.Categories != null ?
                        View(await _context.Categories.ToListAsync()) :
                        Problem("Entity set 'asmIdentityDbContext.Categories'  is null.");
        }

        [Route("/administrator/category")]  
        [HttpGet("/administrator/category/details/{id}")]
        // GET: _Category/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.ID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        [Route("/administrator/category")]  
        // GET: _Category/Create
        [HttpGet("/administrator/category/create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: _Category/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Route("/administrator/category")]  
        [HttpPost("/administrator/category/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Status,Created_at,Updated_at")] Category category)
        {
            if (ModelState.IsValid)
            {
                //Code thêm
                category.Created_at = DateTime.Now;
                category.Updated_at = DateTime.Now;

                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        [Route("/administrator/category")]  
        [HttpGet("/administrator/category/edit/{id}")]
        // GET: _Category/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: _Category/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("/administrator/category")]  
        [HttpPost("/administrator/category/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Status,Created_at,Updated_at")] Category category)
        {
            if (id != category.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Đính kèm đối tượng category vào DbContext
                    _context.Attach(category);

                    // Cập nhật Updated_at thành thời điểm hiện tại
                    category.Updated_at = DateTime.Now;


                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        //// GET: _Category/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Categories == null)
        //    {
        //        return NotFound();
        //    }

        //    var category = await _context.Categories
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (category == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(category);
        //}

        //// POST: _Category/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Categories == null)
        //    {
        //        return Problem("Entity set 'asmIdentityDbContext.Categories'  is null.");
        //    }
        //    var category = await _context.Categories.FindAsync(id);
        //    if (category != null)
        //    {
        //        _context.Categories.Remove(category);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        // [HttpGet("/administrator/_category/delete/{id}")]
        // [HttpPost("/administrator/_category/delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FirstOrDefaultAsync(m => m.ID == id);
            if (category == null)
            {
                return NotFound();
            }

            // Thực hiện xóa ngay lập tức
            if (_context.Categories != null)
            {
                var categoryToDelete = await _context.Categories.FindAsync(id);
                if (categoryToDelete != null)
                {
                    _context.Categories.Remove(categoryToDelete);
                    await _context.SaveChangesAsync();
                }
            }

            // Sau khi xóa, chuyển hướng đến một trang hoặc hành động khác
            return RedirectToAction(nameof(Index));
        }
        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
