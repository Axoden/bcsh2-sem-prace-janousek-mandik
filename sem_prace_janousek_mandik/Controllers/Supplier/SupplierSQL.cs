using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models.Supplier;
using System.Data;

namespace sem_prace_janousek_mandik.Controllers.Supplier
{
	public static class SupplierSQL
	{
		/// <summary>
		/// Metoda vytáhne všechny dodavatele včetně adres
		/// </summary>
		/// <returns>List všech dodavatelů</returns>
		internal static async Task<List<Dodavatele_Adresy>> GetAllSuppliers()
		{
			List<Dodavatele_Adresy> dodavateleAdresy = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM dodavatele d INNER JOIN adresy a ON d.idAdresy = a.idAdresy ORDER BY nazev";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Dodavatele_Adresy? dodavatel = new();
								dodavatel.Dodavatele = new();
								dodavatel.Adresy = new();

								dodavatel.Dodavatele.IdDodavatele = reader.GetInt32(reader.GetOrdinal("idDodavatele"));
								dodavatel.Dodavatele.Nazev = reader["nazev"].ToString();
								dodavatel.Dodavatele.Jmeno = reader["jmeno"].ToString();
								dodavatel.Dodavatele.Prijmeni = reader["prijmeni"].ToString();
								dodavatel.Dodavatele.Telefon = reader["telefon"].ToString();
								dodavatel.Dodavatele.Email = reader["email"].ToString();
								dodavatel.Dodavatele.IdAdresy = reader.GetInt32(reader.GetOrdinal("idAdresy"));

								dodavatel.Adresy.Ulice = reader["ulice"].ToString();
								dodavatel.Adresy.Mesto = reader["mesto"].ToString();
								dodavatel.Adresy.Okres = reader["okres"].ToString();
								dodavatel.Adresy.Zeme = reader["zeme"].ToString();
								dodavatel.Adresy.Psc = reader["psc"].ToString();

								dodavateleAdresy.Add(dodavatel);
							}
						}
					}
				}
			}
			return dodavateleAdresy;
		}

		/// <summary>
		/// Metoda vytáhne ID a jméno všech dodavatelů
		/// </summary>
		/// <returns>List ID a názvů dodavatelů</returns>
		internal static async Task<List<Dodavatele>> GetAllSuppliersName()
		{
			List<Dodavatele> suppliers = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT idDodavatele, nazev FROM dodavatele";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Dodavatele? specificSupplier = new();

								specificSupplier.IdDodavatele = reader.GetInt32(reader.GetOrdinal("idDodavatele"));
								specificSupplier.Nazev = reader["nazev"].ToString();

								suppliers.Add(specificSupplier);
							}
						}
					}
				}
			}
			return suppliers;
		}

		/// <summary>
		/// Zavolá proceduru na úpravu dodavatele
		/// </summary>
		/// <param name="supplier">Model s upravenými daty dodavatele</param>
		/// <returns>Chybovou hlášku, pokud není, tak null</returns>
		internal static async Task<string?> EditSupplier(Dodavatele_Adresy supplier)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_edit.P_UPRAVIT_DODAVATELE_V2", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_iddodavatele", OracleDbType.Int32).Value = supplier.Dodavatele?.IdDodavatele;
						command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = supplier.Dodavatele?.Nazev;
						command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = supplier.Dodavatele?.Jmeno;
						command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = supplier.Dodavatele?.Prijmeni;
						command.Parameters.Add("p_telefon", OracleDbType.Varchar2).Value = supplier.Dodavatele?.Telefon;
						command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = supplier.Dodavatele?.Email;
						command.Parameters.Add("p_idadresy", OracleDbType.Int32).Value = supplier.Dodavatele?.IdAdresy;
						command.Parameters.Add("p_ulice", OracleDbType.Varchar2).Value = supplier.Adresy?.Ulice;
						command.Parameters.Add("p_mesto", OracleDbType.Varchar2).Value = supplier.Adresy?.Mesto;
						command.Parameters.Add("p_okres", OracleDbType.Varchar2).Value = supplier.Adresy?.Okres;
						command.Parameters.Add("p_zeme", OracleDbType.Varchar2).Value = supplier.Adresy?.Zeme;
						command.Parameters.Add("p_psc", OracleDbType.Char).Value = supplier.Adresy?.Psc;

						command.ExecuteNonQuery();
					}
				}
				return null;
			}
			catch (OracleException ex)
			{
				if (ex.Number == 20003)
				{
					string fullMessage = ex.Message;
					string firstLine = fullMessage.Split('\n')[0];
					return firstLine;
				}
				else if (ex.Number == 20004)
				{
					string fullMessage = ex.Message;
					string firstLine = fullMessage.Split('\n')[0];
					return firstLine;
				}
				else
				{
					return ex.Message;
				}
			}
		}

		/// <summary>
		/// Vytáhne dodavatele včetně adresy
		/// </summary>
		/// <param name="idDodavatele">ID dodavatele</param>
		/// <returns>Model konkrétního dodavatele</returns>
		internal static async Task<Dodavatele_Adresy> GetSupplierWithAddress(int idDodavatele)
		{
			Dodavatele_Adresy? dodavatelAdresa = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM dodavatele d INNER JOIN adresy a ON d.idadresy = a.idadresy WHERE idDodavatele = :idDodavatele";
					command.Parameters.Add(":idZamestnance", idDodavatele);
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								dodavatelAdresa = new();
								dodavatelAdresa.Dodavatele = new();
								dodavatelAdresa.Adresy = new();

								dodavatelAdresa.Dodavatele.IdDodavatele = reader.GetInt32(reader.GetOrdinal("idDodavatele"));
								dodavatelAdresa.Dodavatele.Nazev = reader["nazev"].ToString();
								dodavatelAdresa.Dodavatele.Jmeno = reader["jmeno"].ToString();
								dodavatelAdresa.Dodavatele.Prijmeni = reader["prijmeni"].ToString();
								dodavatelAdresa.Dodavatele.Telefon = reader["telefon"].ToString();
								dodavatelAdresa.Dodavatele.Email = reader["email"].ToString();
								dodavatelAdresa.Dodavatele.IdAdresy = reader.GetInt32(reader.GetOrdinal("idAdresy"));

								dodavatelAdresa.Adresy.IdAdresy = reader.GetInt32(reader.GetOrdinal("idAdresy"));
								dodavatelAdresa.Adresy.Ulice = reader["ulice"].ToString();
								dodavatelAdresa.Adresy.Mesto = reader["mesto"].ToString();
								dodavatelAdresa.Adresy.Okres = reader["okres"].ToString();
								dodavatelAdresa.Adresy.Zeme = reader["zeme"].ToString();
								dodavatelAdresa.Adresy.Psc = reader["psc"].ToString();
							}
						}
					}
				}
			}
			return dodavatelAdresa;
		}

		/// <summary>
		/// Zavolání procedury na přidání dodavatele
		/// </summary>
		/// <param name="newSupplier">Model s daty nového dodavatele</param>
		/// <returns>Chybovou hlášku, pokud není, tak null</returns>
		internal static async Task<string?> AddSupplier(Dodavatele_Adresy newSupplier)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_insert.P_VLOZIT_DODAVATELE_V2", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = newSupplier.Dodavatele?.Nazev;
						command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = newSupplier.Dodavatele?.Jmeno;
						command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = newSupplier.Dodavatele?.Prijmeni;
						command.Parameters.Add("p_telefon", OracleDbType.Varchar2).Value = newSupplier.Dodavatele?.Telefon;
						command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = newSupplier.Dodavatele?.Email;
						command.Parameters.Add("p_ulice", OracleDbType.Varchar2).Value = newSupplier.Adresy?.Ulice;
						command.Parameters.Add("p_mesto", OracleDbType.Varchar2).Value = newSupplier.Adresy?.Mesto;
						command.Parameters.Add("p_okres", OracleDbType.Varchar2).Value = newSupplier.Adresy?.Okres;
						command.Parameters.Add("p_zeme", OracleDbType.Varchar2).Value = newSupplier.Adresy?.Zeme;
						command.Parameters.Add("p_psc", OracleDbType.Char).Value = newSupplier.Adresy?.Psc;

						command.ExecuteNonQuery();
					}
				}
				return null;
			}
			catch (OracleException ex)
			{
				if (ex.Number == 20003)
				{
					string fullMessage = ex.Message;
					string firstLine = fullMessage.Split('\n')[0];
					return firstLine;
				}
				else if (ex.Number == 20004)
				{
					string fullMessage = ex.Message;
					string firstLine = fullMessage.Split('\n')[0];
					return firstLine;
				}
				else
				{
					return ex.Message;
				}
			}
		}
	}
}