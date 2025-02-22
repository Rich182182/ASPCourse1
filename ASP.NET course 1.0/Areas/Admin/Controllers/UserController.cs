using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Rich.DataAccess.Data;
using Rich.DataAccess.Repository;
using Rich.DataAccess.Repository.IReposetory;
using Rich.Models;
using Rich.Models.ViewModels;
using Rich.Utility;

namespace ASPRich.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;

        public UserController(ApplicationDbContext db, UserManager<IdentityUser> userManager)
        {
            _db = db;
            _userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult LockUnlock([FromBody] string id)
        {
            var obj = _db.ApplicationUsers.FirstOrDefault(u => u.Id == id);
            if (obj == null)
            {
                return Json(new { success = false, message = "Error while Locking/Unlocking" });
            }

            if (obj.LockoutEnd != null && obj.LockoutEnd > DateTime.Now)
            {
                //user is locked we need to unlock
                obj.LockoutEnd = DateTime.Now;
            }
            else
            {
                obj.LockoutEnd = DateTime.Now.AddYears(1000);
            }
            _db.SaveChanges();
            return Json(new { success = true, message = "Operation successful" });
        }
        [HttpGet]
        public IActionResult Permission(string id)
        {
            string RoleId = _db.UserRoles.FirstOrDefault(ur => ur.UserId == id).RoleId;
            UserVM userVM = new UserVM()
            {
                CompanyList = _db.Companies
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }).ToList(),
                RoleList = _db.Roles
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),

                }).ToList(),
                AppUser = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == id)
            };
            userVM.AppUser.Role = RoleId;
            return View(userVM);

        }
        [HttpPost]
        public IActionResult Permission(UserVM userVM)
        {
            var newRole = _db.Roles.FirstOrDefault(u => u.Id == userVM.AppUser.Role).Name;
            var roleId = _db.UserRoles.FirstOrDefault(ur => ur.UserId == userVM.AppUser.Id).RoleId;
            string oldRole = _db.Roles.FirstOrDefault(u => u.Id.ToLower() == roleId.ToLower()).Name;

            if (newRole != oldRole)
            {
                var user = _db.ApplicationUsers.FirstOrDefault(u => u.Id == userVM.AppUser.Id);

                // Если новая роль - "Company", обновляем CompanyId
                if (newRole == SD.Role_Company)
                {
                    user.CompanyId = userVM.AppUser.CompanyId;
                }
                // Если старая роль была "Company", но новая уже не "Company" — сбрасываем CompanyId
                else if (oldRole == SD.Role_Company)
                {
                    user.CompanyId = null;
                }

                _db.SaveChanges();
                _userManager.RemoveFromRoleAsync(user, oldRole).GetAwaiter().GetResult();
                _userManager.AddToRoleAsync(user, newRole).GetAwaiter().GetResult();
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<ApplicationUser> userList = _db.ApplicationUsers.Include(u => u.Company).OrderBy(user => user.Name).ToList();

            var userRoles = _db.UserRoles.ToList();
            var roles = _db.Roles.ToList();

            foreach (var user in userList)
            {

                var roleId = userRoles.FirstOrDefault(u => u.UserId == user.Id).RoleId;
                var role = roles.FirstOrDefault(u => u.Id == roleId).Name;
                user.Role = role;

                if (user.Company == null)
                {
                    user.Company = new() { Name = " " };
                }
            }

            return Json(new { data = userList });
        }

    }
}
/*var userRole = _db.UserRoles.FirstOrDefault(ur => ur.UserId == id);
            var roleName = userRole != null ? _db.Roles.FirstOrDefault(r => r.Id == userRole.RoleId)?.Name : null;

            UserVM userVM = new UserVM()
            {
                CompanyList = _db.Companies
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }).ToList(),
                RoleList = _db.Roles
                .Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString(),
                    Selected = (roleName != null && u.Name == roleName)
                }),
                AppUser = _db.ApplicationUsers.Include(u => u.Company).FirstOrDefault(u => u.Id == id)
            };
            
            return View(userVM);*/