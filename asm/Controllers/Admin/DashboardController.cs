using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using asm.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace asm.Controllers.Admin
{
    public class DashboardController : Controller
    {

        private readonly asmIdentityDbContext _context;
        private readonly UserManager<User> _userManager;

        public DashboardController(asmIdentityDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        // GET: /<controller>/
        [Route("/administrator/dashboard")]
        [HttpGet("/administrator/dashboard")]
        public async Task<IActionResult> Index()
        {

            var asmIdentityDbContext = _context.Products;
            int productCount = asmIdentityDbContext.Count(p => p.Status == 1 || p.Status == 2);

            // Đếm tổng số người dùng
            var customersTask = _userManager.GetUsersInRoleAsync("Customer");
            var customers = await customersTask;

            int totalCustomers = customers.Count();

            int maleCustomers = customers.Count(u => u.Gender == 1 && _userManager.IsInRoleAsync(u, "Customer").Result);
            int femaleCustomers = customers.Count(u => u.Gender == 2 && _userManager.IsInRoleAsync(u, "Customer").Result);



            ViewBag.totalProduct = productCount;
            ViewBag.totalCustomer = totalCustomers;
            ViewBag.male = maleCustomers;
            ViewBag.female = femaleCustomers;
            return View();
        }
    }
}

