using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models;

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

		// Metoda vytahne vsechny adresy
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
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "EXEC p_upravit_zamestnance(:idZamestnance, :jmeno, :prijmeni, :datumnarozeni, :telefon, :email, :heslo, :idadresy, :idpozice)";
                    command.Parameters.Add(":idZamestnance", zamestnanec.Zamestnanci.IdZamestnance);
                    command.Parameters.Add(":jmeno", zamestnanec.Zamestnanci.Jmeno);
                    command.Parameters.Add(":prijmeni", zamestnanec.Zamestnanci.Prijmeni);
                    command.Parameters.Add(":datumnarozeni", zamestnanec.Zamestnanci.DatumNarozeni);
                    command.Parameters.Add(":telefon", zamestnanec.Zamestnanci.Telefon);
                    command.Parameters.Add(":email", zamestnanec.Zamestnanci.Email);
                    command.Parameters.Add(":heslo", zamestnanec.Zamestnanci.Heslo);
                    command.Parameters.Add(":idadresy", zamestnanec.Zamestnanci.IdAdresy);
                    command.Parameters.Add(":idpozice", zamestnanec.Zamestnanci.IdPozice);
                    using (OracleDataReader reader = command.ExecuteReader())
                    {

                    }
                }
                connection.Close();
            }
        }
    }
}