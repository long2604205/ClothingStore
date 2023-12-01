using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using asm.Areas.Identity.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace asm.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly asmIdentityDbContext _context;
        public AccountController(UserManager<User> userManager, asmIdentityDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        // GET: /<controller>/
        public async Task<IActionResult> Index(string userId)
        {
            if (userId == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(userId);

            ViewBag.firstname = user?.FirstName ?? "NA";
            ViewBag.lastname = user?.LastName ?? "NA";
            ViewBag.image = user?.Image ?? "NA";
            ViewBag.email = user.Email;
            ViewBag.phone = user.PhoneNumber;
            ViewBag.image = user.Image;
            ViewBag.gender = user.Gender;
            ViewBag.address = user.Address;
            ViewBag.city = user.City;
            ViewBag.country = user.Country;
            ViewBag.postcode = user.Postcode;
            ViewBag.Id = user.Id;
            var roles = await _userManager.GetRolesAsync(user);
            ViewBag.userRole = roles.FirstOrDefault();

            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Index(string userId, User updatedUser)
        {
            if (userId != updatedUser.Id)
            {
                // return NotFound();
                // return RedirectToAction("Index", "_User");
            }

            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound();
                }

                // Cập nhật thông tin người dùng
                user.FirstName = updatedUser.FirstName;
                user.LastName = updatedUser.LastName;
                // Cập nhật các thuộc tính khác
                user.Email = updatedUser.Email;
                user.PhoneNumber = updatedUser.PhoneNumber;
                user.Gender = updatedUser.Gender;

                user.Address = updatedUser.Address;
                user.City = updatedUser.City;
                user.Country = updatedUser.Country;
                user.Postcode = updatedUser.Postcode;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Chỉnh sửa thành công, thực hiện các hành động khác nếu cần
                    // return RedirectToAction("Index", "_User"); // Chuyển hướng đến trang profile hoặc trang nào đó
                    TempData["msg"] = "Update information successfully";
                    return RedirectToAction("Index", "Account", new { userId = userId });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(updatedUser);
        }

        public async Task<IActionResult> Manage()
        {

            var currentUser = await _userManager.GetUserAsync(User);
            string currentUserId = currentUser.Id;

            var orders = _context.Orders
                .Where(o => o.UserID == currentUser.Id)
                .ToList();


            ViewData["Orders"] = orders;

            return View();
        }

        public IActionResult Cancel(int id)
        {
            if (id != null)
            {
                var order = _context.Orders.FirstOrDefault(o => o.ID == id);
                if (order != null)
                {
                    // Cập nhật trạng thái thành 5
                    order.Status = 5;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.SaveChangesAsync();

                    return RedirectToAction("Manage");
                }
            }
            return View();
        }
        public IActionResult Details(int id)
        {
            if (id != null)
            {
                var orderWithDetails = _context.OrderDetails
                    .Include(o => o.Product)
                    .Where(o => o.OrderID == id)
                    .ToList();

                ViewData["Details"] = orderWithDetails;
                ViewBag.id = id;

                return View();
            }
            return View();
        }
    }
}

