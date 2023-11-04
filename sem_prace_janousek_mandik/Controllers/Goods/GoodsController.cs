using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Models.Goods;

namespace sem_prace_janousek_mandik.Controllers.Goods
{
	public class GoodsController : BaseController
	{
		// Výpis všech kategorií
		public IActionResult ListCategories()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				string? role = this.HttpContext.Session.GetString("role");
				string? email = this.HttpContext.Session.GetString("email");
				if (role != null)
				{
					ViewBag.Role = role;
					ViewBag.Email = email;
				}

				List<Kategorie> kategorie = GoodsSQL.GetAllCategories();
				ViewBag.ListOfCategories = kategorie;

				return View();
			}
			return RedirectToAction("Index", "Home");
		}

		// Načtení formuláře na přidání nové kategorie
		[HttpGet]
		public IActionResult AddCategory()
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				return View();
			}

			return RedirectToAction(nameof(ListCategories), nameof(Goods));
		}

		// Příjem dat z formuláře na přidání kategorie
		[HttpPost]
		public IActionResult AddCategory(Kategorie novaKategorie)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				if (ModelState.IsValid == true)
				{
					bool uspesnaRegistrace = GoodsSQL.AddCategory(novaKategorie);

					if (uspesnaRegistrace == true)
					{
						// Úspěšná registrace, přesměrování na výpis kategorií
						return RedirectToAction(nameof(ListCategories), nameof(Goods));
					}
				}
				return View(novaKategorie);
			}

			return RedirectToAction(nameof(ListCategories), nameof(Goods));
		}

		// Načtení formuláře na úpravu vybrané kategorie
		[HttpGet]
		public IActionResult EditCategory(int index)
		{
			// Kontrola oprávnění na načtení parametrů kategorie
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				Kategorie kategorie = GoodsSQL.GetCategoryById(index);
				return View(kategorie);
			}

			return RedirectToAction(nameof(ListCategories), nameof(Goods));
		}

		// Příjem upravených dat vybrané kategorie
		[HttpPost]
		public IActionResult EditCategory(Kategorie kategorie, int index)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				kategorie.IdKategorie = index;
				GoodsSQL.EditCategory(kategorie);
			}

			return RedirectToAction(nameof(ListCategories), nameof(Goods));
		}

		// Formální metoda pro odstranění vybrané kategorie
		[HttpGet]
		public IActionResult DeleteCategory(int index)
		{
			if (Role.Equals("Admin"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_KATEGORII", index);
			}

			return RedirectToAction(nameof(ListCategories), nameof(Goods));
		}
	}
}
