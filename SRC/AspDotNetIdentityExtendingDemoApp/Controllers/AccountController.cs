using AspDotNetIdentityOwinDemoApp.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AspDotNetIdentityOwinDemoApp.Controllers
{
    public class AccountController : Controller
    {
        public UserManager<ExtendedUser> UserManager { get { return HttpContext.GetOwinContext().Get<UserManager<ExtendedUser>>(); } }
        public SignInManager<ExtendedUser, string> SignInManager { get { return HttpContext.GetOwinContext().Get<SignInManager<ExtendedUser, string>>(); } }
        
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            var signInResult = SignInManager.PasswordSignInAsync(model.Username, model.Password, isPersistent: true, shouldLockout: true);
            switch (signInResult.Result)
            {
                case SignInStatus.Success:
                    return RedirectToAction("Index", "Home");
                default:
                    ModelState.AddModelError("", "Invalid Credentials");
                    return View(model);
            }
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterModel model)
        {
            var identityUser = await UserManager.FindByNameAsync(model.Username);
            if (identityUser != null)
            {
                return RedirectToAction("Index", "Home");
            }

            var user = new ExtendedUser
            {
                UserName = model.Username,
                FullName = model.FullName,

            };
            user.Addresses.Add(new Address { AddressLine = model.AddressLine, Country = model.Country, UserId = user.Id});
            try
            {
                var identityResult = await UserManager.CreateAsync(user, model.Password);

                if (identityResult.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", identityResult.Errors.FirstOrDefault());
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException ex)
            {
                foreach (var e in ex.EntityValidationErrors)
                ModelState.AddModelError("", e.ValidationErrors.FirstOrDefault().ErrorMessage);
            }

            return View(model);
        }
    }

    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class RegisterModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string FullName { get; set; }
        public string AddressLine { get; set; }
        public string Country { get; set; }
    }
}