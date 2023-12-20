using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models.Employee;
using sem_prace_janousek_mandik.Models.Management;
using System.Data;

namespace sem_prace_janousek_mandik.Controllers.Employee
{
    public class EmployeeSQL
	{
		/// <summary>
		/// Metoda vytáhne všechny zaměstnance
		/// </summary>
		/// <returns>List všech zaměstnanců</returns>
		internal static async Task<List<Zamestnanci_Adresy_Pozice>> GetAllEmployeesWithAddressPosition()
		{
			List<Zamestnanci_Adresy_Pozice> zamestnanci = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM zamestnanci z" +
						" INNER JOIN adresy a ON z.idAdresy = a.idAdresy" +
						" INNER JOIN pozice p ON z.idPozice = p.idPozice ORDER BY prijmeni";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Zamestnanci_Adresy_Pozice? zamestnanec = new();
								zamestnanec.Zamestnanci = new();
								zamestnanec.Adresy = new();
								zamestnanec.Pozice = new();

								zamestnanec.Zamestnanci.IdZamestnance = reader.GetInt32(reader.GetOrdinal("idZamestnance"));
								zamestnanec.Zamestnanci.Jmeno = reader["jmeno"].ToString();
								zamestnanec.Zamestnanci.Prijmeni = reader["prijmeni"].ToString();
								zamestnanec.Zamestnanci.DatumNarozeni = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("datumNarozeni")));
								zamestnanec.Zamestnanci.Telefon = reader["telefon"].ToString();
								zamestnanec.Zamestnanci.Email = reader["email"].ToString();
								zamestnanec.Zamestnanci.Heslo = reader["heslo"].ToString();
								zamestnanec.Zamestnanci.IdAdresy = reader.GetInt32(reader.GetOrdinal("idAdresy"));
								zamestnanec.Zamestnanci.IdPozice = reader.GetInt32(reader.GetOrdinal("idPozice"));

								zamestnanec.Adresy.IdAdresy = reader.GetInt32(reader.GetOrdinal("idAdresy"));
								zamestnanec.Adresy.Ulice = reader["ulice"].ToString();
								zamestnanec.Adresy.Mesto = reader["mesto"].ToString();
								zamestnanec.Adresy.Okres = reader["okres"].ToString();
								zamestnanec.Adresy.Zeme = reader["zeme"].ToString();
								zamestnanec.Adresy.Psc = reader["psc"].ToString();

								zamestnanec.Pozice.IdPozice = reader.GetInt32(reader.GetOrdinal("idPozice"));
								zamestnanec.Pozice.Nazev = reader["nazev"].ToString();

								zamestnanci.Add(zamestnanec);
							}
						}
					}
				}
			}
			return zamestnanci;
		}

		/// <summary>
		/// Metoda vytáhne zaměstnance s jeho adresou a pozicí dle id
		/// </summary>
		/// <param name="idEmployee">ID zaměstnance</param>
		/// <returns>Model konkrétního zaměstnance</returns>
		internal static async Task<Zamestnanci_Adresy_Pozice> GetEmployeeWithAddressPosition(int idEmployee)
		{
			Zamestnanci_Adresy_Pozice? zamestnanecAdresa = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM zamestnanci z INNER JOIN adresy a ON z.idadresy = a.idadresy INNER JOIN pozice p ON p.idpozice = z.idpozice WHERE idZamestnance = :idZamestnance";
					command.Parameters.Add(":idZamestnance", idEmployee);
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								zamestnanecAdresa = new();
								zamestnanecAdresa.Zamestnanci = new();
								zamestnanecAdresa.Adresy = new();
								zamestnanecAdresa.Pozice = new();

								zamestnanecAdresa.Zamestnanci.IdZamestnance = reader.GetInt32(reader.GetOrdinal("idZamestnance"));
								zamestnanecAdresa.Zamestnanci.Jmeno = reader["jmeno"].ToString();
								zamestnanecAdresa.Zamestnanci.Prijmeni = reader["prijmeni"].ToString();
								zamestnanecAdresa.Zamestnanci.DatumNarozeni = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("datumNarozeni")));
								zamestnanecAdresa.Zamestnanci.Telefon = reader["telefon"].ToString();
								zamestnanecAdresa.Zamestnanci.Email = reader["email"].ToString();
								zamestnanecAdresa.Zamestnanci.IdAdresy = reader.GetInt32(reader.GetOrdinal("idAdresy"));
								zamestnanecAdresa.Zamestnanci.IdPozice = reader.GetInt32(reader.GetOrdinal("idPozice"));

								zamestnanecAdresa.Adresy.IdAdresy = reader.GetInt32(reader.GetOrdinal("idAdresy"));
								zamestnanecAdresa.Adresy.Ulice = reader["ulice"].ToString();
								zamestnanecAdresa.Adresy.Mesto = reader["mesto"].ToString();
								zamestnanecAdresa.Adresy.Okres = reader["okres"].ToString();
								zamestnanecAdresa.Adresy.Zeme = reader["zeme"].ToString();
								zamestnanecAdresa.Adresy.Psc = reader["psc"].ToString();

								zamestnanecAdresa.Pozice.IdPozice = reader.GetInt32(reader.GetOrdinal("idPozice"));
								zamestnanecAdresa.Pozice.Nazev = reader["nazev"].ToString();
							}
						}
					}
				}
			}
			return zamestnanecAdresa;
		}

		/// <summary>
		/// Zavolá proceduru na úpravu zaměstnance
		/// </summary>
		/// <param name="employee">Model s daty upraveného zaměstnance</param>
		/// <returns>Chybovou hlášku, pokud není, tak null</returns>
		internal static async Task<string?> EditEmployee(Zamestnanci_Adresy_PoziceList employee)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_edit.P_UPRAVIT_ZAM_V2", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_idzamestnance", OracleDbType.Int32).Value = employee.Zamestnanci?.IdZamestnance;
						command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = employee.Zamestnanci?.Jmeno;
						command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = employee.Zamestnanci?.Prijmeni;
						if (employee.Zamestnanci?.DatumNarozeni == null)
						{
							command.Parameters.Add("p_datumnarozeni", OracleDbType.Date).Value = null;
						}
						else
						{
							command.Parameters.Add("p_datumnarozeni", OracleDbType.Date).Value = employee.Zamestnanci.DatumNarozeni.Value.ToDateTime(TimeOnly.Parse("00:00PM"));
						}
						command.Parameters.Add("p_telefon", OracleDbType.Varchar2).Value = employee.Zamestnanci?.Telefon;
						command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = employee.Zamestnanci?.Email;
						command.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = employee.Zamestnanci?.Heslo;
						command.Parameters.Add("p_idadresy", OracleDbType.Int32).Value = employee.Zamestnanci?.IdAdresy;
						command.Parameters.Add("p_idpozice", OracleDbType.Int32).Value = employee.Zamestnanci?.IdPozice;
						if (employee.Adresy == null)
						{
							employee.Adresy = new();
						}
						command.Parameters.Add("p_ulice", OracleDbType.Varchar2).Value = employee.Adresy.Ulice;
						command.Parameters.Add("p_mesto", OracleDbType.Varchar2).Value = employee.Adresy.Mesto;
						command.Parameters.Add("p_okres", OracleDbType.Varchar2).Value = employee.Adresy.Okres;
						command.Parameters.Add("p_zeme", OracleDbType.Varchar2).Value = employee.Adresy.Zeme;
						command.Parameters.Add("p_psc", OracleDbType.Char).Value = employee.Adresy.Psc;

						command.ExecuteNonQuery();
					}
				}
				return null;
			}
			catch (OracleException ex)
			{
				if (ex.Number == 20001)
				{
					string fullMessage = ex.Message;
					string firstLine = fullMessage.Split('\n')[0];
					return firstLine;
				}
				else if (ex.Number == 20001)
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
		/// Zavolání procedury na registraci zaměstnance
		/// </summary>
		/// <param name="newEmployee">Model s daty nového zaměstnance</param>
		/// <returns>Chybovou hlášku, pokud není, tak null</returns>
		internal static async Task<string?> RegisterEmployee(Zamestnanci_Adresy_PoziceList newEmployee)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_insert.P_VLOZIT_ZAMESTNANCE_V2", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = newEmployee.Zamestnanci.Jmeno;
						command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = newEmployee.Zamestnanci.Prijmeni;
						command.Parameters.Add("p_datumnarozeni", OracleDbType.Date).Value = newEmployee.Zamestnanci.DatumNarozeni.Value.ToDateTime(TimeOnly.Parse("00:00PM"));
						command.Parameters.Add("p_telefon", OracleDbType.Varchar2).Value = newEmployee.Zamestnanci.Telefon;
						command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = newEmployee.Zamestnanci.Email;
						command.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = newEmployee.Zamestnanci.Heslo;
						command.Parameters.Add("p_idpozice", OracleDbType.Int32).Value = newEmployee.Zamestnanci.IdPozice;
						command.Parameters.Add("p_ulice", OracleDbType.Varchar2).Value = newEmployee.Adresy.Ulice;
						command.Parameters.Add("p_mesto", OracleDbType.Varchar2).Value = newEmployee.Adresy.Mesto;
						command.Parameters.Add("p_okres", OracleDbType.Varchar2).Value = newEmployee.Adresy.Okres;
						command.Parameters.Add("p_zeme", OracleDbType.Varchar2).Value = newEmployee.Adresy.Zeme;
						command.Parameters.Add("p_psc", OracleDbType.Char).Value = newEmployee.Adresy.Psc;

						command.ExecuteNonQuery();
					}
				}
				return null;
			}
			catch (OracleException ex)
			{
				if (ex.Number == 20001)
				{
					string fullMessage = ex.Message;
					string firstLine = fullMessage.Split('\n')[0];
					return firstLine;
				}
				else if (ex.Number == 20002)
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
		/// Metoda ověří zaměstnance a vrátí celý objekt Zamestnanec
		/// </summary>
		/// <param name="email">Email zaměstnance</param>
		/// <returns>Model zaměstnance</returns>
		internal static async Task<Zamestnanci?> AuthEmployee(string email)
		{
			Zamestnanci? zamestnanec = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM zamestnanci WHERE email = :email";
					command.Parameters.Add(":email", email);
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								zamestnanec.IdZamestnance = reader.GetInt32(reader.GetOrdinal("idZamestnance"));
								zamestnanec.Jmeno = reader["jmeno"].ToString();
								zamestnanec.Prijmeni = reader["prijmeni"].ToString();
								zamestnanec.DatumNarozeni = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("datumNarozeni")));
								zamestnanec.Telefon = reader["telefon"].ToString();
								zamestnanec.Email = reader["email"].ToString();
								zamestnanec.Heslo = reader["heslo"].ToString();
								zamestnanec.IdAdresy = reader.GetInt32(reader.GetOrdinal("idAdresy"));
								zamestnanec.IdPozice = reader.GetInt32(reader.GetOrdinal("idPozice"));
							}
						}
					}
				}
			}
			return zamestnanec;
		}

		/// <summary>
		/// Metoda získá pracovní pozici a vrátí celý objekt Pozice
		/// </summary>
		/// <param name="idPosition">ID pozice</param>
		/// <returns>Model pozice</returns>
		internal static async Task<Pozice?> GetPosition(int idPosition)
		{
			Pozice? pozice = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM pozice WHERE idPozice = :id";
					command.Parameters.Add(":id", idPosition);
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								pozice.IdPozice = reader.GetInt32(reader.GetOrdinal("idPozice"));
								pozice.Nazev = reader["nazev"].ToString();
							}
						}
					}
				}
			}
			return pozice;
		}

		/// <summary>
		/// Metoda získá ID, jména a příjmení všech zaměstnanců
		/// </summary>
		/// <returns>List všech zaměstnanců</returns>
		internal static async Task<List<Zamestnanci>> GetAllEmployeesNameSurname()
		{
			List<Zamestnanci> zamestnanci = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT idZamestnance, jmeno, prijmeni FROM zamestnanci ORDER BY prijmeni";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								Zamestnanci? zamestnanec = new();

								zamestnanec.IdZamestnance = reader.GetInt32(reader.GetOrdinal("idZamestnance"));
								zamestnanec.Jmeno = reader["jmeno"].ToString();
								zamestnanec.Prijmeni = reader["prijmeni"].ToString();
								zamestnanci.Add(zamestnanec);
							}
						}
					}
				}
			}
			return zamestnanci;
		}

		/// <summary>
		/// Metoda získá ID zaměstnance na základě jeho emailu
		/// </summary>
		/// <param name="email">Email zaměstnance</param>
		/// <returns>ID zaměstnance</returns>
		internal static async Task<int> GetEmployeeIdByEmail(string email)
		{
			int idZamestnance = 0;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT idZamestnance FROM zamestnanci WHERE email = :email";
					command.Parameters.Add(":email", email);
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								idZamestnance = reader.GetInt32(reader.GetOrdinal("idZamestnance"));
							}
						}
					}
				}
			}
			return idZamestnance;
		}

		/// <summary>
		/// Metoda získá email a roli zaměstnance na základě jeho id
		/// </summary>
		/// <param name="idEmployee">ID zaměstnance</param>
		/// <returns>Model zaměstnance s emailem a názvem pozice</returns>
		internal static async Task<Zamestnanci_Pozice> GetEmployeeRoleEmailById(int idEmployee)
		{
			Zamestnanci_Pozice employee = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT z.email, p.nazev FROM zamestnanci z INNER JOIN pozice p ON z.idPozice = p.idPozice WHERE idZamestnance = :idZamestnance";
					command.Parameters.Add(":idZamestnance", idEmployee);
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								employee.Zamestnanci = new();
								employee.Pozice = new();

								employee.Zamestnanci.Email = reader["email"].ToString();
								employee.Pozice.Nazev = reader["nazev"].ToString();
							}
						}
					}
				}
			}
			return employee;
		}
	}
}