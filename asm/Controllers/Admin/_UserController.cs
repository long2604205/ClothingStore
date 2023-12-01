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
    public class _UserController : Controller
    {
        private readonly UserManager<User> _userManager;
        public _UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        // GET: /<controller>/
        public IActionResult Index()
        {
            // Lấy danh sách tất cả người dùng
            var allUsers = _userManager.Users.ToList();

            // Lấy Id của người dùng hiện tại
            var currentUserId = _userManager.GetUserId(User);

            // Lọc danh sách để chỉ giữ lại những người dùng không có vai trò "Customer" và không phải là người dùng hiện tại
            var usersNotInRole = allUsers
                .Where(u => !_userManager.IsInRoleAsync(u, "Customer").Result && u.Id != currentUserId)
                .ToList();

            // Làm bất cứ điều gì bạn muốn với danh sách người dùng ở đây

            // Truyền danh sách người dùng vào ViewData để sử dụng trong view
            ViewData["UsersNotInCustomerRole"] = usersNotInRole;

            return View();
        }
        public async Task<IActionResult> Delete(string userId)
        {
            // Kiểm tra xem có tồn tại người dùng với ID được chỉ định hay không
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                // Người dùng không tồn tại, có thể xử lý thông báo lỗi hoặc chuyển hướng
                return RedirectToAction("Index");
            }

            // Xóa người dùng
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                // Người dùng đã được xóa thành công, có thể chuyển hướng hoặc xử lý theo ý muốn
                return RedirectToAction("Index");
            }
            else
            {
                // Xử lý lỗi nếu cần thiết
                return RedirectToAction("Index");
            }
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

        [HttpGet]
        public async Task<IActionResult> Profile(string userId)
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
        public async Task<IActionResult> Profile(string userId, User updatedUser)
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

                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    // Chỉnh sửa thành công, thực hiện các hành động khác nếu cần
                    // return RedirectToAction("Index", "_User"); // Chuyển hướng đến trang profile hoặc trang nào đó
                    return RedirectToAction("Profile", "_User", new { userId = userId });
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(updatedUser);
        }
    }
}

