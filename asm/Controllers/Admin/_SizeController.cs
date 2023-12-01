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
    public class _SizeController : Controller
    {
        private readonly asmIdentityDbContext _context;

        public _SizeController(asmIdentityDbContext context)
        {
            _context = context;
        }

        // GET: _Size
        [Route("/administrator/size")]
        [HttpGet("/administrator/size")]
        public async Task<IActionResult> Index()
        {
            return _context.Sizes != null ?
                        View(await _context.Sizes.ToListAsync()) :
                        Problem("Entity set 'asmIdentityDbContext.Sizes'  is null.");
        }

        // GET: _Size/Details/5
        [Route("/administrator/size")]
        [HttpGet("/administrator/size/details/{id}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Sizes == null)
            {
                return NotFound();
            }

            var size = await _context.Sizes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (size == null)
            {
                return NotFound();
            }

            return View(size);
        }

        // GET: _Size/Create
        [Route("/administrator/size")]
        // GET: _Category/Create
        [HttpGet("/administrator/size/create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: _Size/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Route("/administrator/size")]
        // GET: _Category/Create
        [HttpPost("/administrator/size/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name")] Size size)
        {
            if (ModelState.IsValid)
            {
                _context.Add(size);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(size);
        }

        [Route("/administrator/size")]
        [HttpGet("/administrator/size/edit/{id}")]
        // GET: _Size/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Sizes == null)
            {
                return NotFound();
            }

            var size = await _context.Sizes.FindAsync(id);
            if (size == null)
            {
                return NotFound();
            }
            return View(size);
        }

        // POST: _Size/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Route("/administrator/size")]
        [HttpPost("/administrator/size/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int? id, [Bind("ID,Name")] Size size)
        {
            if (id != size.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(size);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SizeExists(size.ID))
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
            return View(size);
        }

        // GET: _Size/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Sizes == null)
            {
                return NotFound();
            }

            var size = await _context.Sizes
                .FirstOrDefaultAsync(m => m.ID == id);
            if (size == null)
            {
                return NotFound();
            }

            // Thực hiện xóa ngay lập tức
            if (_context.Sizes != null)
            {
                var SizesToDelete = await _context.Sizes.FindAsync(id);
                if (SizesToDelete != null)
                {
                    _context.Sizes.Remove(SizesToDelete);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // // POST: _Size/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(int? id)
        // {
        //     if (_context.Sizes == null)
        //     {
        //         return Problem("Entity set 'asmIdentityDbContext.Sizes'  is null.");
        //     }
        //     var size = await _context.Sizes.FindAsync(id);
        //     if (size != null)
        //     {
        //         _context.Sizes.Remove(size);
        //     }

        //     await _context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }

        private bool SizeExists(int? id)
        {
            return (_context.Sizes?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
