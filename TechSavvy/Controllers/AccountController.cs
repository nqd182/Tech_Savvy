using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TechSavvy.Models;
using TechSavvy.Models.ViewModels;
using TechSavvy.Repository;

namespace TechSavvy.Controllers
{
    public class AccountController : Controller
    {
        private UserManager<AppUserModel> _userMangage; // quản lý user
        private SignInManager<AppUserModel> _signInManager; // quản lý đăng nhập
        public AccountController(UserManager<AppUserModel> userMangage, SignInManager<AppUserModel> signInManager)
        {
            _userMangage = userMangage;
            _signInManager = signInManager;
        }

        public IActionResult Login(string returnUrl)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl});
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (ModelState.IsValid)
            {
                Microsoft.AspNetCore.Identity.SignInResult result = await _signInManager.PasswordSignInAsync(loginVM.UserName, loginVM.Password, false, false);
                if (result.Succeeded)
                {
                    return Redirect(loginVM.ReturnUrl ?? "/");
                }
                ModelState.AddModelError("", "Sai tên hoặc mật khẩu");
            }
            return View(loginVM);
        }

   
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(UserModel user)
        {
            if (ModelState.IsValid)
            {
                AppUserModel newUser = new AppUserModel { UserName = user.UserName, Email = user.Email};
                IdentityResult result = await _userMangage.CreateAsync(newUser,user.Password);
                if (result.Succeeded)
                {
                    TempData["success"] = "Đăng ký tài khoản thành công";
                    return Redirect("/account/login");
                }
                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }
        public async Task<IActionResult> Logout(string returnUrl = "/")
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl);
        }

    }
}
