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
				List<Kategorie_NadrazenaKategorie> kategorie = GoodsSQL.GetAllCategories();
				return View(kategorie);
			}
			return RedirectToAction("Index", "Home");
		}

		// Načtení formuláře na přidání nové kategorie
		[HttpGet]
		public IActionResult AddCategory()
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				ViewBag.ListOfUpperCategories = GoodsSQL.GetAllCategoriesIdNameAcronym();
				return View();
			}

			return RedirectToAction(nameof(ListCategories), nameof(Goods));
		}

		// Příjem dat z formuláře na přidání kategorie
		[HttpPost]
		public IActionResult AddCategory(Kategorie newCategory)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				ViewBag.ListOfUpperCategories = GoodsSQL.GetAllCategoriesIdNameAcronym();
				if (ModelState.IsValid == true)
				{
					newCategory.Zkratka = newCategory.Zkratka.ToUpper();
					if (GoodsSQL.AddCategory(newCategory))
					{
						// Úspěšné přidání, přesměrování na výpis kategorií
						return RedirectToAction(nameof(ListCategories), nameof(Goods));
					}
				}
				return View(newCategory);
			}

			return RedirectToAction(nameof(ListCategories), nameof(Goods));
		}

		// Načtení formuláře na úpravu vybrané kategorie
		[HttpPost]
		public IActionResult EditCategoryGet(int index)
		{
			// Kontrola oprávnění na načtení parametrů kategorie
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				Kategorie kategorie = GoodsSQL.GetCategoryById(index);
                ViewBag.ListOfUpperCategories = GoodsSQL.GetAllCategoriesIdNameAcronym();
                return View("EditCategory", kategorie);
			}
			return RedirectToAction(nameof(ListCategories), nameof(Goods));
		}

		// Příjem upravených dat vybrané kategorie
		[HttpPost]
		public IActionResult EditCategoryPost(Kategorie kategorie, int index)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
                ViewBag.ListOfUpperCategories = GoodsSQL.GetAllCategoriesIdNameAcronym();
                if (ModelState.IsValid)
				{
					kategorie.IdKategorie = index;
					kategorie.Zkratka = kategorie.Zkratka.ToUpper();
					if (!GoodsSQL.EditCategory(kategorie))
					{
						return View("EditCategory", kategorie);
					}
				}
				else
				{
					return View("EditCategory", kategorie);
				}
			}
			return RedirectToAction(nameof(ListCategories), nameof(Goods));
		}

		// Metoda pro odstranění vybrané kategorie
		[HttpPost]
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
				return View(zboziUmisteniKategorieDodavatel);
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

					if (GoodsSQL.RegisterGoods(newGoods))
					{
						return RedirectToAction(nameof(ListGoods), nameof(Goods));
					}
				}
				return View(newGoods);
			}

			return RedirectToAction(nameof(ListGoods), nameof(Goods));
		}

		// Načtení formuláře na úpravu vybraného zboží
		[HttpGet]
		public IActionResult EditGoodsGet(int index)
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
		public IActionResult EditGoodsPost(Zbozi_Umisteni_Kategorie_Dodavatele zboziUmisteniKategorieDodavatel, int index)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				if (ModelState.IsValid)
				{
					// Kontrola zda již neexistuje zboží s tímto čárovým kódem
					if (GoodsSQL.CheckExistsBarcode(zboziUmisteniKategorieDodavatel.Zbozi.CarovyKod) == true)
					{
						ViewBag.ErrorInfo = "Tento čárový kód je již používán!";
						return View("EditGoods", zboziUmisteniKategorieDodavatel);
					}
					zboziUmisteniKategorieDodavatel.Zbozi.IdZbozi = index;
					if (!GoodsSQL.EditGoods(zboziUmisteniKategorieDodavatel))
					{
						return View("EditGoods", zboziUmisteniKategorieDodavatel);
					}
				}
			}
			return RedirectToAction(nameof(ListGoods), nameof(Goods));
		}

		// Formální metoda pro odstranění vybraného zboží
		[HttpGet]
		public IActionResult DeleteGoods(int index)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_ZBOZI", index);
			}

			return RedirectToAction(nameof(ListGoods), nameof(Goods));
		}

		// Výpis všech umístění
		public IActionResult ListLocations()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				List<Umisteni> umisteni = GoodsSQL.GetAllLocations();
				return View(umisteni);
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
					if (GoodsSQL.AddLocation(noveUmisteni))
					{
						return RedirectToAction(nameof(ListLocations), nameof(Goods));
					}
				}
				return View(noveUmisteni);
			}
			return RedirectToAction(nameof(ListLocations), nameof(Goods));
		}

		// Načtení formuláře na úpravu vybraného umístění
		[HttpPost]
		public IActionResult EditLocationGet(int index)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				Umisteni umisteni = GoodsSQL.GetLocationById(index);
				return View("EditLocation", umisteni);
			}

			return RedirectToAction(nameof(ListLocations), nameof(Goods));
		}

		// Příjem upravených dat vybraného umístění
		[HttpPost]
		public IActionResult EditLocationPost(Umisteni umisteni, int index)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Skladnik"))
			{
				umisteni.IdUmisteni = index;
				if (!GoodsSQL.EditLocation(umisteni))
				{
					return View("EditLocation", umisteni);
				}
			}

			return RedirectToAction(nameof(ListLocations), nameof(Goods));
		}

		// Odstranění vybraného umístění
		[HttpPost]
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