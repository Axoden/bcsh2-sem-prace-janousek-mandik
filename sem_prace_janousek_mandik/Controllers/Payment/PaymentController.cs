using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Home;
using sem_prace_janousek_mandik.Controllers.Management;
using sem_prace_janousek_mandik.Controllers.Order;
using sem_prace_janousek_mandik.Models.Order;
using sem_prace_janousek_mandik.Models.Payment;
using System.Text;

namespace sem_prace_janousek_mandik.Controllers.Payment
{
	public class PaymentController : BaseController
	{
		/// <summary>
		/// Výpis všech faktur včetně plateb
		/// </summary>
		/// <returns></returns>
		public async Task<IActionResult> ListInvoices()
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik)
			{
				Faktury_Platby invoices = new();
				invoices.Faktury = await PaymentSQL.GetAllInvoices();
				invoices.Platby = await PaymentSQL.GetAllPayments();
				return View(invoices);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Vyhledávání ve výpisu všech faktur a plateb
		/// </summary>
		/// <param name="search">Vyhledávaná fráze</param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> SearchInvoices(string search)
		{
			if (Role == Roles.Manazer || Role == Roles.Admin || Role == Roles.Logistik)
			{
				ViewBag.Search = search;
				Faktury_Platby invoices = new();
				invoices.Faktury = await PaymentSQL.GetAllInvoices();
				invoices.Platby = await PaymentSQL.GetAllPayments();
				if (search != null)
				{
					search = search.ToLower();
					invoices.Faktury = invoices.Faktury.Where(lmb => (lmb.CisloFaktury.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb?.DatumVystaveni.ToString()?.ToLower() ?? string.Empty).Contains(search) || (lmb?.DatumSplatnosti?.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb?.CastkaObjednavka.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb?.CastkaDoprava.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb?.Dph.ToString().ToLower() ?? string.Empty).Contains(search)).ToList();
					if(invoices.Faktury.Count == 0)
					{
						invoices.Faktury = await PaymentSQL.GetAllInvoices();
						invoices.Platby = invoices.Platby.Where(lmb => (lmb.DatumPlatby.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb?.Castka.ToString()?.ToLower() ?? string.Empty).Contains(search) || (lmb?.TypPlatby?.ToString().ToLower() ?? string.Empty).Contains(search) || (lmb?.VariabilniSymbol?.ToString().ToLower() ?? string.Empty).Contains(search)).ToList();
						var idFakturyList = invoices.Platby.Select(lmb => lmb.IdFaktury).ToList();
						invoices.Faktury = invoices.Faktury.Where(lmb => idFakturyList.Contains(lmb.IdFaktury)).ToList();
					}
				}
				return View(nameof(ListInvoices), invoices);
			}
			return RedirectToAction(nameof(HomeController.Index), nameof(Home));
		}

		/// <summary>
		/// Načtení formuláře na přidání faktury
		/// </summary>
		/// <returns></returns>
		[HttpGet]
		public IActionResult AddInvoice()
		{
			if (Role == Roles.Admin)
			{
				return View();
			}
			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		/// <summary>
		/// Příjem dat nové faktury
		/// </summary>
		/// <param name="invoice">Model s novými daty faktury</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddInvoice(Faktury invoice)
		{
			if (Role == Roles.Admin)
			{
				if (!ModelState.IsValid)
				{
					return View(invoice);
				}

				if (invoice.DatumVystaveni > DateOnly.FromDateTime(DateTime.Now))
				{
					ViewBag.ErrorInfo = "Datum vystavení nesmí být v budoucnosti!";
					return View(invoice);
				}

				if (invoice.DatumSplatnosti < invoice.DatumVystaveni)
				{
					ViewBag.ErrorInfo = "Datum vystavení musí být před datem splatnosti!";
					return View(invoice);
				}

				await PaymentSQL.AddInvoice(invoice);
				return RedirectToAction(nameof(ListInvoices), nameof(Payment));
			}
			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		/// <summary>
		/// Načtení formuláře na úpravu vybrané faktury
		/// </summary>
		/// <param name="index">ID upravované faktury</param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> EditInvoiceGet(int index)
		{
			if (Role == Roles.Admin)
			{
				Faktury invoice = await PaymentSQL.GetInvoiceById(index);
				return View("EditInvoice", invoice);
			}
			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		/// <summary>
		/// Příjem upravených dat vybrané faktury
		/// </summary>
		/// <param name="invoice"></param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditInvoicePost(Faktury invoice)
		{
			if (Role == Roles.Admin)
			{
				ModelState.Remove("CisloFaktury");
				if (!ModelState.IsValid)
				{
					return View("EditInvoice", invoice);
				}

				if (invoice.DatumVystaveni > DateOnly.FromDateTime(DateTime.Now))
				{
					ViewBag.ErrorInfo = "Datum vystavení nesmí být v budoucnosti!";
					return View("EditInvoice", invoice);
				}

				if (invoice.DatumSplatnosti < invoice.DatumVystaveni)
				{
					ViewBag.ErrorInfo = "Datum vystavení musí být před datem splatnosti!";
					return View("EditInvoice", invoice);
				}

				await PaymentSQL.EditInvoice(invoice);
				return RedirectToAction(nameof(ListInvoices), nameof(Payment));
			}
			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		/// <summary>
		/// Metoda pro odstranění vybrané faktury
		/// </summary>
		/// <param name="index">ID odstraňované faktury</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeleteInvoice(int index)
		{
			if (Role == Roles.Admin)
			{
				await SharedSQL.CallDeleteProcedure("pkg_delete.P_SMAZAT_FAKTURU", index);
			}
			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}


		/// <summary>
		/// Načtení formuláře na úpravu vybrané platby
		/// </summary>
		/// <param name="index">ID upravované platby</param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> EditPaymentGet(int index)
		{
			if (Role == Roles.Admin)
			{
				Platba_Faktury payment = new();
				payment.Platby = await PaymentSQL.GetPaymentById(index);
				payment.Faktury = await PaymentSQL.GetAllInvoices();
				return View("EditPayment", payment);
			}
			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		/// <summary>
		/// Příjem upravených dat vybrané platby
		/// </summary>
		/// <param name="payment">ID platby</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> EditPaymentPost(Platba_Faktury payment)
		{
			if (Role == Roles.Admin)
			{
				if (ModelState.IsValid)
				{
                    if (payment.Platby.TypPlatby.Equals("Prevodem") && payment.Platby.VariabilniSymbol == null)
                    {
                        ViewBag.ErrorInfo = "Při platbě převodem je potřeba variabilní symbol!";
                        payment.Faktury = await PaymentSQL.GetAllInvoices();
                        return View("EditPayment", payment);
                    }
                    await PaymentSQL.EditPayment(payment);
                }
				else
				{
                    payment.Faktury = await PaymentSQL.GetAllInvoices();
                    return View("EditPayment", payment);
				}
			}
			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		/// <summary>
		/// Metoda pro odstranění vybrané faktury
		/// </summary>
		/// <param name="index">ID platby</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> DeletePayment(int index)
		{
			if (Role == Roles.Admin)
			{
				await SharedSQL.CallDeleteProcedure("pkg_delete.P_SMAZAT_PLATBU", index);
			}
			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		/// <summary>
		/// Načtení formuláře na přidání platby
		/// </summary>
		/// <returns></returns>
		[HttpPost]
		public IActionResult AddPaymentGet(int idInvoice)
		{
			if (Role == Roles.Admin)
			{
				PlatbyCustomerForm payment = new();
				payment.IdFaktury = idInvoice;
				return View("AddPayment", payment);
			}
			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		/// <summary>
		/// Příjem dat nové platby
		/// </summary>
		/// <param name="payment">Model s daty nové platby</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddPaymentPost(PlatbyCustomerForm payment)
		{
			if (Role == Roles.Admin)
			{
				if (ModelState.IsValid)
				{
					if (payment.TypPlatby.Equals("Prevodem") && payment.VariabilniSymbol == null)
					{
						ViewBag.ErrorInfo = "Při platbě převodem je potřeba variabilní symbol!";
						return View("AddPayment", payment);
					}
					await PaymentSQL.AddPayment(payment);
				}
				else
				{
					return View("AddPayment", payment);
				}
			}
			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		/// <summary>
		/// Platba faktury zákazníkem
		/// </summary>
		/// <param name="idObjednavky">ID objednávky</param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> AddPaymentCustomerGet(int idObjednavky)
		{
			Objednavky_Zakaznici_Faktury customer = await OrderSQL.GetCustomerOrderInvoice(idObjednavky);

			if (customer.Zakaznici.Email.Equals(Email))
			{
				PlatbyCustomerForm payment = new();
				payment.IdFaktury = customer.Faktury.IdFaktury;

				float zbyvaZaplatit = await RemainsToPay(idObjednavky, customer, payment.IdFaktury);

				payment.Castka = zbyvaZaplatit;
				ViewBag.idObjednavky = idObjednavky;
				return View("AddPaymentCustomer", payment);
			}

			return RedirectToAction(nameof(OrderController.ListOrders), nameof(Order));
		}

		/// <summary>
		/// Metoda zjistí částku, která zbývá k zaplacení objednávky
		/// </summary>
		/// <param name="idObjednavky">ID objednávky</param>
		/// <param name="customer">Model zákazníka</param>
		/// <param name="idInvoice">ID faktury</param>
		/// <returns>Částka, která zbýva zaplatit</returns>
		private static async Task<float> RemainsToPay(int idObjednavky, Objednavky_Zakaznici_Faktury customer, int idInvoice)
		{
			List<ZboziObjednavek> zboziObjednavek = await OrderSQL.GetAllGoodsOrdersById(idObjednavky);
			List<Platby> payments = await PaymentSQL.GetAllPaymentsByInvoiceId(idInvoice);

			var sumJednotkovaCenaBezDph = zboziObjednavek.Where(lmb => lmb.IdObjednavky == idObjednavky).Sum(lmb => (lmb.JednotkovaCena * lmb.Mnozstvi));
			sumJednotkovaCenaBezDph = sumJednotkovaCenaBezDph + customer.Faktury.CastkaDoprava;
			string dph = "1." + customer.Faktury.Dph.ToString();
			var sumJednotkovaCenaDph = (float)Math.Round((sumJednotkovaCenaBezDph * float.Parse(dph)), 2);
			var celkovaSumaPlateb = payments.Where(p => p.IdFaktury == idInvoice).Sum(p => p.Castka);
			float zbyvaZaplatit = sumJednotkovaCenaDph - celkovaSumaPlateb;
			return zbyvaZaplatit;
		}

		/// <summary>
		/// Zpracování formuláře platby faktury zákazníkem
		/// </summary>
		/// <param name="customerPay">Model s daty nové platby</param>
		/// <param name="idFaktury">ID faktury</param>
		/// <param name="idObjednavky">ID objednávky</param>
		/// <returns></returns>
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddPaymentCustomerPost(PlatbyCustomerForm customerPay, int idObjednavky)
		{
			Objednavky_Zakaznici_Faktury customer = await OrderSQL.GetCustomerOrderInvoice(idObjednavky);

			if (customer.Zakaznici.Email.Equals(Email) && customer.Objednavky.IdObjednavky == idObjednavky && customer.Faktury.IdFaktury == customerPay.IdFaktury)
			{

				PlatbyCustomerForm payment = new();
				payment.IdFaktury = customer.Faktury.IdFaktury;

				float zbyvaZaplatit = await RemainsToPay(idObjednavky, customer, payment.IdFaktury);

				payment.Castka = zbyvaZaplatit;
				ViewBag.idObjednavky = idObjednavky;

				if (zbyvaZaplatit < customerPay.Castka)
				{
					ViewBag.ErrorInfo = "Zaplacená částka je vyšší než požadovaná";
					return View("AddPaymentCustomer", payment);
				}

				if (ModelState.IsValid)
				{
					if (customerPay.TypPlatby.Equals("Prevodem") && customerPay.VariabilniSymbol == null)
					{
						ViewBag.ErrorInfo = "Při platbě převodem je potřeba variabilní symbol!";
						return View("AddPaymentCustomer", payment);
					}

					if(!await PaymentSQL.AddCustomerPayment(customerPay))
					{
						return View("AddPaymentCustomer", payment);
					}
				}
				else
				{
					ViewBag.ErrorInfo = "Některá pole nejsou správně vyplněna!";
					return View("AddPaymentCustomer", payment);
				}
			}

			return RedirectToAction(nameof(OrderController.ListOrders), nameof(Order));
		}

		/// <summary>
		/// Metoda slouží ke stažení faktury v PDF
		/// </summary>
		/// <param name="idFaktury">ID faktury</param>
		/// <returns></returns>
		public async Task<IActionResult> DownloadInvoice(int idFaktury)
		{
			Faktury faktura = await PaymentSQL.GetInvoiceById(idFaktury);
			List<ZboziObjednavek_Zbozi> zbozi = await OrderSQL.GetGoodsOrderByIdInvoice(idFaktury);

			MemoryStream memoryStream = new();
			CreateInvoicePDF(memoryStream, faktura, zbozi);
			memoryStream.Position = 0;

			string fileName = $"Faktura_{faktura.CisloFaktury}.pdf";
			return File(memoryStream, "application/pdf", fileName);
		}

		/// <summary>
		/// Metoda slouží k vygenerování obsahu PDF
		/// </summary>
		/// <param name="stream">Stream bytů</param>
		/// <param name="faktura">Model s daty faktury</param>
		/// <param name="zbozi">List zboží náležící k faktuře, resp. objednávce</param>
		private static void CreateInvoicePDF(MemoryStream stream, Faktury faktura, List<ZboziObjednavek_Zbozi> zbozi)
		{
			StreamWriter writer = new(stream, Encoding.UTF8);

			writer.WriteLine("%PDF-1.4");
			writer.WriteLine("1 0 obj");
			writer.WriteLine("<< /Type /Catalog /Pages 2 0 R >>");
			writer.WriteLine("endobj");
			writer.WriteLine("2 0 obj");
			writer.WriteLine("<< /Type /Pages /Kids [3 0 R] /Count 1 >>");
			writer.WriteLine("endobj");
			writer.WriteLine("3 0 obj");
			writer.WriteLine("<< /Type /Page /Parent 2 0 R /MediaBox [0 0 595 842] /Contents 4 0 R >>");
			writer.WriteLine("endobj");
			writer.WriteLine("4 0 obj");
			writer.WriteLine("<< /Length 223 >>");
			writer.WriteLine("stream");
			writer.WriteLine("BT");
			writer.WriteLine("/F1 12 Tf");
			writer.WriteLine("100 750 Td");
			writer.WriteLine($"(Cislo faktury: {faktura.CisloFaktury}) Tj");
			writer.WriteLine("0 -20 Td");
			writer.WriteLine($"(Datum vystaveni: {faktura.DatumVystaveni?.ToString("dd.MM.yyyy")}) Tj");
			writer.WriteLine("0 -20 Td");
			writer.WriteLine($"(Datum splatnosti: {faktura.DatumSplatnosti?.ToString("dd.MM.yyyy")}) Tj");
			writer.WriteLine("0 -20 Td");
			writer.WriteLine($"(Celkova castka za zbozi bez DPH: {faktura.CastkaObjednavka} CZK) Tj");
			writer.WriteLine("0 -20 Td");
			writer.WriteLine($"(Castka za dopravu bez DPH: {faktura.CastkaDoprava} CZK) Tj");
			writer.WriteLine("0 -20 Td");
			writer.WriteLine($"(DPH: {faktura.Dph}%) Tj");

			// Výpis celkové ceny objednávky včetně DPH
			string dph = "1." + faktura.Dph.ToString();
			float celkovaCastka = (float)Math.Round(((faktura.CastkaObjednavka + faktura.CastkaDoprava) * float.Parse(dph)), 2);
			writer.WriteLine("0 -20 Td");
			writer.WriteLine($"(Celkova castka vcetne DPH: {celkovaCastka} CZK) Tj");

			writer.WriteLine("0 -20 Td");
			writer.WriteLine("0 -20 Td");
			writer.WriteLine("0 -20 Td");
			writer.WriteLine($"(Objednane zbozi:) Tj");
			writer.WriteLine("0 -20 Td");

			foreach (ZboziObjednavek_Zbozi item in zbozi)
			{
				writer.WriteLine($"({item.Zbozi.Nazev}, Cena za kus: {item.ZboziObjednavek.JednotkovaCena} CZK, Mnozstvi: {item.ZboziObjednavek.Mnozstvi} ks) Tj");
				writer.WriteLine("0 -20 Td");
			}

			writer.WriteLine("ET");
			writer.WriteLine("endstream");
			writer.WriteLine("endobj");
			writer.WriteLine("xref");
			writer.WriteLine("0 5");
			writer.WriteLine("0000000000 65535 f ");
			writer.WriteLine("0000000018 00000 n ");
			writer.WriteLine("0000000077 00000 n ");
			writer.WriteLine("0000000178 00000 n ");
			writer.WriteLine("0000000305 00000 n ");
			writer.WriteLine("trailer");
			writer.WriteLine("<< /Size 5 /Root 1 0 R >>");
			writer.WriteLine("startxref");
			writer.WriteLine("625");
			writer.WriteLine("%%EOF");
			writer.Flush();
		}
	}
}
