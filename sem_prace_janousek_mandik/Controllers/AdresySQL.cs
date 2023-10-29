using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models;

namespace sem_prace_janousek_mandik.Controllers
{
    public class AdresySQL
    {
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
    }
}
