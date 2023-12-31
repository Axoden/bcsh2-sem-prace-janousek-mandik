using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Home;
using sem_prace_janousek_mandik.Controllers.Management;
using sem_prace_janousek_mandik.Models.Supplier;

namespace sem_prace_janousek_mandik.Controllers.Supplier
{
	public class SupplierController : BaseController
	{
		/// <summary>
		/// Výpis všech dodavatelů
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> ListSuppliers()
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Skladnik || Role == Roles.Logistik)
			{
				List<Dodavatele_Adresy> dodavatele = await SupplierSQL.GetAllSuppliers();
				return View(dodavatele);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech dodavatelů
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> SearchSuppliers(string search)
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik || Role == Roles.Skladnik)
			{
				ViewBag.Search = search;
				List<Dodavatele_Adresy> dodavatele = await SupplierSQL.GetAllSuppliers();
				if (search != null)
				{
					search = search.ToLower();
					dodavatele = dodavatele.Where(lmb => (lmb.Dodavatele?.Nazev ?? string.Empty).ToLower().Contains(search) || (lmb.Dodavatele?.Jmeno ?? string.Empty).ToLower().Contains(search) || (lmb.Dodavatele?.Prijmeni ?? string.Empty).ToLower().Contains(search) || (lmb.Dodavatele?.Telefon ?? string.Empty).ToLower().Contains(search) || (lmb.Dodavatele?.Email ?? string.Empty).ToLower().Contains(search) || (lmb.Adresy?.Ulice ?? string.Empty).ToLower().Contains(search) || (lmb.Adresy?.Mesto ?? string.Empty).ToLower().Contains(search) || (lmb.Adresy?.Okres ?? string.Empty).ToLower().Contains(search) || (lmb.Adresy?.Zeme ?? string.Empty).ToLower().Contains(search) || (lmb.Adresy?.Psc ?? string.Empty).ToLower().Contains(search)).ToList();
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
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik)
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddSupplier(Dodavatele_Adresy novyDodavatel)
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik)
			{
				if (ModelState.IsValid)
				{
					string? err = await SupplierSQL.AddSupplier(novyDodavatel);

					if (err == null)
					{
						return RedirectToAction(nameof(ListSuppliers), nameof(Supplier));
					}
					else
					{
						ViewBag.ErrorInfo = err;
						return View(novyDodavatel);
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
		public async Task<IActionResult> EditSupplierGet(int index)
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik)
			{
				Dodavatele_Adresy dodavateleAdresy = await SupplierSQL.GetSupplierWithAddress(index);
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditSupplierPost(Dodavatele_Adresy editSupplier)
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik)
			{
				if (ModelState.IsValid)
				{
					string? err = await SupplierSQL.EditSupplier(editSupplier);

					if (err == null)
					{
						return RedirectToAction(nameof(ListSuppliers), nameof(Supplier));
					}
					else
					{
						ViewBag.ErrorInfo = err;
						return View("EditSupplier", editSupplier);
					}
				}
				return View("EditSupplier", editSupplier);
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteSupplier(int idDodavatele, int idAdresy)
		{
			if (Role == Roles.Admin)
			{
				await SharedSQL.CallDeleteProcedure("pkg_delete.P_SMAZAT_DODAVATELE", idDodavatele, idAdresy);
			}
			return RedirectToAction(nameof(ListSuppliers), nameof(Supplier));
		}
	}
}