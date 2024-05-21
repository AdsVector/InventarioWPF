using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventarioWPF.PageCS
{
    public class Usuario
    {
        public int IdUser { get; }
        public string CedUser { get; }
        public string PassUser { get; }
        public string Nombre { get; }
        public string Genero { get; }
        public Departamento _Departamento  { get; }
        public string Dir { get; }
        public string Telf { get; }
        public string Movil { get; }
        public string Email { get; }
        public DateTime Nac { get; }
        public Uri IPhoto { get; }
        private DateTime Entry { get; }
        public DateTime Exit { get; set; }

        public Usuario(int idUser, string cedUser, string pass, string nombre, string Gener, int idCargo, string dir, string telf, string movil, string email, DateTime nac, string photo)
        {
            this.IdUser = idUser;
            this.CedUser = cedUser;
            this.PassUser = pass;
            this.Nombre = nombre;
            this.Genero = Gener;
            this._Departamento = new Departamento(idCargo);
            this.Dir = dir;
            this.Telf = telf;
            this.Movil = movil;
            this.Email = email;
            this.Nac = nac;
            this.Entry = DateTime.Now;

            if (!string.IsNullOrEmpty(photo))
                this.IPhoto = new Uri(photo);
            else
                this.IPhoto = new Uri(Directory.GetCurrentDirectory() + "/Resources/customers_48px.png");
        }

        public static bool UsuarioExit(Usuario usuario)
        {
            try
            {
                usuario.Exit = DateTime.Now;
                string consulta = string.Format("INSERT INTO R_Sesion (Id_Emp, Entry, Exit) VALUES ('{0}', '{1}', '{2}')", usuario.IdUser, usuario.Entry, usuario.Exit);
                OleDbCommand dbCommand = new OleDbCommand(consulta, AccessSystem.dbConnection);
                return Convert.ToInt32(dbCommand.ExecuteNonQuery()) == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
