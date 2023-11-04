using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models;
using sem_prace_janousek_mandik.Models.Goods;
using System.Data;

namespace sem_prace_janousek_mandik.Controllers.Goods
{
	public class GoodsSQL
	{

		// Zavolá proceduru na smazání kateogie
        internal static void DeleteCategory(int idKategorie)
        {
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new("P_SMAZAT_KATEGORII", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

					command.Parameters.Add("p_idkategorie", OracleDbType.Int32).Value = idKategorie;

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

		// Zavolá proceduru na úpravu kategorie
        internal static void EditCategory(Kategorie kategorie)
        {
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new("P_UPRAVIT_KATEGORII_V2", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.Add("p_idkategorie", OracleDbType.Int32).Value = kategorie.IdKategorie;
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = kategorie.Nazev;
                    command.Parameters.Add("p_zkratka", OracleDbType.Varchar2).Value = kategorie.Zkratka;
                    command.Parameters.Add("p_popis", OracleDbType.Varchar2).Value = kategorie.Popis;

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        // Metoda vytáhne všechny kategorie
        internal static List<Kategorie> GetAllCategories()
		{
			List<Kategorie> kategorie = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM kategorie";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						Kategorie? jednaKategorie = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								jednaKategorie = new();
								jednaKategorie.IdKategorie = int.Parse(reader["idKategorie"].ToString());
								jednaKategorie.Nazev = reader["nazev"].ToString();
								jednaKategorie.Zkratka = reader["zkratka"].ToString();
								jednaKategorie.Popis = reader["popis"].ToString();

								kategorie.Add(jednaKategorie);
							}
						}
						else
						{
							jednaKategorie = null;
						}
					}
				}
				connection.Close();
			}
			return kategorie;
		}

        internal static bool AddCategory(Kategorie novaKategorie)
        {
            bool registerSuccessful = false;
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new("p_vlozit_kategorii_v2", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Vstupní parametr procedury
                    command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = novaKategorie.Nazev;
                    command.Parameters.Add("p_zkratka", OracleDbType.Varchar2).Value = novaKategorie.Zkratka;
                    command.Parameters.Add("p_popis", OracleDbType.Varchar2).Value = novaKategorie.Popis;

                    command.ExecuteNonQuery();
                }
                connection.Close();
                registerSuccessful = true;
            }
            return registerSuccessful;
        }

        internal static Kategorie GetCategoryById(int index)
        {
            Kategorie getKategorie = new();
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM kategorie WHERE idKategorie = :idKategorie";
                    command.Parameters.Add(":idKategorie", OracleDbType.Int32).Value = index;
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                getKategorie.IdKategorie = int.Parse(reader["idKategorie"].ToString());
                                getKategorie.Nazev = reader["nazev"].ToString();
                                getKategorie.Zkratka = reader["zkratka"].ToString();
                                getKategorie.Popis = reader["popis"].ToString();
                            }
                        }
                    }
                }
                connection.Close();
            }
            return getKategorie;
        }
    }
}
