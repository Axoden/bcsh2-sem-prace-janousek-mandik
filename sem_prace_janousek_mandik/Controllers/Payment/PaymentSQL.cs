using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models.Payment;
using System.Data;

namespace sem_prace_janousek_mandik.Controllers.Payment
{
	public class PaymentSQL
	{
		/// <summary>
		/// Metoda zavolá proceduru na přidání nové faktury
		/// </summary>
		/// <param name="invoice">Nová faktura</param>
		/// <returns>true, pokud procedura proběhla v pořádku, jinak false</returns>
		internal static bool AddInvoice(Faktury invoice)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					connection.Open();
					using (OracleCommand command = new("P_VLOZIT_FAKTURU", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_cisloFaktury", OracleDbType.Int32).Value = invoice.CisloFaktury;
						command.Parameters.Add("p_datumVystaveni", OracleDbType.Date).Value = invoice.DatumVystaveni.Value.ToDateTime(TimeOnly.Parse("00:00PM"));
						command.Parameters.Add("p_datumSplatnosti", OracleDbType.Date).Value = invoice.DatumSplatnosti.Value.ToDateTime(TimeOnly.Parse("00:00PM"));
						command.Parameters.Add("p_castkaObjednavka", OracleDbType.BinaryFloat).Value = invoice.CastkaObjednavka;
						command.Parameters.Add("p_castkaDoprava", OracleDbType.BinaryFloat).Value = invoice.CastkaDoprava;
						command.Parameters.Add("p_dph", OracleDbType.BinaryFloat).Value = invoice.Dph;

						command.ExecuteNonQuery();
					}
					connection.Close();
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Zavolá proceduru na přidání nové platby k faktuře
		/// </summary>
		/// <param name="payment">Nová platba</param>
		/// <returns>true, pokud procedura proběhla v pořádku, jinak false</returns>
		internal static bool AddPayment(PlatbyCustomerForm payment)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					connection.Open();
					using (OracleCommand command = new("P_VLOZIT_PLATBU", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_datumPlatby", OracleDbType.Date).Value = DateTime.Now;
						command.Parameters.Add("p_castka", OracleDbType.BinaryFloat).Value = payment.Castka;
						command.Parameters.Add("p_typPlatby", OracleDbType.Varchar2).Value = payment.TypPlatby;
						command.Parameters.Add("p_variabilniSymbol", OracleDbType.Varchar2).Value = payment.VariabilniSymbol;
						command.Parameters.Add("p_idFaktury", OracleDbType.Int32).Value = payment.IdFaktury;

						command.ExecuteNonQuery();
					}
					connection.Close();
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Metoda zavolá proceduru na úpravu faktury
		/// </summary>
		/// <param name="invoice">Model s upravenými daty faktury</param>
		/// <returns>true, pokud procedura proběhla v pořádku, jinak false</returns>
		internal static bool EditInvoice(Faktury invoice)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					connection.Open();
					using (OracleCommand command = new("P_UPRAVIT_FAKTURU", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_idFaktury", OracleDbType.Int32).Value = invoice.IdFaktury;
						command.Parameters.Add("p_cisloFaktury", OracleDbType.Int32).Value = invoice.CisloFaktury;
						command.Parameters.Add("p_datumVystaveni", OracleDbType.Date).Value = invoice.DatumVystaveni.Value.ToDateTime(TimeOnly.Parse("00:00PM"));
						command.Parameters.Add("p_datumSplatnosti", OracleDbType.Date).Value = invoice.DatumSplatnosti.Value.ToDateTime(TimeOnly.Parse("00:00PM"));
						command.Parameters.Add("p_castkaObjednavka", OracleDbType.BinaryFloat).Value = invoice.CastkaObjednavka;
						command.Parameters.Add("p_castkaDoprava", OracleDbType.BinaryFloat).Value = invoice.CastkaDoprava;
						command.Parameters.Add("p_dph", OracleDbType.BinaryFloat).Value = invoice.Dph;

						command.ExecuteNonQuery();
					}
					connection.Close();
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Metoda zavolá proceduru na úpravu platby
		/// </summary>
		/// <param name="payment">Model s upravenými daty platby</param>
		/// <returns>true, pokud procedura proběhla v pořádku, jinak false</returns>
		internal static bool EditPayment(Platba_Faktury payment)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					connection.Open();
					using (OracleCommand command = new("P_UPRAVIT_PLATBU", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_idPlatby", OracleDbType.Int32).Value = payment.Platby.IdPlatby;
						command.Parameters.Add("p_datumPlatby", OracleDbType.Date).Value = payment.Platby.DatumPlatby;
						command.Parameters.Add("p_castka", OracleDbType.BinaryFloat).Value = payment.Platby.Castka;
						command.Parameters.Add("p_typPlatby", OracleDbType.Varchar2).Value = payment.Platby.TypPlatby;
						command.Parameters.Add("p_variabilniSymbol", OracleDbType.Varchar2).Value = payment.Platby.VariabilniSymbol;
						command.Parameters.Add("p_idFaktury", OracleDbType.Int32).Value = payment.Platby.IdFaktury;

						command.ExecuteNonQuery();
					}
					connection.Close();
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Metoda vytáhne všechny faktury
		/// </summary>
		/// <returns>List se všemi fakturami</returns>
		internal static List<Faktury> GetAllInvoices()
		{
			List<Faktury> invoices = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM faktury";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						Faktury? specificInvoice = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								specificInvoice = new();
								specificInvoice.IdFaktury = int.Parse(reader["idFaktury"].ToString());
								specificInvoice.CisloFaktury = int.Parse(reader["cisloFaktury"].ToString());
								specificInvoice.DatumVystaveni = DateOnly.FromDateTime(DateTime.Parse(reader["datumVystaveni"].ToString()));
								specificInvoice.DatumSplatnosti = DateOnly.FromDateTime(DateTime.Parse(reader["datumSplatnosti"].ToString()));
								specificInvoice.CastkaObjednavka = float.Parse(reader["castkaObjednavka"].ToString());
								specificInvoice.CastkaDoprava = float.Parse(reader["castkaDoprava"].ToString());
								specificInvoice.Dph = float.Parse(reader["dph"].ToString());

								invoices.Add(specificInvoice);
							}
						}
						else
						{
							specificInvoice = null;
						}
					}
				}
				connection.Close();
			}
			return invoices;
		}

		/// <summary>
		/// Metoda vytáhne všechny platby
		/// </summary>
		/// <returns>List se všemi platbami</returns>
		internal static List<Platby> GetAllPayments()
		{
			List<Platby> payments = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM platby";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						Platby? specificPayment = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								specificPayment = new();
								specificPayment.IdPlatby = int.Parse(reader["idPlatby"].ToString());
								specificPayment.DatumPlatby = DateTime.Parse(reader["datumPlatby"].ToString());
								specificPayment.Castka = float.Parse(reader["castka"].ToString());
								specificPayment.TypPlatby = char.Parse(reader["typPlatby"].ToString());
								specificPayment.VariabilniSymbol = reader["variabilniSymbol"].ToString();
								specificPayment.IdFaktury = int.Parse(reader["idFaktury"].ToString());

								payments.Add(specificPayment);
							}
						}
						else
						{
							specificPayment = null;
						}
					}
				}
				connection.Close();
			}
			return payments;
		}

		/// <summary>
		/// Metoda vytáhne konkrétní fakturu na základě ID faktury
		/// </summary>
		/// <param name="idInvoice">ID faktury</param>
		/// <returns>Model konkrétní faktury</returns>
		internal static Faktury GetInvoiceById(int idInvoice)
		{
			Faktury specificInvoice = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM faktury WHERE idFaktury = :idFaktury";
					command.Parameters.Add(":idFaktury", OracleDbType.Int32).Value = idInvoice;
					using (OracleDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								specificInvoice.IdFaktury = int.Parse(reader["idFaktury"].ToString());
								specificInvoice.CisloFaktury = int.Parse(reader["cisloFaktury"].ToString());
								specificInvoice.DatumVystaveni = DateOnly.FromDateTime(DateTime.Parse(reader["datumVystaveni"].ToString()));
								specificInvoice.DatumSplatnosti = DateOnly.FromDateTime(DateTime.Parse(reader["datumSplatnosti"].ToString()));
								specificInvoice.CastkaObjednavka = float.Parse(reader["castkaObjednavka"].ToString());
								specificInvoice.CastkaDoprava = float.Parse(reader["castkaDoprava"].ToString());
								specificInvoice.Dph = float.Parse(reader["dph"].ToString());
							}
						}
					}
				}
				connection.Close();
			}
			return specificInvoice;
		}

		/// <summary>
		/// Metoda vytáhne platbu na základě ID platby
		/// </summary>
		/// <param name="idPayment">ID platby</param>
		/// <returns>Model konkrétní platby</returns>
		internal static PlatbyForm GetPaymentById(int idPayment)
		{
			PlatbyForm specificPayment = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM platby WHERE idPlatby = :idPlatby";
					command.Parameters.Add(":idPlatby", OracleDbType.Int32).Value = idPayment;
					using (OracleDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								specificPayment.IdPlatby = int.Parse(reader["idPlatby"].ToString());
								specificPayment.DatumPlatby = DateTime.Parse(reader["datumPlatby"].ToString());
								specificPayment.Castka = float.Parse(reader["castka"].ToString());
								specificPayment.TypPlatby = reader["typPlatby"].ToString();
								specificPayment.VariabilniSymbol = reader["variabilniSymbol"].ToString();
								specificPayment.IdFaktury = int.Parse(reader["idFaktury"].ToString());
							}
						}
					}
				}
				connection.Close();
			}
			return specificPayment;
		}

		/// <summary>
		/// Metoda vytáhne všechny platby konkrétní faktury
		/// </summary>
		/// <param name="idInvoice">ID faktury</param>
		/// <returns>List všech plateb konkrétní faktury</returns>
		internal static List<Platby> GetAllPaymentsByInvoiceId(int idInvoice)
		{
			List<Platby> payments = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM platby WHERE idFaktury = :idInvoice";
					command.Parameters.Add(":idInvoice", OracleDbType.Int32).Value = idInvoice;
					using (OracleDataReader reader = command.ExecuteReader())
					{
						Platby? specificPayment = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								specificPayment = new();
								specificPayment.IdPlatby = int.Parse(reader["idPlatby"].ToString());
								specificPayment.DatumPlatby = DateTime.Parse(reader["datumPlatby"].ToString());
								specificPayment.Castka = float.Parse(reader["castka"].ToString());
								specificPayment.TypPlatby = char.Parse(reader["typPlatby"].ToString());
								specificPayment.VariabilniSymbol = reader["variabilniSymbol"].ToString();
								specificPayment.IdFaktury = int.Parse(reader["idFaktury"].ToString());

								payments.Add(specificPayment);
							}
						}
						else
						{
							specificPayment = null;
						}
					}
				}
				connection.Close();
			}
			return payments;
		}

		/// <summary>
		/// Metoda zavolá proceduru na přidání platby zaplacenou zákazníkem
		/// </summary>
		/// <param name="payment">Model s daty platby</param>
        internal static void AddCustomerPayment(PlatbyCustomerForm payment)
        {
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new("P_ZAPLAT_OBJEDNAVKU", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("p_idFaktury", OracleDbType.Int32).Value = payment.IdFaktury;
                    command.Parameters.Add("p_typPlatby", OracleDbType.Char).Value = payment.TypPlatby;
                    command.Parameters.Add("p_castka", OracleDbType.BinaryFloat).Value = payment.Castka;
                    command.Parameters.Add("p_variabilniSymbol", OracleDbType.Varchar2).Value = payment.VariabilniSymbol;

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }
	}
}
