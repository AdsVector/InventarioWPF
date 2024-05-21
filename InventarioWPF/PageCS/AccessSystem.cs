using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioWPF.PageCS
{
    public class AccessSystem
    {
        public static OleDbConnection dbConnection;
        public static bool Conectar()
        {
            try {
                if (dbConnection == null || dbConnection.State != System.Data.ConnectionState.Open) {
                    dbConnection = new OleDbConnection();
                    dbConnection.ConnectionString = ConfigurationManager.ConnectionStrings["aConection"].ToString();
                    dbConnection.Open();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool Login(string user, string pass)
        {         
            try
            {
                Conectar();

                string consulta = "SELECT count(*) FROM Empleados WHERE (Oculto=FALSE) AND (C_R = @Ide) AND (Pass = @Pass)";
                OleDbCommand comando = new OleDbCommand(consulta, dbConnection);
                comando.Parameters.AddWithValue("@Ide", user);
                comando.Parameters.AddWithValue("@Pass", pass);
                int count = Convert.ToInt32(comando.ExecuteScalar());

                if (count == 0)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }

        public static void CloseSession(object control)
        {
            dbConnection.Close();
            Login login = new Login();
            login.Show();
            ((MainWindow)control).Close();
        }

        public static Usuario GetEmployee(string ide)
        {
            Usuario usuario = null;
            Conectar();

            string consulta = "SELECT * FROM Empleados WHERE C_R = @Ide";
            OleDbCommand comando = new OleDbCommand(consulta, dbConnection);
            comando.Parameters.AddWithValue("@Ide", ide);

            OleDbDataReader dbDataReader = comando.ExecuteReader();
            if (dbDataReader.Read())
            {
                usuario = new Usuario(Convert.ToInt32(dbDataReader["Id_Emp"]),
                                                      dbDataReader["C_R"].ToString(),
                                                      dbDataReader["Pass"].ToString(),
                                                      dbDataReader["Nombre"].ToString(),
                                                      dbDataReader["Gener"].ToString(),
                                      Convert.ToInt32(dbDataReader["Id_Dep"]),
                                                      dbDataReader["Dir"].ToString(),
                                                      dbDataReader["Telf"].ToString(),
                                                      dbDataReader["Movil"].ToString(),
                                                      dbDataReader["Email"].ToString(),
                                   Convert.ToDateTime(dbDataReader["F_Nac"]),
                                                      dbDataReader["Pict"].ToString());

                dbDataReader.Close();         
                return usuario;
            }
            else
            {
                return null;
            }
        }

        public static void ChangePass(int ide, string newPass)
        {
            string consDep = "UPDATE Empleados SET Pass='" + newPass + "' WHERE Id_Emp=@Emp";
            OleDbCommand command = new OleDbCommand(consDep, dbConnection);
            command.Parameters.AddWithValue("@Emp", ide);
            command.ExecuteNonQuery();
        }

        public static int CountInTable(string table, bool useHidden = false, string where = "")
        {
            Conectar();
            string consulta = "SELECT COUNT(*) FROM " + table + " ";
            if(useHidden) { consulta += "WHERE (Oculto=FALSE) "; }
            consulta += where;

            OleDbCommand dbCommand = new OleDbCommand(consulta, dbConnection);
            object value = dbCommand.ExecuteScalar();
            if (value != DBNull.Value)
                return Convert.ToInt32(value);
            else
                return 0;
        }

        public static decimal SumTotalDate(DateTime dtIni, DateTime dtFin)
        {
            Conectar();
            string consulta = "SELECT SUM(Total) FROM Ventas WHERE (Fecha BETWEEN @dtIni AND @dtFin)";
            OleDbCommand dbCommand = new OleDbCommand(consulta, dbConnection);
            dbCommand.Parameters.AddWithValue("@dtIni", dtIni);
            dbCommand.Parameters.AddWithValue("@dtFin", dtFin);
            object value = dbCommand.ExecuteScalar();
            if (value != DBNull.Value) 
                return Convert.ToDecimal(value);
            else
                return 0; 
        }


        public static DateTime GetFirstOrLastDate(string table, bool last = false)
        {
            string query = string.Format("SELECT TOP 1 Fecha FROM {0} ORDER BY Fecha ", table);
            if (last)
                query += "DESC";
            else
                query += "ASC";

            OleDbCommand oleDbCommand = new OleDbCommand(query, dbConnection);
            object receiver = oleDbCommand.ExecuteScalar();
            if (receiver != DBNull.Value)
                return Convert.ToDateTime(receiver);
            else
                return new DateTime(2001, 6, 3);
        }

        public static DateTime GetFirstOrLastDate(string table, string columnName, bool last = false)
        {
            string query = string.Format("SELECT TOP 1 {1} FROM {0} ORDER BY {1} ", table, columnName);
            if (last)
                query += "DESC";
            else
                query += "ASC";

            OleDbCommand oleDbCommand = new OleDbCommand(query, dbConnection);
            object receiver = oleDbCommand.ExecuteScalar();
            if (receiver != DBNull.Value)
                return Convert.ToDateTime(receiver);
            else
                return new DateTime(2001, 6, 3);
        }
    }
}
