using Ecomm_Book.DataAccess.Repository.IRepository;
using Ecomm_Book.Models;
using Ecomm_Book.Models.ViewModels;
using Ecomm_Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Security.Claims;

namespace Ecomm_Book.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index(string searchString, string searchCriteria)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claims!=null)
            {
                var count = _unitOfWork.MyShoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartSessionCount, count);
            }
            ViewBag.SearchCriteria = new List<SelectListItem>
            {   
                new SelectListItem {Text="Author",Value="author"},
                new SelectListItem {Text="Title",Value ="title"}
            };
            var productList = _unitOfWork.MyProduct.GetAll(includeProperties: "Category,CoverType").Where(p => string.IsNullOrEmpty(searchString)
              || (searchCriteria == "author" && p.Author.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
              (searchCriteria == "title" && p.Title.Contains(searchString, StringComparison.OrdinalIgnoreCase))).ToList();
            ViewBag.SearchString = searchString;
            ViewBag.SelectedSearchCriteria = searchCriteria;
            return View(productList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [HttpGet]
        public IActionResult Details(int id)
        {
            var productInList = _unitOfWork.MyProduct.FirstOrDefault(p=>p.Id==id,includeProperties:"Category,CoverType");
            if (productInList == null) return NotFound();
            var Shoppingcart = new MyShoppingCart()
            {
                MyProduct= productInList,
                MyProductId=productInList.Id
            };
            return View(Shoppingcart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(MyShoppingCart Shoppingcart)
        {
            Shoppingcart.Id = 0;
            if(ModelState.IsValid)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var Claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if(Claims == null) return NotFound();
                Shoppingcart.ApplicationUserId = Claims.Value;
                var shoopingcartindb=_unitOfWork.MyShoppingCart.FirstOrDefault(msc=>msc.ApplicationUserId==Claims.Value 
                && msc.MyProductId==Shoppingcart.MyProductId);
                if (shoopingcartindb == null)
                    _unitOfWork.MyShoppingCart.Add(Shoppingcart);
                else
                    shoopingcartindb.Count += Shoppingcart.Count;
                _unitOfWork.save();
                return RedirectToAction("Index");
            }
            else
            {
                var productInList = _unitOfWork.MyProduct.FirstOrDefault(p => p.Id == Shoppingcart.Id, includeProperties: "Category,CoverType");
                if (productInList == null) return NotFound();
                var Shoppingcartedit = new MyShoppingCart()
                {
                    MyProduct = productInList,
                    MyProductId = productInList.Id
                };
                return View(Shoppingcartedit);
            }
          
        }
    }
}