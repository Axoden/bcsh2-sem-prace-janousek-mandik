using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Models;

namespace sem_prace_janousek_mandik.Controllers.Management
{
	public class ManagementController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ListEmployees()
        {
            string? aktRole = this.HttpContext.Session.GetString("role");

            if (aktRole != null)
            {
                if (aktRole.Equals("Manazer") || aktRole.Equals("Admin") || aktRole.Equals("Skladnik") || aktRole.Equals("Logistik"))
                {
                    List<Zamestnanci> zamestnanci = ManagementSQL.GetAllEmployees();
                    ViewBag.ListOfEmployees = zamestnanci;
                    ViewBag.Role = this.HttpContext.Session.GetString("role");
                    ViewBag.Email = this.HttpContext.Session.GetString("email");

                    return View();
                }
            }

            return RedirectToAction("Index", "Home");
        }
    }
}