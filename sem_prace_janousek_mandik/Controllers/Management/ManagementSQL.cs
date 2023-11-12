using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models;
using System.Data;

namespace sem_prace_janousek_mandik.Controllers.Management
{
    public static class ManagementSQL
	{
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

        // Zavolá proceduru na vložení pozice
		internal static bool RegisterPosition(Pozice novaPozice)
		{
			bool registerSuccessful = false;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = new("p_vlozit_pozici_v2", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

                    // Vstupní parametr procedury
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = novaPozice.Nazev;

					command.ExecuteNonQuery();
				}
				connection.Close();
				registerSuccessful = true;
			}
			return registerSuccessful;
		}

		// Zavolá proceduru na úpravu pozice
		public static void EditPosition(Pozice pozice)
		{
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = new("P_UPRAVIT_POZICI_V2", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					command.Parameters.Add("p_idpozice", OracleDbType.Int32).Value = pozice.IdPozice;
					command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = pozice.Nazev;
					
					command.ExecuteNonQuery();
				}
				connection.Close();
			}
		}

		// Metoda vrátí pozici na základě ID pozice
		public static Pozice GetPositionById(int idPozice)
		{
			Pozice getPozice = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM pozice WHERE idPozice = :idPozice";
					command.Parameters.Add(":idPozice", OracleDbType.Int32).Value = idPozice;
					using (OracleDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								getPozice.IdPozice = int.Parse(reader["idPozice"].ToString());
								getPozice.Nazev = reader["nazev"].ToString();
							}
						}
					}
				}
				connection.Close();
			}
			return getPozice;
		}

		// Metoda vytáhne všechny objekty použité v DB dle vstupního parametru
		public static List<string> GetAllObjects(string name, string dbObject)
		{
			List<string> objectNames = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = $"SELECT {name} AS nazev FROM {dbObject}";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						string? specificObject = null;
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								specificObject = reader["nazev"].ToString();

								objectNames.Add(specificObject);
							}
						}
						else
						{
							specificObject = null;
						}
					}
				}
				connection.Close();
			}
			return objectNames;
		}

		// Metoda vytáhne všechny balíčky použíté v DB
		public static List<string> GetAllPackages()
		{
			List<string> objectNames = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT object_name AS nazev FROM user_objects WHERE object_type = 'PACKAGE'";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						string? specificObject = null;
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								specificObject = reader["nazev"].ToString();

								objectNames.Add(specificObject);
							}
						}
						else
						{
							specificObject = null;
						}
					}
				}
				connection.Close();
			}
			return objectNames;
		}

		// Metoda vytáhne všechny procedury/funkce použíté v DB dle vstupního parametru
		public static List<string> GetAllProceduresFunctions(bool procedure)
		{
			List<string> objectNames = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					if (procedure)
					{
						command.CommandText = "SELECT object_name AS nazev FROM user_procedures WHERE procedure_name IS NULL";
					}
					else
					{
						command.CommandText = "SELECT object_name AS nazev FROM user_procedures WHERE procedure_name IS NOT NULL";
					}
					
					using (OracleDataReader reader = command.ExecuteReader())
					{
						string? specificObject = null;
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								specificObject = reader["nazev"].ToString();

								objectNames.Add(specificObject);
							}
						}
						else
						{
							specificObject = null;
						}
					}
				}
				connection.Close();
			}
			return objectNames;
		}
	}
}