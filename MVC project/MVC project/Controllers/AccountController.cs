using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using MVC_Project.DAL;
using MVC_Project.Models;
using MVC_Project.Utilities.Enums;
using MVC_Project.ViewModels;
using MVC_Project.ViewModels.Account;
using System.Drawing;
using System.Text.RegularExpressions;

namespace MVC_Project.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _useManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AccountController(UserManager<AppUser>useManager, SignInManager<AppUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _useManager = useManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
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
           
            //IdentityResult result2 = await _useManager.CreateAsync( user,uservm.Email);
            //if (!result2.Succeeded)
            //{
            //  string email = (@"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$");
            //    ModelState.AddModelError("Email", "Yanlis deyer");
            //    Regex regex = new Regex(email);
            //    return View();
            //}

           


            IdentityResult result= await _useManager.CreateAsync(user, uservm.Password);
            if (!result.Succeeded)
            {
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError(String.Empty, error.Description);
                }
                return View();
            }

            await _useManager.AddToRoleAsync(user, UseRole.Member.ToString());
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Index","Home");  
        }

        public IActionResult Login()
        {
            return View();
        }


        [HttpPost]  
        public async Task<IActionResult> Login(LoginVM lvm,string? returnurl)
        {
            if(!ModelState.IsValid)return View();
            AppUser user = await _useManager.FindByNameAsync(lvm.UserNameOrEmail);
            if (user is null)
            {
                user = await _useManager.FindByEmailAsync(lvm.UserNameOrEmail);
                if (user is null)
                {
                    ModelState.AddModelError(String.Empty, "Username,Email ve ya Password sehvdir");
                    return View();
                }
            }
          var result=  await _signInManager.PasswordSignInAsync(user,lvm.Password,lvm.IsRemembered,true);
            if(result.IsLockedOut)
            {
                ModelState.AddModelError(String.Empty, "Login olmur birazdan birde sinayin");
                return View();

            }
            if (!result.Succeeded)
            {
                ModelState.AddModelError(String.Empty, "Username,Email ve ya Password sehvdir");
                return View();
            }
            if(returnurl is null)
            {
                return RedirectToAction("Index", "Home");
            }
            return Redirect(returnurl);
        }


        public async Task<IActionResult> LogOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index","Home");    
        }


        public async Task<IActionResult> CreateRole()
        {
            foreach (UseRole role in Enum.GetValues(typeof(UseRole)))
            {
                if(!await _roleManager.RoleExistsAsync(role.ToString()))
                {
                    await _roleManager.CreateAsync(new IdentityRole
                    {
                        Name = role.ToString(),
                    });
                }
               
            }
            return RedirectToAction("Index", "Home");
        }

       
    }
}
