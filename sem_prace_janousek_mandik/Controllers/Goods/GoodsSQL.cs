using Microsoft.VisualBasic;
using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models.Employee;
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

		internal static List<Zbozi_Umisteni_Kategorie_Dodavatele> GetAllGoodsWithLocationCategorySupplier()
		{
			List<Zbozi_Umisteni_Kategorie_Dodavatele> zbozi = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT z.*, k.*, u.*, d.*, k.nazev AS kategorieNazev, d.nazev AS dodavatelNazev, u.poznamka AS poznamkaUmisteni FROM zbozi z INNER JOIN kategorie k ON z.idkategorie = k.idkategorie INNER JOIN umisteni u ON z.idumisteni = u.idumisteni " +
						"INNER JOIN dodavatele d ON z.iddodavatele = d.iddodavatele";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						Zbozi_Umisteni_Kategorie_Dodavatele? jednoZbozi = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								jednoZbozi = new();
								jednoZbozi.Zbozi = new();
								jednoZbozi.Kategorie = new();
								jednoZbozi.Umisteni = new();
								jednoZbozi.Dodavatele = new();

								jednoZbozi.Zbozi.IdZbozi = int.Parse(reader["idZbozi"].ToString());
								jednoZbozi.Zbozi.Nazev = reader["nazev"].ToString();
								jednoZbozi.Zbozi.JednotkovaCena = float.Parse(reader["jednotkovaCena"].ToString());
								jednoZbozi.Zbozi.PocetNaSklade = int.Parse(reader["pocetNaSklade"].ToString());
								jednoZbozi.Zbozi.CarovyKod = int.Parse(reader["carovyKod"].ToString());
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

		internal static bool CheckExistsBarcode(Int64 carovyKod)
		{
			bool exists = true;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM zbozi WHERE carovyKod = :carovyKod";
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

		internal static bool RegisterGoods(Zbozi_Umisteni_Kategorie_Dodavatele newGoods)
		{
			bool registerSuccessful = false;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = new("P_VLOZIT_ZBOZI", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					// Vstupní parametry procedury
					command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = newGoods.Zbozi.Nazev;
					command.Parameters.Add("p_jednotkovaCena", OracleDbType.BinaryFloat).Value = newGoods.Zbozi.JednotkovaCena;
					command.Parameters.Add("p_pocetNaSklade", OracleDbType.Int32).Value = newGoods.Zbozi.PocetNaSklade;
					command.Parameters.Add("p_carovyKod", OracleDbType.Int64).Value = newGoods.Zbozi.CarovyKod;
					command.Parameters.Add("p_poznamka", OracleDbType.Varchar2).Value = newGoods.Zbozi.Poznamka;
					command.Parameters.Add("p_idDodavatele", OracleDbType.Int32).Value = newGoods.Zbozi.IdDodavatele;
					command.Parameters.Add("p_idUmisteni", OracleDbType.Int32).Value = newGoods.Zbozi.IdUmisteni;
					command.Parameters.Add("p_idKategorie", OracleDbType.Int32).Value = newGoods.Zbozi.IdKategorie;

					command.ExecuteNonQuery();
				}
				connection.Close();
				registerSuccessful = true;
			}
			return registerSuccessful;
		}

		internal static Zbozi_Umisteni_Kategorie_Dodavatele GetGoodsById(int index)
		{
			Zbozi_Umisteni_Kategorie_Dodavatele getZbozi = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM zbozi WHERE idZbozi = :idZbozi";
					command.Parameters.Add(":idZbozi", OracleDbType.Int32).Value = index;
					using (OracleDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								getZbozi.Zbozi = new();
								getZbozi.Zbozi.IdZbozi = int.Parse(reader["idZbozi"].ToString());
								getZbozi.Zbozi.Nazev = reader["nazev"].ToString();
								getZbozi.Zbozi.JednotkovaCena = float.Parse(reader["jednotkovaCena"].ToString());
								getZbozi.Zbozi.PocetNaSklade = int.Parse(reader["pocetNaSklade"].ToString());
								getZbozi.Zbozi.CarovyKod = Int64.Parse(reader["carovyKod"].ToString());
								getZbozi.Zbozi.Poznamka = reader["poznamka"].ToString();
								getZbozi.Zbozi.IdDodavatele = int.Parse(reader["idDodavatele"].ToString());
								getZbozi.Zbozi.IdUmisteni = int.Parse(reader["idUmisteni"].ToString());
								getZbozi.Zbozi.IdKategorie = int.Parse(reader["idKategorie"].ToString());
							}
						}
					}
				}
				connection.Close();
			}
			return getZbozi;
		}

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

		internal static void EditGoods(Zbozi_Umisteni_Kategorie_Dodavatele zbozi)
		{
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = new("P_UPRAVIT_ZBOZI_V2", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					// Vstupní parametry procedury
					command.Parameters.Add("p_idZbozi", OracleDbType.Int32).Value = zbozi.Zbozi.IdZbozi;
					command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = zbozi.Zbozi.Nazev;
					command.Parameters.Add("p_jednotkovaCena", OracleDbType.BinaryFloat).Value = zbozi.Zbozi.JednotkovaCena;
					command.Parameters.Add("p_pocetNaSklade", OracleDbType.Int32).Value = zbozi.Zbozi.PocetNaSklade;
					command.Parameters.Add("p_carovyKod", OracleDbType.Int64).Value = zbozi.Zbozi.CarovyKod;
					command.Parameters.Add("p_poznamka", OracleDbType.Varchar2).Value = zbozi.Zbozi.Poznamka;
					command.Parameters.Add("p_idDodavatele", OracleDbType.Int32).Value = zbozi.Zbozi.IdDodavatele;
					command.Parameters.Add("p_idUmisteni", OracleDbType.Int32).Value = zbozi.Zbozi.IdUmisteni;
					command.Parameters.Add("p_idKategorie", OracleDbType.Int32).Value = zbozi.Zbozi.IdKategorie;

					command.ExecuteNonQuery();
				}
				connection.Close();
			}
		}

		internal static bool AddLocation(Umisteni noveUmisteni)
		{
			bool addSuccessful = false;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = new("P_VLOZIT_UMISTENI_V2", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					command.Parameters.Add("p_mistnost", OracleDbType.Varchar2).Value = noveUmisteni.Mistnost;
					command.Parameters.Add("p_rada", OracleDbType.Varchar2).Value = noveUmisteni.Rada;
					command.Parameters.Add("p_regal", OracleDbType.Varchar2).Value = noveUmisteni.Regal;
					command.Parameters.Add("p_pozice", OracleDbType.Varchar2).Value = noveUmisteni.Pozice;
					command.Parameters.Add("p_datum", OracleDbType.Date).Value = noveUmisteni.Datum;
					command.Parameters.Add("p_poznamka", OracleDbType.Varchar2).Value = noveUmisteni.Poznamka;

					command.ExecuteNonQuery();
				}
				connection.Close();
				addSuccessful = true;
			}
			return addSuccessful;
		}

		internal static Umisteni GetLocationById(int index)
		{
			Umisteni getLocation = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM umisteni WHERE idUmisteni = :idUmisteni";
					command.Parameters.Add(":idUmisteni", OracleDbType.Int32).Value = index;
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

		internal static void EditLocation(Umisteni umisteni)
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
		}
	}
}