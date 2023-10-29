using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models;
using sem_prace_janousek_mandik.Models.Dodavatele;
using System.Data;

namespace sem_prace_janousek_mandik.Controllers.Management
{
	public static class DodavateleSQL
	{
		// Metoda vytáhne všechny dodavatele
		public static List<Dodavatele> GetAllSuppliers()
		{
			List<Dodavatele> dodavatele = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM dodavatele";
					using (OracleDataReader reader = command.ExecuteReader())
					{
                        Dodavatele? dodavatel = new();
                        if (reader.HasRows)
						{
							while (reader.Read())
							{
								dodavatel = new();
								dodavatel.IdDodavatele = int.Parse(reader["idDodavatele"].ToString());
                                dodavatel.Nazev = reader["nazev"].ToString();
                                dodavatel.Jmeno = reader["jmeno"].ToString();
								dodavatel.Prijmeni = reader["prijmeni"].ToString();
								dodavatel.Telefon = reader["telefon"].ToString();
								dodavatel.Email = reader["email"].ToString();
								dodavatel.IdAdresy = int.Parse(reader["idAdresy"].ToString());

								dodavatele.Add(dodavatel);
							}
						}
						else
						{
							dodavatel = null;
						}
					}
				}
				connection.Close();
			}
			return dodavatele;
		}

		// Zavolá proceduru na úpravu dodavatele
		public static void EditSupplier(Dodavatele_Adresy dodavatel)
		{
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = new("P_UPRAVIT_DODAVATELE_V2", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					command.Parameters.Add("p_iddodavatele", OracleDbType.Int32).Value = dodavatel.Dodavatele.IdDodavatele;
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = dodavatel.Dodavatele.Nazev;
                    command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = dodavatel.Dodavatele.Jmeno;
					command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = dodavatel.Dodavatele.Prijmeni;
					command.Parameters.Add("p_telefon", OracleDbType.Varchar2).Value = dodavatel.Dodavatele.Telefon;
					command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = dodavatel.Dodavatele.Email;
					command.Parameters.Add("p_idadresy", OracleDbType.Int32).Value = dodavatel.Dodavatele.IdAdresy;
					command.Parameters.Add("p_ulice", OracleDbType.Varchar2).Value = dodavatel.Adresy.Ulice;
					command.Parameters.Add("p_mesto", OracleDbType.Varchar2).Value = dodavatel.Adresy.Mesto;
					command.Parameters.Add("p_okres", OracleDbType.Varchar2).Value = dodavatel.Adresy.Okres;
					command.Parameters.Add("p_zeme", OracleDbType.Varchar2).Value = dodavatel.Adresy.Zeme;
					command.Parameters.Add("p_psc", OracleDbType.Char).Value = dodavatel.Adresy.Psc;

					command.ExecuteNonQuery();
				}
				connection.Close();
			}
		}

		// Vytáhne dodavatele včetně adresy
        public static Dodavatele_Adresy GetSupplierWithAddress(int idDodavatele)
        {
            Dodavatele_Adresy? dodavatelAdresa = new();
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM dodavatele d INNER JOIN adresy a ON d.idadresy = a.idadresy WHERE idDodavatele = :idDodavatele";
                    command.Parameters.Add(":idZamestnance", idDodavatele);
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                dodavatelAdresa = new();
                                dodavatelAdresa.Dodavatele = new();
                                dodavatelAdresa.Adresy = new();

                                dodavatelAdresa.Dodavatele.IdDodavatele = int.Parse(reader["idDodavatele"].ToString());
                                dodavatelAdresa.Dodavatele.Nazev = reader["nazev"].ToString();
                                dodavatelAdresa.Dodavatele.Jmeno = reader["jmeno"].ToString();
                                dodavatelAdresa.Dodavatele.Prijmeni = reader["prijmeni"].ToString();
                                dodavatelAdresa.Dodavatele.Telefon = reader["telefon"].ToString();
                                dodavatelAdresa.Dodavatele.Email = reader["email"].ToString();
                                dodavatelAdresa.Dodavatele.IdAdresy = int.Parse(reader["idAdresy"].ToString());

                                dodavatelAdresa.Adresy.IdAdresy = int.Parse(reader["idAdresy"].ToString());
                                dodavatelAdresa.Adresy.Ulice = reader["ulice"].ToString();
                                dodavatelAdresa.Adresy.Mesto = reader["mesto"].ToString();
                                dodavatelAdresa.Adresy.Okres = reader["okres"].ToString();
                                dodavatelAdresa.Adresy.Zeme = reader["zeme"].ToString();
                                dodavatelAdresa.Adresy.Psc = reader["psc"].ToString();
                            }
                        }
                        else
                        {
                            dodavatelAdresa = null;
                        }
                    }
                }
                connection.Close();
            }
            return dodavatelAdresa;
        }

        // Zavolá proceduru na odstranění dodavatele
        public static void DeleteSupplier(int idDodavatele, int idAdresy)
		{
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = new("P_SMAZAT_DODAVATELE", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					command.Parameters.Add("p_iddodavatele", OracleDbType.Int32).Value = idDodavatele;
					command.Parameters.Add("p_idadresy", OracleDbType.Int32).Value = idAdresy;

					command.ExecuteNonQuery();
				}
				connection.Close();
			}
		}

		// Kontrola existence emailu (dodavatele) v databázi - kontrola při přidávání nového dodavatele
		public static bool CheckExistsSupplier(string email)
		{
			bool exists = true;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM dodavatele WHERE email = :email";
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

		// Zavolání procedury na přidání dodavatele
		public static bool AddSupplier(Dodavatele_Adresy novyDodavatel)
		{
			bool registerSuccessful = false;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = new("P_VLOZIT_DODAVATELE_V2", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

                    // Vstupní parametry procedury
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = novyDodavatel.Dodavatele.Nazev;
                    command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = novyDodavatel.Dodavatele.Jmeno;
					command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = novyDodavatel.Dodavatele.Prijmeni;
					command.Parameters.Add("p_telefon", OracleDbType.Varchar2).Value = novyDodavatel.Dodavatele.Telefon;
					command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = novyDodavatel.Dodavatele.Email;
					command.Parameters.Add("p_ulice", OracleDbType.Varchar2).Value = novyDodavatel.Adresy.Ulice;
					command.Parameters.Add("p_mesto", OracleDbType.Varchar2).Value = novyDodavatel.Adresy.Mesto;
					command.Parameters.Add("p_okres", OracleDbType.Varchar2).Value = novyDodavatel.Adresy.Okres;
					command.Parameters.Add("p_zeme", OracleDbType.Varchar2).Value = novyDodavatel.Adresy.Zeme;
					command.Parameters.Add("p_psc", OracleDbType.Char).Value = novyDodavatel.Adresy.Psc;

					command.ExecuteNonQuery();
				}
				connection.Close();
				registerSuccessful = true;
			}
			return registerSuccessful;
		}
	}
}