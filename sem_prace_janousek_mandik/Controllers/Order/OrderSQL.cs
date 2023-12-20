using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models.Goods;
using sem_prace_janousek_mandik.Models.Order;
using System.Data;

namespace sem_prace_janousek_mandik.Controllers.Order
{
	public class OrderSQL
	{
		/// <summary>
		/// Metoda získá všechny objednávky zákazníka
		/// </summary>
		/// <param name="email">Email zákazníka</param>
		/// <returns>List objednávek</returns>
		internal static async Task<List<Objednavky_Zamestnanci_Zakaznici_Faktury>> GetAllCustomerOrders(string email)
		{
			List<Objednavky_Zamestnanci_Zakaznici_Faktury> objednavky = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT o.*, zam.jmeno, zam.prijmeni, zam.email, f.datumSplatnosti, f.castkaDoprava, f.dph FROM objednavky o" +
						" INNER JOIN zamestnanci zam ON o.idZamestnance = zam.idZamestnance" +
						" INNER JOIN zakaznici zak ON o.idZakaznika = zak.idZakaznika" +
						" INNER JOIN faktury f ON o.idFaktury = f.idFaktury" +
						" WHERE zak.email = :zakEmail ORDER BY cisloObjednavky DESC";

					command.Parameters.Add(":zakEmail", OracleDbType.Varchar2).Value = email;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Objednavky_Zamestnanci_Zakaznici_Faktury? objednavka = new();
								objednavka.Objednavky = new();
								objednavka.Zamestnanci = new();
								objednavka.Faktury = new();

								objednavka.Objednavky.IdObjednavky = reader.GetInt32(reader.GetOrdinal("idObjednavky"));
								objednavka.Objednavky.CisloObjednavky = reader.GetInt32(reader.GetOrdinal("cisloObjednavky"));
								objednavka.Objednavky.DatumPrijeti = reader.GetDateTime(reader.GetOrdinal("datumPrijeti"));
								objednavka.Objednavky.Poznamka = reader["poznamka"].ToString();
								objednavka.Objednavky.Uzavrena = await IsClosedOrder(objednavka.Objednavky.IdObjednavky);
								objednavka.Objednavky.IdFaktury = reader.GetInt32(reader.GetOrdinal("idFaktury"));
								objednavka.Objednavky.IdZakaznika = reader.GetInt32(reader.GetOrdinal("idZakaznika"));
								objednavka.Objednavky.IdZamestnance = reader.GetInt32(reader.GetOrdinal("idZamestnance"));

								objednavka.Zamestnanci.Jmeno = reader["jmeno"].ToString();
								objednavka.Zamestnanci.Prijmeni = reader["prijmeni"].ToString();
								objednavka.Zamestnanci.Email = reader["email"].ToString();

								objednavka.Faktury.IdFaktury = reader.GetInt32(reader.GetOrdinal("idFaktury"));
								objednavka.Faktury.DatumSplatnosti = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("datumSplatnosti")));
								objednavka.Faktury.CastkaDoprava = reader.GetFloat(reader.GetOrdinal("castkaDoprava"));
								objednavka.Faktury.Dph = reader.GetFloat(reader.GetOrdinal("dph"));

								objednavky.Add(objednavka);
							}
						}
					}
				}
			}
			return objednavky;
		}

		/// <summary>
		/// Metoda získá všechny objednávky
		/// </summary>
		/// <returns>List objednávek</returns>
		internal static async Task<List<Objednavky_Zamestnanci_Zakaznici_Faktury>> GetAllOrders()
		{
			List<Objednavky_Zamestnanci_Zakaznici_Faktury> objednavky = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT o.*," +
						" zam.jmeno AS zamJmeno," +
						" zam.prijmeni AS zamPrijmeni," +
						" zak.jmeno AS zakJmeno," +
						" zak.prijmeni AS zakPrijmeni," +
						" f.datumSplatnosti, f.castkaDoprava, f.dph " +
						" FROM objednavky o" +
						" INNER JOIN zamestnanci zam ON o.idZamestnance = zam.idZamestnance" +
						" INNER JOIN zakaznici zak ON o.idZakaznika = zak.idZakaznika" +
						" INNER JOIN faktury f ON o.idFaktury = f.idFaktury" +
						" ORDER BY cisloObjednavky DESC";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Objednavky_Zamestnanci_Zakaznici_Faktury? objednavka = new();
								objednavka.Objednavky = new();
								objednavka.Zamestnanci = new();
								objednavka.Zakaznici = new();
								objednavka.Faktury = new();

								objednavka.Objednavky.IdObjednavky = reader.GetInt32(reader.GetOrdinal("idObjednavky"));
								objednavka.Objednavky.CisloObjednavky = reader.GetInt32(reader.GetOrdinal("cisloObjednavky"));
								objednavka.Objednavky.DatumPrijeti = reader.GetDateTime(reader.GetOrdinal("datumPrijeti"));
								objednavka.Objednavky.Poznamka = reader["poznamka"].ToString();
								objednavka.Objednavky.Uzavrena = await IsClosedOrder(objednavka.Objednavky.IdObjednavky);
								objednavka.Objednavky.IdFaktury = reader.GetInt32(reader.GetOrdinal("idFaktury"));
								objednavka.Objednavky.IdZakaznika = reader.GetInt32(reader.GetOrdinal("idZakaznika"));
								objednavka.Objednavky.IdZamestnance = reader.GetInt32(reader.GetOrdinal("idZamestnance"));

								objednavka.Zamestnanci.Jmeno = reader["zamJmeno"].ToString();
								objednavka.Zamestnanci.Prijmeni = reader["zamPrijmeni"].ToString();

								objednavka.Zakaznici.Jmeno = reader["zakJmeno"].ToString();
								objednavka.Zakaznici.Prijmeni = reader["zakPrijmeni"].ToString();

								objednavka.Faktury.IdFaktury = reader.GetInt32(reader.GetOrdinal("idFaktury"));
								objednavka.Faktury.DatumSplatnosti = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("datumSplatnosti")));
								objednavka.Faktury.CastkaDoprava = reader.GetFloat(reader.GetOrdinal("castkaDoprava"));
								objednavka.Faktury.Dph = reader.GetFloat(reader.GetOrdinal("dph"));

								objednavky.Add(objednavka);
							}
						}
					}
				}
			}
			return objednavky;
		}

		/// <summary>
		/// Metoda získá všechno zboží v objednávkách
		/// </summary>
		/// <returns>List objednávek a zboží</returns>
		internal static async Task<List<ZboziObjednavek_Zbozi>> GetAllGoodsOrders()
		{
			List<ZboziObjednavek_Zbozi> zboziObjednavek = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT zo.*, z.nazev FROM zbozi_objednavek zo INNER JOIN zbozi z ON zo.idZbozi = z.idZbozi";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								ZboziObjednavek_Zbozi? zboziObjednavky = new();
								zboziObjednavky.ZboziObjednavek = new();
								zboziObjednavky.Zbozi = new();

								zboziObjednavky.ZboziObjednavek.IdZboziObjednavky = reader.GetInt32(reader.GetOrdinal("idZboziObjednavky"));
								zboziObjednavky.ZboziObjednavek.Mnozstvi = reader.GetInt32(reader.GetOrdinal("mnozstvi"));
								zboziObjednavky.ZboziObjednavek.JednotkovaCena = reader.GetFloat(reader.GetOrdinal("jednotkovaCena"));
								zboziObjednavky.ZboziObjednavek.IdObjednavky = reader.GetInt32(reader.GetOrdinal("idObjednavky"));
								zboziObjednavky.ZboziObjednavek.IdZbozi = reader.GetInt32(reader.GetOrdinal("idZbozi"));

								zboziObjednavky.Zbozi.Nazev = reader["nazev"].ToString();

								zboziObjednavek.Add(zboziObjednavky);
							}
						}
					}
				}
			}
			return zboziObjednavek;
		}

		/// <summary>
		/// Metoda získá zboží v objednávkách zákazníka
		/// </summary>
		/// <param name="email">Email zákazníka</param>
		/// <returns>List objenávek a zboží</returns>
		internal static async Task<List<ZboziObjednavek_Zbozi>> GetAllGoodsOrdersCustomer(string email)
		{
			List<ZboziObjednavek_Zbozi> zboziObjednavek = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT zo.*, z.nazev FROM zbozi_objednavek zo" +
						" INNER JOIN zbozi z ON zo.idZbozi = z.idZbozi" +
						" INNER JOIN objednavky o ON zo.idObjednavky = o.idObjednavky" +
						" INNER JOIN zakaznici zak ON o.idZakaznika = zak.idZakaznika WHERE zak.email = :email";
					command.Parameters.Add(":zakEmail", OracleDbType.Varchar2).Value = email;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								ZboziObjednavek_Zbozi? zboziObjednavky = new();
								zboziObjednavky.ZboziObjednavek = new();
								zboziObjednavky.Zbozi = new();

								zboziObjednavky.ZboziObjednavek.IdZboziObjednavky = reader.GetInt32(reader.GetOrdinal("idZboziObjednavky"));
								zboziObjednavky.ZboziObjednavek.Mnozstvi = reader.GetInt32(reader.GetOrdinal("mnozstvi"));
								zboziObjednavky.ZboziObjednavek.JednotkovaCena = reader.GetFloat(reader.GetOrdinal("jednotkovaCena"));
								zboziObjednavky.ZboziObjednavek.IdObjednavky = reader.GetInt32(reader.GetOrdinal("idObjednavky"));
								zboziObjednavky.ZboziObjednavek.IdZbozi = reader.GetInt32(reader.GetOrdinal("idZbozi"));

								zboziObjednavky.Zbozi.Nazev = reader["nazev"].ToString();

								zboziObjednavek.Add(zboziObjednavky);
							}
						}
					}
				}
			}
			return zboziObjednavek;
		}

		/// <summary>
		/// Metoda získá konkrétní zboží objednávky dle ID
		/// </summary>
		/// <param name="index">ID zboží objednávky</param>
		/// <returns>Konkrétní zboží objednávek</returns>
		internal static async Task<ZboziObjednavek_Zbozi> GetGoodsOrderById(int index)
		{
			ZboziObjednavek_Zbozi getZbozi = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT zo.*, z.nazev AS nazev FROM zbozi_objednavek zo INNER JOIN zbozi z ON zo.idZbozi = z.idZbozi WHERE idZboziObjednavky = :idZboziObjednavky";
					command.Parameters.Add(":idZboziObjednavky", OracleDbType.Int32).Value = index;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								getZbozi.ZboziObjednavek = new();
								getZbozi.Zbozi = new();
								getZbozi.ZboziObjednavek.IdZboziObjednavky = reader.GetInt32(reader.GetOrdinal("idZboziObjednavky"));
								getZbozi.ZboziObjednavek.Mnozstvi = reader.GetInt32(reader.GetOrdinal("mnozstvi"));
								getZbozi.ZboziObjednavek.JednotkovaCena = reader.GetFloat(reader.GetOrdinal("jednotkovaCena"));
								getZbozi.Zbozi.Nazev = reader["nazev"].ToString();
							}
						}
					}
				}
			}
			return getZbozi;
		}

		/// <summary>
		/// Metoda zavolá proceduru na úpravu zboží objednávky
		/// </summary>
		/// <param name="zboziObjednavek">Upravený model zboží objednávky</param>
		/// <returns>true, pokud procedura proběhla úspěšně, jinak false</returns>
		internal static async Task<bool> EditGoodsOrder(ZboziObjednavek_Zbozi zboziObjednavek)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_edit.p_upravit_zbozi_objednavku", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_idZboziObjednavky", OracleDbType.Int32).Value = zboziObjednavek.ZboziObjednavek?.IdZboziObjednavky;
						command.Parameters.Add("p_mnozstvi", OracleDbType.Int32).Value = zboziObjednavek.ZboziObjednavek?.Mnozstvi;
						command.Parameters.Add("p_jednotkovaCena", OracleDbType.BinaryFloat).Value = zboziObjednavek.ZboziObjednavek?.JednotkovaCena;

						command.ExecuteNonQuery();
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Metoda získá ID, názvy, ceny a čárové kódy veškerého zboží
		/// </summary>
		/// <returns>List veškerého zboží</returns>
		internal static async Task<List<Zbozi>> GetAllGoods()
		{
			List<Zbozi> goods = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT idZbozi, nazev, jednotkovaCena, carovyKod FROM zbozi";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Zbozi? specificGoods = new();
								specificGoods.IdZbozi = reader.GetInt32(reader.GetOrdinal("idZbozi"));
								specificGoods.Nazev = reader["nazev"].ToString();
								specificGoods.JednotkovaCena = reader.GetInt32(reader.GetOrdinal("jednotkovaCena"));
								specificGoods.CarovyKod = reader["carovyKod"].ToString();

								goods.Add(specificGoods);
							}
						}
					}
				}
			}
			return goods;
		}

		/// <summary>
		/// Metoda zavolá proceduru na vložení zboží do objednávky
		/// </summary>
		/// <param name="newGoodsOrder">Model nového zboží objednávky</param>
		/// <returns>true, pokud příkaz proběhl úspěšně, jinak false</returns>
		internal static async Task<string?> AddGoodsToOrder(ZboziObjednavek_ZboziList newGoodsOrder)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_insert.P_VLOZIT_ZBOZI_OBJEDNAVKU", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_mnozstvi", OracleDbType.Int32).Value = newGoodsOrder.ZboziObjednavek?.Mnozstvi;
						command.Parameters.Add("p_jednotkovaCena", OracleDbType.BinaryFloat).Value = newGoodsOrder.ZboziObjednavek?.JednotkovaCena;
						command.Parameters.Add("p_idObjednavky", OracleDbType.Int32).Value = newGoodsOrder.ZboziObjednavek?.IdObjednavky;
						command.Parameters.Add("p_idZbozi", OracleDbType.Int32).Value = newGoodsOrder.ZboziObjednavek?.IdZbozi;

						command.ExecuteNonQuery();
					}
				}
				return null;
			}
			catch (OracleException ex)
			{
				if (ex.Number == 20001)
				{
					string fullMessage = ex.Message;
					string firstLine = fullMessage.Split('\n')[0];
					return firstLine;
				}
				else
				{
					return ex.Message;
				}
			}
		}

		/// <summary>
		/// Metoda zkontroluje stav objednávky
		/// </summary>
		/// <param name="idObjednavky">ID objednávky</param>
		/// <returns>true, pokud je objednávka uzavřena, jinak false</returns>
		internal static async Task<bool> IsClosedOrder(int idObjednavky)
		{
			bool objednavkaUzavrena = false;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT uzavrena FROM objednavky WHERE idObjednavky = :idObjednavky";
					command.Parameters.Add(":idObjednavky", OracleDbType.Int32).Value = idObjednavky;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							int objednavkaInt = 0;
							while (reader.Read())
							{
								objednavkaInt = reader.GetInt32(reader.GetOrdinal("uzavrena"));
							}

							if (objednavkaInt == 1)
							{
								objednavkaUzavrena = true;
							}
						}
					}
				}
			}
			return objednavkaUzavrena;
		}

		/// <summary>
		/// Metoda získá konkrétní objednávku na základě ID
		/// </summary>
		/// <param name="index">ID objednávky</param>
		/// <returns>Model konkrétní objednávky</returns>
		internal static async Task<Objednavky> GetOrderById(int index)
		{
			Objednavky objednavka = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM objednavky WHERE idObjednavky = :idObjednavky";
					command.Parameters.Add(":idObjednavky", OracleDbType.Int32).Value = index;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								objednavka.IdObjednavky = reader.GetInt32(reader.GetOrdinal("idObjednavky"));
								objednavka.CisloObjednavky = reader.GetInt32(reader.GetOrdinal("cisloObjednavky"));
								objednavka.DatumPrijeti = reader.GetDateTime(reader.GetOrdinal("datumPrijeti"));
								objednavka.Poznamka = reader["poznamka"].ToString();
								objednavka.Uzavrena = await IsClosedOrder(objednavka.IdObjednavky);
								objednavka.IdFaktury = reader.GetInt32(reader.GetOrdinal("idFaktury"));
								objednavka.IdZamestnance = reader.GetInt32(reader.GetOrdinal("idZamestnance"));
								objednavka.IdZakaznika = reader.GetInt32(reader.GetOrdinal("idZakaznika"));
							}
						}
					}
				}
			}
			return objednavka;
		}

		/// <summary>
		/// Metoda získá cenu konkrétního zboží
		/// </summary>
		/// <param name="idZbozi">ID zboží</param>
		/// <returns>Cena konkrétního zboží</returns>
		internal static async Task<float> GetPriceForGoods(int idZbozi)
		{
			float jednotkovaCena = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT jednotkovacena FROM zbozi WHERE idZbozi = :idZbozi";
					command.Parameters.Add(":idZbozi", OracleDbType.Int32).Value = idZbozi;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								jednotkovaCena = reader.GetFloat(reader.GetOrdinal("jednotkovaCena"));
							}
						}
					}
				}
			}
			return jednotkovaCena;
		}

		/// <summary>
		/// Metoda zavolá proceduru na uzavření objednávky
		/// </summary>
		/// <param name="idObjednavky">ID objednávky</param>
		/// <returns>true, pokud procedura proběhla bez problému, jinak false</returns>
		internal static async Task<bool> CloseOrder(int idObjednavky)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("P_UZAVRIT_OBJEDNAVKU", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_idObjednavky", OracleDbType.Int32).Value = idObjednavky;

						command.ExecuteNonQuery();
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Metoda zavolá proceduru na vložení nové objednávky
		/// </summary>
		/// <param name="newOrder">Model nové objednávky</param>
		/// <returns>true, pokud procedura proběhla bez problému, jinak false</returns>
		internal static async Task<bool> AddOrder(Objednavky_Faktury_ZakList newOrder)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_insert.P_VLOZIT_OBJEDNAVKU", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_datumPrijeti", OracleDbType.Date).Value = newOrder.Objednavky?.DatumPrijeti;
						command.Parameters.Add("p_poznamka", OracleDbType.Varchar2).Value = newOrder.Objednavky?.Poznamka;
						command.Parameters.Add("p_idZakaznika", OracleDbType.Int32).Value = newOrder.Objednavky?.IdZakaznika;
						command.Parameters.Add("p_idZamestnance", OracleDbType.Int32).Value = newOrder.Objednavky?.IdZamestnance;
						command.Parameters.Add("p_castkaDoprava", OracleDbType.BinaryFloat).Value = newOrder.Faktury?.CastkaDoprava;
						command.Parameters.Add("p_dph", OracleDbType.BinaryFloat).Value = newOrder.Faktury?.Dph;

						command.ExecuteNonQuery();
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}

		/// <summary>
		/// Metoda získá fakturu náležící k objednávce zákazníka
		/// </summary>
		/// <param name="idOrder">ID objednávky</param>
		/// <returns>Model faktury a infa o zákazníkovi a objednávce</returns>
		internal static async Task<Objednavky_Zakaznici_Faktury> GetCustomerOrderInvoice(int idOrder)
		{
			Objednavky_Zakaznici_Faktury customer = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT o.idObjednavky, z.email, f.idFaktury, f.castkaObjednavka, f.castkaDoprava, f.dph FROM objednavky o" +
						" INNER JOIN zakaznici z ON o.idZakaznika = z.idZakaznika" +
						" INNER JOIN faktury f ON o.idFaktury = f.idFaktury" +
						" WHERE idObjednavky = :idObjednavky";
					command.Parameters.Add(":idObjednavky", OracleDbType.Int32).Value = idOrder;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								customer.Objednavky = new();
								customer.Zakaznici = new();
								customer.Faktury = new();

								customer.Zakaznici.Email = reader["email"].ToString();
								customer.Objednavky.IdObjednavky = reader.GetInt32(reader.GetOrdinal("idObjednavky"));
								customer.Faktury.IdFaktury = reader.GetInt32(reader.GetOrdinal("idFaktury"));
								customer.Faktury.CastkaObjednavka = reader.GetFloat(reader.GetOrdinal("castkaObjednavka"));
								customer.Faktury.CastkaDoprava = reader.GetFloat(reader.GetOrdinal("castkaDoprava"));
								customer.Faktury.Dph = reader.GetFloat(reader.GetOrdinal("dph"));
							}
						}
					}
				}
			}
			return customer;
		}

		/// <summary>
		/// Metoda vytáhne všechno objednáné zboží konkrétní objednávky
		/// </summary>
		/// <param name="idObjednavky">ID objednávky</param>
		/// <returns>List veškerého zboží v objednávce</returns>
		internal static async Task<List<ZboziObjednavek>> GetAllGoodsOrdersById(int idObjednavky)
		{
			List<ZboziObjednavek> zboziObjednavek = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM zbozi_objednavek WHERE idObjednavky = :idObjednavky";
					command.Parameters.Add(":idObjednavky", OracleDbType.Int32).Value = idObjednavky;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								ZboziObjednavek? zboziObjednavky = new();

								zboziObjednavky.IdZboziObjednavky = reader.GetInt32(reader.GetOrdinal("idZboziObjednavky"));
								zboziObjednavky.Mnozstvi = reader.GetInt32(reader.GetOrdinal("mnozstvi"));
								zboziObjednavky.JednotkovaCena = reader.GetFloat(reader.GetOrdinal("jednotkovaCena"));
								zboziObjednavky.IdObjednavky = reader.GetInt32(reader.GetOrdinal("idObjednavky"));
								zboziObjednavky.IdZbozi = reader.GetInt32(reader.GetOrdinal("idZbozi"));

								zboziObjednavek.Add(zboziObjednavky);
							}
						}
					}
				}
			}
			return zboziObjednavek;
		}

		/// <summary>
		/// Metoda získá veškeré zboží náležící k faktuře
		/// </summary>
		/// <param name="idFaktury">ID faktury</param>
		/// <returns>List zboží</returns>
		internal static async Task<List<ZboziObjednavek_Zbozi>> GetGoodsOrderByIdInvoice(int idFaktury)
		{
			List<ZboziObjednavek_Zbozi> zbozi = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT zo.*, z.nazev AS nazev FROM zbozi_objednavek zo INNER JOIN zbozi z ON zo.idZbozi = z.idZbozi INNER JOIN objednavky o ON zo.idObjednavky = o.idObjednavky INNER JOIN faktury f ON o.idFaktury = f.idFaktury WHERE f.idFaktury = :idFaktury";
					command.Parameters.Add(":idFaktury", OracleDbType.Int32).Value = idFaktury;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								ZboziObjednavek_Zbozi specificGoods = new();

								specificGoods.ZboziObjednavek = new();
								specificGoods.Zbozi = new();
								specificGoods.ZboziObjednavek.IdZboziObjednavky = reader.GetInt32(reader.GetOrdinal("idZboziObjednavky"));
								specificGoods.ZboziObjednavek.Mnozstvi = reader.GetInt32(reader.GetOrdinal("mnozstvi"));
								specificGoods.ZboziObjednavek.JednotkovaCena = reader.GetFloat(reader.GetOrdinal("jednotkovaCena"));
								specificGoods.Zbozi.Nazev = reader["nazev"].ToString();

								zbozi.Add(specificGoods);
							}
						}
					}
				}
			}
			return zbozi;
		}

		/// <summary>
		/// Metoda zavolá proceduru na úpravu objednávky
		/// </summary>
		/// <param name="order">Model s upravenými daty objednávky</param>
		internal static async Task<bool> EditOrder(Objednavky_Zam_Zak_FakturyList order)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_edit.p_upravit_objednavku", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_idobjednavky", OracleDbType.Int32).Value = order.Objednavky?.IdObjednavky;
						command.Parameters.Add("p_datumPrijeti", OracleDbType.Date).Value = order.Objednavky?.DatumPrijeti;
						command.Parameters.Add("p_poznamka", OracleDbType.Varchar2).Value = order.Objednavky?.Poznamka;
						command.Parameters.Add("p_idfaktury", OracleDbType.Int32).Value = order.Objednavky?.IdFaktury;
						command.Parameters.Add("p_idzakaznika", OracleDbType.Int32).Value = order.Objednavky?.IdZakaznika;
						command.Parameters.Add("p_idzamestnance", OracleDbType.Int32).Value = order.Objednavky?.IdZamestnance;

						command.ExecuteNonQuery();
					}
				}
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
