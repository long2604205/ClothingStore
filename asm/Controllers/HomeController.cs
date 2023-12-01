using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using asm.Models;
using asm.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace asm.Controllers;

public class HomeController : Controller
{
    private readonly asmIdentityDbContext _context;
    // private readonly ILogger<HomeController> _logger;

    public HomeController(asmIdentityDbContext context)
    {
        _context = context;
    }
    // public HomeController(ILogger<HomeController> logger)
    // {
    //     _logger = logger;
    // }

    public IActionResult Index()
    {
        // var products = _context.Products
        //     .OrderByDescending(p => p.Created_at)
        //     .Take(4)
        //     .ToList();

        // var products_hot = _context.Products
        //     .Where(p => !products.Contains(p)) // Loại bỏ sản phẩm mới nhất
        //     .OrderBy(x => Guid.NewGuid()) // Sắp xếp ngẫu nhiên
        //     .Take(4)
        //     .ToList();
        var products = _context.Products
            .Where(p => p.Status == 1)
            .OrderByDescending(p => p.Created_at)
            .Take(4)
            .ToList();

        var products_hot = _context.Products
            .Where(p => p.Status == 1 && !products.Contains(p)) // Loại bỏ sản phẩm mới nhất
            .OrderBy(x => Guid.NewGuid()) // Sắp xếp ngẫu nhiên
            .Take(4)
            .ToList();

        var productss = _context.Products.ToList();

        var productSizes = _context.ProductSizes
            .Include(ps => ps.Size)
            .GroupBy(ps => ps.ProductID)
            .ToDictionary(group => group.Key, group => group.Select(ps => ps.Size).ToList());

        ViewData["ProductSizes"] = productSizes;
        ViewData["RandomProducts"] = products_hot;
        ViewData["NewestProducts"] = products;

        return View();
    }

    public IActionResult ProductDetails(int Id)
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

