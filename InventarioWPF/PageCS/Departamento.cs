using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioWPF.PageCS
{
    public class Departamento
    {
        public string NameCargo { get; }
        public bool AccessInv { get; }
        public bool AccessPro { get; }
        public bool AccessVen { get; }
        public bool AccessEmp { get; }
        public bool AccessDep { get; }
        public bool AccessReS { get; }

        public Departamento(int idCargo)
        {
            string consDep = "SELECT * FROM Departamento WHERE Id_Dep = @Dep";
            OleDbCommand command = new OleDbCommand(consDep, AccessSystem.dbConnection);
            command.Parameters.AddWithValue("@Dep", idCargo);

            OleDbDataReader dbDep = command.ExecuteReader();
            if (dbDep.Read())
            {
                NameCargo = dbDep[1].ToString();
                AccessInv = Convert.ToBoolean(dbDep[2]);
                AccessPro = Convert.ToBoolean(dbDep[3]);
                AccessVen = Convert.ToBoolean(dbDep[4]);
                AccessEmp = Convert.ToBoolean(dbDep[5]);
                AccessDep = Convert.ToBoolean(dbDep[6]);
                AccessReS = Convert.ToBoolean(dbDep[7]);
            }
        }

    }
}
