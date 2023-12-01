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

    public class _ProductController : Controller
    {
        private readonly asmIdentityDbContext _context;

        public _ProductController(asmIdentityDbContext context)
        {
            _context = context;
        }

        // [Authorize(Roles = "role3")]
        [Route("/administrator/product")]
        [HttpGet("/administrator/product")]
        // GET: _Product
        //[HttpGet("/administrator/_product")] // Đặt đường dẫn tùy chỉnh cho action
        public async Task<IActionResult> Index()
        {
            var asmIdentityDbContext = _context.Products.Include(p => p.Category);
            return View(await asmIdentityDbContext.ToListAsync());
        }

        [Route("/administrator/product")]
        [HttpGet("/administrator/product/details/{id}")]
        //[HttpGet("/administrator/_product/details/{id}")]
        // GET: _Product/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }
            // Truy vấn danh sách ProductSize liên quan đến sản phẩm
            product.Sizes = await _context.ProductSizes
                .Where(ps => ps.ProductID == product.ID)
                .Include(ps => ps.Size)
                .ToListAsync();

            return View(product);
        }
        [Route("/administrator/product")]
        [HttpGet("/administrator/product/create")]
        // GET: _Product/Create
        // [HttpGet("/administrator/_product/create")] // Đặt đường dẫn tùy chỉnh cho action
        //[Route("administrator/_product/create")]
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Name");
            return View();
        }

        // POST: _Product/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("ID,Name,Price,Quantity,Content,Summary,Sku,Alias,Image,Images,Status,CategoryID,Created_at,Updated_at")] Product product)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(product);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Name", product.CategoryID);
        //    return View(product);
        //}
        // [HttpGet("/administrator/_product/create")] // Đặt đường dẫn tùy chỉnh cho action
        //[HttpPost]
        [Route("/administrator/product")]
        [HttpPost("/administrator/product/create")]
        //[Route("administrator/_product/create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,Price,Quantity,Content,Summary,Sku,Alias,Images,Status,CategoryID,Created_at,Updated_at")] Product product, IFormFile File, List<IFormFile> ImageFiles)
        {
            if (ModelState.IsValid)
            {
                //// Xử lý tệp tin nếu nó đã được tải lên
                if (File != null && File.Length > 0)
                {
                    // Lưu tệp tin vào thư mục hoặc lưu đường dẫn vào cơ sở dữ liệu
                    string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "product", File.FileName);

                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await File.CopyToAsync(stream);
                    }

                    // Lưu đường dẫn ảnh vào model
                    product.Image = "~/uploads/product/" + File.FileName;
                }

                // Xử lý tệp tin nếu nó đã được tải lên
                foreach (var file in ImageFiles)
                {
                    if (file != null && file.Length > 0)
                    {
                        // Lưu tệp tin vào thư mục hoặc lưu đường dẫn vào cơ sở dữ liệu
                        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "product", "products", file.FileName);

                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }

                        // Lưu đường dẫn ảnh vào danh sách ảnh trong model
                        product.Images.Add("~/uploads/product/products/" + file.FileName);
                    }
                }

                //Code thêm
                product.Created_at = DateTime.Now;
                product.Updated_at = DateTime.Now;

                // Thêm sản phẩm vào cơ sở dữ liệu
                _context.Add(product);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            // Nếu ModelState không hợp lệ, xử lý lỗi cụ thể
            //var errorsList = new List<string>();

            //foreach (var modelStateEntry in ModelState.Values)
            //{
            //    foreach (var error in modelStateEntry.Errors)
            //    {
            //        // Thêm thông báo lỗi vào danh sách
            //        errorsList.Add(error.ErrorMessage);
            //    }
            //}

            // Lưu danh sách lỗi vào ViewData
            //ViewData["ErrorsList"] = errorsList;

            // Nếu ModelState không hợp lệ, trả về lại view với model và danh sách thể loại
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Name", product.CategoryID);
            return View(product);
        }

        [Route("/administrator/product")]
        [HttpGet("/administrator/product/edit/{id}")]
        //[HttpGet("/administrator/_product/edit/{id}")]
        // GET: _Product/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Name", product.CategoryID);
            return View(product);
        }

        // POST: _Product/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Price,Quantity,Content,Summary,Sku,Alias,Image,Images,Status,CategoryID,Created_at,Updated_at")] Product product)
        //{
        //    if (id != product.ID)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(product);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!ProductExists(product.ID))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Name", product.CategoryID);
        //    return View(product);
        //}



        //[HttpGet("/administrator/_product/edit/{id}")]
        // POST: _Product/Edit/5
        //[HttpPost]
        [Route("/administrator/product")]
        [HttpPost("/administrator/product/edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,Price,Quantity,Content,Summary,Sku,Alias,Image,Images,Status,CategoryID,Created_at,Updated_at")] Product product, IFormFile File, List<IFormFile> ImageFiles)
        {
            if (id != product.ID)
            {
                return NotFound();
            }

            ModelState.Remove("File"); // Xóa lỗi validation của trường File
            ModelState.Remove("ImageFiles"); // Xóa lỗi validation của trường File


            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy thông tin sản phẩm hiện tại từ database
                    var existingProduct = await _context.Products.FindAsync(id);

                    var _pro = await _context.Products.FindAsync(id);

                    // Cập nhật các trường chỉ khi có dữ liệu mới
                    existingProduct.Name = product.Name;
                    existingProduct.Price = product.Price;
                    existingProduct.Quantity = product.Quantity;
                    existingProduct.Content = product.Content;
                    existingProduct.Summary = product.Summary;
                    existingProduct.Sku = product.Sku;
                    existingProduct.Alias = product.Alias;
                    existingProduct.Status = product.Status;
                    existingProduct.CategoryID = product.CategoryID;


                    //if (File == null && File.Length < 0)
                    //{
                    //    existingProduct.Image = _pro.Image;
                    //}

                    // Kiểm tra và cập nhật ảnh mới nếu có
                    if (File != null && File.Length > 0)
                    {
                        // Xử lý tệp tin nếu nó đã được tải lên
                        string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "product", File.FileName);
                        using (var stream = new FileStream(imagePath, FileMode.Create))
                        {
                            await File.CopyToAsync(stream);
                        }
                        existingProduct.Image = "~/uploads/product/" + File.FileName;
                    }


                    //if (File == null && File.Length < 0)
                    //{
                    //    existingProduct.Images = _pro.Images;
                    //}


                    // Kiểm tra và cập nhật danh sách ảnh mới nếu có
                    if (ImageFiles != null && ImageFiles.Any())
                    {
                        existingProduct.Images.Clear(); // Xóa hết ảnh cũ
                        foreach (var file in ImageFiles)
                        {
                            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "product", "products", file.FileName);
                            using (var stream = new FileStream(imagePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            existingProduct.Images.Add("~/uploads/product/products/" + file.FileName);
                        }
                    }

                    existingProduct.Created_at = _pro.Created_at;
                    existingProduct.Updated_at = DateTime.Now;

                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }

            // Nếu ModelState không hợp lệ, xử lý lỗi cụ thể
            var errorsList = new List<string>();

            foreach (var modelStateEntry in ModelState.Values)
            {
                foreach (var error in modelStateEntry.Errors)
                {
                    // Thêm thông báo lỗi vào danh sách
                    errorsList.Add(error.ErrorMessage);
                }
            }

            // Lưu danh sách lỗi vào ViewData
            ViewData["ErrorsList"] = errorsList;

            ViewData["CategoryID"] = new SelectList(_context.Categories, "ID", "Name", product.CategoryID);
            return View(product);
        }




        //--------------

        // GET: _Product/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null || _context.Products == null)
        //    {
        //        return NotFound();
        //    }

        //    var product = await _context.Products
        //        .Include(p => p.Category)
        //        .FirstOrDefaultAsync(m => m.ID == id);
        //    if (product == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(product);
        //}

        //// POST: _Product/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    if (_context.Products == null)
        //    {
        //        return Problem("Entity set 'asmIdentityDbContext.Products'  is null.");
        //    }
        //    var product = await _context.Products.FindAsync(id);
        //    if (product != null)
        //    {
        //        _context.Products.Remove(product);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

    // [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Products == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FirstOrDefaultAsync(m => m.ID == id);
            if (product == null)
            {
                return NotFound();
            }

            // Thực hiện xóa ngay lập tức
            if (_context.Products != null)
            {
                var productToDelete = await _context.Products.FindAsync(id);
                if (productToDelete != null)
                {
                    _context.Products.Remove(productToDelete);
                    await _context.SaveChangesAsync();
                }
            }

            // Sau khi xóa, chuyển hướng đến một trang hoặc hành động khác
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return (_context.Products?.Any(e => e.ID == id)).GetValueOrDefault();
        }
    }
}
