using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models;
using sem_prace_janousek_mandik.Models.Goods;
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
		internal static bool EditCategory(Kategorie kategorie)
		{
			try
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
						command.Parameters.Add("p_idnadrazenekategorie", OracleDbType.Int32).Value = kategorie.IdNadrazeneKategorie;

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
		/// Metoda vytáhne všechny kategorie
		/// </summary>
		/// <returns>List všech kategorií</returns>
		internal static List<Kategorie_NadrazenaKategorie> GetAllCategories()
		{
			List<Kategorie_NadrazenaKategorie> kategorie = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();

				using (OracleCommand cmd = new("PROCEDURE1", connection))
				{
					cmd.CommandType = CommandType.StoredProcedure;
					Kategorie_NadrazenaKategorie specificCategory = new();
					// Define the REF CURSOR output parameter
					OracleParameter p_hierarchy = new();
					p_hierarchy.OracleDbType = OracleDbType.RefCursor;
					p_hierarchy.Direction = ParameterDirection.Output;
					cmd.Parameters.Add(p_hierarchy);

					// Execute the command
					using (OracleDataReader reader = cmd.ExecuteReader())
					{
						while (reader.Read())
						{
							// Assuming you're fetching data from the reader
							// Replace these with the actual column names and data types
							specificCategory = new();
							specificCategory.Kategorie = new();
							specificCategory.NadrazenaKategorie = new();

							specificCategory.Kategorie.IdKategorie = reader.GetInt32(0);
							specificCategory.Kategorie.Nazev = reader.GetString(1);
							specificCategory.Kategorie.Zkratka = reader.GetString(2);
							specificCategory.Kategorie.Popis = reader.IsDBNull(3) ? null : reader.GetString(3);
							specificCategory.NadrazenaKategorie.Nazev = reader.IsDBNull(4) ? null : reader.GetString(4);

							kategorie.Add(specificCategory);
						}
					}
				}
				connection.Close();
			}
			return kategorie;
		}

		/// <summary>
		/// Metoda vytáhne ID a název všech kategorií
		/// </summary>
		/// <returns>List všech kategorií</returns>
		internal static List<Kategorie> GetAllCategoriesNameAcronym()
		{
			List<Kategorie> category = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT idKategorie, nazev, zkratka FROM kategorie";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						Kategorie? specificCateogory = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								specificCateogory = new();

								specificCateogory.IdKategorie = int.Parse(reader["idKategorie"].ToString());
								specificCateogory.Nazev = reader["nazev"].ToString();
								specificCateogory.Zkratka = reader["zkratka"].ToString();

								category.Add(specificCateogory);
							}
						}
						else
						{
							specificCateogory = null;
						}
					}
				}
				connection.Close();
			}
			return category;
		}

		/// <summary>
		/// Metoda zavolá proceduru na přidání nové kategorie
		/// </summary>
		/// <param name="newCategory">Model nové kategorie</param>
		/// <returns>vrací true, pokud SQL příkaz proběhne úspěšně, jinak false</returns>
		internal static bool AddCategory(Kategorie newCategory)
		{
			try
			{


				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					connection.Open();
					using (OracleCommand command = new("p_vlozit_kategorii_v2", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						// Vstupní parametr procedury
						command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = newCategory.Nazev;
						command.Parameters.Add("p_zkratka", OracleDbType.Varchar2).Value = newCategory.Zkratka;
						command.Parameters.Add("p_popis", OracleDbType.Varchar2).Value = newCategory.Popis;
						command.Parameters.Add("p_idnadrazenekategorie", OracleDbType.Varchar2).Value = newCategory.IdNadrazeneKategorie;

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
		/// Metoda vytáhne konkrétní kategorii na základě jejího ID
		/// </summary>
		/// <param name="idKategorie">ID kategorie</param>
		/// <returns>Model konkrétní kategorie</returns>
		internal static Kategorie GetCategoryById(int idKategorie)
		{
			Kategorie getKategorie = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM kategorie WHERE idKategorie = :idKategorie";
					command.Parameters.Add(":idKategorie", OracleDbType.Int32).Value = idKategorie;
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
								if (!reader["idnadrazeneKategorie"].ToString().Equals(""))
								{
									getKategorie.IdNadrazeneKategorie = int.Parse(reader["idnadrazeneKategorie"].ToString());
								}
							}
						}
					}
				}
				connection.Close();
			}
			return getKategorie;
		}

		/// <summary>
		/// Metoda vytáhne veškeré zboží včetně navazujících informací o dodavatelích, kategoriích, umístěních a souborech
		/// </summary>
		/// <returns>List veškerého zboží</returns>
		internal static List<Zbozi_Um_Kat_Dod_Soubory> GetAllGoodsWithLocationCategorySupplier()
		{
			List<Zbozi_Um_Kat_Dod_Soubory> zbozi = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT z.*, k.*, u.*, d.*, k.nazev AS kategorieNazev, d.nazev AS dodavatelNazev, u.poznamka AS poznamkaUmisteni," +
						" s.data, s.nazev AS souborNazev FROM zbozi z INNER JOIN kategorie k ON z.idkategorie = k.idkategorie INNER JOIN umisteni u ON z.idumisteni = u.idumisteni " +
						"INNER JOIN dodavatele d ON z.iddodavatele = d.iddodavatele LEFT JOIN soubory s ON z.idsouboru = s.idsouboru";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						Zbozi_Um_Kat_Dod_Soubory? jednoZbozi = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								jednoZbozi = new();
								jednoZbozi.Zbozi = new();
								jednoZbozi.Kategorie = new();
								jednoZbozi.Umisteni = new();
								jednoZbozi.Dodavatele = new();
								jednoZbozi.Soubory = new();

								jednoZbozi.Zbozi.IdZbozi = int.Parse(reader["idZbozi"].ToString());
								jednoZbozi.Zbozi.Nazev = reader["nazev"].ToString();
								jednoZbozi.Zbozi.JednotkovaCena = float.Parse(reader["jednotkovaCena"].ToString());
								jednoZbozi.Zbozi.PocetNaSklade = int.Parse(reader["pocetNaSklade"].ToString());
								jednoZbozi.Zbozi.CarovyKod = reader["carovyKod"].ToString();
								jednoZbozi.Zbozi.Poznamka = reader["poznamka"].ToString();
								jednoZbozi.Zbozi.IdDodavatele = int.Parse(reader["idDodavatele"].ToString());
								jednoZbozi.Zbozi.IdUmisteni = int.Parse(reader["idUmisteni"].ToString());
								jednoZbozi.Zbozi.IdKategorie = int.Parse(reader["idKategorie"].ToString());

								jednoZbozi.Kategorie.IdKategorie = int.Parse(reader["idkategorie"].ToString());
								jednoZbozi.Kategorie.Nazev = reader["kategorieNazev"].ToString();
								jednoZbozi.Kategorie.Zkratka = reader["zkratka"].ToString();
								jednoZbozi.Kategorie.Popis = reader["popis"].ToString();

								jednoZbozi.Umisteni.IdUmisteni = int.Parse(reader["idumisteni"].ToString());
								jednoZbozi.Umisteni.Mistnost = reader["mistnost"].ToString();
								jednoZbozi.Umisteni.Rada = reader["rada"].ToString();
								jednoZbozi.Umisteni.Regal = reader["regal"].ToString();
								jednoZbozi.Umisteni.Pozice = reader["pozice"].ToString();
								jednoZbozi.Umisteni.Datum = DateTime.Parse(reader["datum"].ToString());
								jednoZbozi.Umisteni.Poznamka = reader["poznamkaUmisteni"].ToString();

								jednoZbozi.Dodavatele.IdDodavatele = int.Parse(reader["iddodavatele"].ToString());
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
						else
						{
							jednoZbozi = null;
						}
					}
				}
				connection.Close();
			}
			return zbozi;
		}

		/// <summary>
		/// Metoda zkontroluje, zda již neexistuje kategorie se stejnou zkratkou
		/// </summary>
		/// <param name="zkratka">Zkratka, která má být zkontrolována</param>
		/// <returns>true, pokud zkratka již existuje, jinak false</returns>
		internal static bool CheckExistsAcronym(string zkratka)
		{
			bool exists = true;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT idKategorie FROM kategorie WHERE zkratka = :zkratka";
					command.Parameters.Add(":zkratka", zkratka);
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
		/// Metoda získá zkratku konkrétní kategorie na základě ID kategorie
		/// </summary>
		/// <param name="idKategorie">ID kategorie</param>
		/// <returns>Název zkratky</returns>
		internal static string GetAcronymByIdCategory(int idKategorie)
		{
			string? zkratka = null;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT zkratka FROM kategorie WHERE idKategorie = :idKategorie";
					command.Parameters.Add(":idKategorie", idKategorie);
					using (OracleDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								zkratka = reader["zkratka"].ToString();
							}
						}
					}
				}
				connection.Close();
			}
			return zkratka;
		}

		/// <summary>
		/// Metoda zkontroluje, zda již neexistuje stejný čárový kód
		/// </summary>
		/// <param name="carovyKod">Čárový kód, který má být zkontrolován</param>
		/// <returns>true, pokud zkratka již existuje, jinak false</returns>
		internal static bool CheckExistsBarcode(string carovyKod)
		{
			bool exists = true;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT idZbozi FROM zbozi WHERE carovyKod = :carovyKod";
					command.Parameters.Add(":carovyKod", carovyKod);
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
		/// Metoda získá čárový konkrétního zboží na základě ID zboží
		/// </summary>
		/// <param name="idZbozi">ID zboží</param>
		/// <returns>Čárový kód</returns>
		internal static string GetBarcodeByIdGoods(int idZbozi)
		{
			string? barcode = null;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT carovykod FROM zbozi WHERE idZbozi = :idZbozi";
					command.Parameters.Add(":idZbozi", idZbozi);
					using (OracleDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								barcode = reader["carovyKod"].ToString();
							}
						}
					}
				}
				connection.Close();
			}
			return barcode;
		}

		/// <summary>
		/// Metoda zavolá proceduru na vložení nového zboží
		/// </summary>
		/// <param name="newGoods">Model nového zboží</param>
		/// <param name="files">Obrázek včetně parametrů (nepovinný)</param>
		/// <returns>vrací true, pokud SQL příkaz proběhne úspěšně, jinak false</returns>
		internal static string? AddGoods(Zbozi newGoods, Soubory files)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					connection.Open();
					using (OracleCommand command = new("P_VLOZIT_ZBOZI", connection))
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
						command.Parameters.Add("p_idZamestnanec", OracleDbType.Int32).Value = files.idZamestnance;

						command.ExecuteNonQuery();
					}
					connection.Close();
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
		internal static Zbozi GetGoodsById(int idZbozi)
		{
			Zbozi getZbozi = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM zbozi WHERE idZbozi = :idZbozi";
					command.Parameters.Add(":idZbozi", OracleDbType.Int32).Value = idZbozi;
					using (OracleDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								getZbozi = new();
								getZbozi.IdZbozi = int.Parse(reader["idZbozi"].ToString());
								getZbozi.Nazev = reader["nazev"].ToString();
								getZbozi.JednotkovaCena = float.Parse(reader["jednotkovaCena"].ToString());
								getZbozi.PocetNaSklade = int.Parse(reader["pocetNaSklade"].ToString());
								getZbozi.CarovyKod = reader["carovyKod"].ToString();
								getZbozi.Poznamka = reader["poznamka"].ToString();
								getZbozi.IdDodavatele = int.Parse(reader["idDodavatele"].ToString());
								getZbozi.IdUmisteni = int.Parse(reader["idUmisteni"].ToString());
								getZbozi.IdKategorie = int.Parse(reader["idKategorie"].ToString());
								if (reader["idSouboru"].ToString().Equals(""))
								{
									getZbozi.IdSouboru = 0;
                                }
								else
								{
									getZbozi.IdSouboru = int.Parse(reader["idSouboru"].ToString());
								}
                            }
						}
					}
				}
				connection.Close();
			}
			return getZbozi;
		}

		/// <summary>
		/// Metoda získá veškerá umístění
		/// </summary>
		/// <returns>List všech umístění</returns>
		internal static List<Umisteni> GetAllLocations()
		{
			List<Umisteni> umisteni = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM umisteni";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						Umisteni? jednoUmisteni = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								jednoUmisteni = new();

								jednoUmisteni.IdUmisteni = int.Parse(reader["idumisteni"].ToString());
								jednoUmisteni.Mistnost = reader["mistnost"].ToString();
								jednoUmisteni.Rada = reader["rada"].ToString();
								jednoUmisteni.Regal = reader["regal"].ToString();
								jednoUmisteni.Pozice = reader["pozice"].ToString();
								jednoUmisteni.Datum = DateTime.Parse(reader["datum"].ToString());
								jednoUmisteni.Poznamka = reader["poznamka"].ToString();

								umisteni.Add(jednoUmisteni);
							}
						}
						else
						{
							jednoUmisteni = null;
						}
					}
				}
				connection.Close();
			}
			return umisteni;
		}
		
		/// <summary>
		/// Metoda získá vybrané parametry všech umístění
		/// </summary>
		/// <returns>List všech umístění</returns>
		internal static List<Umisteni> GetAllLocationsPositions()
		{
			List<Umisteni> locations = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT idUmisteni, mistnost, rada, regal, pozice FROM umisteni";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						Umisteni? specificLocation = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								specificLocation = new();

								specificLocation.IdUmisteni = int.Parse(reader["idumisteni"].ToString());
								specificLocation.Mistnost = reader["mistnost"].ToString();
								specificLocation.Rada = reader["rada"].ToString();
								specificLocation.Regal = reader["regal"].ToString();
								specificLocation.Pozice = reader["pozice"].ToString();

								locations.Add(specificLocation);
							}
						}
						else
						{
							specificLocation = null;
						}
					}
				}
				connection.Close();
			}
			return locations;
		}

		/// <summary>
		/// Metoda zavolá proceduru na úpravu zboží
		/// </summary>
		/// <param name="zbozi">Model upraveného zboží</param>
		/// <param name="files">Obrázek včetně parametrů (nepovinný)</param>
		/// <returns>vrací true, pokud SQL příkaz proběhne úspěšně, jinak false</returns>
		internal static string? EditGoods(Zbozi zbozi, Soubory files)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					connection.Open();
					using (OracleCommand command = new("P_UPRAVIT_ZBOZI_V2", connection))
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
						command.Parameters.Add("p_idZamestnance", OracleDbType.Int32).Value = files.idZamestnance;

						command.ExecuteNonQuery();
					}
					connection.Close();
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
		internal static bool AddLocation(Umisteni newLocation)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					connection.Open();
					using (OracleCommand command = new("P_VLOZIT_UMISTENI_V2", connection))
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
		/// Metoda získá umístění na základě ID umístění
		/// </summary>
		/// <param name="idUmisteni">ID umístění</param>
		/// <returns>Model konkrétního umístění</returns>
		internal static Umisteni GetLocationById(int idUmisteni)
		{
			Umisteni getLocation = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM umisteni WHERE idUmisteni = :idUmisteni";
					command.Parameters.Add(":idUmisteni", OracleDbType.Int32).Value = idUmisteni;
					using (OracleDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								getLocation.IdUmisteni = int.Parse(reader["idUmisteni"].ToString());
								getLocation.Mistnost = reader["mistnost"].ToString();
								getLocation.Rada = reader["rada"].ToString();
								getLocation.Regal = reader["regal"].ToString();
								getLocation.Pozice = reader["pozice"].ToString();
								getLocation.Datum = DateTime.Parse(reader["datum"].ToString());
								getLocation.Poznamka = reader["poznamka"].ToString();
							}
						}
					}
				}
				connection.Close();
			}
			return getLocation;
		}

		/// <summary>
		/// Metoda zavolá proceduru na úpravu umístění
		/// </summary>
		/// <param name="umisteni">Model upravovaného umístění</param>
		/// <returns>vrací true, pokud SQL příkaz proběhne úspěšně, jinak false</returns>
		internal static bool EditLocation(Umisteni umisteni)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					connection.Open();
					using (OracleCommand command = new("P_UPRAVIT_UMISTENI_V2", connection))
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
		/// Metoda vytáhne ID, název, zkratku všech kategorií
		/// </summary>
		/// <returns>List všech kategorií</returns>
		internal static List<Kategorie> GetAllCategoriesIdNameAcronym()
		{
			List<Kategorie> kategorie = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT idkategorie, nazev, zkratka FROM kategorie";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						Kategorie? specificCateogory = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								specificCateogory = new();

								specificCateogory.IdKategorie = int.Parse(reader["idKategorie"].ToString());
								specificCateogory.Nazev = reader["nazev"].ToString();
								specificCateogory.Zkratka = reader["zkratka"].ToString();

								kategorie.Add(specificCateogory);
							}
						}
						else
						{
							specificCateogory = null;
						}
					}
				}
				connection.Close();
			}
			return kategorie;
		}
	}
}