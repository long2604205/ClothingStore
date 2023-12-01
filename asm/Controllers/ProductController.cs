using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using asm.Areas.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using asm.Models;
// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace asm.Controllers
{
    public class ProductController : Controller
    {
        private readonly asmIdentityDbContext _context;

        public ProductController(asmIdentityDbContext context)
        {
            _context = context;
        }
        // GET: /<controller>/
        public IActionResult axx(int? id, string sortOrder, int page = 1, int pageSize = 12)
        {
            IQueryable<Product> productsQuery = _context.Products.Where(p => p.Status == 1);

            var categories = _context.Categories
                .Where(c => c.Status == 1)
                .ToList();


            if (id.HasValue)
            {
                // Lấy danh mục và sản phẩm tương ứng nếu có Id được cung cấp
                var categoryWithProducts = _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefault(c => c.ID == id && c.Status == 1);

                if (categoryWithProducts == null)
                {
                    // Xử lý khi không tìm thấy danh mục
                    return NotFound();
                }

                // Nếu có Id, chỉ lấy sản phẩm của danh mục đó
                productsQuery = productsQuery.Where(p => p.CategoryID == id);
            }

            // Xử lý sắp xếp
            switch (sortOrder)
            {
                case "NewIn":
                    productsQuery = productsQuery.OrderByDescending(p => p.Created_at);
                    break;
                case "PriceLowToHigh":
                    productsQuery = productsQuery.OrderBy(p => p.Price);
                    break;
                case "PriceHighToLow":
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
                default:
                    // Sắp xếp mặc định (có thể là sắp xếp mới nhất trước)
                    productsQuery = productsQuery.OrderByDescending(p => p.Created_at);
                    break;
            }


            int ProductCount = productsQuery.Count();
            // Phân trang
            int totalPages = (int)Math.Ceiling((double)ProductCount / pageSize);
            productsQuery = productsQuery.Skip((page - 1) * pageSize).Take(pageSize);


            var products = productsQuery.ToList();

            ViewData["SortOrder"] = sortOrder;
            ViewData["ListProduct"] = products;
            ViewData["ListCategory"] = categories;
            ViewData["ProductCount"] = ProductCount;

            ViewData["TotalPages"] = (int)totalPages;
            ViewData["CurrentPage"] = (int)page;

            return View();
        }
        public IActionResult Details(int Id)
        {
            var product = _context.Products
                .Include(p => p.Sizes)
                .ThenInclude(ps => ps.Size)
                .FirstOrDefault(p => p.ID == Id);

            if (product == null)
            {
                return NotFound();
            }

            var products_relate = _context.Products
                .OrderBy(x => Guid.NewGuid()) // Sắp xếp ngẫu nhiên
                .Take(8)
                .ToList();

            ViewData["RelatedProducts"] = products_relate;
            ViewData["Product"] = product;
            ViewData["ProductSizes"] = product.Sizes.Select(ps => ps.Size).ToList();

            return View();
        }

        public IActionResult Index(int? id, string search, string sortOrder, int page = 1, int pageSize = 12)
        {
            IQueryable<Product> productsQuery = _context.Products.Where(p => p.Status == 1);

            var categories = _context.Categories
                .Where(c => c.Status == 1)
                .ToList();


            if (id.HasValue)
            {
                // Lấy danh mục và sản phẩm tương ứng nếu có Id được cung cấp
                var categoryWithProducts = _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefault(c => c.ID == id && c.Status == 1);

                if (categoryWithProducts == null)
                {
                    // Xử lý khi không tìm thấy danh mục
                    return NotFound();
                }

                // Nếu có Id, chỉ lấy sản phẩm của danh mục đó
                productsQuery = productsQuery.Where(p => p.CategoryID == id);
            }

            if (!string.IsNullOrEmpty(search))
            {
                // Lọc sản phẩm theo tên hoặc SKU chứa chuỗi tìm kiếm
                productsQuery = productsQuery.Where(p => p.Name.Contains(search) || p.Sku.Contains(search));
            }

            // Xử lý sắp xếp
            switch (sortOrder)
            {
                case "NewIn":
                    productsQuery = productsQuery.OrderByDescending(p => p.Created_at);
                    break;
                case "PriceLowToHigh":
                    productsQuery = productsQuery.OrderBy(p => p.Price);
                    break;
                case "PriceHighToLow":
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
                default:
                    // Sắp xếp mặc định (có thể là sắp xếp mới nhất trước)
                    productsQuery = productsQuery.OrderByDescending(p => p.Created_at);
                    break;
            }


            int ProductCount = productsQuery.Count();
            // Phân trang
            int totalPages = (int)Math.Ceiling((double)ProductCount / pageSize);
            productsQuery = productsQuery.Skip((page - 1) * pageSize).Take(pageSize);


            var products = productsQuery.ToList();

            ViewData["SortOrder"] = sortOrder;
            ViewData["ListProduct"] = products;
            ViewData["ListCategory"] = categories;
            ViewData["ProductCount"] = ProductCount;

            ViewData["TotalPages"] = (int)totalPages;
            ViewData["CurrentPage"] = (int)page;

            return View();
        }


        public IActionResult Search(int? id, string search, string sortOrder, int page = 1, int pageSize = 12)
        {
            IQueryable<Product> productsQuery = _context.Products.Where(p => p.Status == 1);

            if (!string.IsNullOrEmpty(search))
            {
                // Lọc sản phẩm theo tên hoặc SKU chứa chuỗi tìm kiếm
                productsQuery = productsQuery.Where(p => p.Name.Contains(search) || p.Sku.Contains(search));
            }

            // Xử lý sắp xếp
            switch (sortOrder)
            {
                case "NewIn":
                    productsQuery = productsQuery.OrderByDescending(p => p.Created_at);
                    break;
                case "PriceLowToHigh":
                    productsQuery = productsQuery.OrderBy(p => p.Price);
                    break;
                case "PriceHighToLow":
                    productsQuery = productsQuery.OrderByDescending(p => p.Price);
                    break;
                default:
                    // Sắp xếp mặc định (có thể là sắp xếp mới nhất trước)
                    productsQuery = productsQuery.OrderByDescending(p => p.Created_at);
                    break;
            }


            int ProductCount = productsQuery.Count();
            // Phân trang
            int totalPages = (int)Math.Ceiling((double)ProductCount / pageSize);
            productsQuery = productsQuery.Skip((page - 1) * pageSize).Take(pageSize);


            var products = productsQuery.ToList();

            ViewData["SortOrder"] = sortOrder;
            ViewData["ListProduct"] = products;
            ViewData["ProductCount"] = ProductCount;

            ViewData["TotalPages"] = (int)totalPages;
            ViewData["CurrentPage"] = (int)page;

            return View();
        }
    }
}

