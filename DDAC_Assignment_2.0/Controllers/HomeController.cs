using DDAC_Assignment_2._0.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace DDAC_Assignment_2._0.Controllers
{
    public class HomeController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        BlobtheController blob = new BlobtheController();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult SubmitIdea()
        {
            blob.CreateBlobContainer();
            return View();
        }
        [HttpPost]
        public IActionResult SubmitIdea(IdeaVM model, IFormFile images)
        {

            blob.UploadBlob(images);
            List<String> x = blob.DisplayBlob(images);
            ViewData["Image"] = x;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model) 
        {
            if (ModelState.IsValid) 
            {
                var result = await _signInManager.PasswordSignInAsync(model.email, model.password,model.rememberMe, false);
                if (result.Succeeded) 
                {
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Invalid login attempts.");
            }
            return View();
        }

        [HttpGet]
        public IActionResult Login() 
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Register() 
        {
            if (!_roleManager.RoleExistsAsync("Customer").GetAwaiter().GetResult()) 
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
                await _roleManager.CreateAsync(new IdentityRole("Staff"));
                await _roleManager.CreateAsync(new IdentityRole("Customer"));
            } 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model) 
        {
            if (ModelState.IsValid) 
            {
                var user = new ApplicationUser
                {
                    Email = model.email,
                    UserName = model.userName,
                    PhoneNumber = model.contactNumber,
                    roleName = "Customer"
                }; 

                var result = await _userManager.CreateAsync(user, model.password);
                if (result.Succeeded) 
                {
                    
                    await _userManager.AddToRoleAsync(user, model.roleName);
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                } 
            }
            return View();
        }
    }   
}
