using Ecomm_Book.Models;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm_Book.Areas.Customer.Controllers
{
    public class AddressAnotherController : Controller
    {
        [Area("Customer")]
        public IActionResult Index()
        {
            AddressAnother addressAnother = new AddressAnother();
            return View(addressAnother);
        }
        [HttpPost]
        public IActionResult Upsert(AddressAnother addressAnother)
        {
            return RedirectToAction("Index", "Home");   
        }
    }
}
