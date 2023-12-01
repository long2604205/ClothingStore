using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using asm.Areas.Identity.Data;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using asm.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Identity;



// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace asm.Controllers
{
    public class CartController : Controller
    {

        private readonly UserManager<User> _userManager;

        private readonly asmIdentityDbContext _context;

        public CartController(asmIdentityDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            // Kiểm tra xem người dùng đã đăng nhập hay chưa
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                // Truy vấn giỏ hàng của người dùng
                Cart cart = _context.Carts
                    .Include(c => c.CartItems)
                    .SingleOrDefault(c => c.UserId == userId);


                if (cart == null)
                {
                    // Truyền thông tin giỏ hàng trống thông qua ViewData
                    ViewData["CartItems"] = new List<CartItem>();
                }
                else
                {
                    // Truyền danh sách CartItem thông qua ViewData
                    ViewData["CartItems"] = cart.CartItems.ToList();
                }

                return View();
                // Người dùng chưa đăng nhập, chuyển hướng đến trang đăng nhập

            }
            else
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

        }


        public IActionResult AddToCart(int productId, int sizeId, int quantity, double price, string productName, string sizeName, string image)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                // Kiểm tra xem Cart của UserId đã tồn tại chưa
                Cart cart = _context.Carts.Include(c => c.CartItems).SingleOrDefault(c => c.UserId == userId);

                // Nếu Cart chưa tồn tại, tạo mới
                if (cart == null)
                {
                    cart = new Cart { UserId = userId, CartItems = new List<CartItem>() };
                    _context.Carts.Add(cart);
                }

                // Kiểm tra xem sản phẩm đã tồn tại trong Cart chưa
                CartItem existingCartItem = cart.CartItems.SingleOrDefault(ci => ci.ProductId == productId && ci.SizeId == sizeId);

                // Kiểm tra số lượng trong ProductSize
                ProductSize productSize = _context.ProductSizes.SingleOrDefault(ps => ps.ProductID == productId && ps.SizeID == sizeId);

                if (productSize != null && productSize.StockQuantity >= quantity)
                {
                    // Nếu sản phẩm đã tồn tại và số lượng đủ, cập nhật số lượng
                    if (existingCartItem != null)
                    {
                        existingCartItem.Quantity += quantity;
                    }
                    else
                    {
                        // Nếu sản phẩm chưa tồn tại, thêm mới CartItem
                        existingCartItem = new CartItem
                        {
                            ProductId = productId,
                            SizeId = sizeId,
                            Quantity = quantity,
                            Price = price,
                            ProductName = productName,
                            SizeName = sizeName,
                            Image = image,
                        };

                        cart.CartItems.Add(existingCartItem);
                    }

                    // Giảm số lượng trong ProductSize
                    // productSize.StockQuantity -= quantity;

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.SaveChanges();

                    return RedirectToAction("Index");
                }
                else
                {
                    // Số lượng không đủ, thực hiện hành động phù hợp, ví dụ trả về một view với thông báo lỗi
                    ModelState.AddModelError(string.Empty, "Số lượng không đủ");
                    return View("Error");
                }
            }
            else
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
        }

        [HttpPost]
        public ActionResult UpdateCartItem(int cartItemId, int newQuantity)
        {
            var cartItem = _context.CartItems.Find(cartItemId);

            if (cartItem != null)
            {
                if (newQuantity > 0)
                {
                    cartItem.Quantity = newQuantity;
                }
                else
                {
                    // Nếu số lượng mới là 0, xóa sản phẩm khỏi giỏ hàng
                    _context.CartItems.Remove(cartItem);
                }

                _context.SaveChanges();
            }
            // Handle the case when the cart item is not found

            return RedirectToAction("Index", "Cart"); // Chuyển hướng đến trang giỏ hàng sau khi cập nhật
        }

        public ActionResult RemoveCartItem(int cartItemId)
        {
            var cartItem = _context.CartItems.Find(cartItemId);

            if (cartItem != null)
            {
                _context.CartItems.Remove(cartItem);
                _context.SaveChanges();
            }
            // Handle the case when the cart item is not found

            return RedirectToAction("Index");
        }
    }
}

