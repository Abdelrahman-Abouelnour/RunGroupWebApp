using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RunGroupWebApp.Data;
using RunGroupWebApp.Models;
using RunGroupWebApp.ViewModels;

namespace RunGroupWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ApplicationDbContext _context;
        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }
        public IActionResult Login()
        {
            var response = new LoginVM();
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginVM login)
        {
            if(!ModelState.IsValid) return View(login);
            var user = await _userManager.FindByEmailAsync(login.EmailAddress);
            if(user == null)
            {
                TempData["Error"] = "Wrong credentials";
                return View(login);
            }
            var passwordCheck = await _userManager.CheckPasswordAsync(user, login.Password);
            if (passwordCheck)
            {
                var result = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            TempData["Error"] = "Wrong credentials";
            return View(login);
        }
        public IActionResult Register()
        {
            var response = new RegisterVM();
            return View(response);
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM register)
        {
            if (!ModelState.IsValid) return View(register);
            var user = await _userManager.FindByEmailAsync(register.EmailAddress);
            if (user != null)
            {
                TempData["Error"] = "This email address is already in use";
                return View(register);
            }
            var newUser = new AppUser()
            {
                Email = register.EmailAddress,
                UserName = register.EmailAddress
            };
            var newUserResponse = await _userManager.CreateAsync(newUser, register.Password);
            if (newUserResponse.Succeeded) { await _userManager.AddToRoleAsync(newUser, UserRoles.User); }
            return RedirectToAction("Index", "Home");

        }
        [HttpPost]
        public async Task<IActionResult> Logout() { 
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Race");
        }


    }
}
