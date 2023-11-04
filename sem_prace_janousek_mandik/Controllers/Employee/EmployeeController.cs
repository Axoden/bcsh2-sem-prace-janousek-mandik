using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Management;
using sem_prace_janousek_mandik.Models;
using sem_prace_janousek_mandik.Models.Employee;

namespace sem_prace_janousek_mandik.Controllers.Employee
{
	public class EmployeeController : BaseController
	{
		// Výpis všech zaměstnanců
		public IActionResult ListEmployees()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Skladnik") || Role.Equals("Logistik"))
			{
				List<Zamestnanci> zamestnanci = EmployeeSQL.GetAllEmployees();
				List<Adresy> adresy = SharedSQL.GetAllAddresses();
				List<Pozice> pozice = ManagementSQL.GetAllPositions();
				ViewBag.ListOfEmployees = zamestnanci;
				ViewBag.ListOfAddresses = adresy;
				ViewBag.ListOfPositions = pozice;

				return View();
			}

			// Přesměrování, pokud uživatel nemá povolen přístup
			return RedirectToAction("Index", "Home");
		}

		// Načtení formuláře na přidání nového zaměstnance
		[HttpGet]
		public IActionResult AddEmployee()
		{
			// Dostupné pouze pro administrátora
			if (Role.Equals("Admin"))
			{
				ViewBag.ListOfPositions = ManagementSQL.GetAllPositions();
				return View();
			}

			return RedirectToAction(nameof(ListEmployees), nameof(Employee));
		}

		// Příjem dat z formuláře na přidání zaměstnance
		[HttpPost]
		public IActionResult AddEmployee(Zamestnanci_Adresy_Pozice novyZamestnanec)
		{
			// Dostupné pouze pro administrátora
			if (Role.Equals("Admin"))
			{
				if (ModelState.IsValid == true)
				{
					// Kontrola zda již není zaregistrován zaměstnanec s tímto emailem
					if (EmployeeSQL.CheckExistsEmployee(novyZamestnanec.Zamestnanci.Email) == true)
					{
						ViewBag.ErrorInfo = "Tento email je již zaregistrován!";
						return View(novyZamestnanec);
					}

					Zamestnanci_Adresy_Pozice inputZamestnanecEdited = novyZamestnanec;
					inputZamestnanecEdited.Zamestnanci.Heslo = SharedSQL.HashPassword(novyZamestnanec.Zamestnanci.Heslo);
					int idPozice = EmployeeSQL.GetPositionIdByName(novyZamestnanec.Pozice.Nazev);
					inputZamestnanecEdited.Zamestnanci.IdPozice = idPozice;
					bool uspesnaRegistrace = EmployeeSQL.RegisterEmployee(inputZamestnanecEdited);

					if (uspesnaRegistrace == true)
					{
						// Úspěšná registrace, přesměrování na výpis zaměstnanců
						return RedirectToAction(nameof(ListEmployees), nameof(Employee));
					}
				}
				return View(novyZamestnanec);
			}

			return RedirectToAction(nameof(ListEmployees), nameof(Employee));
		}

		// Načtení formuláře na úpravu vybraného zaměstnance
		[HttpGet]
		public IActionResult EditEmployee(int index)
		{
			Zamestnanci_Adresy_Pozice zamestnanciAdresyPozice = EmployeeSQL.GetEmployeeWithAddressPosition(index);

			// Kontrola oprávnění na načtení parametrů zaměstnance
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Skladnik") && zamestnanciAdresyPozice.Zamestnanci.Email.Equals(Email) ||
				Role.Equals("Logistik") && zamestnanciAdresyPozice.Zamestnanci.Email.Equals(Email))
			{
				ViewBag.ListOfPositions = ManagementSQL.GetAllPositions();
				return View(zamestnanciAdresyPozice);
			}

			return RedirectToAction(nameof(ListEmployees), nameof(Employee));
		}

		// Příjem upravených dat vybraného zaměstnance
		[HttpPost]
		public IActionResult EditEmployee(Zamestnanci_Adresy_Pozice zamestnanciAdresyPozice, int idZamestnance, int idAdresy, int idPozice)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Skladnik") || Role.Equals("Logistik"))
			{
				zamestnanciAdresyPozice.Zamestnanci.IdZamestnance = idZamestnance;
				zamestnanciAdresyPozice.Zamestnanci.IdAdresy = idAdresy;
				if (zamestnanciAdresyPozice.Pozice != null)
				{
					idPozice = EmployeeSQL.GetPositionIdByName(zamestnanciAdresyPozice.Pozice.Nazev);
				}

				zamestnanciAdresyPozice.Zamestnanci.IdPozice = idPozice;
				EmployeeSQL.EditEmployee(zamestnanciAdresyPozice);
			}

			return RedirectToAction(nameof(ListEmployees), nameof(Employee));
		}

		// Formální metoda pro odstranění vybraného zaměstnance
		[HttpGet]
		public IActionResult DeleteEmployee(int idZamestnance, int idAdresy)
		{
			if (Role.Equals("Admin"))
			{
				EmployeeSQL.DeleteEmployee(idZamestnance, idAdresy);
			}

			return RedirectToAction(nameof(ListEmployees), nameof(Employee));
		}

		// Načtení přihlašovacího formuláře pro zaměstnance
		[HttpGet]
		public IActionResult LoginEmployee()
		{
			return View();
		}

		// Příjem dat z přihlašovacího formuláře pro zaměstnance
		[HttpPost]
		public IActionResult LoginEmployee(ZamestnanciLoginForm inputZamestnanec)
		{
			// Informativní zpráva při chybném vyplnění
			ViewBag.ErrorInfo = "Přihlašovací jméno nebo heslo je špatně!";

			if (ModelState.IsValid)
			{
				Zamestnanci? dbZamestnanec = EmployeeSQL.AuthEmployee(inputZamestnanec.Email);

				if (dbZamestnanec != null)
				{
					// Kontrola hashe hesel
					if (dbZamestnanec.Heslo.Equals(SharedSQL.HashPassword(inputZamestnanec.Heslo)))
					{
						Pozice? pozice = EmployeeSQL.GetPosition(dbZamestnanec.IdPozice);
						if (pozice != null)
						{
							// Nastavení session
							HttpContext.Session.SetString("role", pozice.Nazev);
							HttpContext.Session.SetString("email", dbZamestnanec.Email);
						}

						// Přesměrování při úspěšném přihlášení
						return RedirectToAction("Index", "Home");
					}
				}
			}
			return View(inputZamestnanec);
		}
	}
}
