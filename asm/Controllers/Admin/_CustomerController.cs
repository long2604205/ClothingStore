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
    public class _CustomerController : Controller
    {
        private readonly UserManager<User> _userManager;
        public _CustomerController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        // GET: /<controller>/
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách người dùng có role là "Customer"
            var usersInRole = await _userManager.GetUsersInRoleAsync("Customer");

            // Truyền danh sách người dùng vào view hoặc thực hiện các thao tác khác
            ViewData["CustomerUsers"] = usersInRole;

            return View();
        }

        public async Task<IActionResult> UpdateStatus(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                // Đảo ngược trạng thái (nếu là 1 thì chuyển thành 2 và ngược lại)
                user.Status = (user.Status == 1) ? 2 : 1;

                // Lưu thay đổi
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Cập nhật thành công, chuyển hướng về trang danh sách
                    return RedirectToAction("Index");
                }
                else
                {
                    // Xử lý khi có lỗi trong quá trình cập nhật
                    ModelState.AddModelError(string.Empty, "Error updating user status.");
                    return View("Error");
                }
            }

            // Xử lý khi không tìm thấy người dùng
            return NotFound();
        }
    }
}

