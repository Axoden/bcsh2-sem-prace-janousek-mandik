using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;
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

        // Metoda vrátí všechny logy o změnách dat v tabulkách
        internal static List<LogTableInsUpdDel> GetAllLogs()
        {
            List<LogTableInsUpdDel> logs = new();
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM log_table_ins_upd_del ORDER BY change_time DESC";
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        LogTableInsUpdDel? log = new();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                log = new();
                                log.LogId = int.Parse(reader["log_id"].ToString());
                                log.TableName = reader["table_name"].ToString();
                                log.Operation = reader["operation"].ToString();
                                log.ChangeTime = DateTime.Parse(reader["change_time"].ToString());
                                log.Username = reader["username"].ToString();
                                log.OldData = reader["old_data"].ToString();
                                log.NewData = reader["new_data"].ToString();

                                logs.Add(log);
                            }
                        }
                        else
                        {
                            log = null;
                        }
                    }
                }
                connection.Close();
            }
            return logs;
        }

        internal static Sestavy GetAllReports(bool fullPriv)
        {
            Sestavy report = new();
            report.Faktury = new();
            report.ZamestnanciObjednavky = new();
            report.ZboziSklad = new();
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM V_FAKTURY_INFO ORDER BY datumVystaveni DESC";
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        Faktury_Zak_Zam? invoice = new();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                invoice = new();
                                invoice.IdFaktury = int.Parse(reader["idFaktury"].ToString());
                                invoice.CisloFaktury = int.Parse(reader["cisloFaktury"].ToString());
                                invoice.DatumVystaveni = DateOnly.FromDateTime(DateTime.Parse(reader["datumVystaveni"].ToString()));
                                invoice.DatumSplatnosti = DateOnly.FromDateTime(DateTime.Parse(reader["datumSplatnosti"].ToString()));
                                invoice.CastkaObjednavka = float.Parse(reader["castkaObjednavka"].ToString());
                                invoice.CastkaDoprava = float.Parse(reader["castkaDoprava"].ToString());
                                invoice.Dph = float.Parse(reader["dph"].ToString());
                                invoice.ZakaznikJmeno = reader["zakaznik"].ToString();
                                invoice.AdresaZakaznika = reader["adresa_zakaznika"].ToString();
                                invoice.ZamestnanecJmeno = reader["vytvoreno_zamestnancem"].ToString();

                                report.Faktury.Add(invoice);
                            }
                        }
                    }

                    command.CommandText = "SELECT * FROM V_ZAMESTNANCI_INFO ORDER BY pocet_objednavek DESC";
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        Zamestnanci_Objednavky? emp = new();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                emp = new();
                                emp.IdZamestnance = int.Parse(reader["idZamestnance"].ToString());
                                emp.Jmeno = reader["jmeno"].ToString();
                                emp.Prijmeni = reader["prijmeni"].ToString();
                                emp.NazevPozice = reader["pozice"].ToString();
                                emp.Adresa = reader["jmeno"].ToString();
                                emp.PocetObjednavek = int.Parse(reader["pocet_objednavek"].ToString());

                                report.ZamestnanciObjednavky.Add(emp);
                            }
                        }
                    }

                    command.CommandText = "SELECT * FROM V_ZBOZI_NA_SKLADE ORDER BY kategorie_nazev";
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        Zbozi_Sklad? goods = new();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                goods = new();
                                goods.IdZbozi = int.Parse(reader["idZbozi"].ToString());
                                goods.Nazev = reader["nazev"].ToString();
                                goods.JednotkovaCena = float.Parse(reader["jednotkovaCena"].ToString());
                                goods.PocetNaSklade = int.Parse(reader["pocetNaSklade"].ToString());
                                goods.NazevKategorie = reader["kategorie_nazev"].ToString();
                                goods.NadrazenaKategorie = reader["nadrazena_kategorie"].ToString();
                                goods.NazevDodavatele = reader["dodavatel_nazev"].ToString();
                                goods.Umisteni = reader["umisteni"].ToString();

                                report.ZboziSklad.Add(goods);
                            }
                        }
                    }
                }
                connection.Close();
            }
            return report;
        }

        public static float ListOverViewCus(int idZakaznika)
        {
            float celkovaHodnota;
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new("Celkova_Hodnota_Objednavek_Zakaznika", connection))
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
                connection.Close();
            }
            return celkovaHodnota;
        }

        internal static string ListOverViewSuppliers()
        {
            string supplierOutput;
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new("Nejvetsi_Dodavatel", connection))
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
                connection.Close();
            }
            return supplierOutput;
        }

        internal static int ListOverViewCategories(int idKategorie)
        {
            int idGoods = 0;
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new("Nejvice_Objednane_Zbozi_V_Kategorii", connection))
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
                connection.Close();
            }
            return idGoods;
        }
    }
}