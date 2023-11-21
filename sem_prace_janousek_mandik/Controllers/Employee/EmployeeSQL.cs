using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models;
using sem_prace_janousek_mandik.Models.Employee;
using System.Data;

namespace sem_prace_janousek_mandik.Controllers.Employee
{
    public class EmployeeSQL
    {
        /// <summary>
        /// Metoda vytáhne všechny zaměstnance
        /// </summary>
        /// <returns>List všech zaměstnanců</returns>
        public static List<Zamestnanci_Adresy_Pozice> GetAllEmployeesWithAddressPosition()
        {
            List<Zamestnanci_Adresy_Pozice> zamestnanci = new();
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM zamestnanci z" +
                        " INNER JOIN adresy a ON z.idAdresy = a.idAdresy" +
                        " INNER JOIN pozice p ON z.idPozice = p.idPozice";
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        Zamestnanci_Adresy_Pozice? zamestnanec = new();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                zamestnanec = new();
                                zamestnanec.Zamestnanci = new();
                                zamestnanec.Adresy = new();
                                zamestnanec.Pozice = new();

                                zamestnanec.Zamestnanci.IdZamestnance = int.Parse(reader["idZamestnance"].ToString());
                                zamestnanec.Zamestnanci.Jmeno = reader["jmeno"].ToString();
                                zamestnanec.Zamestnanci.Prijmeni = reader["prijmeni"].ToString();
                                zamestnanec.Zamestnanci.DatumNarozeni = DateOnly.FromDateTime(DateTime.Parse(reader["datumNarozeni"].ToString()));
                                zamestnanec.Zamestnanci.Telefon = reader["telefon"].ToString();
                                zamestnanec.Zamestnanci.Email = reader["email"].ToString();
                                zamestnanec.Zamestnanci.Heslo = reader["heslo"].ToString();
                                zamestnanec.Zamestnanci.IdAdresy = int.Parse(reader["idAdresy"].ToString());
                                zamestnanec.Zamestnanci.IdPozice = int.Parse(reader["idPozice"].ToString());

                                zamestnanec.Adresy.IdAdresy = int.Parse(reader["idAdresy"].ToString());
                                zamestnanec.Adresy.Ulice = reader["ulice"].ToString();
                                zamestnanec.Adresy.Mesto = reader["mesto"].ToString();
                                zamestnanec.Adresy.Okres = reader["okres"].ToString();
                                zamestnanec.Adresy.Zeme = reader["zeme"].ToString();
                                zamestnanec.Adresy.Psc = reader["psc"].ToString();

                                zamestnanec.Pozice.IdPozice = int.Parse(reader["idPozice"].ToString());
                                zamestnanec.Pozice.Nazev = reader["nazev"].ToString();

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

        /// <summary>
        /// Metoda vytáhne zaměstnance s jeho adresou a pozicí dle id
        /// </summary>
        /// <param name="idEmployee">ID zaměstnance</param>
        /// <returns>Model konkrétního zaměstnance</returns>
        public static Zamestnanci_Adresy_Pozice GetEmployeeWithAddressPosition(int idEmployee)
        {
            Zamestnanci_Adresy_Pozice? zamestnanecAdresa = new();
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM zamestnanci z INNER JOIN adresy a ON z.idadresy = a.idadresy INNER JOIN pozice p ON p.idpozice = z.idpozice WHERE idZamestnance = :idZamestnance";
                    command.Parameters.Add(":idZamestnance", idEmployee);
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

        /// <summary>
        /// Zavolá proceduru na úpravu zaměstnance
        /// </summary>
        /// <param name="employee">Model s daty upraveného zaměstnance</param>
        /// <returns>true, pokud příkaz proběhl úspěšně, jinak false</returns>
        public static bool EditEmployee(Zamestnanci_Adresy_PoziceList employee)
        {
            try
            {
                using (OracleConnection connection = OracleDbContext.GetConnection())
                {
                    connection.Open();
                    using (OracleCommand command = new("P_UPRAVIT_ZAM_V2", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        command.Parameters.Add("p_idzamestnance", OracleDbType.Int32).Value = employee.Zamestnanci.IdZamestnance;
                        command.Parameters.Add("p_jmeno", OracleDbType.Varchar2).Value = employee.Zamestnanci.Jmeno;
                        command.Parameters.Add("p_prijmeni", OracleDbType.Varchar2).Value = employee.Zamestnanci.Prijmeni;
                        if (employee.Zamestnanci.DatumNarozeni == null)
                        {
                            command.Parameters.Add("p_datumnarozeni", OracleDbType.Date).Value = null;
                        }
                        else
                        {
                            command.Parameters.Add("p_datumnarozeni", OracleDbType.Date).Value = employee.Zamestnanci.DatumNarozeni.Value.ToDateTime(TimeOnly.Parse("00:00PM"));
                        }
                        command.Parameters.Add("p_telefon", OracleDbType.Varchar2).Value = employee.Zamestnanci.Telefon;
                        command.Parameters.Add("p_email", OracleDbType.Varchar2).Value = employee.Zamestnanci.Email;
                        command.Parameters.Add("p_heslo", OracleDbType.Varchar2).Value = employee.Zamestnanci.Heslo;
                        command.Parameters.Add("p_idadresy", OracleDbType.Int32).Value = employee.Zamestnanci.IdAdresy;
                        command.Parameters.Add("p_idpozice", OracleDbType.Int32).Value = employee.Zamestnanci.IdPozice;
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
        /// Kontrola existence emailu (zaměstnance) v databázi - kontrola při vytváření nového zaměstnance
        /// </summary>
        /// <param name="email">Kontrolovaný email</param>
        /// <returns>true, pokud již existuje zaměstnanec s tímto emailem, jinak false</returns>
        public static bool CheckExistsEmployee(string email)
        {
            bool exists = true;
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT idZamestnance FROM zamestnanci WHERE email = :email";
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

        /// <summary>
        /// Zavolání procedury na registraci zaměstnance
        /// </summary>
        /// <param name="newEmployee">Model s daty nového zaměstnance</param>
        /// <returns>true, pokud příkaz proběhl úspěšně, jinak false</returns>
        public static bool RegisterEmployee(Zamestnanci_Adresy_PoziceList newEmployee)
        {
            try
            {
                using (OracleConnection connection = OracleDbContext.GetConnection())
                {
                    connection.Open();
                    using (OracleCommand command = new("P_VLOZIT_ZAMESTNANCE_V2", connection))
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
        /// Metoda ověří zaměstnance a vrátí celý objekt Zamestnanec
        /// </summary>
        /// <param name="email">Email zaměstnance</param>
        /// <returns>Model zaměstnance</returns>
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

        /// <summary>
        /// Metoda získá pracovní pozici a vrátí celý objekt Pozice
        /// </summary>
        /// <param name="idPosition">ID pozice</param>
        /// <returns>Model pozice</returns>
        public static Pozice? GetPosition(int idPosition)
        {
            Pozice? pozice = new();
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM pozice WHERE idPozice = :id";
                    command.Parameters.Add(":id", idPosition);
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

        /// <summary>
        /// Metoda získá ID, jména a příjmení všech zaměstnanců
        /// </summary>
        /// <returns>List všech zaměstnanců</returns>
        internal static List<Zamestnanci> GetAllEmployeesNameSurname()
        {
            List<Zamestnanci> zamestnanci = new();
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT idZamestnance, jmeno, prijmeni FROM zamestnanci";
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        Zamestnanci? zamestnanec = new();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                zamestnanec = new();
                                zamestnanec.IdZamestnance = int.Parse(reader["idZamestnance"].ToString());
                                zamestnanec.Jmeno = reader["jmeno"].ToString();
                                zamestnanec.Prijmeni = reader["prijmeni"].ToString();
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

        /// <summary>
        /// Metoda získá ID zaměstnance na základě jeho emailu
        /// </summary>
        /// <param name="email">Email zaměstnance</param>
        /// <returns>ID zaměstnance</returns>
        public static int GetEmployeeIdByEmail(string email)
        {
            int idZamestnance = 0;
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT idZamestnance FROM zamestnanci WHERE email = :email";
                    command.Parameters.Add(":email", email);
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                idZamestnance = int.Parse(reader["idZamestnance"].ToString());
                            }
                        }
                    }
                }
                connection.Close();
            }
            return idZamestnance;
        }

        /// <summary>
        /// Metoda získá email a roli zaměstnance na základě jeho id
        /// </summary>
        /// <param name="idEmployee">ID zaměstnance</param>
        /// <returns>Model zaměstnance s emailem a názvem pozice</returns>
        public static Zamestnanci_Pozice GetEmployeeRoleEmailById(int idEmployee)
        {
            Zamestnanci_Pozice employee = new();
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT z.email, p.nazev FROM zamestnanci z INNER JOIN pozice p ON z.idPozice = p.idPozice WHERE idZamestnance = :idZamestnance";
                    command.Parameters.Add(":idZamestnance", idEmployee);
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                employee.Zamestnanci = new();
                                employee.Pozice = new();

                                employee.Zamestnanci.Email = reader["email"].ToString();
                                employee.Pozice.Nazev = reader["nazev"].ToString();
                            }
                        }
                    }
                }
                connection.Close();
            }
            return employee;
        }
    }
}