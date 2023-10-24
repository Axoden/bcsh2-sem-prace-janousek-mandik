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

        public static bool RegisterCustomer(Zakaznici_Adresy novyZakaznikVcetneAdresy)
        {
            int? idAdresy = InsertAddressAndReturnId(novyZakaznikVcetneAdresy.Adresy);
            bool uspesneRegistrovan = false;

            if(idAdresy != null)
            {
                using (OracleConnection connection = OracleDbContext.GetConnection())
                {
                    connection.Open();
                    using (OracleCommand command = connection.CreateCommand())
                    {
                        command.CommandText = "INSERT INTO zakaznici (jmeno, prijmeni, telefon, email, idAdresy, heslo) VALUES (:jmeno, :prijmeni, :telefon, :email, :idAdresy, :heslo)";
                        command.Parameters.Add(":jmeno", novyZakaznikVcetneAdresy.Zakaznici.Jmeno);
                        command.Parameters.Add(":prijmeni", novyZakaznikVcetneAdresy.Zakaznici.Prijmeni);
                        command.Parameters.Add(":telefon", novyZakaznikVcetneAdresy.Zakaznici.Telefon);
                        command.Parameters.Add(":email", novyZakaznikVcetneAdresy.Zakaznici.Email);
                        command.Parameters.Add(":idAdresy", (int)idAdresy);
                        command.Parameters.Add(":heslo", LoginController.HashPassword(novyZakaznikVcetneAdresy.Zakaznici.Heslo));

                        command.ExecuteNonQuery();
                    }
                    connection.Close();
                    uspesneRegistrovan = true;
                }
            }
            return uspesneRegistrovan;
        }

        private static int? InsertAddressAndReturnId(Adresy novaAdresa)
        {
            int? idAdresy = null;
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new OracleCommand("vlozit_novou_adresu", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Vstupní parametry procedury vlozit_novou_adresu
                    command.Parameters.Add("v_ulice", OracleDbType.Varchar2).Value = novaAdresa.Ulice;
                    command.Parameters.Add("v_mesto", OracleDbType.Varchar2).Value = novaAdresa.Mesto;
                    command.Parameters.Add("v_okres", OracleDbType.Varchar2).Value = novaAdresa.Okres;
                    command.Parameters.Add("v_zeme", OracleDbType.Varchar2).Value = novaAdresa.Zeme;
                    command.Parameters.Add("v_psc", OracleDbType.Char).Value = novaAdresa.Psc;

                    // Výstupní parametr id nové adresy
                    command.Parameters.Add("v_nove_id", OracleDbType.Int32).Direction = ParameterDirection.Output;

                    command.ExecuteNonQuery();

                    idAdresy = int.Parse(command.Parameters["v_nove_id"].Value.ToString());
                }

                connection.Close();
            }

            return idAdresy;
        }
    }
}
