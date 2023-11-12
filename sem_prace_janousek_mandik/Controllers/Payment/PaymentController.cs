using Microsoft.AspNetCore.Mvc;
using sem_prace_janousek_mandik.Controllers.Order;
using sem_prace_janousek_mandik.Models.Order;
using sem_prace_janousek_mandik.Models.Payment;

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
		public IActionResult AddPaymentCustomerGet(int index)
		{
			Objednavky_Zakaznici_Faktury customer = OrderSQL.GetCustomerOrderInvoice(index);

			if (customer.Zakaznici.Email.Equals(Email))
			{
				PlatbyCustomerForm payment = new();
				payment.IdFaktury = customer.Faktury.IdFaktury;
				// spocitat celkovou castku
				return View("AddPaymentCustomer", payment);
			}

			return RedirectToAction(nameof(ListInvoices), nameof(Payment));
		}

        // Zpracování formuláře platby faktury zákazníkem
        /*[HttpPost]
        public IActionResult AddPaymentCustomerPost(int idFaktury)
        {
            Objednavky_Zakaznici_Faktury customer = OrderSQL.GetCustomerOrderInvoice(index);

            if (customer.Zakaznici.Email.Equals(Email))
            {
                PlatbyCustomerForm payment = new();
                payment.IdFaktury = customer.Faktury.IdFaktury;
                // spocitat celkovou castku
                return View("AddPaymentCustomer", payment);
            }

            return RedirectToAction(nameof(ListInvoices), nameof(Payment));
        }*/
    }
}
