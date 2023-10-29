using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Models;
using sem_prace_janousek_mandik.Models.Dodavatele;
using System.Security.Cryptography;
using System.Text;

namespace sem_prace_janousek_mandik.Controllers.Management
{
	public class DodavateleController : Controller
	{
		// Výpis všech dodavatelů
		public IActionResult ListSuppliers()
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

					List<Dodavatele> dodavatele = DodavateleSQL.GetAllSuppliers();
					List<Adresy> adresy = AdresySQL.GetAllAddresses();
					ViewBag.ListOfSuppliers = dodavatele;
					ViewBag.ListOfAddresses = adresy;
					ViewBag.Role = role;
					ViewBag.Email = email;

					return View();
				}
			}

			// Přesměrování, pokud uživatel nemá povolen přístup
			return RedirectToAction("Index", "Home");
		}

		// Načtení formuláře na přidání nového dodavatele
		[HttpGet]
		public IActionResult AddSupplier()
		{
			string? aktRole = this.HttpContext.Session.GetString("role");

			if (aktRole != null)
			{
				if (aktRole.Equals("Manazer") || aktRole.Equals("Admin") || aktRole.Equals("Logistik"))
				{
					string? role = this.HttpContext.Session.GetString("role");
					string? email = this.HttpContext.Session.GetString("email");
					if (role != null)
					{
						ViewBag.Role = role;
						ViewBag.Email = email;
					}                    

                    return View();
				}
			}
			return RedirectToAction("ListSuppliers", "Dodavatele");
		}

		// Příjem dat z formuláře na přidání dodavatele
		[HttpPost]
		public IActionResult AddSupplier(Dodavatele_Adresy novyDodavatel)
		{
			string? aktRole = this.HttpContext.Session.GetString("role");

			if (aktRole != null)
			{
                if (aktRole.Equals("Manazer") || aktRole.Equals("Admin") || aktRole.Equals("Logistik"))
                {
					string? role = this.HttpContext.Session.GetString("role");
					string? email = this.HttpContext.Session.GetString("email");
					if (role != null)
					{
						ViewBag.Role = role;
						ViewBag.Email = email;
					}

					if (ModelState.IsValid == true)
					{
						// Kontrola zda již není zaregistrován dodavatel s tímto emailem
						if (DodavateleSQL.CheckExistsSupplier(novyDodavatel.Dodavatele.Email) == true)
						{
							ViewBag.ErrorInfo = "Tento email je již zaregistrován!";
							return View(novyDodavatel);
						}

						bool uspesnaRegistrace = DodavateleSQL.AddSupplier(novyDodavatel);

						if (uspesnaRegistrace == true)
						{
							// Úspěšná registrace, přesměrování na výpis dodavatelů
							return RedirectToAction("ListSuppliers", "Dodavatele");
						}
					}
					return View(novyDodavatel);
				}
			}
			return RedirectToAction("ListSuppliers", "Dodavatele");
		}

		// Načtení formuláře na úpravu vybraného dodavatele
		[HttpGet]
		public IActionResult EditSupplier(int index)
		{
			string? aktRole = this.HttpContext.Session.GetString("role");
			Dodavatele_Adresy dodavateleAdresy = DodavateleSQL.GetSupplierWithAddress(index);
			string? role = this.HttpContext.Session.GetString("role");
			string? email = this.HttpContext.Session.GetString("email");
			if (role != null)
			{
				ViewBag.Role = role;
				ViewBag.Email = email;
			}

			if (aktRole != null)
			{
				// Kontrola oprávnění na načtení parametrů dodavatele
				if (aktRole.Equals("Manazer") || aktRole.Equals("Admin") || aktRole.Equals("Logistik"))
				{
					return View(dodavateleAdresy);
				}
			}
			return RedirectToAction("ListSuppliers", "Dodavatele");
		}

		// Příjem upravených dat vybraného dodavatele
		[HttpPost]
		public IActionResult EditSupplier(Dodavatele_Adresy dodavateleAdresy, int idDodavatele, int idAdresy)
		{
			string? aktRole = this.HttpContext.Session.GetString("role");
			if (aktRole != null)
			{
				if (aktRole.Equals("Manazer") || aktRole.Equals("Admin") || aktRole.Equals("Logistik"))
				{
					dodavateleAdresy.Dodavatele.IdDodavatele = idDodavatele;
					dodavateleAdresy.Dodavatele.IdAdresy = idAdresy;
					DodavateleSQL.EditSupplier(dodavateleAdresy);
					return RedirectToAction("ListSuppliers", "Dodavatele");
				}
			}
			return RedirectToAction("ListSuppliers", "Dodavatele");
		}

		// Formální metoda pro odstranění vybraného zaměstnance
		[HttpGet]
		public IActionResult DeleteSupplier(int idDodavatele, int idAdresy)
		{
			string? aktRole = this.HttpContext.Session.GetString("role");

			if (aktRole != null)
			{
				if (aktRole.Equals("Admin"))
				{
					DodavateleSQL.DeleteSupplier(idDodavatele, idAdresy);
					return RedirectToAction("ListSuppliers", "Dodavatele");
				}
			}
			return RedirectToAction("ListSuppliers", "Dodavatele");
		}
	}
}