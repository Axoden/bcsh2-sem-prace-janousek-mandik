using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models.Customer;
using sem_prace_janousek_mandik.Models.Employee;
using System.Data;

namespace sem_prace_janousek_mandik.Controllers.Customer
{
    public class CustomerSQL
    {
		// Metoda ověří zákazníka a vrátí celý objekt Zakaznik
		internal static Zakaznici? AuthCustomer(string email)
		{
			Zakaznici? zakaznik = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM zakaznici WHERE email = :email";
					command.Parameters.Add(":email", email);
					using (OracleDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								zakaznik.IdZakaznika = int.Parse(reader["idZakaznika"].ToString());
								zakaznik.Jmeno = reader["jmeno"].ToString();
								zakaznik.Prijmeni = reader["prijmeni"].ToString();
								zakaznik.Telefon = reader["telefon"].ToString();
								zakaznik.Email = reader["email"].ToString();
								zakaznik.Heslo = reader["heslo"].ToString();
								zakaznik.IdAdresy = int.Parse(reader["idAdresy"].ToString());
							}
						}
						else
						{
							zakaznik = null;
						}
					}
				}
				connection.Close();
			}
			return zakaznik;
		}

		// Kontrola existence emailu (zákazníka) v databázi - kontrola při registraci
		internal static bool CheckExistsCustomer(string email)
		{
			bool exists = true;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM zakaznici WHERE email = :email";
					command.Parameters.Add(":email", email);
					using (OracleDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							exists = true;
						}
						else
						{
							exists = false;
						}
					}
				}
				connection.Close();
			}
			return exists;
		}

		// Metoda zavolá proceduru na úpravu zákazníka
        internal static void EditCustomer(Zakaznici_Adresy zakaznikAdresa)
        {
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = new("P_UPRAVIT_ZAKAZNIKA_V2", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					command.Parameters.Add("p_idzakaznika", OracleDbType.Int32).Value = zakaznikAdresa.Zakaznici.IdZakaznika;
					command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = zakaznikAdresa.Zakaznici.Jmeno;
					command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = zakaznikAdresa.Zakaznici.Prijmeni;
					command.Parameters.Add("p_telefon", OracleDbType.Varchar2).Value = zakaznikAdresa.Zakaznici.Telefon;
					command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = zakaznikAdresa.Zakaznici.Email;
					command.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = zakaznikAdresa.Zakaznici.Heslo;
					command.Parameters.Add("p_idadresy", OracleDbType.Int32).Value = zakaznikAdresa.Zakaznici.IdAdresy;

					command.Parameters.Add("p_ulice", OracleDbType.Varchar2).Value = zakaznikAdresa.Adresy.Ulice;
					command.Parameters.Add("p_mesto", OracleDbType.Varchar2).Value = zakaznikAdresa.Adresy.Mesto;
					command.Parameters.Add("p_okres", OracleDbType.Varchar2).Value = zakaznikAdresa.Adresy.Okres;
					command.Parameters.Add("p_zeme", OracleDbType.Varchar2).Value = zakaznikAdresa.Adresy.Zeme;
					command.Parameters.Add("p_psc", OracleDbType.Char).Value = zakaznikAdresa.Adresy.Psc;

					command.ExecuteNonQuery();
				}
				connection.Close();
			}
		}

		// Metoda vytáhne všechny zákazníky včetně adres
        internal static List<Zakaznici_Adresy> GetAllCustomersWithAddresses()
        {
			List<Zakaznici_Adresy> zakazniciAdresy = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM zakaznici z INNER JOIN adresy a ON z.idadresy = a.idadresy";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						Zakaznici_Adresy? zakaznikAdresa = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								zakaznikAdresa = new();
								zakaznikAdresa.Zakaznici = new();
								zakaznikAdresa.Adresy = new();

								zakaznikAdresa.Zakaznici.IdZakaznika = int.Parse(reader["idZakaznika"].ToString());
								zakaznikAdresa.Zakaznici.Jmeno = reader["jmeno"].ToString();
								zakaznikAdresa.Zakaznici.Prijmeni = reader["prijmeni"].ToString();
								zakaznikAdresa.Zakaznici.Telefon = reader["telefon"].ToString();
								zakaznikAdresa.Zakaznici.Email = reader["email"].ToString();
								zakaznikAdresa.Zakaznici.IdAdresy = int.Parse(reader["idAdresy"].ToString());
								zakaznikAdresa.Adresy.Ulice = reader["ulice"].ToString();
								zakaznikAdresa.Adresy.Mesto = reader["Mesto"].ToString();
								zakaznikAdresa.Adresy.Okres = reader["Okres"].ToString();
								zakaznikAdresa.Adresy.Zeme = reader["Zeme"].ToString();
								zakaznikAdresa.Adresy.Psc = reader["Psc"].ToString();

								zakazniciAdresy.Add(zakaznikAdresa);
							}
						}
						else
						{
							zakaznikAdresa = null;
						}
					}
				}
				connection.Close();
			}
			return zakazniciAdresy;
		}

		// Metoda vytáhne konkrétního zákazníka včetně adresy na základě ID zákazníka
        internal static Zakaznici_Adresy GetCustomerWithAddress(int index)
        {
			Zakaznici_Adresy? zakaznikAdresa = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM zakaznici z INNER JOIN adresy a ON z.idadresy = a.idadresy WHERE idZakaznika = :idZakaznika";
					command.Parameters.Add(":idZakaznika", index);
					using (OracleDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								zakaznikAdresa = new();
								zakaznikAdresa.Zakaznici = new();
								zakaznikAdresa.Adresy = new();

								zakaznikAdresa.Zakaznici.IdZakaznika = int.Parse(reader["idZakaznika"].ToString());
								zakaznikAdresa.Zakaznici.Jmeno = reader["jmeno"].ToString();
								zakaznikAdresa.Zakaznici.Prijmeni = reader["prijmeni"].ToString();
								zakaznikAdresa.Zakaznici.Telefon = reader["telefon"].ToString();
								zakaznikAdresa.Zakaznici.Email = reader["email"].ToString();
								zakaznikAdresa.Zakaznici.IdAdresy = int.Parse(reader["idAdresy"].ToString());

								zakaznikAdresa.Adresy.Ulice = reader["ulice"].ToString();
								zakaznikAdresa.Adresy.Mesto = reader["mesto"].ToString();
								zakaznikAdresa.Adresy.Okres = reader["okres"].ToString();
								zakaznikAdresa.Adresy.Zeme = reader["zeme"].ToString();
								zakaznikAdresa.Adresy.Psc = reader["psc"].ToString();
							}
						}
						else
						{
							zakaznikAdresa = null;
						}
					}
				}
				connection.Close();
			}
			return zakaznikAdresa;
		}

		// Metoda zavolá proceduru na registraci zákazníka
        internal static bool RegisterCustomer(Zakaznici_Adresy novyZakaznik)
		{
			bool registerSuccessful = false;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = new("P_VLOZIT_ZAKAZNIKA_V2", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					// Vstupní parametry procedury
					command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = novyZakaznik.Zakaznici.Jmeno;
					command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = novyZakaznik.Zakaznici.Prijmeni;
					command.Parameters.Add("p_telefon", OracleDbType.Varchar2).Value = novyZakaznik.Zakaznici.Telefon;
					command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = novyZakaznik.Zakaznici.Email;
					command.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = novyZakaznik.Zakaznici.Heslo;
					command.Parameters.Add("p_ulice", OracleDbType.Varchar2).Value = novyZakaznik.Adresy.Ulice;
					command.Parameters.Add("p_mesto", OracleDbType.Varchar2).Value = novyZakaznik.Adresy.Mesto;
					command.Parameters.Add("p_okres", OracleDbType.Varchar2).Value = novyZakaznik.Adresy.Okres;
					command.Parameters.Add("p_zeme", OracleDbType.Varchar2).Value = novyZakaznik.Adresy.Zeme;
					command.Parameters.Add("p_psc", OracleDbType.Char).Value = novyZakaznik.Adresy.Psc;

					command.ExecuteNonQuery();
				}
				connection.Close();
				registerSuccessful = true;
			}
			return registerSuccessful;
		}
	}
}
