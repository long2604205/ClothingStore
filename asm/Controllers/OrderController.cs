using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using asm.Areas.Identity.Data;
using asm.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;
using System.Text.Json;
using System.ComponentModel;
using Newtonsoft.Json;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace asm.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<User> _userManager;

        private readonly asmIdentityDbContext _context;

        public OrderController(asmIdentityDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                // Lấy thông tin người dùng từ UserManager
                var user = await _userManager.FindByIdAsync(userId);

                // Truy vấn giỏ hàng của người dùng
                Cart cart = _context.Carts
                    .Include(c => c.CartItems)
                    .SingleOrDefault(c => c.UserId == userId);

                if (cart == null)
                {
                    RedirectToAction("Index", "Cart");
                }
                else
                {
                    // Truyền danh sách CartItem thông qua ViewData
                    ViewData["CartItems"] = cart.CartItems.ToList();
                }

                ViewData["UserAddress"] = user.Address;
                ViewData["UserCountry"] = user.Country;
                ViewData["UserCity"] = user.City;
                ViewData["UserPostcode"] = user.Postcode;
                ViewData["UserPhone"] = user.PhoneNumber;
                ViewData["UserEmail"] = user.Email;

                return View();
            }
            else
            {
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }
        }

        public async Task<ActionResult> List(string note)
        {
            var _payment = "Cash";
            var _note = note;
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            Cart cart = _context.Carts
                .Include(c => c.CartItems)
                .SingleOrDefault(c => c.UserId == userId);
            var cart_Id = cart.Id;

            List<CartItem> cartItems = await _context.CartItems
                .Where(ci => ci.CartId == cart_Id)
                .ToListAsync();


            // Cấu hình JsonSerializerSettings để xử lý vòng lặp tự tham chiếu
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                MaxDepth = 64 // Đặt giới hạn độ sâu nếu cần thiết
            };

            // Chuyển đổi List<CartItem> thành chuỗi JSON
            string cartItemsJson = JsonConvert.SerializeObject(cartItems, settings);

            // Lưu trữ chuỗi JSON vào TempData
            TempData["Note"] = _note;
            TempData["Payment"] = _payment;
            TempData["CartItems"] = cartItemsJson;
            TempData["CartId"] = cart.Id;
            TempData["CartUserId"] = cart.UserId;

            return RedirectToAction("Checkout");
        }

        public async Task<ActionResult> ListPay(string note)
        {
            var _payment = "Paypal";
            var _note = note;
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.FindByIdAsync(userId);

            Cart cart = _context.Carts
                .Include(c => c.CartItems)
                .SingleOrDefault(c => c.UserId == userId);
            var cart_Id = cart.Id;

            List<CartItem> cartItems = await _context.CartItems
                .Where(ci => ci.CartId == cart_Id)
                .ToListAsync();

            double total = 0;
            foreach (var _item in cartItems)
            {
                total += _item.Price * _item.Quantity;
            }

            TempData["Total"] = total.ToString();

            // Cấu hình JsonSerializerSettings để xử lý vòng lặp tự tham chiếu
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                MaxDepth = 64 // Đặt giới hạn độ sâu nếu cần thiết
            };

            // Chuyển đổi List<CartItem> thành chuỗi JSON
            string cartItemsJson = JsonConvert.SerializeObject(cartItems, settings);

            // Lưu trữ chuỗi JSON vào TempData
            TempData["Note"] = _note;
            TempData["Payment"] = _payment;
            TempData["CartItems"] = cartItemsJson;
            TempData["CartId"] = cart.Id;
            TempData["CartUserId"] = cart.UserId;

            return RedirectToAction("PaymentWithPaypal");
        }

        private string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            Random random = new Random();
            var code = new string(Enumerable.Repeat(chars, length)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return code;
        }
        public ActionResult Checkout()
        {
            // Lấy chuỗi JSON từ TempData
            string cartItemsJson = TempData["CartItems"] as string;

            // Cấu hình JsonSerializerSettings để xử lý vòng lặp tự tham chiếu
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                MaxDepth = 64 // Đặt giới hạn độ sâu nếu cần thiết
            };

            List<CartItem> _cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartItemsJson, settings);


            double total_this = 0;
            foreach (var _item in _cartItems)
            {
                total_this += _item.Price * _item.Quantity;
            }

            var _UserId = TempData["CartUserId"];
            var _note = TempData["Note"].ToString();
            var _payment = TempData["Payment"].ToString();

            _context.ChangeTracker.Clear();

            // Tạo một đối tượng Order mới (có thể sao chép dữ liệu từ existingOrder nếu cần)
            asm.Models.Order newOrder = new asm.Models.Order
            {
                Code = GenerateRandomCode(12),
                TotalAmount = total_this,
                Payment = _payment,
                Note = _note,
                Status = 1,
                Created_at = DateTime.Now,
                Updated_at = DateTime.Now,
                UserID = _UserId.ToString(),
                OrderDetails = new List<OrderDetail>() // Khởi tạo danh sách OrderDetails
            };

            // // Thêm đối tượng Order mới vào DbSet của DbContext
            // _context.Orders.Add(newOrder);

            // Thêm từng OrderDetail mới vào DbSet của DbContext
            foreach (var existingOrderDetail in _cartItems)
            {
                OrderDetail newOrderDetail = new OrderDetail
                {
                    Qty = existingOrderDetail.Quantity,
                    Price = existingOrderDetail.Price,
                    Size = existingOrderDetail.SizeName,
                    Created_at = DateTime.Now,
                    Updated_at = DateTime.Now,
                    ProductID = existingOrderDetail.ProductId,
                };
                newOrder.OrderDetails.Add(newOrderDetail);
                // newOrder.OrderDetails = new List<OrderDetail> { newOrderDetail };

                // Sử dụng AsNoTracking() để truy vấn mà không theo dõi đối tượng
                ProductSize productSize = _context.ProductSizes
                    .AsNoTracking()
                    .SingleOrDefault(ps => ps.ProductID == existingOrderDetail.ProductId && ps.Size.Name == existingOrderDetail.SizeName);

                if (productSize != null)
                {
                    productSize.StockQuantity -= existingOrderDetail.Quantity;
                    _context.Update(productSize); // Đánh dấu đối tượng là "đã thay đổi"
                }
            }
            _context.Orders.Add(newOrder);
            // Lưu thay đổi vào database
            _context.SaveChanges();


            TempData["UserIdCart"] = _UserId;
            return RedirectToAction("ClearCart");
        }

        public ActionResult ClearCart()
        {
            var userId = TempData["UserIdCart"].ToString();
            var cartToDelete = _context.Carts
                .Include(c => c.CartItems) // Đảm bảo nạp các CartItem
                .SingleOrDefault(c => c.UserId == userId);

            if (cartToDelete != null)
            {
                _context.Carts.Remove(cartToDelete);
                _context.SaveChanges();
            }
            return View("SuccessView");
        }


        public ActionResult PaymentWithPaypal(string Cancel = null)
        {
            //getting the apiContext  
            APIContext apiContext = PaypalConfiguration.GetAPIContext();
            try
            {
                //A resource representing a Payer that funds a payment Payment Method as paypal  
                //Payer Id will be returned when payment proceeds or click to pay  
                string payerId = Request.Query["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    //this section will be executed first because PayerID doesn't exist  
                    //it is returned by the create function call of the payment class  
                    // Creating a payment  
                    // baseURL is the url on which paypal sendsback the data.  
                    string baseURI = Request.Scheme + "://" + Request.Host + "/Order/PaymentWithPayPal?";                    //here we are generating guid for storing the paymentID received in session  
                    //which will be used in the payment execution  
                    var guid = Convert.ToString((new Random()).Next(100000));
                    //CreatePayment function gives us the payment approval url  
                    //on which payer is redirected for paypal account payment  
                    var createdPayment = this.CreatePayment(apiContext, baseURI + "guid=" + guid);
                    //get links returned from paypal in response to Create function call  
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            //saving the payapalredirect URL to which user will be redirected for payment  
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    // saving the paymentID in the key guid  
                    TempData["PaymentId"] = createdPayment.id;
                    TempData["Guid"] = guid;
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // This function exectues after receving all parameters for the payment  
                    var guid = Request.Query["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, TempData["PaymentId"] as string);
                    //If executed payment failed then we will show payment failure message to user  
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = ex.Message; // Truyền thông điệp lỗi đến view
                return View("FailureView");
            }
            //on successful payment, show success page to user.  
            return RedirectToAction("Checkout", "Order");
            // return View("SuccessView");
        }
        [HttpGet]
        public IActionResult SuccessView()
        {
            return View("SuccessView");
        }
        private PayPal.Api.Payment payment;
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }
        private Payment CreatePayment(APIContext apiContext, string redirectUrl)
        {

            // Lấy chuỗi JSON từ TempData
            // string cartItemsJson = TempData["CartItems"] as string;

            // // Cấu hình JsonSerializerSettings để xử lý vòng lặp tự tham chiếu
            // var settings = new JsonSerializerSettings
            // {
            //     ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            //     MaxDepth = 64 // Đặt giới hạn độ sâu nếu cần thiết
            // };

            // List<CartItem> _cartItems = JsonConvert.DeserializeObject<List<CartItem>>(cartItemsJson, settings);

            var total = TempData["Total"].ToString();
            //create itemlist and add item objects to it  
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            // double total = 0;
            // foreach (var _item in _cartItems)
            // {
            //     total += _item.Price * _item.Quantity;
            // }
            //Adding Item Details like name, currency, price etc  
            itemList.items.Add(new Item()
            {
                name = "Payment",
                currency = "USD",
                price = total,
                quantity = "1",
                sku = "sku"
            });
            var payer = new Payer()
            {
                payment_method = "paypal"
            };
            // Configure Redirect Urls here with RedirectUrls object  
            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };
            // Adding Tax, shipping and Subtotal details  
            var details = new Details()
            {
                tax = "0",
                shipping = "0",
                subtotal = total
            };
            //Final amount with details  
            var amount = new Amount()
            {
                currency = "USD",
                total = total, // Total must be equal to sum of tax, shipping and subtotal.  
                details = details
            };
            var transactionList = new List<Transaction>();
            // Adding description about the transaction  
            var paypalOrderId = DateTime.Now.Ticks;
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(), //Generate an Invoice No    
                amount = amount,
                item_list = itemList
            });
            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }

    }
}

