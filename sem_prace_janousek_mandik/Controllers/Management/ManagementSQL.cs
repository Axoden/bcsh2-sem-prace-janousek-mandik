using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models;
using System.Data;

namespace sem_prace_janousek_mandik.Controllers.Management
{
	public static class ManagementSQL
	{
		// Metoda vytáhne všechny zaměstnance
		public static List<Zamestnanci> GetAllEmployees()
		{
			Zamestnanci? zamestnanec = new Zamestnanci();
			List<Zamestnanci> zamestnanci = new List<Zamestnanci>();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM zamestnanci";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								zamestnanec = new Zamestnanci();
								zamestnanec.IdZamestnance = int.Parse(reader["idZamestnance"].ToString());
								zamestnanec.Jmeno = reader["jmeno"].ToString();
								zamestnanec.Prijmeni = reader["prijmeni"].ToString();
								zamestnanec.DatumNarozeni = DateOnly.FromDateTime(DateTime.Parse(reader["datumNarozeni"].ToString()));
								zamestnanec.Telefon = reader["telefon"].ToString();
								zamestnanec.Email = reader["email"].ToString();
								zamestnanec.Heslo = reader["heslo"].ToString();
								zamestnanec.IdAdresy = int.Parse(reader["idAdresy"].ToString());
								zamestnanec.IdPozice = int.Parse(reader["idPozice"].ToString());

								zamestnanci.Add(zamestnanec);
							}
						}
						else
						{
							zamestnanec = null;
						}
					}
				}
				connection.Close();
			}
			return zamestnanci;
		}

		// Metoda vytáhne všechny adresy
		public static List<Adresy> GetAllAddresses()
		{
			Adresy? adresa = new();
			List<Adresy> adresy = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM adresy";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								adresa = new Adresy();
								adresa.IdAdresy = int.Parse(reader["idAdresy"].ToString());
								adresa.Ulice = reader["ulice"].ToString();
								adresa.Mesto = reader["mesto"].ToString();
								adresa.Okres = reader["okres"].ToString();
								adresa.Zeme = reader["zeme"].ToString();
								adresa.Psc = reader["psc"].ToString();

								adresy.Add(adresa);
							}
						}
						else
						{
							adresa = null;
						}
					}
				}
				connection.Close();
			}
			return adresy;
		}

		// Metoda vytáhne všechny pozice
		public static List<Pozice> GetAllPositions()
		{
			List<Pozice> pozice = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM pozice";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						Pozice? jednaPozice = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								jednaPozice = new();
								jednaPozice.IdPozice = int.Parse(reader["idPozice"].ToString());
								jednaPozice.Nazev = reader["nazev"].ToString();

								pozice.Add(jednaPozice);
							}
						}
						else
						{
							jednaPozice = null;
						}
					}
				}
				connection.Close();
			}
			return pozice;
		}

		// Metoda vytáhne zaměstnance s jeho adresou a pozicí dle id
		public static Zamestnanci_Adresy_Pozice GetEmployeeWithAddressPosition(int idZamestnance)
		{
			Zamestnanci_Adresy_Pozice? zamestnanecAdresa = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM zamestnanci z INNER JOIN adresy a ON z.idadresy = a.idadresy INNER JOIN pozice p ON p.idpozice = z.idpozice WHERE idZamestnance = :idZamestnance";
					command.Parameters.Add(":idZamestnance", idZamestnance);
					using (OracleDataReader reader = command.ExecuteReader())
					{

						if (reader.HasRows)
						{
							while (reader.Read())
							{
								zamestnanecAdresa = new();
								zamestnanecAdresa.Zamestnanci = new();
								zamestnanecAdresa.Adresy = new();
								zamestnanecAdresa.Pozice = new();

								zamestnanecAdresa.Zamestnanci.IdZamestnance = int.Parse(reader["idZamestnance"].ToString());
								zamestnanecAdresa.Zamestnanci.Jmeno = reader["jmeno"].ToString();
								zamestnanecAdresa.Zamestnanci.Prijmeni = reader["prijmeni"].ToString();
								zamestnanecAdresa.Zamestnanci.DatumNarozeni = DateOnly.FromDateTime(DateTime.Parse(reader["datumNarozeni"].ToString()));
								zamestnanecAdresa.Zamestnanci.Telefon = reader["telefon"].ToString();
								zamestnanecAdresa.Zamestnanci.Email = reader["email"].ToString();
								zamestnanecAdresa.Zamestnanci.Heslo = reader["heslo"].ToString();
								zamestnanecAdresa.Zamestnanci.IdAdresy = int.Parse(reader["idAdresy"].ToString());
								zamestnanecAdresa.Zamestnanci.IdPozice = int.Parse(reader["idPozice"].ToString());

								zamestnanecAdresa.Adresy.IdAdresy = int.Parse(reader["idAdresy"].ToString());
								zamestnanecAdresa.Adresy.Ulice = reader["ulice"].ToString();
								zamestnanecAdresa.Adresy.Mesto = reader["mesto"].ToString();
								zamestnanecAdresa.Adresy.Okres = reader["okres"].ToString();
								zamestnanecAdresa.Adresy.Zeme = reader["zeme"].ToString();
								zamestnanecAdresa.Adresy.Psc = reader["psc"].ToString();

								zamestnanecAdresa.Pozice.IdPozice = int.Parse(reader["idPozice"].ToString());
								zamestnanecAdresa.Pozice.Nazev = reader["nazev"].ToString();
							}
						}
						else
						{
							zamestnanecAdresa = null;
						}
					}
				}
				connection.Close();
			}
			return zamestnanecAdresa;
		}

		// Zavolá proceduru na úpravu zaměstnance
		public static void EditEmployee(Zamestnanci_Adresy_Pozice zamestnanec)
		{
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = new("P_UPRAVIT_ZAM_V2", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					command.Parameters.Add("p_idzamestnance", OracleDbType.Int32).Value = zamestnanec.Zamestnanci.IdZamestnance;
					command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = zamestnanec.Zamestnanci.Jmeno;
					command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = zamestnanec.Zamestnanci.Prijmeni;
					command.Parameters.Add("p_datumnarozeni", OracleDbType.Date).Value = zamestnanec.Zamestnanci.DatumNarozeni.Value.ToDateTime(TimeOnly.Parse("00:00PM"));
					command.Parameters.Add("p_telefon", OracleDbType.Varchar2).Value = zamestnanec.Zamestnanci.Telefon;
					command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = zamestnanec.Zamestnanci.Email;
					command.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = zamestnanec.Zamestnanci.Heslo;
					command.Parameters.Add("p_idadresy", OracleDbType.Int32).Value = zamestnanec.Zamestnanci.IdAdresy;
					command.Parameters.Add("p_idpozice", OracleDbType.Int32).Value = zamestnanec.Zamestnanci.IdPozice;
					command.Parameters.Add("p_ulice", OracleDbType.Varchar2).Value = zamestnanec.Adresy.Ulice;
					command.Parameters.Add("p_mesto", OracleDbType.Varchar2).Value = zamestnanec.Adresy.Mesto;
					command.Parameters.Add("p_okres", OracleDbType.Varchar2).Value = zamestnanec.Adresy.Okres;
					command.Parameters.Add("p_zeme", OracleDbType.Varchar2).Value = zamestnanec.Adresy.Zeme;
					command.Parameters.Add("p_psc", OracleDbType.Char).Value = zamestnanec.Adresy.Psc;

					command.ExecuteNonQuery();
				}
				connection.Close();
			}
		}

		// Zavolá proceduru na odstranění zaměstnance
		public static void DeleteEmployee(int idZamestnance, int idAdresy)
		{
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = new("P_SMAZAT_ZAMESTNANCE", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					command.Parameters.Add("p_idzamestnance", OracleDbType.Int32).Value = idZamestnance;
					command.Parameters.Add("p_idadresy", OracleDbType.Int32).Value = idAdresy;

					command.ExecuteNonQuery();
				}
				connection.Close();
			}
		}
	}
}