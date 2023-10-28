﻿using Microsoft.AspNetCore.Mvc;
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
					ViewBag.Role = role;
					ViewBag.Email = email;

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
		public IActionResult EditEmployee(Zamestnanci_Adresy_Pozice zamestnanciAdresyPozice, int idZamestnance, int idAdresy, int idPozice)
		{
			string? aktRole = this.HttpContext.Session.GetString("role");
			if (aktRole != null)
			{
				if (aktRole.Equals("Manazer") || aktRole.Equals("Admin") || aktRole.Equals("Skladnik") || aktRole.Equals("Logistik"))
				{
					zamestnanciAdresyPozice.Zamestnanci.IdZamestnance = idZamestnance;
					zamestnanciAdresyPozice.Zamestnanci.IdAdresy = idAdresy;
					zamestnanciAdresyPozice.Zamestnanci.IdPozice = idPozice;
					ManagementSQL.EditEmployee(zamestnanciAdresyPozice);
					return RedirectToAction("ListEmployees", "Management");
				}
			}
			return RedirectToAction("ListEmployees", "Management");
		}

		// Formální metoda pro odstranění vybraného zaměstnance
		[HttpGet]
		public IActionResult DeleteEmployee(int idZamestnance, int idAdresy)
		{
			string? aktRole = this.HttpContext.Session.GetString("role");

			if (aktRole != null)
			{
				if (aktRole.Equals("Admin"))
				{
					ManagementSQL.DeleteEmployee(idZamestnance, idAdresy);
					return RedirectToAction("ListEmployees", "Management");
				}
			}
			return RedirectToAction("ListEmployees", "Management");
		}

		public IActionResult ListPositions()
		{
			string? aktRole = this.HttpContext.Session.GetString("role");

			if (aktRole != null)
			{
				if (aktRole.Equals("Manazer") || aktRole.Equals("Admin"))
				{
					string? role = this.HttpContext.Session.GetString("role");
					string? email = this.HttpContext.Session.GetString("email");
					if (role != null)
					{
						ViewBag.Role = role;
						ViewBag.Email = email;
					}

					List<Pozice> pozice = ManagementSQL.GetAllPositions();
					ViewBag.ListOfPositions = pozice;
					ViewBag.Role = role;
					ViewBag.Email = email;

					return View();
				}
			}

			// Přesměrování, pokud uživatel nemá povolen přístup
			return RedirectToAction("Index", "Home");
		}
	}
}