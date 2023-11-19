using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Order;
using sem_prace_janousek_mandik.Models.Order;
using sem_prace_janousek_mandik.Models.Payment;
using System.Text;

namespace sem_prace_janousek_mandik.Controllers.Payment
{
	public class PaymentController : BaseController
	{
		// Výpis všech faktur včetně plateb
		public IActionResult ListInvoices()
		{
			if (Role.Equals("Manazer") || Role.Equals("Admin") || Role.Equals("Logistik"))
			{
				List<Faktury> invoice = PaymentSQL.GetAllInvoices();
				List<Platby> payments = PaymentSQL.GetAllPayments();
				ViewBag.ListOfPayments = payments;

				return View(invoice);
			}

			// Přesměrování, pokud uživatel nemá povolen přístup
			return RedirectToAction("Index", "Home");
		}

		// Načtení formuláře na přidání faktury
		[HttpGet]
		public IActionResult AddInvoice()
		{
			if (Role.Equals("Admin"))
			{
				return View();
			}

			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		// Příjem dat nové faktury
		[HttpPost]
		public IActionResult AddInvoice(Faktury invoice)
		{
			if (Role.Equals("Admin"))
			{
				PaymentSQL.AddInvoice(invoice);
			}

			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		// Načtení formuláře na úpravu vybrané faktury
		[HttpGet]
		public IActionResult EditInvoice(int index)
		{
			// Kontrola oprávnění na načtení parametrů faktury
			if (Role.Equals("Admin"))
			{
				Faktury invoice = PaymentSQL.GetInvoiceById(index);
				return View(invoice);
			}

			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		// Příjem upravených dat vybrané faktury
		[HttpPost]
		public IActionResult EditInvoice(Faktury invoice, int index)
		{
			if (Role.Equals("Admin"))
			{
				invoice.IdFaktury = index;
				PaymentSQL.EditInvoice(invoice);
			}

			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		// Formální metoda pro odstranění vybrané faktury
		[HttpGet]
		public IActionResult DeleteInvoice(int index)
		{
			if (Role.Equals("Admin"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_FAKTURU", index);
			}

			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}


		// Načtení formuláře na úpravu vybrané platby
		[HttpGet]
		public IActionResult EditPayment(int index)
		{
			// Kontrola oprávnění na načtení parametrů platby
			if (Role.Equals("Admin"))
			{
				Platby payment = PaymentSQL.GetPaymentById(index);
				List<Faktury> invoices = PaymentSQL.GetAllInvoices();
				ViewBag.ListOfInvoices = invoices;
				return View(payment);
			}

			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		// Příjem upravených dat vybrané platby
		[HttpPost]
		public IActionResult EditPayment(Platby payment, int index)
		{
			if (Role.Equals("Admin"))
			{
				payment.IdPlatby = index;
				PaymentSQL.EditPayment(payment);
			}

			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		// Formální metoda pro odstranění vybrané faktury
		[HttpGet]
		public IActionResult DeletePayment(int index)
		{
			if (Role.Equals("Admin"))
			{
				SharedSQL.CallDeleteProcedure("P_SMAZAT_PLATBU", index);
			}

			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		// Načtení formuláře na přidání platby
		[HttpGet]
		public IActionResult AddPayment()
		{
			if (Role.Equals("Admin"))
			{
				return View();
			}

			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		// Příjem dat nové platby
		[HttpPost]
		public IActionResult AddPayment(Platby payment)
		{
			if (Role.Equals("Admin"))
			{
				PaymentSQL.AddPayment(payment);
			}

			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

		// Platba faktury zákazníkem
		[HttpPost]
		public IActionResult AddPaymentCustomerGet(int idObjednavky)
		{
			Objednavky_Zakaznici_Faktury customer = OrderSQL.GetCustomerOrderInvoice(idObjednavky);

			if (customer.Zakaznici.Email.Equals(Email))
			{
				PlatbyCustomerForm payment = new();
				payment.IdFaktury = customer.Faktury.IdFaktury;

				float zbyvaZaplatit = RemainsToPay(idObjednavky, customer, payment);

				payment.Castka = zbyvaZaplatit;
				ViewBag.idObjednavky = idObjednavky;
				return View("AddPaymentCustomer", payment);
			}

			return RedirectToAction(nameof(OrderController.ListOrders), nameof(Order));
		}

		private static float RemainsToPay(int idObjednavky, Objednavky_Zakaznici_Faktury customer, PlatbyCustomerForm payment)
		{
			List<ZboziObjednavek> zboziObjednavek = OrderSQL.GetAllGoodsOrdersById(idObjednavky);
			List<Platby> payments = PaymentSQL.GetAllPaymentsByInvoiceId(payment.IdFaktury);

			var sumJednotkovaCenaBezDph = zboziObjednavek.Where(lmb => lmb.IdObjednavky == idObjednavky).Sum(lmb => (lmb.JednotkovaCena * lmb.Mnozstvi));
			sumJednotkovaCenaBezDph = sumJednotkovaCenaBezDph + customer.Faktury.CastkaDoprava;
			string dph = "1." + customer.Faktury.Dph.ToString();
			var sumJednotkovaCenaDph = (float)Math.Round((sumJednotkovaCenaBezDph * float.Parse(dph)), 2);
			var celkovaSumaPlateb = payments.Where(p => p.IdFaktury == payment.IdFaktury).Sum(p => p.Castka);
			float zbyvaZaplatit = sumJednotkovaCenaDph - celkovaSumaPlateb;
			return zbyvaZaplatit;
		}

		// Zpracování formuláře platby faktury zákazníkem
		[HttpPost]
		public IActionResult AddPaymentCustomerPost(PlatbyCustomerForm customerPay, int idFaktury, int idObjednavky)
		{
			Objednavky_Zakaznici_Faktury customer = OrderSQL.GetCustomerOrderInvoice(idObjednavky);

			if (customer.Zakaznici.Email.Equals(Email) && customer.Objednavky.IdObjednavky == idObjednavky && customer.Faktury.IdFaktury == idFaktury)
			{

				PlatbyCustomerForm payment = new();
				payment.IdFaktury = customer.Faktury.IdFaktury;

				float zbyvaZaplatit = RemainsToPay(idObjednavky, customer, payment);

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
					customerPay.IdFaktury = idFaktury;
					PaymentSQL.AddCustomerPayment(customerPay);
				}
				else
				{
					ViewBag.ErrorInfo = "Některá pole nejsou správně vyplněna!";
					return View("AddPaymentCustomer", payment);
				}
			}

			return RedirectToAction(nameof(OrderController.ListOrders), nameof(Order));
		}

		public IActionResult DownloadInvoice(int idFaktury)
		{
			Faktury faktura = PaymentSQL.GetInvoiceById(idFaktury);
			List<ZboziObjednavek_Zbozi> zbozi = OrderSQL.GetGoodsOrderByIdInvoice(idFaktury);

			var memoryStream = new MemoryStream();
			CreateInvoicePDF(memoryStream, faktura, zbozi);
			memoryStream.Position = 0;

			string fileName = $"Faktura_{faktura.CisloFaktury}.pdf";
			return File(memoryStream, "application/pdf", fileName);
		}

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
