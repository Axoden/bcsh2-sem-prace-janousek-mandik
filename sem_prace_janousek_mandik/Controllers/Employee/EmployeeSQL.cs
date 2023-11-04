﻿using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models;
using sem_prace_janousek_mandik.Models.Employee;
using System.Data;

namespace sem_prace_janousek_mandik.Controllers.Employee
{
    public class EmployeeSQL
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



        // Metoda vrátí idpozice na základě názvu pozice
        public static int GetPositionIdByName(string nazevPozice)
        {
            int idPozice = 0;
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT idpozice FROM pozice WHERE nazev = :nazev";
                    command.Parameters.Add(":nazev", OracleDbType.Varchar2).Value = nazevPozice;
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                idPozice = int.Parse(reader["idPozice"].ToString());
                            }
                        }
                    }
                }
                connection.Close();
            }
            return idPozice;
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
                    if (zamestnanec.Zamestnanci.DatumNarozeni == null)
                    {
                        command.Parameters.Add("p_datumnarozeni", OracleDbType.Date).Value = null;
                    }
                    else
                    {
                        command.Parameters.Add("p_datumnarozeni", OracleDbType.Date).Value = zamestnanec.Zamestnanci.DatumNarozeni.Value.ToDateTime(TimeOnly.Parse("00:00PM"));
                    }
                    command.Parameters.Add("p_telefon", OracleDbType.Varchar2).Value = zamestnanec.Zamestnanci.Telefon;
                    command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = zamestnanec.Zamestnanci.Email;
                    command.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = zamestnanec.Zamestnanci.Heslo;
                    command.Parameters.Add("p_idadresy", OracleDbType.Int32).Value = zamestnanec.Zamestnanci.IdAdresy;
                    command.Parameters.Add("p_idpozice", OracleDbType.Int32).Value = zamestnanec.Zamestnanci.IdPozice;
                    if (zamestnanec.Adresy == null)
                    {
                        zamestnanec.Adresy = new();
                    }
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

        // Kontrola existence emailu (zaměstnance) v databázi - kontrola při vytváření nového zaměstnance
        public static bool CheckExistsEmployee(string email)
        {
            bool exists = true;
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

        // Zavolání procedury na registraci zaměstnance
        public static bool RegisterEmployee(Zamestnanci_Adresy_Pozice novyZamestnanec)
        {
            bool registerSuccessful = false;
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new("P_VLOZIT_ZAMESTNANCE_V2", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Vstupní parametry procedury
                    command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = novyZamestnanec.Zamestnanci.Jmeno;
                    command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = novyZamestnanec.Zamestnanci.Prijmeni;
                    command.Parameters.Add("p_datumnarozeni", OracleDbType.Date).Value = novyZamestnanec.Zamestnanci.DatumNarozeni.Value.ToDateTime(TimeOnly.Parse("00:00PM"));
                    command.Parameters.Add("p_telefon", OracleDbType.Varchar2).Value = novyZamestnanec.Zamestnanci.Telefon;
                    command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = novyZamestnanec.Zamestnanci.Email;
                    command.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = novyZamestnanec.Zamestnanci.Heslo;
                    command.Parameters.Add("p_idpozice", OracleDbType.Int32).Value = novyZamestnanec.Zamestnanci.IdPozice;
                    command.Parameters.Add("p_ulice", OracleDbType.Varchar2).Value = novyZamestnanec.Adresy.Ulice;
                    command.Parameters.Add("p_mesto", OracleDbType.Varchar2).Value = novyZamestnanec.Adresy.Mesto;
                    command.Parameters.Add("p_okres", OracleDbType.Varchar2).Value = novyZamestnanec.Adresy.Okres;
                    command.Parameters.Add("p_zeme", OracleDbType.Varchar2).Value = novyZamestnanec.Adresy.Zeme;
                    command.Parameters.Add("p_psc", OracleDbType.Char).Value = novyZamestnanec.Adresy.Psc;

                    command.ExecuteNonQuery();
                }
                connection.Close();
                registerSuccessful = true;
            }
            return registerSuccessful;
        }

		// Metoda ověří zaměstnance a vrátí celý objekt Zamestnanec
		public static Zamestnanci? AuthEmployee(string email)
		{
			Zamestnanci? zamestnanec = new();
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

		// Metoda získá pracovní pozici a vrátí celý objekt Pozice
		public static Pozice? GetPosition(int id)
		{
			Pozice? pozice = new();
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
	}
}