using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models.Employee;
using sem_prace_janousek_mandik.Models.Goods;
using sem_prace_janousek_mandik.Models.Order;
using System;
using System.Data;
using System.Reflection;

namespace sem_prace_janousek_mandik.Controllers.Order
{
	public class OrderSQL
	{
		internal static List<Objednavky_Zamestnanci_Faktury> GetAllCustomerOrders(string email)
		{
			List<Objednavky_Zamestnanci_Faktury> objednavky = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT o.*, zam.jmeno, zam.prijmeni, zam.email, f.datumSplatnosti, f.castkaDoprava, f.dph FROM objednavky o" +
						" INNER JOIN zamestnanci zam ON o.idZamestnance = zam.idZamestnance" +
						" INNER JOIN zakaznici zak ON o.idZakaznika = zak.idZakaznika" +
						" INNER JOIN faktury f ON o.idFaktury = f.idFaktury" +
						" WHERE zak.email = :zakEmail";


					command.Parameters.Add(":zakEmail", OracleDbType.Varchar2).Value = email;
					using (OracleDataReader reader = command.ExecuteReader())
					{
						Objednavky_Zamestnanci_Faktury? objednavka = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								objednavka = new();
								objednavka.Objednavky = new();
								objednavka.Zamestnanci = new();
								objednavka.Faktury = new();

								objednavka.Objednavky.IdObjednavky = int.Parse(reader["idObjednavky"].ToString());
								objednavka.Objednavky.CisloObjednavky = int.Parse(reader["cisloObjednavky"].ToString());
								objednavka.Objednavky.DatumPrijeti = DateTime.Parse(reader["datumPrijeti"].ToString());
								objednavka.Objednavky.Poznamka = reader["poznamka"].ToString();
								objednavka.Objednavky.Uzavrena = IsClosedOrder(objednavka.Objednavky.IdObjednavky);
								objednavka.Objednavky.IdFaktury = int.Parse(reader["idFaktury"].ToString());
								objednavka.Objednavky.IdZakaznika = int.Parse(reader["idZakaznika"].ToString());
								objednavka.Objednavky.IdZamestnance = int.Parse(reader["idZamestnance"].ToString());

								objednavka.Zamestnanci.Jmeno = reader["jmeno"].ToString();
								objednavka.Zamestnanci.Prijmeni = reader["prijmeni"].ToString();
								objednavka.Zamestnanci.Email = reader["email"].ToString();

								objednavka.Faktury.IdFaktury = int.Parse(reader["idFaktury"].ToString());
								objednavka.Faktury.DatumSplatnosti = DateOnly.FromDateTime(DateTime.Parse(reader["datumSplatnosti"].ToString()));
								objednavka.Faktury.CastkaDoprava = float.Parse(reader["castkaDoprava"].ToString());
								objednavka.Faktury.Dph = float.Parse(reader["dph"].ToString());

								objednavky.Add(objednavka);
							}
						}
						else
						{
							objednavka = null;
						}
					}
				}
				connection.Close();
			}
			return objednavky;
		}

		internal static List<Objednavky_Zamestnanci_Zakaznici_Faktury> GetAllOrders()
		{
			List<Objednavky_Zamestnanci_Zakaznici_Faktury> objednavky = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
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
						" INNER JOIN faktury f ON o.idFaktury = f.idFaktury";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						Objednavky_Zamestnanci_Zakaznici_Faktury? objednavka = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								objednavka = new();
								objednavka.Objednavky = new();
								objednavka.Zamestnanci = new();
								objednavka.Zakaznici = new();
								objednavka.Faktury = new();

								objednavka.Objednavky.IdObjednavky = int.Parse(reader["idObjednavky"].ToString());
								objednavka.Objednavky.CisloObjednavky = int.Parse(reader["cisloObjednavky"].ToString());
								objednavka.Objednavky.DatumPrijeti = DateTime.Parse(reader["datumPrijeti"].ToString());
								objednavka.Objednavky.Poznamka = reader["poznamka"].ToString();
								objednavka.Objednavky.Uzavrena = IsClosedOrder(objednavka.Objednavky.IdObjednavky);
								objednavka.Objednavky.IdFaktury = int.Parse(reader["idFaktury"].ToString());
								objednavka.Objednavky.IdZakaznika = int.Parse(reader["idZakaznika"].ToString());
								objednavka.Objednavky.IdZamestnance = int.Parse(reader["idZamestnance"].ToString());

								objednavka.Zamestnanci.Jmeno = reader["zamJmeno"].ToString();
								objednavka.Zamestnanci.Prijmeni = reader["zamPrijmeni"].ToString();

								objednavka.Zakaznici.Jmeno = reader["zakJmeno"].ToString();
								objednavka.Zakaznici.Prijmeni = reader["zakPrijmeni"].ToString();

								objednavka.Faktury.IdFaktury = int.Parse(reader["idFaktury"].ToString());
								objednavka.Faktury.DatumSplatnosti = DateOnly.FromDateTime(DateTime.Parse(reader["datumSplatnosti"].ToString()));
								objednavka.Faktury.CastkaDoprava = float.Parse(reader["castkaDoprava"].ToString());
								objednavka.Faktury.Dph = float.Parse(reader["dph"].ToString());

								objednavky.Add(objednavka);
							}
						}
						else
						{
							objednavka = null;
						}
					}
				}
				connection.Close();
			}
			return objednavky;
		}

		internal static List<ZboziObjednavek_Zbozi> GetAllGoodsOrders()
		{
			List<ZboziObjednavek_Zbozi> zboziObjednavek = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT zo.*, z.nazev FROM zbozi_objednavek zo INNER JOIN zbozi z ON zo.idZbozi = z.idZbozi";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						ZboziObjednavek_Zbozi? zboziObjednavky = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								zboziObjednavky = new();
								zboziObjednavky.ZboziObjednavek = new();
								zboziObjednavky.Zbozi = new();

								zboziObjednavky.ZboziObjednavek.IdZboziObjednavky = int.Parse(reader["idZboziObjednavky"].ToString());
								zboziObjednavky.ZboziObjednavek.Mnozstvi = int.Parse(reader["mnozstvi"].ToString());
								zboziObjednavky.ZboziObjednavek.JednotkovaCena = float.Parse(reader["jednotkovaCena"].ToString());
								zboziObjednavky.ZboziObjednavek.IdObjednavky = int.Parse(reader["idObjednavky"].ToString());
								zboziObjednavky.ZboziObjednavek.IdZbozi = int.Parse(reader["idZbozi"].ToString());

								zboziObjednavky.Zbozi.Nazev = reader["nazev"].ToString();

								zboziObjednavek.Add(zboziObjednavky);
							}
						}
						else
						{
							zboziObjednavky = null;
						}
					}
				}
				connection.Close();
			}
			return zboziObjednavek;
		}

		internal static List<ZboziObjednavek_Zbozi> GetAllGoodsOrdersCustomer(string email)
		{
			List<ZboziObjednavek_Zbozi> zboziObjednavek = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT zo.*, z.nazev FROM zbozi_objednavek zo" +
						" INNER JOIN zbozi z ON zo.idZbozi = z.idZbozi" +
						" INNER JOIN objednavky o ON zo.idObjednavky = o.idObjednavky" +
						" INNER JOIN zakaznici zak ON o.idZakaznika = zak.idZakaznika WHERE zak.email = :email";
					command.Parameters.Add(":zakEmail", OracleDbType.Varchar2).Value = email;
					using (OracleDataReader reader = command.ExecuteReader())
					{
						ZboziObjednavek_Zbozi? zboziObjednavky = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								zboziObjednavky = new();
								zboziObjednavky.ZboziObjednavek = new();
								zboziObjednavky.Zbozi = new();

								zboziObjednavky.ZboziObjednavek.IdZboziObjednavky = int.Parse(reader["idZboziObjednavky"].ToString());
								zboziObjednavky.ZboziObjednavek.Mnozstvi = int.Parse(reader["mnozstvi"].ToString());
								zboziObjednavky.ZboziObjednavek.JednotkovaCena = float.Parse(reader["jednotkovaCena"].ToString());
								zboziObjednavky.ZboziObjednavek.IdObjednavky = int.Parse(reader["idObjednavky"].ToString());
								zboziObjednavky.ZboziObjednavek.IdZbozi = int.Parse(reader["idZbozi"].ToString());

								zboziObjednavky.Zbozi.Nazev = reader["nazev"].ToString();

								zboziObjednavek.Add(zboziObjednavky);
							}
						}
						else
						{
							zboziObjednavky = null;
						}
					}
				}
				connection.Close();
			}
			return zboziObjednavek;
		}

		internal static ZboziObjednavek_Zbozi GetGoodsOrderById(int index)
		{
			ZboziObjednavek_Zbozi getZbozi = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT zo.*, z.nazev AS nazev FROM zbozi_objednavek zo INNER JOIN zbozi z ON zo.idZbozi = z.idZbozi WHERE idZboziObjednavky = :idZboziObjednavky";
					command.Parameters.Add(":idZboziObjednavky", OracleDbType.Int32).Value = index;
					using (OracleDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								getZbozi.ZboziObjednavek = new();
                                getZbozi.Zbozi = new();
                                getZbozi.ZboziObjednavek.IdZboziObjednavky = int.Parse(reader["idZboziObjednavky"].ToString());
								getZbozi.ZboziObjednavek.Mnozstvi = int.Parse(reader["mnozstvi"].ToString());
								getZbozi.ZboziObjednavek.JednotkovaCena = float.Parse(reader["jednotkovaCena"].ToString());
								getZbozi.Zbozi.Nazev = reader["nazev"].ToString();
							}
						}
					}
				}
				connection.Close();
			}
			return getZbozi;
		}

        internal static void EditGoodsOrder(ZboziObjednavek_Zbozi zboziObjednavek)
        {
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new("P_UPRAVIT_ZBOZIOBJEDNAVEK_V2", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Vstupní parametry procedury
                    command.Parameters.Add("p_idZboziObjednavky", OracleDbType.Int32).Value = zboziObjednavek.ZboziObjednavek.IdZboziObjednavky;
                    command.Parameters.Add("p_jednotkovaCena", OracleDbType.BinaryFloat).Value = zboziObjednavek.ZboziObjednavek.JednotkovaCena;
                    command.Parameters.Add("p_pocetNaSklade", OracleDbType.Int32).Value = zboziObjednavek.ZboziObjednavek.Mnozstvi;

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        internal static List<Zbozi> GetAllGoods()
        {
            List<Zbozi> goods = new();
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT idZbozi, nazev, jednotkovaCena, carovyKod FROM zbozi";
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        Zbozi? specificGoods = new();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                specificGoods = new();
                                specificGoods.IdZbozi = int.Parse(reader["idZbozi"].ToString());
                                specificGoods.Nazev = reader["nazev"].ToString();
                                specificGoods.JednotkovaCena = float.Parse(reader["jednotkovaCena"].ToString());
                                specificGoods.CarovyKod = reader["carovyKod"].ToString();

                                goods.Add(specificGoods);
                            }
                        }
                        else
                        {
                            specificGoods = null;
                        }
                    }
                }
                connection.Close();
            }
            return goods;
        }

        internal static bool AddGoodsToOrder(ZboziObjednavek_Zbozi addZboziObjednavek)
        {
            bool addSuccessful = false;
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new("P_VLOZIT_ZBOZI_OBJEDNAVKU", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Vstupní parametr procedury
                    command.Parameters.Add("p_mnozstvi", OracleDbType.Int32).Value = addZboziObjednavek.ZboziObjednavek.Mnozstvi;
                    command.Parameters.Add("p_jednotkovaCena", OracleDbType.BinaryFloat).Value = addZboziObjednavek.ZboziObjednavek.JednotkovaCena;
                    command.Parameters.Add("p_idObjednavky", OracleDbType.Int32).Value = addZboziObjednavek.ZboziObjednavek.IdObjednavky;
                    command.Parameters.Add("p_idZbozi", OracleDbType.Int32).Value = addZboziObjednavek.ZboziObjednavek.IdZbozi;

                    command.ExecuteNonQuery();
                }
                connection.Close();
                addSuccessful = true;
            }
            return addSuccessful;
        }

        internal static bool IsClosedOrder(int idObjednavky)
        {
            bool objednavkaUzavrena = false;
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT uzavrena FROM objednavky WHERE idObjednavky = :idObjednavky";
                    command.Parameters.Add(":idObjednavky", OracleDbType.Int32).Value = idObjednavky;
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
							int objednavkaInt = 0;
                            while (reader.Read())
                            {
                                objednavkaInt = int.Parse(reader["uzavrena"].ToString());
                            }

							if(objednavkaInt == 1)
							{
								objednavkaUzavrena = true;
							}
                        }
                    }
                }
                connection.Close();
            }
            return objednavkaUzavrena;
        }

        internal static Objednavky GetOrderById(int index)
        {
            Objednavky objednavka = new();
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM objednavky WHERE idObjednavky = :idObjednavky";
                    command.Parameters.Add(":idObjednavky", OracleDbType.Int32).Value = index;
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {                            
                                objednavka.IdObjednavky = int.Parse(reader["idObjednavky"].ToString());
                                objednavka.CisloObjednavky = int.Parse(reader["cisloObjednavky"].ToString());
                                objednavka.DatumPrijeti = DateTime.Parse(reader["datumPrijeti"].ToString());
                                objednavka.Poznamka = reader["poznamka"].ToString();
								objednavka.Uzavrena = IsClosedOrder(objednavka.IdObjednavky);
                                objednavka.IdFaktury = int.Parse(reader["idFaktury"].ToString());
                                objednavka.IdZamestnance = int.Parse(reader["idZamestnance"].ToString());
                                objednavka.IdZakaznika = int.Parse(reader["idZakaznika"].ToString());
                            }
                        }
                    }
                }
                connection.Close();
            }
            return objednavka;
        }

        internal static float GetPriceForGoods(int idZbozi)
        {
            float jednotkovaCena = new();
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT jednotkovacena FROM zbozi WHERE idZbozi = :idZbozi";
                    command.Parameters.Add(":idZbozi", OracleDbType.Int32).Value = idZbozi;
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
								jednotkovaCena = float.Parse(reader["jednotkovaCena"].ToString());
                            }
                        }
                    }
                }
                connection.Close();
            }
            return jednotkovaCena;
        }

        internal static void CloseOrder(int idObjednavky)
        {
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new("P_UZAVRIT_OBJEDNAVKU", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

					// Vstupní parametr procedury                    
					command.Parameters.Add("p_idObjednavky", OracleDbType.Int32).Value = idObjednavky;

                    command.ExecuteNonQuery();
                }
                connection.Close();
            }
        }

        internal static bool AddOrder(Objednavy_Faktury newOrder)
        {
            bool addSuccessful = false;
            using (OracleConnection connection = OracleDbContext.GetConnection())
            {
                connection.Open();
                using (OracleCommand command = new("P_VLOZIT_OBJEDNAVKU", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

					// Vstupní parametry procedury
					command.Parameters.Add("p_datumPrijeti", OracleDbType.Date).Value = newOrder.Objednavky.DatumPrijeti;
                    command.Parameters.Add("p_poznamka", OracleDbType.Varchar2).Value = newOrder.Objednavky.Poznamka;
                    command.Parameters.Add("p_idZakaznika", OracleDbType.Int32).Value = newOrder.Objednavky.IdZakaznika;
                    command.Parameters.Add("p_idZamestnance", OracleDbType.Int32).Value = newOrder.Objednavky.IdZamestnance;
                    command.Parameters.Add("p_castkaDoprava", OracleDbType.BinaryFloat).Value = newOrder.Faktury.CastkaDoprava;
                    command.Parameters.Add("p_dph", OracleDbType.BinaryFloat).Value = newOrder.Faktury.Dph;

                    command.ExecuteNonQuery();
                }
                connection.Close();
                addSuccessful = true;
            }
            return addSuccessful;
        }

		internal static Objednavky_Zakaznici_Faktury GetCustomerOrderInvoice(int index)
		{
			Objednavky_Zakaznici_Faktury customer = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT z.email, f.idFaktury, f.castkaObjednavka, f.castkaDoprava, f.dph FROM objednavky o" +
						" INNER JOIN zakaznici z ON o.idZakaznika = z.idZakaznika" +
						" INNER JOIN faktury f ON o.idFaktury = f.idFaktury" +
						" WHERE idObjednavky = :idObjednavky";
					command.Parameters.Add(":idObjednavky", OracleDbType.Int32).Value = index;
					using (OracleDataReader reader = command.ExecuteReader())
					{
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								customer.Objednavky = new();
								customer.Zakaznici = new();
								customer.Faktury = new();

								customer.Zakaznici.Email = reader["email"].ToString();
								customer.Faktury.IdFaktury = int.Parse(reader["idFaktury"].ToString());
								customer.Faktury.CastkaObjednavka = float.Parse(reader["castkaObjednavka"].ToString());
								customer.Faktury.CastkaDoprava = float.Parse(reader["castkaDoprava"].ToString());
								customer.Faktury.Dph = float.Parse(reader["dph"].ToString());
							}
						}
					}
				}
				connection.Close();
			}
			return customer;
		}
	}
}
