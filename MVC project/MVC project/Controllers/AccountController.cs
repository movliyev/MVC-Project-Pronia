using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MVC_Project.DAL;
using MVC_Project.Models;
using MVC_Project.ViewModels;
using System.Drawing;
using System.Text.RegularExpressions;

namespace MVC_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _useManager;
        private readonly SignInManager<AppUser> _signInManager;
        private object originalName;

        public AccountController(UserManager<AppUser>useManager, SignInManager<AppUser> signInManager)
        {
            _useManager = useManager;
            _signInManager = signInManager;
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task <IActionResult> Register(RegisterVM uservm)
        {
            if (!ModelState.IsValid) return View();
            AppUser user = new AppUser
            {
                Name = uservm.Name,
                Surname = uservm.Surname,
                UserName = uservm.UserName,
                Gender=uservm.Gender,
                Email = uservm.Email,   
            };
           
            IdentityResult result2 = await _useManager.CreateAsync( user,uservm.Email);
            if (!result2.Succeeded)
            {
              string email = (@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
                ModelState.AddModelError("Email", "Yanlis deyer");
                Regex regex = new Regex(email);
                return View();
            }

           


            IdentityResult result= await _useManager.CreateAsync(user, uservm.Password);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
                return View();
            }
           
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index","Home");  
        }

        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");    
        }
       
    }
}
