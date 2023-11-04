using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Models.Goods;
using sem_prace_janousek_mandik.Controllers.Supplier;

namespace sem_prace_janousek_mandik.Controllers.Goods
{
	public class GoodsController : BaseController
	{
		// Výpis všech kategorií
		public IActionResult ListCategories()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
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


		// Výpis všeho zboží
		public IActionResult ListGoods()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Skladnik") || Role.Equals("Logistik"))
			{
				List<Zbozi_Umisteni_Kategorie_Dodavatele> zboziUmisteniKategorieDodavatel = GoodsSQL.GetAllGoodsWithLocationCategorySupplier();
				ViewBag.ListOfGoodsWithLocationCategorySupplier = zboziUmisteniKategorieDodavatel;
				return View();
			}

			// Přesměrování, pokud uživatel nemá povolen přístup
			return RedirectToAction("Index", "Home");
		}

		// Načtení formuláře na přidání nového zboží
		[HttpGet]
		public IActionResult AddGoods()
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				ViewBag.ListOfLocations = GoodsSQL.GetAllLocations();
				ViewBag.ListOfCategories = GoodsSQL.GetAllCategories();
				ViewBag.ListOfSuppliers = SupplierSQL.GetAllSuppliers();
				return View();
			}

			return RedirectToAction(nameof(ListGoods), nameof(Goods));
		}

		// Příjem dat z formuláře na přidání nového zboží
		[HttpPost]
		public IActionResult AddGoods(Zbozi_Umisteni_Kategorie_Dodavatele newGoods)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				if (ModelState.IsValid == true)
				{
					// Kontrola zda již neexistuje zboží s tímto čárovým kódem
					if (GoodsSQL.CheckExistsBarcode(newGoods.Zbozi.CarovyKod) == true)
					{
						ViewBag.ErrorInfo = "Tento čárový kód je již používán!";
						return View(newGoods);
					}

					bool uspesnaRegistrace = GoodsSQL.RegisterGoods(newGoods);
					if (uspesnaRegistrace == true)
					{
						// Úspěšná registrace, přesměrování na výpis zaměstnanců
						return RedirectToAction(nameof(ListGoods), nameof(Goods));
					}
				}
				return View(newGoods);
			}

			return RedirectToAction(nameof(ListGoods), nameof(Goods));
		}

		// Načtení formuláře na úpravu vybraného zboží
		[HttpGet]
		public IActionResult EditGoods(int index)
		{
			Zbozi_Umisteni_Kategorie_Dodavatele zbozi = GoodsSQL.GetGoodsById(index);

			// Kontrola oprávnění na načtení parametrů zboží
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				ViewBag.ListOfLocations = GoodsSQL.GetAllLocations();
				ViewBag.ListOfCategories = GoodsSQL.GetAllCategories();
				ViewBag.ListOfSuppliers = SupplierSQL.GetAllSuppliers();
				return View(zbozi);
			}

			return RedirectToAction(nameof(ListGoods), nameof(Goods));
		}

		// Příjem upravených dat vybraného zboží
		[HttpPost]
		public IActionResult EditGoods(Zbozi_Umisteni_Kategorie_Dodavatele zboziUmisteniKategorieDodavatel, int idZbozi)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				zboziUmisteniKategorieDodavatel.Zbozi.IdZbozi = idZbozi;
				GoodsSQL.EditGoods(zboziUmisteniKategorieDodavatel);
			}

			return RedirectToAction(nameof(ListGoods), nameof(Goods));
		}

		// Formální metoda pro odstranění vybraného zboží
		[HttpGet]
		public IActionResult DeleteGoods(int idZbozi)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_ZBOZI", idZbozi);
			}

			return RedirectToAction(nameof(ListGoods), nameof(Goods));
		}


		// Výpis všech umístění
		public IActionResult ListLocations()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				List<Umisteni> umisteni = GoodsSQL.GetAllLocations();
				ViewBag.ListOfLocations = umisteni;

				return View();
			}
			return RedirectToAction("Index", "Home");
		}

		// Načtení formuláře na přidání nového umístění
		[HttpGet]
		public IActionResult AddLocation()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				return View();
			}

			return RedirectToAction(nameof(ListLocations), nameof(Goods));
		}

		// Příjem dat z formuláře na přidání umístění
		[HttpPost]
		public IActionResult AddLocation(Umisteni noveUmisteni)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				if (ModelState.IsValid == true)
				{
					bool uspesnePridani = GoodsSQL.AddLocation(noveUmisteni);
					if (uspesnePridani == true)
					{
						return RedirectToAction(nameof(ListLocations), nameof(Goods));
					}
				}
				return View(noveUmisteni);
			}

			return RedirectToAction(nameof(ListLocations), nameof(Goods));
		}

		// Načtení formuláře na úpravu vybraného umístění
		[HttpGet]
		public IActionResult EditLocation(int index)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				Umisteni umisteni = GoodsSQL.GetLocationById(index);
				return View(umisteni);
			}

			return RedirectToAction(nameof(ListLocations), nameof(Goods));
		}

		// Příjem upravených dat vybraného umístění
		[HttpPost]
		public IActionResult EditLocation(Umisteni umisteni, int index)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				umisteni.IdUmisteni = index;
				GoodsSQL.EditLocation(umisteni);
			}

			return RedirectToAction(nameof(ListLocations), nameof(Goods));
		}

		// Odstranění vybraného umístění
		[HttpGet]
		public IActionResult DeleteLocation(int index)
		{
			if (Role.Equals("Admin"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_UMISTENI", index);
			}

			return RedirectToAction(nameof(ListLocations), nameof(Goods));
		}
	}
}