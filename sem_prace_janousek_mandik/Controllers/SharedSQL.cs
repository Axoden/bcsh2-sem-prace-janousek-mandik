using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace sem_prace_janousek_mandik.Controllers
{
    public class SharedSQL
    {
        // Metoda vytáhne všechny adresy
        public static List<Adresy> GetAllAddresses()
        {
            Adresy? adresa;
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
                                adresa = new();
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

        // Vygenerování hashe z hesla
        public static string? HashPassword(string password)
        {
            if (password != null)
            {
                var encoded = Encoding.Default.GetBytes(password);
                var hashedPassword = SHA256.HashData(encoded);
                return Convert.ToBase64String(hashedPassword);
            }
            return null;
        }

        // Zavolá proceduru na smazání
        internal static void CallDeleteProcedure(string procedureName, int id)
        {
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new(procedureName, connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;
                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

		// Zavolá proceduru na smazání vč. adresy
		internal static void CallDeleteProcedure(string procedureName, int id, int idAdresy)
		{
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = new(procedureName, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;
					command.Parameters.Add("p_idadresy", OracleDbType.Int32).Value = idAdresy;
					command.ExecuteNonQuery();
				}
				connection.Close();
			}
		}
	}
}
