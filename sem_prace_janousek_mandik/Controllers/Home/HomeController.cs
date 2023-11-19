using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Models;
using System.Diagnostics;

namespace sem_prace_janousek_mandik.Controllers.Home
{
    public class HomeController : BaseController
    {
        /// <summary>
        /// Úvodní stránka
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Odhlášení a odstranění session
        /// </summary>
        /// <returns></returns>
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        /// <summary>
        /// Stránka, která zobrazuje info v případě chyby
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}