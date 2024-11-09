using Ecomm_Book.DataAccess.Repository.IRepository;
using Ecomm_Book.Models;
using Ecomm_Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm_Book.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var categoryList = _unitOfWork.Category.GetAll();
            return Json(new { data=categoryList});
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryInDb = _unitOfWork.Category.Get(id);
            if (categoryInDb == null)
                return Json(new {success=false,message="Something went wrong while deleting data !"});
            _unitOfWork.Category.Remove(categoryInDb);
            _unitOfWork.save();
            return Json(new { success = true, message = "Data Successfully Deleted!" });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            Category category=new Category();
            if(id==null) return View(category);//create
            //edit
            category = _unitOfWork.Category.Get(id.GetValueOrDefault());
            if (category==null) return NotFound();
            return View(category);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if(category==null) return NotFound();   
            if(!ModelState.IsValid) return View(category);
            if(category.Id==0)
                _unitOfWork.Category.Add(category);
            else
                _unitOfWork.Category.Update(category);
            _unitOfWork.save();
            return RedirectToAction("Index");
        }
    }
}
