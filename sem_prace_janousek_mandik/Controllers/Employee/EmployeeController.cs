using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Home;
using sem_prace_janousek_mandik.Controllers.Management;
using sem_prace_janousek_mandik.Models;
using sem_prace_janousek_mandik.Models.Employee;

namespace sem_prace_janousek_mandik.Controllers.Employee
{
    public class EmployeeController : BaseController
	{
		/// <summary>
		/// Výpis všech zaměstnanců
		/// </summary>
		/// <returns></returns>
		public IActionResult ListEmployees()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Skladnik") || Role.Equals("Logistik"))
			{
				List<Zamestnanci_Adresy_Pozice> zamestnanci = EmployeeSQL.GetAllEmployeesWithAddressPosition();
				return View(zamestnanci);
			}
            return RedirectToAction(nameof(HomeController.Index), nameof(Home));
        }

		/// <summary>
		/// Metoda pro vyhledávání ve výpisu zaměstnanců
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
        [HttpPost]
        public IActionResult SearchEmployees(string search)
        {
            if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
            {
                ViewBag.Search = search;
                List<Zamestnanci_Adresy_Pozice> zamestnanci = EmployeeSQL.GetAllEmployeesWithAddressPosition();
                if (search != null)
                {
                    zamestnanci = zamestnanci.Where(lmb => lmb.Zamestnanci.Jmeno.ToLower().Contains(search.ToLower()) || lmb.Zamestnanci.Prijmeni.ToLower().Contains(search.ToLower()) || lmb.Zamestnanci.DatumNarozeni.ToString().ToLower().Contains(search.ToLower()) || lmb.Zamestnanci.Telefon.ToLower().Contains(search.ToLower()) || lmb.Zamestnanci.Email.ToLower().Contains(search.ToLower()) || lmb.Adresy.Ulice.ToLower().Contains(search.ToLower()) || lmb.Adresy.Mesto.ToLower().Contains(search.ToLower()) || lmb.Adresy.Okres.ToLower().Contains(search.ToLower()) || lmb.Adresy.Zeme.ToLower().Contains(search.ToLower()) || lmb.Adresy.Psc.ToLower().Contains(search.ToLower())).ToList();
                }
                return View(nameof(ListEmployees), zamestnanci);
            }
            return RedirectToAction(nameof(HomeController.Index), nameof(Home));
        }

        /// <summary>
        /// Načtení formuláře na přidání nového zaměstnance
        /// </summary>
        /// <returns></returns>
        [HttpGet]
		public IActionResult AddEmployee()
		{
			if (Role.Equals("Admin"))
			{
				Zamestnanci_Adresy_PoziceList employees = new();
				employees.Pozice = ManagementSQL.GetAllPositions();
				return View(employees);
			}
			return RedirectToAction(nameof(ListEmployees), nameof(Employee));
		}

		/// <summary>
		/// Příjem dat z formuláře na přidání zaměstnance
		/// </summary>
		/// <param name="newEmployee">Model s daty nového zaměstnance</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult AddEmployee(Zamestnanci_Adresy_PoziceList newEmployee)
		{
			if (Role.Equals("Admin"))
			{
                if (ModelState.IsValid == true)
				{
					// Kontrola zda již není zaregistrován zaměstnanec s tímto emailem
					if (EmployeeSQL.CheckExistsEmployee(newEmployee.Zamestnanci.Email) == true)
					{
						ViewBag.ErrorInfo = "Tento email je již zaregistrován!";
						return ReturnBad();

                    }

                    newEmployee.Zamestnanci.Heslo = SharedSQL.HashPassword(newEmployee.Zamestnanci.Heslo);
					if (EmployeeSQL.RegisterEmployee(newEmployee))
					{
						return RedirectToAction(nameof(ListEmployees), nameof(Employee));
					}
				}
				return ReturnBad();

            }

            IActionResult ReturnBad()
			{
                newEmployee.Pozice = ManagementSQL.GetAllPositions();
                return View(newEmployee);
            }

            return RedirectToAction(nameof(ListEmployees), nameof(Employee));
		}

		/// <summary>
		/// Načtení formuláře na úpravu vybraného zaměstnance
		/// </summary>
		/// <param name="index">ID upravovaného zaměstnance</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult EditEmployeeGet(int index)
		{
            Zamestnanci_Adresy_Pozice employee = EmployeeSQL.GetEmployeeWithAddressPosition(index);
            if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Skladnik") && employee.Zamestnanci.Email.Equals(Email) ||
				Role.Equals("Logistik") && employee.Zamestnanci.Email.Equals(Email))
			{
				Zamestnanci_Adresy_PoziceList emp = new();
				emp.Zamestnanci = employee.Zamestnanci;
				emp.Adresy = employee.Adresy;
				emp.Pozice = ManagementSQL.GetAllPositions();
                return View("EditEmployee", emp);
			}
			return RedirectToAction(nameof(ListEmployees), nameof(Employee));
		}

		/// <summary>
		/// Příjem upravených dat vybraného zaměstnance
		/// </summary>
		/// <param name="editedEmployee">Model s upravenými daty zaměstnance</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult EditEmployeePost(Zamestnanci_Adresy_PoziceList editedEmployee)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Skladnik") || Role.Equals("Logistik"))
			{
				editedEmployee.Zamestnanci.Heslo = SharedSQL.HashPassword(editedEmployee.Zamestnanci.Heslo);
				EmployeeSQL.EditEmployee(editedEmployee);
			}
			return RedirectToAction(nameof(ListEmployees), nameof(Employee));
		}

		/// <summary>
		/// Metoda pro odstranění vybraného zaměstnance
		/// </summary>
		/// <param name="idZamestnance">ID odstraňovaného zaměstnance</param>
		/// <param name="idAdresy">ID odstraňované adresy</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult DeleteEmployee(int idZamestnance, int idAdresy)
		{
			if (Role.Equals("Admin"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_ZAMESTNANCE", idZamestnance, idAdresy);
			}
			return RedirectToAction(nameof(ListEmployees), nameof(Employee));
		}

		/// <summary>
		/// Načtení přihlašovacího formuláře pro zaměstnance
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult LoginEmployee()
		{
			return View();
		}

		/// <summary>
		/// Příjem dat z přihlašovacího formuláře pro zaměstnance
		/// </summary>
		/// <param name="inputEmployee">Model s emailem a heslem zaměstnance</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult LoginEmployee(ZamestnanciLoginForm inputEmployee)
		{
			ViewBag.ErrorInfo = "Přihlašovací jméno nebo heslo je špatně!";
			if (ModelState.IsValid)
			{
				Zamestnanci? dbZamestnanec = EmployeeSQL.AuthEmployee(inputEmployee.Email);
				if (dbZamestnanec != null)
				{
					// Kontrola hashe hesel
					if (dbZamestnanec.Heslo.Equals(SharedSQL.HashPassword(inputEmployee.Heslo)))
					{
						Pozice? pozice = EmployeeSQL.GetPosition(dbZamestnanec.IdPozice);
						if (pozice != null)
						{
							// Nastavení session
							HttpContext.Session.SetString("role", pozice.Nazev);
							HttpContext.Session.SetString("email", dbZamestnanec.Email);
						}
                        return RedirectToAction(nameof(HomeController.Index), nameof(Home));
                    }
				}
			}
			return View(inputEmployee);
		}
	}
}