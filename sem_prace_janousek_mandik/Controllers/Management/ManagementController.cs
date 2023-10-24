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
					List<Adresy> adresy = ManagementSQL.GetAllAddresses();
					List<Pozice> pozice = ManagementSQL.GetAllPositions();
					ViewBag.ListOfEmployees = zamestnanci;
					ViewBag.ListOfAddresses = adresy;
                    ViewBag.ListOfPositions = pozice;
					ViewBag.Role = this.HttpContext.Session.GetString("role");
                    ViewBag.Email = this.HttpContext.Session.GetString("email");

                    return View();
                }
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult EditEmployee(int index)
        {
			Zamestnanci_Adresy_Pozice zamestnanciAdresyPozice = ManagementSQL.GetEmployeeWithAddressPosition(index);
			return View(zamestnanciAdresyPozice);
		}

        [HttpPost]
        public IActionResult EditEmployee(Zamestnanci_Adresy_Pozice zamestnanciAdresyPozice)
        {
            ManagementSQL.EditEmployee(zamestnanciAdresyPozice);
            return RedirectToAction("ListEmployees", "Management");
        }

        public IActionResult DeleteEmployee(int index)
		{
			return View();
		}
	}
}