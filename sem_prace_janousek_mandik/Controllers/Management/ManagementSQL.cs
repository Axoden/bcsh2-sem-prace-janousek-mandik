using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models;

namespace sem_prace_janousek_mandik.Controllers.Management
{
    public static class ManagementSQL
    {
        // Metoda vytahne vsechny zaměstnance
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
                                zamestnanec.DatumNarozeni = DateTime.Parse(reader["datumNarozeni"].ToString());
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
    }
}