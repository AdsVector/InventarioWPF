using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using InventarioWPF.PageCS;

namespace InventarioWPF
{
    /// <summary>
    /// Lógica de interacción para Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!AccessSystem.Conectar())
            {
                MessageBox.Show("Lo sentimos hemos detectado un error con la base de datos.\n" +
                    "Contacte con un supervisor a cargo.", "Error - Cerraremos por seguridad.", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
            }
            string user = Properties.Settings.Default.UserSave;
            if (user.Length > 0)
            {
                txtUsu.Text = user;
                txtPass.Focus();
            }
        }

        public void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void BtnMini_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        public void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            if (txtPass.Password.Length == 0 || txtUsu.Text.Length == 0)
            {
                MessageBox.Show("Inserte Usuario y Contraseña.", "Error de validación", MessageBoxButton.OK, MessageBoxImage.Information);

                return;
            }
            else
            {
                if (!AccessSystem.Login(txtUsu.Text, txtPass.Password))
                {
                    MessageBox.Show("Verifique el usuario y la contraseña, estan incorrectos.", "Error de datos", MessageBoxButton.OK, MessageBoxImage.Warning);
                    this.txtUsu.Focus();
                    return;
                }

                if (Properties.Settings.Default.RecordUser)
                {
                    Properties.Settings.Default.UserSave = txtUsu.Text;
                    Properties.Settings.Default.Save();
                }

                MainWindow main = new MainWindow(txtUsu.Text);
                main.Show();
                this.Close();
            }
        }

        public void BtnHelp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("1er Recuadro - Usuario: este campo se rellena con tu cédula.\n" +
                            "2do Recuadro - Contraseña: este campo se rellena con tu contraseña.\n" +
                            "Si no recuerdas tu contraseña o ha sido cambiada por A o B motivo, por favor acercarse con supervisor o encargado.");
        }

        public void Textbox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BtnOk_Click(sender, null);
            }
        }

      
    }
}
