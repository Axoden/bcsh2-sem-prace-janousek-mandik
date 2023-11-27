using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Customer;
using sem_prace_janousek_mandik.Controllers.Employee;
using sem_prace_janousek_mandik.Controllers.Home;
using sem_prace_janousek_mandik.Controllers.Payment;
using sem_prace_janousek_mandik.Models.Order;

namespace sem_prace_janousek_mandik.Controllers.Order
{
	public class OrderController : BaseController
	{
		/// <summary>
		/// Výpis všech objednávek včetně objednaného zboží
		/// </summary>
		/// <returns></returns>
		public IActionResult ListOrders()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Zakaznik"))
			{
				Objednavky_List orders = new();
				orders = FillDataListOrders();

				return View(orders);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda vytáhne data pro výpis objednávek + zboží + plateb
		/// </summary>
		/// <returns>Model dat s objednávkama, zbožím a platbama</returns>
		Objednavky_List FillDataListOrders()
		{
			Objednavky_List orders = new();
			orders.Platby = PaymentSQL.GetAllPayments();
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik"))
			{
				orders.Objednavky_Zam_Zak_Fak = OrderSQL.GetAllOrders();
				orders.ZboziObjednavek_Zbozi = OrderSQL.GetAllGoodsOrders();
			}

			if (Role.Equals("Zakaznik"))
			{
				orders.Objednavky_Zam_Zak_Fak = OrderSQL.GetAllCustomerOrders(Email);
				orders.ZboziObjednavek_Zbozi = OrderSQL.GetAllGoodsOrdersCustomer(Email);
			}

			return orders;
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech objednávek
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult SearchOrders(string search)
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik") || Role.Equals("Zakaznik"))
			{
				ViewBag.Search = search;
				Objednavky_List orders = new();

				orders = FillDataListOrders();

				if (search != null)
				{
					search = search.ToLower();
					orders.Objednavky_Zam_Zak_Fak = orders.Objednavky_Zam_Zak_Fak?.Where(lmb => (lmb.Objednavky?.CisloObjednavky.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb.Objednavky?.DatumPrijeti?.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb.Objednavky?.Poznamka?.ToLower() ?? string.Empty).Contains(search) || (lmb.Zamestnanci?.Jmeno?.ToLower() ?? string.Empty).Contains(search) || (lmb.Zamestnanci?.Prijmeni?.ToLower() ?? string.Empty).Contains(search) || (lmb.Faktury?.CastkaDoprava.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb.Faktury?.CastkaObjednavka.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb.Zakaznici?.Jmeno?.ToLower() ?? string.Empty).Contains(search) || (lmb.Zakaznici?.Prijmeni?.ToLower() ?? string.Empty).Contains(search)).ToList();
					if (orders.Objednavky_Zam_Zak_Fak?.Count == 0)
					{
						orders = FillDataListOrders();
						orders.ZboziObjednavek_Zbozi = orders.ZboziObjednavek_Zbozi?.Where(lmb => (lmb.Zbozi?.Nazev?.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb.ZboziObjednavek?.Mnozstvi.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb.ZboziObjednavek?.JednotkovaCena.ToString().ToLower() ?? string.Empty).Contains(search)).ToList();
						var idObjednavkyList = orders.ZboziObjednavek_Zbozi?.Select(lmb => lmb.ZboziObjednavek?.IdObjednavky).ToList();
						orders.Objednavky_Zam_Zak_Fak = orders.Objednavky_Zam_Zak_Fak?.Where(lmb => idObjednavkyList.Contains(lmb.Objednavky?.IdObjednavky)).ToList();
						if (orders.ZboziObjednavek_Zbozi?.Count == 0)
						{
							orders = FillDataListOrders();
							orders.Platby = orders.Platby?.Where(lmb => (lmb.DatumPlatby.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb.Castka.ToString().ToLower() ?? string.Empty).Contains(search)).ToList();
							var idFakturyPlatbyList = orders.Platby?.Select(lmb => lmb.IdFaktury).ToList();
							orders.Objednavky_Zam_Zak_Fak = orders.Objednavky_Zam_Zak_Fak?.Where(lmb => idFakturyPlatbyList.Contains(lmb.Objednavky.IdFaktury)).ToList();
						}
					}
				}
				return View(nameof(ListOrders), orders);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda na načtení formuláře na úpravu zboží v objednávce
		/// </summary>
		/// <param name="index">ID zboží objednávky</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult EditGoodsOrderGet(int index)
		{
			if (Role.Equals("Admin"))
			{
				ZboziObjednavek_Zbozi zboziObjednavek = OrderSQL.GetGoodsOrderById(index);
				return View("EditGoodsOrder", zboziObjednavek);
			}
			return RedirectToAction(nameof(ListOrders), nameof(Order));
		}

		/// <summary>
		/// Metoda pro zpracování upravených dat konkrétního zboží z objednávky
		/// </summary>
		/// <param name="zboziObjednavky">Model s upravenými daty zboží objednávky</param>
		/// <param name="index"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult EditGoodsOrderPost(ZboziObjednavek_Zbozi zboziObjednavky)
		{
			if (Role.Equals("Admin"))
			{
				OrderSQL.EditGoodsOrder(zboziObjednavky);
			}
			return RedirectToAction(nameof(ListOrders), nameof(Order));
		}

		/// <summary>
		/// Metoda pro načtení formuláře na vytvoření objednávky
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult AddOrder()
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				ViewBag.ListOfCustomers = CustomerSQL.GetAllCustomersNameSurname();
				return View();
			}
			return RedirectToAction(nameof(ListOrders), nameof(Order));
		}

		/// <summary>
		/// Metoda pro zpracování dat nové objednávky
		/// </summary>
		/// <param name="newOrder">Model nové objednávky</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult AddOrder(Objednavy_Faktury newOrder)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				//if (ModelState.IsValid == true)
				//{
				newOrder.Objednavky.IdZamestnance = EmployeeSQL.GetEmployeeIdByEmail(Email);
				bool uspesnePridani = OrderSQL.AddOrder(newOrder);

				if (uspesnePridani == true)
				{
					return RedirectToAction(nameof(ListOrders), nameof(Order));
				}
				//}
				return View(newOrder);
			}

			return RedirectToAction(nameof(ListOrders), nameof(Order));
		}

		/// <summary>
		/// Metoda pro načtení formuláře na úpravu objednávky
		/// </summary>
		/// <param name="index">ID objednávky</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult EditOrderGet(int idObjednavky)
		{
			if (Role.Equals("Admin"))
			{
				Objednavky_Zam_Zak_FakturyList order = new();
				order.Objednavky = OrderSQL.GetOrderById(idObjednavky);
				order.Faktury = PaymentSQL.GetAllInvoices();
				order.Zamestnanci = EmployeeSQL.GetAllEmployeesNameSurname();
				order.Zakaznici = CustomerSQL.GetAllCustomersNameSurname();

				return View("EditOrder", order);
			}
			return RedirectToAction(nameof(ListOrders), nameof(Order));
		}

		/// <summary>
		/// Metoda pro zpracování dat upravené objednávky
		/// </summary>
		/// <param name="index"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult EditOrderPost(Objednavky_Zam_Zak_FakturyList order)
		{
			if (Role.Equals("Admin"))
			{
				if (!OrderSQL.EditOrder(order))
				{
					order.Faktury = PaymentSQL.GetAllInvoices();
					order.Zamestnanci = EmployeeSQL.GetAllEmployeesNameSurname();
					order.Zakaznici = CustomerSQL.GetAllCustomersNameSurname();
					return View("EditOrder", order);
				}
			}
			return RedirectToAction(nameof(ListOrders), nameof(Order));
		}

		/// <summary>
		/// Metoda pro načtení formuláře na přidání zboží do objednávky
		/// </summary>
		/// <param name="idObjednavky">ID objednávky</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult AddGoodsToOrderGet(int idObjednavky)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				// Kontrola, zda je objednávka otevřena
				if (OrderSQL.IsClosedOrder(idObjednavky) == false)
				{
					ViewBag.IdObjednavky = idObjednavky;
					ViewBag.ListOfGoods = OrderSQL.GetAllGoods();
				}
			}
			return View("AddGoodsToOrder");
		}

		/// <summary>
		/// Metoda pro zpracování dat přidání zboží do objednávky
		/// </summary>
		/// <param name="addZboziObjednavek">Model s daty přidávaného zboží</param>
		/// <param name="idObjednavky"></param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult AddGoodsToOrderPost(ZboziObjednavek_Zbozi addZboziObjednavek)
		{

			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				// Kontrola, zda je objednávka otevřena
				if (OrderSQL.IsClosedOrder(addZboziObjednavek.ZboziObjednavek.IdObjednavky) == false)
				{
					// TODO: Validace vstupu
					float jednotkovaCena = OrderSQL.GetPriceForGoods(addZboziObjednavek.ZboziObjednavek.IdZbozi);
					addZboziObjednavek.ZboziObjednavek.JednotkovaCena = jednotkovaCena;

					if (OrderSQL.AddGoodsToOrder(addZboziObjednavek))
					{
						return RedirectToAction(nameof(ListOrders), nameof(Order));
					}
				}
			}
			return View("AddGoodsToOrder");
		}

		/// <summary>
		/// Metoda pro uzavření objednávky
		/// </summary>
		/// <param name="idObjednavky">ID objednávky</param>
		/// <returns></returns>
		public IActionResult CloseOrder(int idObjednavky)
		{
			if (Role.Equals("Admin") || Role.Equals("Manazer") || Role.Equals("Logistik"))
			{
				OrderSQL.CloseOrder(idObjednavky);
			}
			return RedirectToAction(nameof(ListOrders), nameof(Order));
		}

		/// <summary>
		/// Metoda pro odstranění vybrané objednávky
		/// </summary>
		/// <param name="index">ID objednávky</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult DeleteOrder(int index)
		{
			if (Role.Equals("Admin"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_OBJEDNAVKU", index);
			}
			return RedirectToAction(nameof(ListOrders), nameof(Order));
		}

		/// <summary>
		/// Metoda pro odstranění vybraného zboží z objednávky
		/// </summary>
		/// <param name="index">ID zboží objednávky</param>
		/// <returns></returns>
		[HttpPost]
		public IActionResult DeleteGoodsOrder(int index)
		{
			if (Role.Equals("Admin"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_ZBOZI_OBJEDNAVEK", index);
			}
			return RedirectToAction(nameof(ListOrders), nameof(Order));
		}
	}
}
