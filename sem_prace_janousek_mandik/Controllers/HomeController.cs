using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Models;
using System.Diagnostics;

namespace sem_prace_janousek_mandik.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string? role = this.HttpContext.Session.GetString("role");
            string? email = this.HttpContext.Session.GetString("email");
            if (role != null)
            {
                ViewBag.Role = role;
                ViewBag.Email = email;
            }
            else
            {
                ViewBag.Role = "nikdo neprihlasen";
            }
                
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
    }
}