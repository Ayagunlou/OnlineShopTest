using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Data;
using OnlineShop.Models;

namespace OnlineShop.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class UserController : Controller
    {
        UserManager<IdentityUser> _userManager;
        ApplicationDbContext _db;
        public UserController(UserManager<IdentityUser> userManager, ApplicationDbContext db)
        {
            _userManager= userManager;
            _db= db;
        }
        public IActionResult Index()
        {
            return View(_db.ApplicationUsers.ToList());
        }
        public async Task<IActionResult>Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(ApplicationUser user)
        {
            if(ModelState.IsValid)
            {
                user.EmailConfirmed= true;
                user.Email= user.UserName;
                var result =await _userManager.CreateAsync(user,user.PasswordHash+"Aa*");
                if(result.Succeeded)
                {
                    var isSaveRole = await _userManager.AddToRoleAsync(user,"USER");
                    TempData["save"] = "User has been create successfully";
                    return RedirectToAction(nameof(Index));
                }
                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View();
        }
        public async Task<IActionResult> Edit(string id)
        {
            var user =await _db.ApplicationUsers.FirstOrDefaultAsync(c=>c.Id == id);
            if(user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Edit (ApplicationUser user)
        {
            var userinfo = await _db.ApplicationUsers.FirstOrDefaultAsync(c => c.Id == user.Id);
            if(userinfo == null)
            {
                return NotFound();
            }
            userinfo.FristName= user.FristName;
            userinfo.LastName= user.LastName;
            userinfo.PhoneNumber = user.PhoneNumber;
            userinfo.Address = user.Address;
            var result = await _userManager.UpdateAsync(userinfo);
            if (result.Succeeded)
            {
                TempData["edit"] = "User has been update successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public async Task<IActionResult> Details(string id)
        {
            var user = _db.ApplicationUsers.FirstOrDefault(c => c.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        public async Task<IActionResult> Lockout(string id) 
        {
            if(id == null)
            {
                return NotFound();
            }
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(c=>c.Id== id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Lockout(ApplicationUser user)
        {
            var userinfo = await _db.ApplicationUsers.FirstOrDefaultAsync(c => c.Id == user.Id);
            if (userinfo == null)
            {
                return NotFound();
            }
            userinfo.LockoutEnd= DateTime.Now.AddYears(100);
            int rowAffected=await _db.SaveChangesAsync();
            if(rowAffected>0)
            {
                TempData["delete"] = "User has been lockout successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(userinfo);
        }

        public async Task<IActionResult>Active(string id)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(c => c.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Active(ApplicationUser user)
        {
            var userinfo = await _db.ApplicationUsers.FirstOrDefaultAsync(c => c.Id == user.Id);
            if (userinfo == null)
            {
                return NotFound();
            }
            userinfo.LockoutEnd = DateTime.Now.AddDays(-1);
            int rowAffected = await _db.SaveChangesAsync();
            if (rowAffected > 0)
            {
                TempData["save"] = "User has been active successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(userinfo);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var user = await _db.ApplicationUsers.FirstOrDefaultAsync(c => c.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            return View(user);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(ApplicationUser user)
        {
            var userinfo = await _db.ApplicationUsers.FirstOrDefaultAsync(c => c.Id == user.Id);
            if (userinfo == null)
            {
                return NotFound();
            }
            _db.ApplicationUsers.Remove(userinfo);
            int rowAffected = await _db.SaveChangesAsync();
            if (rowAffected > 0)
            {
                TempData["delete"] = "User has been active successfully";
                return RedirectToAction(nameof(Index));
            }
            return View(userinfo);
        }

    }
}
