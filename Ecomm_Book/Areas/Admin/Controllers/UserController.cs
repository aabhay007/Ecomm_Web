using Ecomm_Book.DataAccess.Data;
using Ecomm_Book.DataAccess.Repository.IRepository;
using Ecomm_Book.Models;
using Ecomm_Book.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;

namespace Ecomm_Book.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin + "," + SD.Role_Employee)]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public UserController(IUnitOfWork unitOfWork,ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        public IActionResult GetAll()
        {
            var userList=_context.ApplicationUsers.ToList();/////////ASPNETUSERS
            var roles=_context.Roles.ToList();///////ASPNETROlES
            var userRoles=_context.UserRoles.ToList();/////////ASPNET USERROLES
            foreach(var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(r=>r.Id==roleId).Name;
                if(user.CompanyId!=null)
                {
                    user.Company = new Company()
                    {
                        Name=_unitOfWork.Company.Get(Convert.ToInt32(user.CompanyId)).Name
                    };

                }
                if(user.CompanyId==null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
            ///remove Admin Role users from user list
            var adminUsers = userList.FirstOrDefault(u => u.Role == SD.Role_Admin);
            userList.Remove(adminUsers);
            return Json(new {data=userList});
        }
        
        [HttpPost]
        public IActionResult LockUnlock([FromBody]string id)
        {
            bool isLocked = false;
            var userindb=_context.ApplicationUsers.FirstOrDefault(u=>u.Id==id);
            if (userindb == null)
                return Json(new { success=false,message="Something went wrong" });
            if(userindb!=null && userindb.LockoutEnd>DateTime.Now)
            {
                userindb.LockoutEnd=DateTime.Now;
                isLocked= false;
            }
            else
            {
                userindb.LockoutEnd=DateTime.Now.AddYears(100);
                isLocked= true;
            }
            _context.SaveChanges();
            return Json(new { success = true, message = isLocked == true ? "User Successfully Locked" : "User successfully Unlocked" });
        }
        #endregion
    }
}
