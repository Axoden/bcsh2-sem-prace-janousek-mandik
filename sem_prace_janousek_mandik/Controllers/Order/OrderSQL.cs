using Oracle.ManagedDataAccess.Client;
using sem_prace_janousek_mandik.Models.Goods;
using sem_prace_janousek_mandik.Models.Order;
using System;
using System.Data;
using System.Reflection;

namespace sem_prace_janousek_mandik.Controllers.Order
{
	public class OrderSQL
	{
		internal static List<Objednavky_Zamestnanci> GetAllCustomerOrders(string email)
		{
			List<Objednavky_Zamestnanci> objednavky = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT o.*, zam.jmeno, zam.prijmeni, zam.email FROM objednavky o" +
						" INNER JOIN zamestnanci zam ON o.idZamestnance = zam.idZamestnance" +
						" INNER JOIN zakaznici zak ON o.idZakaznika = zak.idZakaznika" +
						" WHERE zak.email = :zakEmail";


					command.Parameters.Add(":zakEmail", OracleDbType.Varchar2).Value = email;
					using (OracleDataReader reader = command.ExecuteReader())
					{
						Objednavky_Zamestnanci? objednavka = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								objednavka = new();
								objednavka.Objednavky = new();
								objednavka.Zamestnanci = new();

								objednavka.Objednavky.IdObjednavky = int.Parse(reader["idObjednavky"].ToString());
								objednavka.Objednavky.DatumPrijeti = DateTime.Parse(reader["datumPrijeti"].ToString());
								objednavka.Objednavky.Poznamka = reader["poznamka"].ToString();
								objednavka.Objednavky.IdFaktury = int.Parse(reader["idFaktury"].ToString());
								objednavka.Objednavky.IdZakaznika = int.Parse(reader["idZakaznika"].ToString());
								objednavka.Objednavky.IdZamestnance = int.Parse(reader["idZamestnance"].ToString());

								objednavka.Zamestnanci.Jmeno = reader["jmeno"].ToString();
								objednavka.Zamestnanci.Prijmeni = reader["prijmeni"].ToString();
								objednavka.Zamestnanci.Email = reader["email"].ToString();

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

		internal static List<Objednavky_Zamestnanci_Zakaznici> GetAllOrders()
		{
			List<Objednavky_Zamestnanci_Zakaznici> objednavky = new();
			using (OracleConnection connection = OracleDbContext.GetConnection())
			{
				connection.Open();
				using (OracleCommand command = connection.CreateCommand())
				{
					command.CommandText = "SELECT o.*, zam.jmeno AS zamJmeno, zam.prijmeni AS zamPrijmeni, zak.jmeno AS zakJmeno, zak.prijmeni AS zakPrijmeni" +
						" FROM objednavky o" +
						" INNER JOIN zamestnanci zam ON o.idZamestnance = zam.idZamestnance" +
						" INNER JOIN zakaznici zak ON o.idZakaznika = zak.idZakaznika";
					using (OracleDataReader reader = command.ExecuteReader())
					{
						Objednavky_Zamestnanci_Zakaznici? objednavka = new();
						if (reader.HasRows)
						{
							while (reader.Read())
							{
								objednavka = new();
								objednavka.Objednavky = new();
								objednavka.Zamestnanci = new();
								objednavka.Zakaznici = new();

								objednavka.Objednavky.IdObjednavky = int.Parse(reader["idObjednavky"].ToString());
								objednavka.Objednavky.DatumPrijeti = DateTime.Parse(reader["datumPrijeti"].ToString());
								objednavka.Objednavky.Poznamka = reader["poznamka"].ToString();
								objednavka.Objednavky.IdFaktury = int.Parse(reader["idFaktury"].ToString());
								objednavka.Objednavky.IdZakaznika = int.Parse(reader["idZakaznika"].ToString());
								objednavka.Objednavky.IdZamestnance = int.Parse(reader["idZamestnance"].ToString());

								objednavka.Zamestnanci.Jmeno = reader["zamJmeno"].ToString();
								objednavka.Zamestnanci.Prijmeni = reader["zamPrijmeni"].ToString();

								objednavka.Zakaznici.Jmeno = reader["zakJmeno"].ToString();
								objednavka.Zakaznici.Prijmeni = reader["zakPrijmeni"].ToString();

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
                                specificGoods.CarovyKod = Int64.Parse(reader["carovyKod"].ToString());

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
                using (OracleCommand command = new("P_VLOZIT_ZBOZIOBJEDNAVEK_v2", connection))
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

        internal static bool ClosedOrder(int idObjednavky)
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
                                objednavkaInt = int.Parse(reader["nazev"].ToString());
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
								objednavka.Uzavrena = ClosedOrder(objednavka.IdObjednavky);
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
    }
}
