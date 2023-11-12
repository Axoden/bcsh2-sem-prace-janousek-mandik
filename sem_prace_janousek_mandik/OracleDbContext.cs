using Oracle.ManagedDataAccess.Client;

namespace sem_prace_janousek_mandik
{
    public static class OracleDbContext
    {      
        public static OracleConnection GetConnection()
        {
            string connectionString = "User Id=st67064;Password=abcde;Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=fei-sql3.upceucebny.cz)(PORT=1521))(CONNECT_DATA=(SID=BDAS)(SERVER=DEDICATED)))";
            return new OracleConnection(connectionString);
        }
    }
}
