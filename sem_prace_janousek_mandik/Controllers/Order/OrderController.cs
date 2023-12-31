using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Customer;
using sem_prace_janousek_mandik.Controllers.Employee;
using sem_prace_janousek_mandik.Controllers.Home;
using sem_prace_janousek_mandik.Controllers.Management;
using sem_prace_janousek_mandik.Controllers.Payment;
using sem_prace_janousek_mandik.Models.Order;
using System;

namespace sem_prace_janousek_mandik.Controllers.Order
{
	public class OrderController : BaseController
	{
		/// <summary>
		/// Výpis všech objednávek včetně objednaného zboží
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> ListOrders()
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik || Role == Roles.Zakaznik)
			{
				Objednavky_List orders = new();
				orders = await FillDataListOrders();

				return View(orders);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Metoda vytáhne data pro výpis objednávek + zboží + plateb
		/// </summary>
		/// <returns>Model dat s objednávkama, zbožím a platbama</returns>
		async Task<Objednavky_List> FillDataListOrders()
		{
			Objednavky_List orders = new();
			orders.Platby = await PaymentSQL.GetAllPayments();
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik)
			{
				orders.Objednavky_Zam_Zak_Fak = await OrderSQL.GetAllOrders();
				orders.ZboziObjednavek_Zbozi = await OrderSQL.GetAllGoodsOrders();
			}

			if (Role == Roles.Zakaznik)
			{
				orders.Objednavky_Zam_Zak_Fak = await OrderSQL.GetAllCustomerOrders(Email);
				orders.ZboziObjednavek_Zbozi = await OrderSQL.GetAllGoodsOrdersCustomer(Email);
			}

			return orders;
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech objednávek
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> SearchOrders(string search)
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik || Role == Roles.Zakaznik)
			{
				ViewBag.Search = search;
				Objednavky_List orders = new();

				orders = await FillDataListOrders();

				if (search != null)
				{
					search = search.ToLower();
					orders.Objednavky_Zam_Zak_Fak = orders.Objednavky_Zam_Zak_Fak?.Where(lmb => (lmb.Objednavky?.CisloObjednavky.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb.Objednavky?.DatumPrijeti.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb.Objednavky?.Poznamka?.ToLower() ?? string.Empty).Contains(search) || (lmb.Zamestnanci?.Jmeno?.ToLower() ?? string.Empty).Contains(search) || (lmb.Zamestnanci?.Prijmeni?.ToLower() ?? string.Empty).Contains(search) || (lmb.Faktury?.CastkaDoprava.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb.Faktury?.CastkaObjednavka.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb.Zakaznici?.Jmeno?.ToLower() ?? string.Empty).Contains(search) || (lmb.Zakaznici?.Prijmeni?.ToLower() ?? string.Empty).Contains(search)).ToList();
					if (orders.Objednavky_Zam_Zak_Fak?.Count == 0)
					{
						orders = await FillDataListOrders();
						orders.ZboziObjednavek_Zbozi = orders.ZboziObjednavek_Zbozi?.Where(lmb => (lmb.Zbozi?.Nazev?.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb.ZboziObjednavek?.Mnozstvi.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb.ZboziObjednavek?.JednotkovaCena.ToString().ToLower() ?? string.Empty).Contains(search)).ToList();
						var idObjednavkyList = orders.ZboziObjednavek_Zbozi?.Select(lmb => lmb.ZboziObjednavek?.IdObjednavky).ToList();
						orders.Objednavky_Zam_Zak_Fak = orders.Objednavky_Zam_Zak_Fak?.Where(lmb => idObjednavkyList.Contains(lmb.Objednavky?.IdObjednavky)).ToList();
						if (orders.ZboziObjednavek_Zbozi?.Count == 0)
						{
							orders = await FillDataListOrders();
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
		public async Task<IActionResult> EditGoodsOrderGet(int index)
		{
			if (Role == Roles.Admin)
			{
				ZboziObjednavek_Zbozi zboziObjednavek = await OrderSQL.GetGoodsOrderById(index);
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditGoodsOrderPost(ZboziObjednavek_Zbozi zboziObjednavky)
		{
			if (Role == Roles.Admin)
			{
				if (zboziObjednavky.ZboziObjednavek.Mnozstvi > 0)
				{ 
					await OrderSQL.EditGoodsOrder(zboziObjednavky);
				}
				else
				{
					ViewBag.ErrorInfo = "Množství nesmí být záporné!";
					return View("EditGoodsOrder", zboziObjednavky);
				}
			}
			return RedirectToAction(nameof(ListOrders), nameof(Order));
		}

		/// <summary>
		/// Metoda pro načtení formuláře na vytvoření objednávky
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> AddOrder()
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				Objednavky_Faktury_ZakList order = new();
				order.Zakaznici = await CustomerSQL.GetAllCustomersNameSurname();
				return View(order);
			}
			return RedirectToAction(nameof(ListOrders), nameof(Order));
		}

		/// <summary>
		/// Metoda pro zpracování dat nové objednávky
		/// </summary>
		/// <param name="newOrder">Model nové objednávky</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddOrder(Objednavky_Faktury_ZakList newOrder)
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				if (newOrder.Objednavky.DatumPrijeti > DateTime.Now)
				{
					ViewBag.ErrorInfo = "Datum přijetí nesmí být v budoucnosti!";
					return View(newOrder);
				}

				if (newOrder.Faktury.CastkaDoprava >= 0 && newOrder.Faktury.Dph >= 0)
				{
					newOrder.Objednavky.IdZamestnance = await EmployeeSQL.GetEmployeeIdByEmail(Email);
					if (await OrderSQL.AddOrder(newOrder))
					{
						return RedirectToAction(nameof(ListOrders), nameof(Order));
					}
				}
				else
				{
					ViewBag.Error = "Částka za dopravu a DPH je povinná a nesmí být záporná";
				}
				newOrder.Zakaznici = await CustomerSQL.GetAllCustomersNameSurname();
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
		public async Task<IActionResult> EditOrderGet(int idObjednavky)
		{
			if (Role == Roles.Admin)
			{
				Objednavky_Zam_Zak_FakturyList order = new();
				order.Objednavky = await OrderSQL.GetOrderById(idObjednavky);
				order.Faktury = await PaymentSQL.GetAllInvoices();
				order.Zamestnanci = await EmployeeSQL.GetAllEmployeesNameSurname();
				order.Zakaznici = await CustomerSQL.GetAllCustomersNameSurname();

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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditOrderPost(Objednavky_Zam_Zak_FakturyList order)
		{
			if (Role == Roles.Admin)
			{
				if (order.Objednavky.DatumPrijeti > DateTime.Now)
				{
					ViewBag.ErrorInfo = "Datum přijetí nesmí být v budoucnosti!";
					return View("EditOrder", order);
				}

				if (!await OrderSQL.EditOrder(order))
				{
					order.Faktury = await PaymentSQL.GetAllInvoices();
					order.Zamestnanci = await EmployeeSQL.GetAllEmployeesNameSurname();
					order.Zakaznici = await CustomerSQL.GetAllCustomersNameSurname();
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
		public async Task<IActionResult> AddGoodsToOrderGet(int idObjednavky)
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				// Kontrola, zda je objednávka otevřena
				if (await OrderSQL.IsClosedOrder(idObjednavky) == false)
				{
					ZboziObjednavek_ZboziList goods = new();
					goods.Zbozi = await OrderSQL.GetAllGoods();
					goods.IdObjednavky = idObjednavky;
					return View("AddGoodsToOrder", goods);
				}
			}
			return RedirectToAction(nameof(ListOrders), nameof(Order));
		}

		/// <summary>
		/// Metoda pro zpracování dat přidání zboží do objednávky
		/// </summary>
		/// <param name="addZboziObjednavek">Model s daty přidávaného zboží</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddGoodsToOrderPost(ZboziObjednavek_ZboziList addZboziObjednavek)
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				// Kontrola, zda je objednávka otevřena
				if (await OrderSQL.IsClosedOrder(addZboziObjednavek.ZboziObjednavek.IdObjednavky) == false)
				{
					if (addZboziObjednavek.ZboziObjednavek.Mnozstvi > 0)
					{
						float jednotkovaCena = await OrderSQL.GetPriceForGoods(addZboziObjednavek.ZboziObjednavek.IdZbozi);
						addZboziObjednavek.ZboziObjednavek.JednotkovaCena = jednotkovaCena;

						string? err = await OrderSQL.AddGoodsToOrder(addZboziObjednavek);
						if (err == null)
						{
							return RedirectToAction(nameof(ListOrders), nameof(Order));
						}
						else
						{
							ViewBag.ErrorInfo = err;
						}
					}
				}
				addZboziObjednavek.IdObjednavky = addZboziObjednavek.ZboziObjednavek.IdObjednavky;
				addZboziObjednavek.Zbozi = await OrderSQL.GetAllGoods();
			}
			return View("AddGoodsToOrder", addZboziObjednavek);
		}

		/// <summary>
		/// Metoda pro uzavření objednávky
		/// </summary>
		/// <param name="idObjednavky">ID objednávky</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> CloseOrder(int idObjednavky)
		{
			if (Role == Roles.Admin || Role == Roles.Manazer || Role == Roles.Logistik)
			{
				await OrderSQL.CloseOrder(idObjednavky);
			}
			return RedirectToAction(nameof(ListOrders), nameof(Order));
		}

		/// <summary>
		/// Metoda pro odstranění vybrané objednávky
		/// </summary>
		/// <param name="index">ID objednávky</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteOrder(int index)
		{
			if (Role == Roles.Admin)
			{
				await SharedSQL.CallDeleteProcedure("pkg_delete.P_SMAZAT_OBJEDNAVKU", index);
			}
			return RedirectToAction(nameof(ListOrders), nameof(Order));
		}

		/// <summary>
		/// Metoda pro odstranění vybraného zboží z objednávky
		/// </summary>
		/// <param name="index">ID zboží objednávky</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteGoodsOrder(int index)
		{
			if (Role == Roles.Admin)
			{
				await SharedSQL.CallDeleteProcedure("pkg_delete.P_SMAZAT_ZBOZI_OBJEDNAVEK", index);
			}
			return RedirectToAction(nameof(ListOrders), nameof(Order));
		}
	}
}
