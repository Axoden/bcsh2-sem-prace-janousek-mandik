using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Home;
using sem_prace_janousek_mandik.Models.Supplier;

namespace sem_prace_janousek_mandik.Controllers.Supplier
{
    public class SupplierController : BaseController
	{
		/// <summary>
		/// Výpis všech dodavatelů
		/// </summary>
		/// <returns></returns>
		public IActionResult ListSuppliers()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Skladnik") || Role.Equals("Logistik"))
			{
				List<Dodavatele_Adresy> dodavatele = SupplierSQL.GetAllSuppliers();
				return View(dodavatele);
			}
			// Přesměrování, pokud uživatel nemá povolen přístup
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech dodavatelů
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult SearchSuppliers(string search)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				ViewBag.Search = search;
				List<Dodavatele_Adresy> dodavatele = SupplierSQL.GetAllSuppliers();
				if (search != null)
				{
					dodavatele = dodavatele.Where(lmb => lmb.Dodavatele.Nazev.ToLower().Contains(search.ToLower()) || lmb.Dodavatele.Jmeno.ToLower().Contains(search.ToLower()) || lmb.Dodavatele.Prijmeni.ToLower().Contains(search.ToLower()) || lmb.Dodavatele.Telefon.ToLower().Contains(search.ToLower()) || lmb.Dodavatele.Email.ToLower().Contains(search.ToLower()) || lmb.Adresy.Ulice.ToLower().Contains(search.ToLower()) || lmb.Adresy.Mesto.ToLower().Contains(search.ToLower()) || lmb.Adresy.Okres.ToLower().Contains(search.ToLower()) || lmb.Adresy.Zeme.ToLower().Contains(search.ToLower()) || lmb.Adresy.Psc.ToLower().Contains(search.ToLower())).ToList();
				}
				return View(nameof(ListSuppliers), dodavatele);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Načtení formuláře na přidání nového dodavatele
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult AddSupplier()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik"))
			{
				return View();
			}
			return RedirectToAction(nameof(ListSuppliers), nameof(Supplier));
		}

		/// <summary>
		// Příjem dat z formuláře na přidání dodavatele
		/// </summary>
		/// <param name="novyDodavatel">Model s daty nového dodavatele</param>
		/// <returns></returns>
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

					if (SupplierSQL.AddSupplier(novyDodavatel))
					{
						return RedirectToAction(nameof(ListSuppliers), nameof(Supplier));
					}
				}
				return View(novyDodavatel);
			}
			return RedirectToAction(nameof(ListSuppliers), nameof(Supplier));
		}

		/// <summary>
		/// Načtení formuláře na úpravu vybraného dodavatele
		/// </summary>
		/// <param name="index">ID upravovaného dodavatele</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult EditSupplierGet(int index)
		{
			// Kontrola oprávnění na načtení parametrů dodavatele
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik"))
			{
				Dodavatele_Adresy dodavateleAdresy = SupplierSQL.GetSupplierWithAddress(index);
				return View("EditSupplier", dodavateleAdresy);
			}
			return RedirectToAction(nameof(ListSuppliers), nameof(Supplier));
		}

		/// <summary>
		/// Příjem upravených dat vybraného dodavatele
		/// </summary>
		/// <param name="editSupplier">Model s upravenými daty dodavatele</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult EditSupplierPost(Dodavatele_Adresy editSupplier)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik"))
			{
				if (ModelState.IsValid)
				{
                    string emailSupplier = SupplierSQL.GetEmailByIdSupplier(editSupplier.Dodavatele.IdDodavatele);
                    // Kontrola zda již neexistuje dodavatel s tímto emailem
                    if (!emailSupplier.Equals(editSupplier.Dodavatele.Email))
                    {
                        if (SupplierSQL.CheckExistsSupplier(editSupplier.Dodavatele.Email) == true)
                        {
                            ViewBag.ErrorInfo = "Tento email je již zaregistrován!";
                            return View("EditSupplier", editSupplier);
                        }
                    }

					if (!SupplierSQL.EditSupplier(editSupplier)){
                        return View("EditSupplier", editSupplier);
                    }
				}
				else
				{
					return View("EditSupplier", editSupplier);
				}
			}
			return RedirectToAction(nameof(ListSuppliers), nameof(Supplier));
		}

		/// <summary>
		/// Metoda pro odstranění vybraného zaměstnance
		/// </summary>
		/// <param name="idDodavatele">ID dodavatele</param>
		/// <param name="idAdresy">ID adresy</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult DeleteSupplier(int idDodavatele, int idAdresy)
		{
			if (Role.Equals("Admin"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_DODAVATELE", idDodavatele, idAdresy);
			}
			return RedirectToAction(nameof(ListSuppliers), nameof(Supplier));
		}
	}
}