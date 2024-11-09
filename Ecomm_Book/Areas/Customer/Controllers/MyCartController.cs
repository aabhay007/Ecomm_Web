using Ecomm_Book.DataAccess.Repository.IRepository;
using Ecomm_Book.Models;
using Ecomm_Book.Models.ViewModels;
using Ecomm_Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Stripe;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text;
using Ecomm_Book.DataAccess.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecomm_Book.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class MyCartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private static bool isEmailConfirm = false;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;
        public MyCartController(IUnitOfWork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claims == null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<MyShoppingCart>()
                };
                return View(ShoppingCartVM);
            }
            //*
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.MyShoppingCart.GetAll(msc => msc.ApplicationUserId == claims.Value, includeProperties: "MyProduct"),
                MyOrderHeader = new MyOrderHeader()
            };
            ShoppingCartVM.MyOrderHeader.OrderTotal = 0;
            ShoppingCartVM.MyOrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault
                (au => au.Id == claims.Value);
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.MyProduct.Price, list.MyProduct.Price50, list.MyProduct.Price100);
                ShoppingCartVM.MyOrderHeader.OrderTotal += (list.Price * list.Count);
                if (list.MyProduct.Description.Length > 100)
                {
                    list.MyProduct.Description = list.MyProduct.Description.Substring(0, 99) + "...";
                }
            }
            //  Email Confirm
            if (!isEmailConfirm)
            {
                ViewBag.EmailMessage = "Email has been sent Kindly Verify Your Email";
                ViewBag.EmailCSS = "text-success";
                isEmailConfirm = false;
            }
            else
            {
                ViewBag.EmailMessage = "Email Must Be Confirm For Authorize Customer";
                ViewBag.EmailCSS = "text-danger";
            }
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.ApplicationUser.FirstOrDefault(au => au.Id == claims.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email Is Empty");
            }
            else
            {
                //email
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code, },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                //***
                isEmailConfirm = true;
            }
            return RedirectToAction("Index");
        }
        public IActionResult plus(int id)
        {
            var cart = _unitOfWork.MyShoppingCart.FirstOrDefault(msc => msc.Id == id);
            cart.Count += 1;
            _unitOfWork.save();
            return RedirectToAction("Index");
        }
        public IActionResult minus(int id)
        {
            var cart = _unitOfWork.MyShoppingCart.FirstOrDefault(msc => msc.Id == id);
            if (cart.Count == 1)
                cart.Count = 1;
            cart.Count -= 1;
            _unitOfWork.save();
            return RedirectToAction("Index");
        }
        public IActionResult delete(int id)
        {
            var cart = _unitOfWork.MyShoppingCart.FirstOrDefault(msc => msc.Id == id);
            _unitOfWork.MyShoppingCart.Remove(cart);
            _unitOfWork.save();
            //session
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var count = _unitOfWork.MyShoppingCart.GetAll(msc => msc.ApplicationUserId == claims.Value).ToList().Count;
            HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            return RedirectToAction("Index");
        }
        public IActionResult Summary(int[] check)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string aa = string.Join(",", check);
            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = _unitOfWork.MyShoppingCart.GetAll(s => aa.Contains(s.MyProductId.ToString()) && s.ApplicationUserId == claims.Value, includeProperties: "MyProduct"
                ),
                //ListCart = _unitOfWork.MyShoppingCart.GetAll(msc => msc.ApplicationUserId == claims.Value, includeProperties: "MyProduct"),
                MyOrderHeader = new MyOrderHeader(),
                AddressAnother = _context.AddressAnothers.Where(x => x.ApplicationUserId == claims.Value).Select(cl => new SelectListItem()
                {
                    Text = cl.AddressName,
                    Value = cl.Id.ToString()
                }).ToList(),
            };

            ShoppingCartVM.MyOrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.FirstOrDefault
                (au => au.Id == claims.Value);
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.MyProduct.Price, list.MyProduct.Price50,
                    list.MyProduct.Price100);
                ShoppingCartVM.MyOrderHeader.OrderTotal += (list.Price * list.Count);
                if (list.MyProduct.Description.Length > 100)
                {
                    list.MyProduct.Description = list.MyProduct.Description.Substring(0, 99) + "...";
                }
            }
            ShoppingCartVM.MyOrderHeader.Name = ShoppingCartVM.MyOrderHeader.ApplicationUser.Name;
            ShoppingCartVM.MyOrderHeader.StreetAddress = ShoppingCartVM.MyOrderHeader.ApplicationUser.StreetAddress;
            ShoppingCartVM.MyOrderHeader.State = ShoppingCartVM.MyOrderHeader.ApplicationUser.State;
            ShoppingCartVM.MyOrderHeader.City = ShoppingCartVM.MyOrderHeader.ApplicationUser.City;
            ShoppingCartVM.MyOrderHeader.PostalCode = ShoppingCartVM.MyOrderHeader.ApplicationUser.PostalCode;
            ShoppingCartVM.MyOrderHeader.PhoneNumber = ShoppingCartVM.MyOrderHeader.ApplicationUser.PhoneNumber;
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[ActionName("Summary")]
        public IActionResult SummaryPost(string stripeToken, string paypalToken)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            ShoppingCartVM.MyOrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.
                FirstOrDefault(au => au.Id == claims.Value);
            ShoppingCartVM.ListCart = _unitOfWork.MyShoppingCart.GetAll
                (sc => sc.ApplicationUserId == claims.Value, includeProperties: "MyProduct");
            ShoppingCartVM.MyOrderHeader.OrderStatus = SD.OrderStatusPending;
            ShoppingCartVM.MyOrderHeader.PaymentStatus = SD.PaymentStatusPending;
            ShoppingCartVM.MyOrderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.MyOrderHeader.ApplicationUserId = claims.Value;
            _unitOfWork.MyOrderHeader.Add(ShoppingCartVM.MyOrderHeader);
            _unitOfWork.save();
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedOnQuantity(list.Count, list.MyProduct.Price,
                    list.MyProduct.Price50, list.MyProduct.Price100);
                MyOrderDetail myOrderDetail = new MyOrderDetail()
                {
                    MyProductId = list.MyProductId,
                    MyOrderHeaderId = ShoppingCartVM.MyOrderHeader.Id,
                    Price = list.Price,
                    Count = list.Count
                };
                ShoppingCartVM.MyOrderHeader.OrderTotal += (list.Price * list.Count);
                _unitOfWork.MyOrderDetail.Add(myOrderDetail);
                _unitOfWork.save();
            }
            _unitOfWork.MyShoppingCart.RemoveRange(ShoppingCartVM.ListCart);
            _unitOfWork.save();
            //session 
            HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, 0);
            #region Stripe
            if (stripeToken == null)
            {
                ShoppingCartVM.MyOrderHeader.PaymentDueDate = DateTime.Now.AddDays(30);
                ShoppingCartVM.MyOrderHeader.PaymentStatus = SD.PaymentStatusDelayPayment;
                ShoppingCartVM.MyOrderHeader.OrderStatus = SD.OrderStatusApproved;
            }
            else
            {
                //payment process
                //var options = new ChargeCreateOptions()
                //{
                //    Amount=Convert.ToInt32(ShoppingCartVM.MyOrderHeader.OrderTotal),
                //    Currency="usd",
                //    Description="Order Id:" + ShoppingCartVM.MyOrderHeader.Id,
                //    Source=stripeToken
                //};
                //var service = new ChargeService();
                //Charge charge = service.Create(options);
                //if(charge.BalanceTransactionId==null)
                //    ShoppingCartVM.MyOrderHeader.PaymentStatus= SD.PaymentStatusRejected;
                //else
                //    ShoppingCartVM.MyOrderHeader.TransactionId=charge.BalanceTransactionId;
                //if(charge.Status.ToLower()=="succeeded")
                //{
                //    ShoppingCartVM.MyOrderHeader.OrderStatus = SD.OrderStatusApproved;
                //    ShoppingCartVM.MyOrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                //    ShoppingCartVM.MyOrderHeader.OrderDate = DateTime.Now;
                //}
                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(ShoppingCartVM.MyOrderHeader.OrderTotal),
                    Currency = "usd",
                    Description = "Order Id:" + ShoppingCartVM.MyOrderHeader.Id,
                    Source = stripeToken
                };
                var service = new ChargeService();
                Charge charge = service.Create(options);
                if (charge.BalanceTransactionId == null)
                    ShoppingCartVM.MyOrderHeader.PaymentStatus = SD.PaymentStatusRejected;
                else
                    ShoppingCartVM.MyOrderHeader.TransactionId = charge.BalanceTransactionId;
                if (charge.Status.ToLower() == "succeeded")
                {
                    ShoppingCartVM.MyOrderHeader.OrderStatus = SD.OrderStatusApproved;
                    ShoppingCartVM.MyOrderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    ShoppingCartVM.MyOrderHeader.OrderDate = DateTime.Now;
                }
                _unitOfWork.save();
            }
            #endregion
            return RedirectToAction("OrderConfirmation", "MyCart", new { id = ShoppingCartVM.MyOrderHeader.Id });
        }
        public IActionResult OrderConfirmation(int id)
        {
            return View(id);
        }
    }
}
