using Ecomm_Book.DataAccess.Repository.IRepository;
using Ecomm_Book.Models;
using Ecomm_Book.Models.ViewModels;
using Ecomm_Book.Utility;
using MessagePack;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ecomm_Book.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize (Roles =SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnviorment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnviorment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnviorment = webHostEnviorment;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            return Json(new {data=_unitOfWork.MyProduct.GetAll()});
        }
        public IActionResult Delete(int id)
        {
            var productInDb=_unitOfWork.MyProduct.Get(id);
            if (productInDb == null)
                return Json(new { success = false, message = "Something Went Wrong" });
            _unitOfWork.MyProduct.Remove(productInDb);
            _unitOfWork.save();
            return Json(new { success = true, message = "data deleted successfully" });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                MyProduct = new MyProduct(),
                CategoryList = _unitOfWork.Category.GetAll().Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value=cl.Id.ToString()
                }),
                CoverTypeList = _unitOfWork.CoverType.GetAll().Select(ctl => new SelectListItem()
                {
                    Text = ctl.Name,
                    Value = ctl.Id.ToString()
                }
                )
            };
            if (id == null) return View(productVM);
            productVM.MyProduct = _unitOfWork.MyProduct.Get(id.GetValueOrDefault());
            if (productVM.MyProduct == null) return NotFound();
            return View(productVM);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var webRootPath = _webHostEnviorment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    if (productVM.MyProduct.Id != 0)
                    {
                        var imageExists = _unitOfWork.MyProduct.Get(productVM.MyProduct.Id).ImageUrl;
                        productVM.MyProduct.ImageUrl = imageExists;
                    }
                    if (productVM.MyProduct.ImageUrl != null)
                    {
                        var imagePath = Path.Combine(webRootPath, productVM.MyProduct.ImageUrl.Trim('\\'));
                        if (System.IO.File.Exists(imagePath))
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var fileStream = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(fileStream);
                    }
                    productVM.MyProduct.ImageUrl = @"\images\products\" + fileName + extension;
                }
                else
                {
                    if (productVM.MyProduct.Id != 0)
                    {
                        var imageExists = _unitOfWork.MyProduct.Get(productVM.MyProduct.Id).ImageUrl;
                        productVM.MyProduct.ImageUrl = imageExists;
                    }
                }
                if (productVM.MyProduct.Id == 0)

                    _unitOfWork.MyProduct.Add(productVM.MyProduct);//create
                else
                    _unitOfWork.MyProduct.Update(productVM.MyProduct);//edit
                _unitOfWork.save();
                return RedirectToAction("Index");

            }
            else
            {
                productVM = new ProductVM()
                {
                    MyProduct = new MyProduct(),
                    CategoryList = _unitOfWork.Category.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.Id.ToString()
                    }),
                    CoverTypeList = _unitOfWork.CoverType.GetAll().Select(ctl => new SelectListItem()
                    {
                        Text = ctl.Name,
                        Value = ctl.Id.ToString()
                    }
                 )
                };
                if(productVM.MyProduct.Id == 0)
                {
                    productVM.MyProduct = _unitOfWork.MyProduct.Get(productVM.MyProduct.Id);
                }
                return View("productVM");
            }
        
        }
    }

}


