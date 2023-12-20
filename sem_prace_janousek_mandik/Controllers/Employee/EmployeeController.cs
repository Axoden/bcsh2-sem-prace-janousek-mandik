using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Home;
using sem_prace_janousek_mandik.Controllers.Management;
using sem_prace_janousek_mandik.Models.Employee;
using sem_prace_janousek_mandik.Models.Management;

namespace sem_prace_janousek_mandik.Controllers.Employee
{
    public class EmployeeController : BaseController
	{
		/// <summary>
		/// Výpis všech zaměstnanců
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> ListEmployees()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Skladnik") || Role.Equals("Logistik"))
			{
				List<Zamestnanci_Adresy_Pozice> zamestnanci = await EmployeeSQL.GetAllEmployeesWithAddressPosition();
				return View(zamestnanci);
			}
            return RedirectToAction(nameof(HomeController.Index), nameof(Home));
        }

		/// <summary>
		/// Metoda pro vyhledávání ve výpisu zaměstnanců
		/// </summary>
		/// <param name="search"></param>
		/// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchEmployees(string search)
        {
            if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
            {
                ViewBag.Search = search;
                List<Zamestnanci_Adresy_Pozice> zamestnanci = await EmployeeSQL.GetAllEmployeesWithAddressPosition();
                if (search != null)
                {
					search = search.ToLower();
					zamestnanci = zamestnanci.Where(lmb => (lmb.Zamestnanci?.Jmeno ?? string.Empty).ToLower().Contains(search) || (lmb.Zamestnanci?.Prijmeni ?? string.Empty).ToLower().Contains(search) || (lmb.Zamestnanci?.DatumNarozeni != null && lmb.Zamestnanci.DatumNarozeni.ToString().ToLower().Contains(search)) || (lmb.Zamestnanci?.Telefon ?? string.Empty).ToLower().Contains(search) || (lmb.Zamestnanci?.Email ?? string.Empty).ToLower().Contains(search) || (lmb.Adresy?.Ulice ?? string.Empty).ToLower().Contains(search) || (lmb.Adresy?.Mesto ?? string.Empty).ToLower().Contains(search) || (lmb.Adresy?.Okres ?? string.Empty).ToLower().Contains(search) || (lmb.Adresy?.Zeme ?? string.Empty).ToLower().Contains(search) || (lmb.Adresy?.Psc ?? string.Empty).ToLower().Contains(search)).ToList();
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
		public async Task<IActionResult> AddEmployee()
		{
			if (Role.Equals("Admin"))
			{
				Zamestnanci_Adresy_PoziceList employees = new();
				employees.Pozice = await ManagementSQL.GetAllPositions();
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddEmployee(Zamestnanci_Adresy_PoziceList newEmployee)
		{
			if (Role.Equals("Admin"))
			{
                if (ModelState.IsValid == true)
				{
                    newEmployee.Zamestnanci.Heslo = SharedSQL.HashPassword(newEmployee.Zamestnanci.Heslo);

					string? err = await EmployeeSQL.RegisterEmployee(newEmployee);

					if (err == null)
					{
						return RedirectToAction(nameof(ListEmployees), nameof(Employee));
					}
					else
					{
						ViewBag.ErrorInfo = err;
						return await ReturnBad();
					}
				}
				return await ReturnBad();
            }

			async Task<IActionResult> ReturnBad()
			{
                newEmployee.Pozice = await ManagementSQL.GetAllPositions();
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
		public async Task<IActionResult> EditEmployeeGet(int index)
		{
            Zamestnanci_Adresy_Pozice employee = await EmployeeSQL.GetEmployeeWithAddressPosition(index);
            if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Skladnik") && employee.Zamestnanci.Email.Equals(Email) ||
				Role.Equals("Logistik") && employee.Zamestnanci.Email.Equals(Email))
			{
				Zamestnanci_Adresy_PoziceList emp = new();
				emp.Zamestnanci = employee.Zamestnanci;
				emp.Adresy = employee.Adresy;
				emp.Pozice = await ManagementSQL.GetAllPositions();
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditEmployeePost(Zamestnanci_Adresy_PoziceList editedEmployee)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Skladnik") || Role.Equals("Logistik"))
			{
				if (editedEmployee.Zamestnanci.Heslo != null)
				{
					editedEmployee.Zamestnanci.Heslo = SharedSQL.HashPassword(editedEmployee.Zamestnanci.Heslo);
				}

				string? err = await EmployeeSQL.EditEmployee(editedEmployee);

				if (err == null)
				{
					return RedirectToAction(nameof(ListEmployees), nameof(Employee));
				}
				else
				{
					ViewBag.ErrorInfo = err;
					return View("EditEmployee", editedEmployee);
				}
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteEmployee(int idZamestnance, int idAdresy)
		{
			if (Role.Equals("Admin"))
			{
				await SharedSQL.CallDeleteProcedure("pkg_delete.P_SMAZAT_ZAMESTNANCE", idZamestnance, idAdresy);
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LoginEmployee(ZamestnanciLoginForm inputEmployee)
		{
			ViewBag.ErrorInfo = "Přihlašovací jméno nebo heslo je špatně!";
			if (ModelState.IsValid)
			{
				Zamestnanci? dbZamestnanec = await EmployeeSQL.AuthEmployee(inputEmployee.Email);
				if (dbZamestnanec.Heslo != null)
				{
					// Kontrola hashe hesel
					if (dbZamestnanec.Heslo.Equals(SharedSQL.HashPassword(inputEmployee.Heslo)))
					{
						Pozice? pozice = await EmployeeSQL.GetPosition(dbZamestnanec.IdPozice);
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