using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Models;

namespace sem_prace_janousek_mandik.Controllers.Management
{
	public class ManagementController : Controller
    {
        // Výpis všech zaměstnanců
        public IActionResult ListEmployees()
        {
            string? aktRole = this.HttpContext.Session.GetString("role");

            if (aktRole != null)
            {
                if (aktRole.Equals("Manazer") || aktRole.Equals("Admin") || aktRole.Equals("Skladnik") || aktRole.Equals("Logistik"))
                {
                    string? role = this.HttpContext.Session.GetString("role");
                    string? email = this.HttpContext.Session.GetString("email");
                    if (role != null)
                    {
                        ViewBag.Role = role;
                        ViewBag.Email = email;
                    }

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

            // Přesměrování, pokud uživatel nemá povolen přístup
            return RedirectToAction("Index", "Home");
        }

        // Načtení formuláře na úpravu vybraného zaměstnance
        [HttpGet]
        public IActionResult EditEmployee(int index)
        {
			string? aktRole = this.HttpContext.Session.GetString("role");
            Zamestnanci_Adresy_Pozice zamestnanciAdresyPozice = ManagementSQL.GetEmployeeWithAddressPosition(index);
            string? role = this.HttpContext.Session.GetString("role");
            string? email = this.HttpContext.Session.GetString("email");
            if (role != null)
            {
                ViewBag.Role = role;
                ViewBag.Email = email;
            }

            if (aktRole != null)
            {
                // Kontrola oprávnění na načtení parametrů zaměstnance
                if (aktRole.Equals("Manazer") || aktRole.Equals("Admin") || aktRole.Equals("Skladnik") && zamestnanciAdresyPozice.Zamestnanci.Email.Equals(email) ||
                    aktRole.Equals("Logistik") && zamestnanciAdresyPozice.Zamestnanci.Email.Equals(email))
                {
                    ViewBag.ListOfPositions = ManagementSQL.GetAllPositions();
                    return View(zamestnanciAdresyPozice);
				}
            }
			return RedirectToAction("ListEmployees", "Management");
		}

        // Příjem upravených dat vybraného zaměstnance
        [HttpPost]
        public IActionResult EditEmployee(Zamestnanci_Adresy_Pozice zamestnanciAdresyPozice)
        {
			string? aktRole = this.HttpContext.Session.GetString("role");
            /*if (aktRole != null)
            {
                if (aktRole.Equals("Manazer") || aktRole.Equals("Admin") || aktRole.Equals("Skladnik") || aktRole.Equals("Logistik"))
                {
                    ManagementSQL.EditEmployee(zamestnanciAdresyPozice);
                    return RedirectToAction("ListEmployees", "Management");
                }
            }*/
			return RedirectToAction("ListEmployees", "Management");
		}

        // Formální metoda pro odstranění vybraného zaměstnance
        public IActionResult DeleteEmployee(int index)
		{
			string? aktRole = this.HttpContext.Session.GetString("role");

            if (aktRole != null)
            {
                if (aktRole.Equals("Manazer") || aktRole.Equals("Admin") || aktRole.Equals("Skladnik") || aktRole.Equals("Logistik"))
                {
                    return RedirectToAction("ListEmployees", "Management");
                }
            }
			return RedirectToAction("ListEmployees", "Management");
		}
	}
}