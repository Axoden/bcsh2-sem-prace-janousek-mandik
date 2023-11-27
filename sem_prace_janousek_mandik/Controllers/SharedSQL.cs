using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace sem_prace_janousek_mandik.Controllers
{
    public class SharedSQL
    {
        /// <summary>
        /// Vygenerování hashe z hesla
        /// </summary>
        /// <param name="password">Nezahashované heslo</param>
        /// <returns>Zahashované heslo</returns>
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

        /// <summary>
        /// Zavolá proceduru na smazání
        /// </summary>
        /// <param name="procedureName">Název procedury</param>
        /// <param name="id">Vstupní parametr ID procedury</param>
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

        /// <summary>
		// Zavolá proceduru na smazání
        /// </summary>
        /// <param name="procedureName">Název procedury</param>
        /// <param name="id">Vstupní parametr ID procedury</param>
        /// <param name="idAdresy">Vstupní parametr ID procedury</param>
		internal static void CallDeleteProcedure(string procedureName, int id, int secondId)
		{
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = new(procedureName, connection))
				{
					command.CommandType = CommandType.StoredProcedure;
					command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;
					command.Parameters.Add("p_idě", OracleDbType.Int32).Value = secondId;
					command.ExecuteNonQuery();
				}
				connection.Close();
			}
		}
	}
}
