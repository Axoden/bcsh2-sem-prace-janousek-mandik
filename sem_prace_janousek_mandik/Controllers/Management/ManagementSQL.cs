using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
using sem_prace_janousek_mandik.Models.Employee;
using sem_prace_janousek_mandik.Models.Goods;
using sem_prace_janousek_mandik.Models.Management;
using sem_prace_janousek_mandik.Models.Payment;
using System.Data;

namespace sem_prace_janousek_mandik.Controllers.Management
{
	public static class ManagementSQL
	{
		/// <summary>
		/// Metoda vytáhne všechny pozice
		/// </summary>
		/// <returns>List všech pozic</returns>
		internal static async Task<List<Pozice>> GetAllPositions()
		{
			List<Pozice> pozice = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM pozice";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						while (await reader.ReadAsync())
						{
							Pozice? jednaPozice = new();
							jednaPozice.IdPozice = reader.GetInt32(reader.GetOrdinal("idPozice"));
							jednaPozice.Nazev = reader["nazev"].ToString();

							pozice.Add(jednaPozice);
						}
					}
				}
			}
			return pozice;
		}

		/// <summary>
		/// Zavolá proceduru na vložení pozice
		/// </summary>
		/// <param name="newPosition">Model nové pozice</param>
		/// <returns>true, pokud příkaz proběhl úspěšně, jinak false</returns>
		internal static async Task<bool> RegisterPosition(Pozice newPosition)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_insert.p_vlozit_pozici_v2", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = newPosition.Nazev;

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
		/// Zavolá proceduru na úpravu pozice
		/// </summary>
		/// <param name="pozice">Model s upravenými daty pozice</param>
		/// <returns>true, pokud příkaz proběhl úspěšně, jinak false</returns>
		internal static async Task<bool> EditPosition(Pozice pozice)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_edit.P_UPRAVIT_POZICI_V2", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_idpozice", OracleDbType.Int32).Value = pozice.IdPozice;
						command.Parameters.Add("p_nazev", OracleDbType.Varchar2).Value = pozice.Nazev;

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
		/// Metoda vrátí pozici na základě ID pozice
		/// </summary>
		/// <param name="idPozice">ID pozice</param>
		/// <returns>Model konkrétní pozice</returns>
		internal static async Task<Pozice> GetPositionById(int idPozice)
		{
			Pozice getPozice = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM pozice WHERE idPozice = :idPozice";
					command.Parameters.Add(":idPozice", OracleDbType.Int32).Value = idPozice;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								getPozice.IdPozice = reader.GetInt32(reader.GetOrdinal("idPozice"));
								getPozice.Nazev = reader["nazev"].ToString();
							}
						}
					}
				}
			}
			return getPozice;
		}

		/// <summary>
		/// Metoda vytáhne všechny objekty použité v DB dle vstupního parametru
		/// </summary>
		/// <param name="name">Název sloupce</param>
		/// <param name="dbObject">Název tabulky</param>
		/// <returns>List vybraných objektů</returns>
		internal static async Task<List<string>> GetAllObjects(string name, string dbObject)
		{
			List<string> objectNames = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = $"SELECT {name} AS nazev FROM {dbObject}";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								string? specificObject = reader["nazev"].ToString();

								objectNames.Add(specificObject);
							}
						}
					}
				}
			}
			return objectNames;
		}

		/// <summary>
		/// Metoda vytáhne všechny balíčky použíté v DB
		/// </summary>
		/// <returns>List balíčků</returns>
		internal static async Task<List<string>> GetAllPackages()
		{
			List<string> objectNames = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT object_name AS nazev FROM user_objects WHERE object_type = 'PACKAGE'";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								string? specificObject = reader["nazev"].ToString();

								objectNames.Add(specificObject);
							}
						}
					}
				}
			}
			return objectNames;
		}

		/// <summary>
		/// Metoda vytáhne všechny procedury/funkce použíté v DB dle vstupního parametru
		/// </summary>
		/// <param name="procedure">true => procedury, false => funkce</param>
		/// <returns>List procedur/funkcí</returns>
		internal static async Task<List<string>> GetAllProceduresFunctions(bool procedure)
		{
			List<string> objectNames = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
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

					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								string? specificObject = reader["nazev"].ToString();

								objectNames.Add(specificObject);
							}
						}
					}
				}
			}
			return objectNames;
		}

		/// <summary>
		/// Metoda vrátí všechny logy o změnách dat v tabulkách
		/// </summary>
		/// <returns>List všech logů</returns>
		internal static async Task<List<LogTableInsUpdDel>> GetAllLogs()
		{
			List<LogTableInsUpdDel> logs = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM log_table_ins_upd_del ORDER BY change_time DESC";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								LogTableInsUpdDel? log = new();
								log.LogId = reader.GetInt32(reader.GetOrdinal("log_id"));
								log.TableName = reader["table_name"].ToString();
								log.Operation = reader["operation"].ToString();
								log.ChangeTime = reader.GetDateTime(reader.GetOrdinal("change_time"));
								log.Username = reader["username"].ToString();
								log.OldData = reader["old_data"].ToString();
								log.NewData = reader["new_data"].ToString();

								logs.Add(log);
							}
						}
					}
				}
			}
			return logs;
		}

		internal static async Task<Sestavy> GetAllReports()
		{
			Sestavy report = new();
			report.Faktury = new();
			report.ZamestnanciObjednavky = new();
			report.ZboziSklad = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM V_FAKTURY_INFO ORDER BY datumVystaveni DESC";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Faktury_Zak_Zam? invoice = new();
								invoice.IdFaktury = reader.GetInt32(reader.GetOrdinal("idFaktury"));
								invoice.CisloFaktury = reader.GetInt32(reader.GetOrdinal("cisloFaktury"));
								invoice.DatumVystaveni = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("datumVystaveni")));
								invoice.DatumSplatnosti = DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("datumSplatnosti")));
								invoice.CastkaObjednavka = reader.GetFloat(reader.GetOrdinal("castkaObjednavka"));
								invoice.CastkaDoprava = reader.GetFloat(reader.GetOrdinal("castkaDoprava"));
								invoice.Dph = reader.GetFloat(reader.GetOrdinal("dph"));
								invoice.ZakaznikJmeno = reader["zakaznik"].ToString();
								invoice.AdresaZakaznika = reader["adresa_zakaznika"].ToString();
								invoice.ZamestnanecJmeno = reader["vytvoreno_zamestnancem"].ToString();

								report.Faktury.Add(invoice);
							}
						}
					}

					command.CommandText = "SELECT * FROM V_ZAMESTNANCI_INFO ORDER BY pocet_objednavek DESC";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Zamestnanci_Objednavky? emp = new();
								emp = new();
								emp.IdZamestnance = reader.GetInt32(reader.GetOrdinal("idZamestnance"));
								emp.Jmeno = reader["jmeno"].ToString();
								emp.Prijmeni = reader["prijmeni"].ToString();
								emp.NazevPozice = reader["pozice"].ToString();
								emp.Adresa = reader["adresa_zamestnance"].ToString();
								emp.PocetObjednavek = reader.GetInt32(reader.GetOrdinal("pocet_objednavek"));

								report.ZamestnanciObjednavky.Add(emp);
							}
						}
					}

					command.CommandText = "SELECT * FROM V_ZBOZI_NA_SKLADE ORDER BY kategorie_nazev";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Zbozi_Sklad? goods = new();
								goods.IdZbozi = reader.GetInt32(reader.GetOrdinal("idZbozi"));
								goods.Nazev = reader["nazev"].ToString();
								goods.JednotkovaCena = reader.GetFloat(reader.GetOrdinal("jednotkovaCena"));
								goods.PocetNaSklade = reader.GetInt32(reader.GetOrdinal("pocetNaSklade"));
								goods.NazevKategorie = reader["kategorie_nazev"].ToString();
								goods.NadrazenaKategorie = reader["nadrazena_kategorie"].ToString();
								goods.NazevDodavatele = reader["dodavatel_nazev"].ToString();
								goods.Umisteni = reader["umisteni"].ToString();

								report.ZboziSklad.Add(goods);
							}
						}
					}
				}
			}
			return report;
		}

		/// <summary>
		/// Metoda spustí funkci celkové hodnoty objenávek zákazníka
		/// </summary>
		/// <param name="idZakaznika">ID zákazníka</param>
		/// <returns>Hodnota objednávek</returns>
		internal static float ListOverViewCus(int idZakaznika)
		{
			float celkovaHodnota;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = new("pkg_funkce.Celkova_Hodnota_Objednavek_Zakaznika", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					OracleParameter returnValue = new()
					{
						ParameterName = "RETURN_VALUE",
						OracleDbType = OracleDbType.BinaryFloat,
						Direction = ParameterDirection.ReturnValue
					};
					command.Parameters.Add(returnValue);

					command.Parameters.Add("p_id_zakaznika", OracleDbType.Int32).Value = idZakaznika;
					command.ExecuteNonQuery();

					celkovaHodnota = float.Parse(returnValue.Value.ToString());
				}
			}
			return celkovaHodnota;
		}

		/// <summary>
		/// Metoda zavolá funkci na zjistění největšího dodavatele
		/// </summary>
		/// <returns>Název dodavatele</returns>
		internal static async Task<string?> ListOverViewSuppliers()
		{
			string? supplierOutput;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = new("pkg_funkce.Nejvetsi_Dodavatel", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					OracleParameter returnValue = new()
					{
						ParameterName = "RETURN_VALUE",
						OracleDbType = OracleDbType.Varchar2,
						Size = 1000,
						Direction = ParameterDirection.ReturnValue
					};
					command.Parameters.Add(returnValue);

					command.ExecuteNonQuery();

					supplierOutput = returnValue.Value.ToString();
				}
			}
			return supplierOutput;
		}

		/// <summary>
		/// Metoda zavolá funkci na zjištění nejvíce objednávaného zboží v kategorii
		/// </summary>
		/// <param name="idKategorie">ID kategorie</param>
		/// <returns>ID zboží</returns>
		internal static async Task<int> ListOverViewCategories(int idKategorie)
		{
			int idGoods = 0;
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = new("pkg_funkce.Nejvice_Objednane_Zbozi_V_Kategorii", connection))
				{
					command.CommandType = CommandType.StoredProcedure;

					OracleParameter returnValue = new()
					{
						ParameterName = "RETURN_VALUE",
						OracleDbType = OracleDbType.Int32,
						Direction = ParameterDirection.ReturnValue
					};
					command.Parameters.Add(returnValue);

					command.Parameters.Add("p_idkategorie", OracleDbType.Int32).Value = idKategorie;
					command.ExecuteNonQuery();

					if (!((OracleDecimal)returnValue.Value).IsNull)
					{
						idGoods = int.Parse(returnValue.Value.ToString());
					}
				}
			}
			return idGoods;
		}

		/// <summary>
		/// Metoda získá všechny soubory vč. podrobností
		/// </summary>
		/// <returns>List všech souborů</returns>
		internal static async Task<List<Soubory_Vypis>> GetAllFiles()
		{
			List<Soubory_Vypis> files = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT s.*, z.jmeno, z.prijmeni, zb.nazev AS nazevZbozi FROM soubory s INNER JOIN zamestnanci z ON z.idZamestnance = s.idZamestnance INNER JOIN zbozi zb ON zb.idSouboru = s.idSouboru";
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								Soubory_Vypis? specificFile = new();
								specificFile.Soubory = new();
								specificFile.Soubory.IdSouboru = reader.GetInt32(reader.GetOrdinal("idSouboru"));
								specificFile.Soubory.Nazev = reader["nazev"].ToString();
								specificFile.Soubory.TypSouboru = reader["typSouboru"].ToString();
								specificFile.Soubory.PriponaSouboru = reader["priponaSouboru"].ToString();
								specificFile.Soubory.DatumNahrani = reader.GetDateTime(reader.GetOrdinal("datumNahrani"));
								specificFile.Soubory.DatumModifikace = reader.GetDateTime(reader.GetOrdinal("datumModifikace"));
								specificFile.Soubory.Data = reader["data"] as byte[];
								specificFile.Soubory.IdZamestnance = reader.GetInt32(reader.GetOrdinal("idZamestnance"));

								specificFile.JmenoZamestnance = reader["jmeno"].ToString();
								specificFile.PrijmeniZamestnance = reader["prijmeni"].ToString();
								specificFile.KdePouzito = reader["nazevZbozi"].ToString();
								files.Add(specificFile);
							}
						}
					}
				}
			}
			return files;
		}

		internal static async Task<Soubory?> GetFileById(int idSouboru)
		{
			Soubory getFiles = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				await connection.OpenAsync();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT * FROM soubory WHERE idSouboru = :idSouboru";
					command.Parameters.Add(":idSouboru", OracleDbType.Int32).Value = idSouboru;
					using (OracleDataReader reader = (OracleDataReader)await command.ExecuteReaderAsync())
					{
						if (reader.HasRows)
						{
							while (await reader.ReadAsync())
							{
								getFiles.IdSouboru = reader.GetInt32(reader.GetOrdinal("idSouboru"));
								getFiles.Nazev = reader["nazev"].ToString();
								getFiles.TypSouboru = reader["typSouboru"].ToString();
								getFiles.PriponaSouboru = reader["priponaSouboru"].ToString();
								getFiles.DatumNahrani = reader.GetDateTime(reader.GetOrdinal("datumNahrani"));
								getFiles.DatumModifikace = reader.GetDateTime(reader.GetOrdinal("datumModifikace"));
								getFiles.IdZamestnance = reader.GetInt32(reader.GetOrdinal("idZamestnance"));
							}
						}
					}
				}
			}
			return getFiles;
		}

		internal static async Task<bool> EditFile(Soubory_Edit fileEdit)
		{
			try
			{
				using (OracleConnection connection = OracleDbContext.GetConnection())
				{
					await connection.OpenAsync();
					using (OracleCommand command = new("pkg_edit.P_UPRAVIT_SOUBOR", connection))
					{
						command.CommandType = CommandType.StoredProcedure;

						command.Parameters.Add("p_idsouboru", OracleDbType.Int32).Value = fileEdit.Soubory?.IdSouboru;
						command.Parameters.Add("p_nazev_souboru", OracleDbType.Varchar2).Value = fileEdit.Soubory?.Nazev;
						command.Parameters.Add("p_typsouboru", OracleDbType.Varchar2).Value = fileEdit.Soubory?.TypSouboru;
						command.Parameters.Add("p_priponasouboru", OracleDbType.Varchar2).Value = fileEdit.Soubory?.PriponaSouboru;
						command.Parameters.Add("p_datumnahrani", OracleDbType.Date).Value = fileEdit.Soubory?.DatumNahrani;
						command.Parameters.Add("p_datummodifikace", OracleDbType.Date).Value = fileEdit.Soubory?.DatumModifikace;
						command.Parameters.Add("p_data", OracleDbType.Blob).Value = fileEdit.Soubory?.Data;
						command.Parameters.Add("p_idzamestnance", OracleDbType.Int32).Value = fileEdit.Soubory?.IdZamestnance;

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