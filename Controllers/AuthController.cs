using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TodoApp.Data;
using TodoApp.Models.Auth;


namespace TodoApp.Controllers
{
    /// <summary>
    /// Controller responsible for handling user authentication operations.
    /// </summary>
    public class AuthController : Controller
    {
        private readonly UserManager<UserAccountModel> _userManager;
        private readonly SignInManager<UserAccountModel> _signInManager;
        private readonly TodoDbContext _context;
        private readonly IConfiguration _config;
   

        /// <summary>
        /// Constructor for AuthController, initializing necessary services.
        /// </summary>
        public AuthController(UserManager<UserAccountModel> userManager, SignInManager<UserAccountModel> sigInManager, TodoDbContext context, IConfiguration config)
        {
            _userManager = userManager;
            _signInManager = sigInManager;
            _context = context;
            _config = config;
          
        }

        /// <summary>
        /// Displays the registration page.
        /// </summary>
        [HttpGet]
        public IActionResult Registration()
        {
            return View();
        }

        /// <summary>
        /// Handles user registration process.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registration(RegistrationViewModel registration)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Create new user
                    var user = new UserAccountModel { UserName = registration.UserName };
                    var result = await _userManager.CreateAsync(user, registration.Password);

                    if (result.Succeeded)
                    {
                        ModelState.Clear();
                        ViewBag.RegistrationMessage = $"Username {registration.UserName} registered successfully. You can now log-in.";
                        return RedirectToAction("Success", "Auth");
                    }
                    else
                    {
                        // Add errors to ModelState if registration fails
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle unexpected errors
                    TempData["ErrMessage"] = "An unexpected error occurred during registration. Please try again later.";
                    TempData["ErrDetails"] = ex.Message.Trim();
                    return RedirectToAction("Error", "Home");
                }
            }

            return View(registration);
        }

        /// <summary>
        /// Displays sucess page after successful registration.
        /// </summary>
        [HttpGet]
        public IActionResult Success()
        {
            return View();
        }

        /// <summary>
        /// Displays the login page.
        /// </summary>
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        /// <summary>
        /// Hadles the user login process
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Attempt to sign in user
                    var result = await _signInManager.PasswordSignInAsync(login.UserName, login.Password, isPersistent: false, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index", "Todos");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Username or password were incorrect. Try again");
                    }
                }
                catch (Exception ex)
                {
                    // Handle unexpected errors
                    TempData["ErrMessage"] = "An unexpected error occurred during login. Please try again later.";
                    TempData["ErrDetails"] = ex.Message.Trim();
                    return RedirectToAction("Error", "Home");
                }
            }
            return View(login);
        }

        /// <summary>
        /// Initiates google auth
        /// </summary>
        [HttpGet]
        public IActionResult GoogleLogin()
        {
            string redirectUrl = Url.Action("GoogleResponse", "Auth")!;
            var properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", redirectUrl);
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        /// <summary>
        /// Handles the response from Google auth--
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GoogleResponse()
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();

            if (info == null)
                return RedirectToAction("Login", "Auth");
            try
            {
                // Attempt to sign in with external 
                var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
                if (result.Succeeded)
                {
                    // User already exists, log him in
                    var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
                    if (user == null || user.UserName == null)
                    {
                        throw new Exception("Username not found. Cannot complete registration.");
                    }
                    
                    return RedirectToAction("Index", "Todos");
                }
                else
                {
                    // New user, create account
                    var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                    var username = info.Principal.FindFirstValue(ClaimTypes.Name);

                    var user = new UserAccountModel { UserName = email, Email = email };

                    var createResult = await _userManager.CreateAsync(user);

                    if (createResult.Succeeded)
                    {
                        await _userManager.AddLoginAsync(user, info);
                        await _signInManager.SignInAsync(user, isPersistent: false);
                 
                        return RedirectToAction("Index", "Todos");
                    }
                    else
                    {   //In case something happens when creating the user
                        throw new Exception("There was an error in completing your registration.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                TempData["ErrMessage"] = "An unexpected error occurred during Google sign-in. Please try again later.";
                TempData["ErrDetails"] = ex.Message.Trim();
                return RedirectToAction("Error", "Home");
            }

         }

        /// <summary>
        /// Handles user logout
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                // Sign out the user
                await _signInManager.SignOutAsync();

                // Remove JWT token cookie
                Response.Cookies.Delete("jwtToken");

                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Handle unexpected errors
                TempData["ErrMessage"] = "An unexpected error occurred during Sign Out. Please try again later.";
                TempData["ErrDetails"] = ex.Message.Trim();
                return RedirectToAction("Error", "Home");
            }
        }
    }
}