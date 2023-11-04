using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Models;
using sem_prace_janousek_mandik.Models.Supplier;
using System.Text;

namespace sem_prace_janousek_mandik.Controllers.Supplier
{
	public class SupplierController : BaseController
	{
		// Výpis všech dodavatelů
		public IActionResult ListSuppliers()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Skladnik") || Role.Equals("Logistik"))
			{
				List<Dodavatele> dodavatele = SupplierSQL.GetAllSuppliers();
				List<Adresy> adresy = SharedSQL.GetAllAddresses();
				ViewBag.ListOfSuppliers = dodavatele;
				ViewBag.ListOfAddresses = adresy;

				return View();
			}

			// Přesměrování, pokud uživatel nemá povolen přístup
			return RedirectToAction("Index", "Home");
		}

		// Načtení formuláře na přidání nového dodavatele
		[HttpGet]
		public IActionResult AddSupplier()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik"))
			{
				return View();
			}

			return RedirectToAction(nameof(ListSuppliers), nameof(Supplier));
		}

		// Příjem dat z formuláře na přidání dodavatele
		[HttpPost]
		public IActionResult AddSupplier(Dodavatele_Adresy novyDodavatel)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik"))
			{
				if (ModelState.IsValid == true)
				{
					// Kontrola zda již není zaregistrován dodavatel s tímto emailem
					if (SupplierSQL.CheckExistsSupplier(novyDodavatel.Dodavatele.Email) == true)
					{
						ViewBag.ErrorInfo = "Tento email je již zaregistrován!";
						return View(novyDodavatel);
					}

					bool uspesnaRegistrace = SupplierSQL.AddSupplier(novyDodavatel);

					if (uspesnaRegistrace == true)
					{
						// Úspěšná registrace, přesměrování na výpis dodavatelů
						return RedirectToAction(nameof(ListSuppliers), nameof(Supplier));
					}
				}
				return View(novyDodavatel);
			}

			return RedirectToAction(nameof(ListSuppliers), nameof(Supplier));
		}

		// Načtení formuláře na úpravu vybraného dodavatele
		[HttpGet]
		public IActionResult EditSupplier(int index)
		{
			// Kontrola oprávnění na načtení parametrů dodavatele
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik"))
			{
				Dodavatele_Adresy dodavateleAdresy = SupplierSQL.GetSupplierWithAddress(index);
				return View(dodavateleAdresy);
			}

			return RedirectToAction(nameof(ListSuppliers), nameof(Supplier));
		}

		// Příjem upravených dat vybraného dodavatele
		[HttpPost]
		public IActionResult EditSupplier(Dodavatele_Adresy dodavateleAdresy, int idDodavatele, int idAdresy)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik"))
			{
				dodavateleAdresy.Dodavatele.IdDodavatele = idDodavatele;
				dodavateleAdresy.Dodavatele.IdAdresy = idAdresy;
				SupplierSQL.EditSupplier(dodavateleAdresy);
			}

			return RedirectToAction(nameof(ListSuppliers), nameof(Supplier));
		}

		// Formální metoda pro odstranění vybraného zaměstnance
		[HttpGet]
		public IActionResult DeleteSupplier(int idDodavatele, int idAdresy)
		{
			if (Role.Equals("Admin"))
			{
				SupplierSQL.DeleteSupplier(idDodavatele, idAdresy);
			}

			return RedirectToAction(nameof(ListSuppliers), nameof(Supplier));
		}
	}
}