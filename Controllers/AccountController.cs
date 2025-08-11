using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using CorporateCMS.Models;
using CorporateCMS.ViewModels;

namespace CorporateCMS.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    
                    // Kullanıcı bilgilerini al ve rolü kontrol et
                    var user = await _userManager.FindByEmailAsync(model.Email);
                    if (user != null)
                    {
                        var isAdmin = await _userManager.IsInRoleAsync(user, "Admin") || 
                                     await _userManager.IsInRoleAsync(user, "SuperAdmin");
                                     
                        // Admin/SuperAdmin ise admin paneline, değilse kullanıcı paneline veya ana sayfaya yönlendir
                        if (isAdmin)
                        {
                            return LocalRedirect(returnUrl ?? "/Admin/Admin");
                        }
                        else
                        {
                            return LocalRedirect(returnUrl ?? "/User/Dashboard");
                        }
                    }
                    
                    return LocalRedirect(returnUrl ?? "/");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz e-posta veya şifre.");
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult Register(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                
                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl ?? "/");
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return RedirectToAction("Index", "Home");
        }
        
        [HttpPost]
        [HttpGet]
        public IActionResult ExternalLogin(string provider, string? returnUrl = null)
        {
            // Request a redirect to the external login provider
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return Challenge(properties, provider);
        }
        
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string? returnUrl = null, string? remoteError = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            
            if (remoteError != null)
            {
                ModelState.AddModelError(string.Empty, $"Dış sağlayıcıdan hata: {remoteError}");
                return RedirectToAction(nameof(Login));
            }
            
            // Get the login info from external provider
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            
            if (result.Succeeded)
            {
                _logger.LogInformation("{Name} logged in with {LoginProvider} provider.", info.Principal.Identity?.Name ?? "Unknown user", info.LoginProvider);
                
                // Get user info for role check
                var email = info.Principal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;
                var user = !string.IsNullOrEmpty(email) ? await _userManager.FindByEmailAsync(email) : null;
                if (user != null)
                {
                    var isAdmin = await _userManager.IsInRoleAsync(user, "Admin") || 
                                 await _userManager.IsInRoleAsync(user, "SuperAdmin");
                                 
                    // Admin/SuperAdmin ise admin paneline, değilse kullanıcı paneline veya ana sayfaya yönlendir
                    if (isAdmin)
                    {
                        return LocalRedirect("/Admin/Admin");
                    }
                    else
                    {
                        return LocalRedirect("/User/Dashboard");
                    }
                }
                
                return LocalRedirect(returnUrl);
            }
            
            // If the user does not have an account, then create one
            if (result.IsLockedOut)
            {
                return RedirectToPage("./Lockout");
            }
            else
            {
                // Get the email from the external login provider
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                if (email != null)
                {
                    // Create a new user without a password if one doesn't already exist
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new IdentityUser
                        {
                            UserName = email,
                            Email = email,
                            EmailConfirmed = true // Since we got this from external provider, we can trust it's confirmed
                        };
                        
                        var createResult = await _userManager.CreateAsync(user);
                        if (!createResult.Succeeded)
                        {
                            foreach (var error in createResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View("Login");
                        }
                    }
                    
                    // Add the external login to the user
                    var addLoginResult = await _userManager.AddLoginAsync(user, info);
                    if (addLoginResult.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.LogInformation("User created an account using {Name} provider.", info.LoginProvider);
                        return LocalRedirect(returnUrl);
                    }
                }
                
                ModelState.AddModelError(string.Empty, "Dış sağlayıcıdan e-posta alınamadı.");
                return View("Login");
            }
        }
    }
}
