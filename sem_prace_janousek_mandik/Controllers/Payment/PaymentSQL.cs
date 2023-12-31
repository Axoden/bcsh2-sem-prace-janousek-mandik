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
		internal static async Task<bool> AddInvoice(Faktury invoice)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_insert.P_VLOZIT_FAKTURU", connection))
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
		internal static async Task<bool> AddPayment(PlatbyCustomerForm payment)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_insert.P_VLOZIT_PLATBU", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_datumPlatby", OracleDbType.Date).Value = DateTime.Now;
						command.Parameters.Add("p_castka", OracleDbType.Decimal).Value = payment.Castka;
						command.Parameters.Add("p_typPlatby", OracleDbType.Varchar2).Value = payment.TypPlatby;
						command.Parameters.Add("p_variabilniSymbol", OracleDbType.Varchar2).Value = payment.VariabilniSymbol;
						command.Parameters.Add("p_idFaktury", OracleDbType.Int32).Value = payment.IdFaktury;

						command.ExecuteNonQuery();
					}
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
		internal static async Task<bool> EditInvoice(Faktury invoice)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_edit.P_UPRAVIT_FAKTURU", connection))
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
		internal static async Task<bool> EditPayment(Platba_Faktury payment)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_edit.P_UPRAVIT_PLATBU", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_idPlatby", OracleDbType.Int32).Value = payment.Platby?.IdPlatby;
						command.Parameters.Add("p_datumPlatby", OracleDbType.Date).Value = payment.Platby?.DatumPlatby;
						command.Parameters.Add("p_castka", OracleDbType.BinaryFloat).Value = payment.Platby?.Castka;
						command.Parameters.Add("p_typPlatby", OracleDbType.Varchar2).Value = payment.Platby?.TypPlatby;
						command.Parameters.Add("p_variabilniSymbol", OracleDbType.Varchar2).Value = payment.Platby?.VariabilniSymbol;
						command.Parameters.Add("p_idFaktury", OracleDbType.Int32).Value = payment.Platby?.IdFaktury;

						command.ExecuteNonQuery();
					}
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
		internal static async Task<List<Faktury>> GetAllInvoices()
		{
			List<Faktury> invoices = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM faktury ORDER BY cisloFaktury DESC";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Faktury? specificInvoice = new();

								specificInvoice.IdFaktury = reader.GetInt32(reader.GetOrdinal("idFaktury"));
								specificInvoice.CisloFaktury = reader.GetInt32(reader.GetOrdinal("cisloFaktury"));
								specificInvoice.DatumVystaveni = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("datumVystaveni")));
								specificInvoice.DatumSplatnosti = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("datumSplatnosti")));
								specificInvoice.CastkaObjednavka = reader.GetFloat(reader.GetOrdinal("castkaObjednavka"));
								specificInvoice.CastkaDoprava = reader.GetFloat(reader.GetOrdinal("castkaDoprava"));
								specificInvoice.Dph = reader.GetFloat(reader.GetOrdinal("dph"));

								invoices.Add(specificInvoice);
							}
						}
					}
				}
			}
			return invoices;
		}

		/// <summary>
		/// Metoda vytáhne všechny platby
		/// </summary>
		/// <returns>List se všemi platbami</returns>
		internal static async Task<List<Platby>> GetAllPayments()
		{
			List<Platby> payments = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM platby";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Platby? specificPayment = new();

								specificPayment.IdPlatby = reader.GetInt32(reader.GetOrdinal("idPlatby"));
								specificPayment.DatumPlatby = reader.GetDateTime(reader.GetOrdinal("datumPlatby"));
								specificPayment.Castka = reader.GetFloat(reader.GetOrdinal("castka"));
								specificPayment.TypPlatby = reader["typPlatby"].ToString()?[0];
								specificPayment.VariabilniSymbol = reader["variabilniSymbol"].ToString();
								specificPayment.IdFaktury = reader.GetInt32(reader.GetOrdinal("idFaktury"));

								payments.Add(specificPayment);
							}
						}
					}
				}
			}
			return payments;
		}

		/// <summary>
		/// Metoda vytáhne konkrétní fakturu na základě ID faktury
		/// </summary>
		/// <param name="idInvoice">ID faktury</param>
		/// <returns>Model konkrétní faktury</returns>
		internal static async Task<Faktury> GetInvoiceById(int idInvoice)
		{
			Faktury specificInvoice = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM faktury WHERE idFaktury = :idFaktury";
					command.Parameters.Add(":idFaktury", OracleDbType.Int32).Value = idInvoice;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								specificInvoice.IdFaktury = reader.GetInt32(reader.GetOrdinal("idFaktury"));
								specificInvoice.CisloFaktury = reader.GetInt32(reader.GetOrdinal("cisloFaktury"));
								specificInvoice.DatumVystaveni = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("datumVystaveni")));
								specificInvoice.DatumSplatnosti = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("datumSplatnosti")));
								specificInvoice.CastkaObjednavka = reader.GetFloat(reader.GetOrdinal("castkaObjednavka"));
								specificInvoice.CastkaDoprava = reader.GetFloat(reader.GetOrdinal("castkaDoprava"));
								specificInvoice.Dph = reader.GetFloat(reader.GetOrdinal("dph"));
							}
						}
					}
				}
			}
			return specificInvoice;
		}

		/// <summary>
		/// Metoda vytáhne platbu na základě ID platby
		/// </summary>
		/// <param name="idPayment">ID platby</param>
		/// <returns>Model konkrétní platby</returns>
		internal static async Task<PlatbyForm> GetPaymentById(int idPayment)
		{
			PlatbyForm specificPayment = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM platby WHERE idPlatby = :idPlatby";
					command.Parameters.Add(":idPlatby", OracleDbType.Int32).Value = idPayment;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								specificPayment.IdPlatby = reader.GetInt32(reader.GetOrdinal("idPlatby"));
								specificPayment.DatumPlatby = reader.GetDateTime(reader.GetOrdinal("datumPlatby"));
								specificPayment.Castka = reader.GetFloat(reader.GetOrdinal("castka"));
								specificPayment.TypPlatby = reader["typPlatby"].ToString();
								specificPayment.VariabilniSymbol = reader["variabilniSymbol"].ToString();
								specificPayment.IdFaktury = reader.GetInt32(reader.GetOrdinal("idFaktury"));
							}
						}
					}
				}
			}
			return specificPayment;
		}

		/// <summary>
		/// Metoda vytáhne všechny platby konkrétní faktury
		/// </summary>
		/// <param name="idInvoice">ID faktury</param>
		/// <returns>List všech plateb konkrétní faktury</returns>
		internal static async Task<List<Platby>> GetAllPaymentsByInvoiceId(int idInvoice)
		{
			List<Platby> payments = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM platby WHERE idFaktury = :idInvoice";
					command.Parameters.Add(":idInvoice", OracleDbType.Int32).Value = idInvoice;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Platby? specificPayment = new();

								specificPayment.IdPlatby = reader.GetInt32(reader.GetOrdinal("idPlatby"));
								specificPayment.DatumPlatby = reader.GetDateTime(reader.GetOrdinal("datumPlatby"));
								specificPayment.Castka = reader.GetFloat(reader.GetOrdinal("castka"));
								specificPayment.TypPlatby = char.Parse(reader["typPlatby"].ToString());
								specificPayment.VariabilniSymbol = reader["variabilniSymbol"].ToString();
								specificPayment.IdFaktury = reader.GetInt32(reader.GetOrdinal("idFaktury"));

								payments.Add(specificPayment);
							}
						}
					}
				}
			}
			return payments;
		}

		/// <summary>
		/// Metoda zavolá proceduru na přidání platby zaplacenou zákazníkem
		/// </summary>
		/// <param name="payment">Model s daty platby</param>
		internal static async Task<bool> AddCustomerPayment(PlatbyCustomerForm payment)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("P_ZAPLAT_OBJEDNAVKU", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_idFaktury", OracleDbType.Int32).Value = payment.IdFaktury;
						command.Parameters.Add("p_typPlatby", OracleDbType.Char).Value = payment.TypPlatby;
						command.Parameters.Add("p_castka", OracleDbType.Decimal).Value = payment.Castka;
						command.Parameters.Add("p_variabilniSymbol", OracleDbType.Varchar2).Value = payment.VariabilniSymbol;

						command.ExecuteNonQuery();
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
