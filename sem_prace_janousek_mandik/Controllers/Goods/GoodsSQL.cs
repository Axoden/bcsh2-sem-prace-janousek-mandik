using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models.Goods;
using sem_prace_janousek_mandik.Models.Management;
using System.Data;

namespace sem_prace_janousek_mandik.Controllers.Goods
{
    public class GoodsSQL
	{
		/// <summary>
		/// Metoda zavolá proceduru na úpravu kategorie
		/// </summary>
		/// <param name="kategorie">Model upravené kategorie</param>
		/// <returns>vrací true, pokud dotaz proběhne úspěšně, jinak false</returns>
		internal static async Task<bool> EditCategory(Kategorie kategorie)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_edit.P_UPRAVIT_KATEGORII_V2", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_idkategorie", OracleDbType.Int32).Value = kategorie.IdKategorie;
						command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = kategorie.Nazev;
						command.Parameters.Add("p_zkratka", OracleDbType.Varchar2).Value = kategorie.Zkratka;
						command.Parameters.Add("p_popis", OracleDbType.Varchar2).Value = kategorie.Popis;
						command.Parameters.Add("p_idnadrazenekategorie", OracleDbType.Int32).Value = kategorie.IdNadrazeneKategorie;

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
		/// Metoda vytáhne všechny kategorie
		/// </summary>
		/// <returns>List všech kategorií</returns>
		internal static async Task<List<Kategorie_NadrazenaKategorie>> GetAllCategories()
		{
			List<Kategorie_NadrazenaKategorie> kategorie = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();

				using (OracleCommand command = new("PROCEDURE1", connection))
				{
					command.CommandText = "SELECT c.idkategorie, c.nazev, c.zkratka, c.popis, k.nazev AS nazevNadrizena FROM kategorie c LEFT JOIN kategorie k ON c.idnadrazenekategorie = k.idkategorie START WITH c.idnadrazenekategorie IS NULL CONNECT BY PRIOR c.idkategorie = c.idnadrazenekategorie ORDER BY zkratka";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							Kategorie_NadrazenaKategorie specificCategory = new();
							specificCategory.Kategorie = new();
							specificCategory.NadrazenaKategorie = new();

							specificCategory.Kategorie.IdKategorie = reader.GetInt32(reader.GetOrdinal("idKategorie"));
							specificCategory.Kategorie.Nazev = reader["nazev"].ToString();
							specificCategory.Kategorie.Zkratka = reader["zkratka"].ToString();
							specificCategory.Kategorie.Popis = reader["popis"].ToString();
							specificCategory.NadrazenaKategorie.Nazev = reader["nazevNadrizena"].ToString();

							kategorie.Add(specificCategory);
						}
					}
				}
			}
			return kategorie;
		}

		/// <summary>
		/// Metoda zavolá funkci na zjištění pohybů zboží na skladě
		/// </summary>
		/// <returns>List pohybů zboží</returns>
		internal static async Task<List<Movements>> GetAllLocationsMovements()
        {
            List<Movements> movements = new();
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
				await connection.OpenAsync();

				using (OracleCommand command = new("pohyby_na_sklade", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					OracleParameter refCursorParam = new()
					{
						OracleDbType = OracleDbType.RefCursor,
						Direction = ParameterDirection.ReturnValue
					};
					command.Parameters.Add(refCursorParam);

					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							Movements movement = new();
							string message = reader.GetString(0);
							if (message.StartsWith("UPDATE"))
							{
								string[] updateSplited = message.Split('\n');
								string[] splittedOld = updateSplited[0].Split(';');
								movement.Operation = splittedOld[0];
								movement.IdZbozi = int.Parse(splittedOld[1]);
								movement.IdUmisteni = int.Parse(splittedOld[2]);

								movements.Add(movement);

								movement = new();
								string[] splittedNew = updateSplited[1].Split(';');
								movement.Operation = splittedNew[0];
								movement.IdZbozi = int.Parse(splittedNew[1]);
								movement.IdUmisteni = int.Parse(splittedNew[2]);
								movements.Add(movement);
							}
							else
							{
								string[] splitted = message.Split(';');
								movement.Operation = splitted[0];
								movement.IdZbozi = int.Parse(splitted[1]);
								movement.IdUmisteni = int.Parse(splitted[2]);
							movements.Add(movement);
							}

						}
					}
				}
			}
            return movements;
        }

		/// <summary>
		/// Metoda vytáhne ID a název veškerého zboží
		/// </summary>
		/// <returns>List veškerého zboží</returns>
		internal static async Task<List<Zbozi>> GetAllGoodsIdName()
		{
			List<Zbozi> goods = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT idZbozi, nazev, idUmisteni FROM zbozi";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Zbozi? specificGoods = new();

								specificGoods.IdZbozi = reader.GetInt32(reader.GetOrdinal("idZbozi"));
								specificGoods.Nazev = reader["nazev"].ToString();
								specificGoods.IdUmisteni = reader.GetInt32(reader.GetOrdinal("idUmisteni"));

								goods.Add(specificGoods);
							}
						}
					}
				}
			}
			return goods;
		}

		/// <summary>
		/// Metoda vytáhne ID a název všech kategorií
		/// </summary>
		/// <returns>List všech kategorií</returns>
		internal static async Task<List<Kategorie>> GetAllCategoriesNameAcronym()
		{
			List<Kategorie> category = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT idKategorie, nazev, zkratka FROM kategorie ORDER BY nazev";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Kategorie? specificCategory = new();

								specificCategory.IdKategorie = reader.GetInt32(reader.GetOrdinal("idKategorie"));
								specificCategory.Nazev = reader["nazev"].ToString();
								specificCategory.Zkratka = reader["zkratka"].ToString();

								category.Add(specificCategory);
							}
						}
					}
				}
			}
			return category;
		}

		/// <summary>
		/// Metoda zavolá proceduru na přidání nové kategorie
		/// </summary>
		/// <param name="newCategory">Model nové kategorie</param>
		/// <returns>vrací true, pokud SQL příkaz proběhne úspěšně, jinak false</returns>
		internal static async Task<bool> AddCategory(Kategorie newCategory)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_insert.p_vlozit_kategorii_v2", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = newCategory.Nazev;
						command.Parameters.Add("p_zkratka", OracleDbType.Varchar2).Value = newCategory.Zkratka;
						command.Parameters.Add("p_popis", OracleDbType.Varchar2).Value = newCategory.Popis;
						command.Parameters.Add("p_idnadrazenekategorie", OracleDbType.Varchar2).Value = newCategory.IdNadrazeneKategorie;

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
		/// Metoda vytáhne konkrétní kategorii na základě jejího ID
		/// </summary>
		/// <param name="idKategorie">ID kategorie</param>
		/// <returns>Model konkrétní kategorie</returns>
		internal static async Task<Kategorie> GetCategoryById(int idKategorie)
		{
			Kategorie getKategorie = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM kategorie WHERE idKategorie = :idKategorie";
					command.Parameters.Add(":idKategorie", OracleDbType.Int32).Value = idKategorie;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								getKategorie.IdKategorie = reader.GetInt32(reader.GetOrdinal("idKategorie"));
								getKategorie.Nazev = reader["nazev"].ToString();
								getKategorie.Zkratka = reader["zkratka"].ToString();
								getKategorie.Popis = reader["popis"].ToString();
								if (!reader["idnadrazeneKategorie"].ToString().Equals(""))
								{
									getKategorie.IdNadrazeneKategorie = reader.GetInt32(reader.GetOrdinal("idNadrazeneKategorie"));
								}
							}
						}
					}
				}
			}
			return getKategorie;
		}

		/// <summary>
		/// Metoda vytáhne veškeré zboží včetně navazujících informací o dodavatelích, kategoriích, umístěních a souborech
		/// </summary>
		/// <returns>List veškerého zboží</returns>
		internal static async Task<List<Zbozi_Um_Kat_Dod_Soubory>> GetAllGoodsWithLocationCategorySupplier()
		{
			List<Zbozi_Um_Kat_Dod_Soubory> zbozi = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT z.*, k.*, u.*, d.*, k.nazev AS kategorieNazev, d.nazev AS dodavatelNazev, u.poznamka AS poznamkaUmisteni," +
						" s.data, s.nazev AS souborNazev FROM zbozi z INNER JOIN kategorie k ON z.idkategorie = k.idkategorie INNER JOIN umisteni u ON z.idumisteni = u.idumisteni " +
						"INNER JOIN dodavatele d ON z.iddodavatele = d.iddodavatele LEFT JOIN soubory s ON z.idsouboru = s.idsouboru ORDER BY z.nazev";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Zbozi_Um_Kat_Dod_Soubory? jednoZbozi = new();
								jednoZbozi.Zbozi = new();
								jednoZbozi.Kategorie = new();
								jednoZbozi.Umisteni = new();
								jednoZbozi.Dodavatele = new();
								jednoZbozi.Soubory = new();

								jednoZbozi.Zbozi.IdZbozi = reader.GetInt32(reader.GetOrdinal("idZbozi"));
								jednoZbozi.Zbozi.Nazev = reader["nazev"].ToString();
								jednoZbozi.Zbozi.JednotkovaCena = reader.GetFloat(reader.GetOrdinal("jednotkovaCena"));
								jednoZbozi.Zbozi.PocetNaSklade = reader.GetInt32(reader.GetOrdinal("pocetNaSklade"));
								jednoZbozi.Zbozi.CarovyKod = reader["carovyKod"].ToString();
								jednoZbozi.Zbozi.Poznamka = reader["poznamka"].ToString();
								jednoZbozi.Zbozi.IdDodavatele = reader.GetInt32(reader.GetOrdinal("idDodavatele"));
								jednoZbozi.Zbozi.IdUmisteni = reader.GetInt32(reader.GetOrdinal("idUmisteni"));
								jednoZbozi.Zbozi.IdKategorie = reader.GetInt32(reader.GetOrdinal("idKategorie"));

								jednoZbozi.Kategorie.IdKategorie = reader.GetInt32(reader.GetOrdinal("idKategorie"));
								jednoZbozi.Kategorie.Nazev = reader["kategorieNazev"].ToString();
								jednoZbozi.Kategorie.Zkratka = reader["zkratka"].ToString();

								jednoZbozi.Umisteni.IdUmisteni = reader.GetInt32(reader.GetOrdinal("idumisteni"));
								jednoZbozi.Umisteni.Mistnost = reader["mistnost"].ToString();
								jednoZbozi.Umisteni.Rada = reader["rada"].ToString();
								jednoZbozi.Umisteni.Regal = reader["regal"].ToString();
								jednoZbozi.Umisteni.Pozice = reader["pozice"].ToString();
								jednoZbozi.Umisteni.Datum = reader.GetDateTime(reader.GetOrdinal("datum"));
								jednoZbozi.Umisteni.Poznamka = reader["poznamkaUmisteni"].ToString();

								jednoZbozi.Dodavatele.IdDodavatele = reader.GetInt32(reader.GetOrdinal("iddodavatele"));
								jednoZbozi.Dodavatele.Nazev = reader["dodavatelNazev"].ToString();
								jednoZbozi.Dodavatele.Jmeno = reader["jmeno"].ToString();
								jednoZbozi.Dodavatele.Prijmeni = reader["prijmeni"].ToString();
								jednoZbozi.Dodavatele.Telefon = reader["telefon"].ToString();
								jednoZbozi.Dodavatele.Email = reader["email"].ToString();

								jednoZbozi.Soubory.Data = reader["data"] as byte[];
								jednoZbozi.Soubory.Nazev = reader["souborNazev"].ToString();

								zbozi.Add(jednoZbozi);
							}
						}
					}
				}
			}
			return zbozi;
		}

		/// <summary>
		/// Metoda zkontroluje, zda již neexistuje kategorie se stejnou zkratkou
		/// </summary>
		/// <param name="zkratka">Zkratka, která má být zkontrolována</param>
		/// <returns>true, pokud zkratka již existuje, jinak false</returns>
		internal static async Task<bool> CheckExistsAcronym(string zkratka)
		{
			bool exists = true;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT idKategorie FROM kategorie WHERE zkratka = :zkratka";
					command.Parameters.Add(":zkratka", zkratka);
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
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
			}
			return exists;
		}

		/// <summary>
		/// Metoda získá zkratku konkrétní kategorie na základě ID kategorie
		/// </summary>
		/// <param name="idKategorie">ID kategorie</param>
		/// <returns>Název zkratky</returns>
		internal static async Task<string?> GetAcronymByIdCategory(int idKategorie)
		{
			string? zkratka = null;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT zkratka FROM kategorie WHERE idKategorie = :idKategorie";
					command.Parameters.Add(":idKategorie", idKategorie);
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								zkratka = reader["zkratka"].ToString();
							}
						}
					}
				}
			}
			return zkratka;
		}

		/// <summary>
		/// Metoda zkontroluje, zda již neexistuje stejný čárový kód
		/// </summary>
		/// <param name="carovyKod">Čárový kód, který má být zkontrolován</param>
		/// <returns>true, pokud zkratka již existuje, jinak false</returns>
		internal static async Task<bool> CheckExistsBarcode(string carovyKod)
		{
			bool exists = true;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT idZbozi FROM zbozi WHERE carovyKod = :carovyKod";
					command.Parameters.Add(":carovyKod", carovyKod);
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
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
			}
			return exists;
		}

		/// <summary>
		/// Metoda získá čárový konkrétního zboží na základě ID zboží
		/// </summary>
		/// <param name="idZbozi">ID zboží</param>
		/// <returns>Čárový kód</returns>
		internal static async Task<string?> GetBarcodeByIdGoods(int idZbozi)
		{
			string? barcode = null;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT carovykod FROM zbozi WHERE idZbozi = :idZbozi";
					command.Parameters.Add(":idZbozi", idZbozi);
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								barcode = reader["carovyKod"].ToString();
							}
						}
					}
				}
			}
			return barcode;
		}

		/// <summary>
		/// Metoda zavolá proceduru na vložení nového zboží
		/// </summary>
		/// <param name="newGoods">Model nového zboží</param>
		/// <param name="files">Obrázek včetně parametrů (nepovinný)</param>
		/// <returns>vrací true, pokud SQL příkaz proběhne úspěšně, jinak false</returns>
		internal static async Task<string?> AddGoods(Zbozi newGoods, Soubory files)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_insert.P_VLOZIT_ZBOZI", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = newGoods.Nazev;
						command.Parameters.Add("p_jednotkovaCena", OracleDbType.BinaryFloat).Value = newGoods.JednotkovaCena;
						command.Parameters.Add("p_pocetNaSklade", OracleDbType.Int32).Value = newGoods.PocetNaSklade;
						command.Parameters.Add("p_carovyKod", OracleDbType.Varchar2).Value = newGoods.CarovyKod;
						command.Parameters.Add("p_poznamka", OracleDbType.Varchar2).Value = newGoods.Poznamka;
						command.Parameters.Add("p_idDodavatele", OracleDbType.Int32).Value = newGoods.IdDodavatele;
						command.Parameters.Add("p_idUmisteni", OracleDbType.Int32).Value = newGoods.IdUmisteni;
						command.Parameters.Add("p_idKategorie", OracleDbType.Int32).Value = newGoods.IdKategorie;

						command.Parameters.Add("p_nazevSouboru", OracleDbType.Varchar2).Value = files.Nazev;
						command.Parameters.Add("p_typ", OracleDbType.Varchar2).Value = files.TypSouboru;
						command.Parameters.Add("p_pripona", OracleDbType.Varchar2).Value = files.PriponaSouboru;
						command.Parameters.Add("p_data", OracleDbType.Blob).Value = files.Data;
						command.Parameters.Add("p_idZamestnanec", OracleDbType.Int32).Value = files.IdZamestnance;

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
				else if (ex.Number == 20002)
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
		/// Metoda získá konkrétní zboží na základě ID zboží
		/// </summary>
		/// <param name="idZbozi">ID zboží</param>
		/// <returns>Model konkrétního zboží</returns>
		internal static async Task<Zbozi> GetGoodsById(int idZbozi)
		{
			Zbozi getZbozi = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM zbozi WHERE idZbozi = :idZbozi";
					command.Parameters.Add(":idZbozi", OracleDbType.Int32).Value = idZbozi;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								getZbozi = new();
								getZbozi.IdZbozi = reader.GetInt32(reader.GetOrdinal("idZbozi"));
								getZbozi.Nazev = reader["nazev"].ToString();
								getZbozi.JednotkovaCena = reader.GetFloat(reader.GetOrdinal("jednotkovaCena"));
								getZbozi.PocetNaSklade = reader.GetInt32(reader.GetOrdinal("pocetNaSklade"));
								getZbozi.CarovyKod = reader["carovyKod"].ToString();
								getZbozi.Poznamka = reader["poznamka"].ToString();
								getZbozi.IdDodavatele = reader.GetInt32(reader.GetOrdinal("idDodavatele"));
								getZbozi.IdUmisteni = reader.GetInt32(reader.GetOrdinal("idUmisteni"));
								getZbozi.IdKategorie = reader.GetInt32(reader.GetOrdinal("idKategorie"));
								if (reader["idSouboru"].ToString().Equals(""))
								{
									getZbozi.IdSouboru = 0;
								}
								else
								{
									getZbozi.IdSouboru = reader.GetInt32(reader.GetOrdinal("idSouboru"));
								}
							}
						}
					}
				}
			}
			return getZbozi;
		}

		/// <summary>
		/// Metoda získá veškerá umístění
		/// </summary>
		/// <returns>List všech umístění</returns>
		internal static async Task<List<Umisteni>> GetAllLocations()
		{
			List<Umisteni> umisteni = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM umisteni ORDER BY mistnost, rada, regal, pozice";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Umisteni? jednoUmisteni = new();

								jednoUmisteni.IdUmisteni = reader.GetInt32(reader.GetOrdinal("idUmisteni"));
								jednoUmisteni.Mistnost = reader["mistnost"].ToString();
								jednoUmisteni.Rada = reader["rada"].ToString();
								jednoUmisteni.Regal = reader["regal"].ToString();
								jednoUmisteni.Pozice = reader["pozice"].ToString();
								jednoUmisteni.Datum = reader.GetDateTime(reader.GetOrdinal("datum"));
								jednoUmisteni.Poznamka = reader["poznamka"].ToString();

								umisteni.Add(jednoUmisteni);
							}
						}
					}
				}
			}
			return umisteni;
		}

		/// <summary>
		/// Metoda získá vybrané parametry všech umístění
		/// </summary>
		/// <returns>List všech umístění</returns>
		internal static async Task<List<Umisteni>> GetAllLocationsPositions()
		{
			List<Umisteni> locations = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT idUmisteni, mistnost, rada, regal, pozice FROM umisteni ORDER BY mistnost, rada, regal, pozice";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								Umisteni? specificLocation = new();

								specificLocation.IdUmisteni = reader.GetInt32(reader.GetOrdinal("idUmisteni"));
								specificLocation.Mistnost = reader["mistnost"].ToString();
								specificLocation.Rada = reader["rada"].ToString();
								specificLocation.Regal = reader["regal"].ToString();
								specificLocation.Pozice = reader["pozice"].ToString();

								locations.Add(specificLocation);
							}
						}
					}
				}
			}
			return locations;
		}

		/// <summary>
		/// Metoda zavolá proceduru na úpravu zboží
		/// </summary>
		/// <param name="zbozi">Model upraveného zboží</param>
		/// <param name="files">Obrázek včetně parametrů (nepovinný)</param>
		/// <returns>vrací true, pokud SQL příkaz proběhne úspěšně, jinak false</returns>
		internal static async Task<string?> EditGoods(Zbozi zbozi, Soubory files)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_edit.P_UPRAVIT_ZBOZI_V2", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_idZbozi", OracleDbType.Int32).Value = zbozi.IdZbozi;
						command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = zbozi.Nazev;
						command.Parameters.Add("p_jednotkovaCena", OracleDbType.BinaryFloat).Value = zbozi.JednotkovaCena;
						command.Parameters.Add("p_pocetNaSklade", OracleDbType.Int32).Value = zbozi.PocetNaSklade;
						command.Parameters.Add("p_carovyKod", OracleDbType.Varchar2).Value = zbozi.CarovyKod;
						command.Parameters.Add("p_poznamka", OracleDbType.Varchar2).Value = zbozi.Poznamka;
						command.Parameters.Add("p_idDodavatele", OracleDbType.Int32).Value = zbozi.IdDodavatele;
						command.Parameters.Add("p_idUmisteni", OracleDbType.Int32).Value = zbozi.IdUmisteni;
						command.Parameters.Add("p_idKategorie", OracleDbType.Int32).Value = zbozi.IdKategorie;

						command.Parameters.Add("p_idSouboru", OracleDbType.Int32).Value = zbozi.IdSouboru;
						command.Parameters.Add("p_nazevSouboru", OracleDbType.Varchar2).Value = files.Nazev;
						command.Parameters.Add("p_typ", OracleDbType.Varchar2).Value = files.TypSouboru;
						command.Parameters.Add("p_pripona", OracleDbType.Varchar2).Value = files.PriponaSouboru;
						command.Parameters.Add("p_data", OracleDbType.Blob).Value = files.Data;
						command.Parameters.Add("p_idZamestnance", OracleDbType.Int32).Value = files.IdZamestnance;

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
				else if (ex.Number == 20002)
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
		/// Metoda zavolá proceduru na vložení nového umístění
		/// </summary>
		/// <param name="newLocation">Model nového umístění</param>
		/// <returns>vrací true, pokud SQL příkaz proběhne úspěšně, jinak false</returns>
		internal static async Task<bool> AddLocation(Umisteni newLocation)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_insert.P_VLOZIT_UMISTENI_V2", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_mistnost", OracleDbType.Varchar2).Value = newLocation.Mistnost;
						command.Parameters.Add("p_rada", OracleDbType.Varchar2).Value = newLocation.Rada;
						command.Parameters.Add("p_regal", OracleDbType.Varchar2).Value = newLocation.Regal;
						command.Parameters.Add("p_pozice", OracleDbType.Varchar2).Value = newLocation.Pozice;
						command.Parameters.Add("p_datum", OracleDbType.Date).Value = newLocation.Datum;
						command.Parameters.Add("p_poznamka", OracleDbType.Varchar2).Value = newLocation.Poznamka;

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
		/// Metoda získá umístění na základě ID umístění
		/// </summary>
		/// <param name="idUmisteni">ID umístění</param>
		/// <returns>Model konkrétního umístění</returns>
		internal static async Task<Umisteni> GetLocationById(int idUmisteni)
		{
			Umisteni getLocation = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM umisteni WHERE idUmisteni = :idUmisteni";
					command.Parameters.Add(":idUmisteni", OracleDbType.Int32).Value = idUmisteni;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								getLocation.IdUmisteni = reader.GetInt32(reader.GetOrdinal("idUmisteni"));
								getLocation.Mistnost = reader["mistnost"].ToString();
								getLocation.Rada = reader["rada"].ToString();
								getLocation.Regal = reader["regal"].ToString();
								getLocation.Pozice = reader["pozice"].ToString();
								getLocation.Datum = reader.GetDateTime(reader.GetOrdinal("datum"));
								getLocation.Poznamka = reader["poznamka"].ToString();
							}
						}
					}
				}
			}
			return getLocation;
		}

		/// <summary>
		/// Metoda zavolá proceduru na úpravu umístění
		/// </summary>
		/// <param name="umisteni">Model upravovaného umístění</param>
		/// <returns>vrací true, pokud SQL příkaz proběhne úspěšně, jinak false</returns>
		internal static async Task<bool> EditLocation(Umisteni umisteni)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_edit.P_UPRAVIT_UMISTENI_V2", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_idUmisteni", OracleDbType.Int32).Value = umisteni.IdUmisteni;
						command.Parameters.Add("p_mistnost", OracleDbType.Varchar2).Value = umisteni.Mistnost;
						command.Parameters.Add("p_rada", OracleDbType.Varchar2).Value = umisteni.Rada;
						command.Parameters.Add("p_regal", OracleDbType.Varchar2).Value = umisteni.Regal;
						command.Parameters.Add("p_pozice", OracleDbType.Varchar2).Value = umisteni.Pozice;
						command.Parameters.Add("p_datum", OracleDbType.Date).Value = umisteni.Datum;
						command.Parameters.Add("p_poznamka", OracleDbType.Varchar2).Value = umisteni.Poznamka;

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
		/// Metoda vytáhne ID, název, zkratku všech kategorií
		/// </summary>
		/// <returns>List všech kategorií</returns>
		internal static async Task<List<Kategorie>> GetAllCategoriesIdNameAcronym()
		{
			List<Kategorie> kategorie = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT idkategorie, nazev, zkratka FROM kategorie ORDER BY nazev";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Kategorie? specificCateogory = new();

								specificCateogory.IdKategorie = reader.GetInt32(reader.GetOrdinal("idKategorie"));
								specificCateogory.Nazev = reader["nazev"].ToString();
								specificCateogory.Zkratka = reader["zkratka"].ToString();

								kategorie.Add(specificCateogory);
							}
						}
					}
				}
			}
			return kategorie;
		}

		/// <summary>
		/// Zavolá proceduru na smazání kategorie
		/// </summary>
		/// <param name="id">Vstupní parametr ID procedury</param>
		/// <returns>null, pokud procedura proběhla úspěšně, jinak false</returns>
		internal static async Task<string?> DeleteCategory(int id)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_delete.P_SMAZAT_KATEGORII", connection))
					{
						command.CommandType = CommandType.StoredProcedure;
						command.Parameters.Add("p_id", OracleDbType.Int32).Value = id;
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
	}
}