using Dapper;
using Ecomm_Book.DataAccess.Repository.IRepository;
using Ecomm_Book.Models;
using Ecomm_Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecomm_Book.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize (Roles =SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll() 
        {
            // var CoverTypeList=_unitOfWork.CoverType.GetAll();
            var CoverTypeList = _unitOfWork.SP_CALL.List<CoverType>(SD.Proc_GetCoverTypes);
            return Json(new {data=CoverTypeList});
        }
        #endregion
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var coverTypeInDb = _unitOfWork.CoverType.Get(id);
            if (coverTypeInDb == null)
                return Json(new { success = false, message = "Something went wrong while delete data !" });
           DynamicParameters parameters= new DynamicParameters();
            parameters.Add("@id", id);
            _unitOfWork.SP_CALL.Execute(SD.Proc_DeleteCoverType, parameters);
            // _unitOfWork.CoverType.Remove(coverTypeInDb);
            //_unitOfWork.save();
            return Json(new { success = true, message = "Data Successfull Deleted!" });
        }
        

        public IActionResult Upsert(int? id)
        {
           CoverType coverType=new CoverType();
            if(id==null) return View(coverType);
          //  coverType=_unitOfWork.CoverType.Get(id.GetValueOrDefault());
          DynamicParameters param =new DynamicParameters();
            param.Add("@id", id);
            coverType = _unitOfWork.SP_CALL.OneRecord<CoverType>(SD.Proc_GetCoverType, param);
            if(coverType==null) return NotFound();
            return View(coverType);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if(coverType== null) return NotFound();
            if(!ModelState.IsValid) return View(coverType);
            DynamicParameters param=new DynamicParameters();
            param.Add("@name", coverType.Name);
            if(coverType.Id==0)
             //   _unitOfWork.CoverType.Add(coverType);
             _unitOfWork.SP_CALL.Execute(SD.Proc_CreateCoverType, param);
            else
            {
                param.Add("@id",coverType.Id);
                _unitOfWork.SP_CALL.Execute(SD.Proc_UpdateCoverType, param);
            }
              //  _unitOfWork.CoverType.Update(coverType);
            //_unitOfWork.save();
            return RedirectToAction("Index");
        }
    }
}
