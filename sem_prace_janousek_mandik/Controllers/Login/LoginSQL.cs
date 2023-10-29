using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models;
using System.Data;

namespace sem_prace_janousek_mandik.Controllers.Login
{
    public static class LoginSQL
    {
        // Metoda overi zamestnance a vrati cely objekt Zamestnanec
        public static Zamestnanci? AuthEmployee(string email)
        {
            Zamestnanci? zamestnanec = new Zamestnanci();
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM zamestnanci WHERE email = :email";
                    command.Parameters.Add(":email", email);
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                zamestnanec.IdZamestnance = int.Parse(reader["idZamestnance"].ToString());
                                zamestnanec.Jmeno = reader["jmeno"].ToString();
                                zamestnanec.Prijmeni = reader["prijmeni"].ToString();
                                zamestnanec.DatumNarozeni = DateOnly.FromDateTime(DateTime.Parse(reader["datumNarozeni"].ToString()));
                                zamestnanec.Telefon = reader["telefon"].ToString();
                                zamestnanec.Email = reader["email"].ToString();
                                zamestnanec.Heslo = reader["heslo"].ToString();
                                zamestnanec.IdAdresy = int.Parse(reader["idAdresy"].ToString());
                                zamestnanec.IdPozice = int.Parse(reader["idPozice"].ToString());
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
            return zamestnanec;
        }

        // Metoda ziska pracovni pozici a vrati cely objekt Pozice
        public static Pozice? GetPosition(int id)
        {
            Pozice? pozice = new Pozice();
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM pozice WHERE idPozice = :id";
                    command.Parameters.Add(":id", id);
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                pozice.IdPozice = int.Parse(reader["idPozice"].ToString());
                                pozice.Nazev = reader["nazev"].ToString();
                            }
                        }
                        else
                        {
                            pozice = null;
                        }
                    }
                }
                connection.Close();
            }
            return pozice;
        }


        // Metoda overi zakaznika a vrati cely objekt Zakaznik
        public static Zakaznici? AuthCustomer(string email)
        {
            Zakaznici? zakaznik = new Zakaznici();
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
        public static bool CheckExistsCustomer(string email)
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

        public static bool RegisterCustomer(Zakaznici_Adresy novyZakaznik)
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
