using System;
using System.Windows;
using System.Windows.Media.Imaging;
using InventarioWPF.PageCS;
using InventarioWPF.Ventanas;

namespace InventarioWPF
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>

    public class BoolToStringConverter : BoolToValueConverter<String> { }

    public partial class MainWindow : Window
    {
        bool logOut;
        public Usuario thisUsu;

        public MainWindow(string ide)
        {
            InitializeComponent();

            thisUsu = AccessSystem.GetEmployee(ide);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            txtName.Text = thisUsu.Nombre;
            txtCargo.Text = thisUsu._Departamento.NameCargo;

            //Establecer los datos como deben ser

            idU.Text = thisUsu.CedUser;
            nom.Text = thisUsu.Nombre;
            gen.Text = thisUsu.Genero;
            dep.Text = thisUsu._Departamento.NameCargo;
            dir.Text = thisUsu.Dir;
            tel.Text = thisUsu.Telf;
            mol.Text = thisUsu.Movil;
            ema.Text = thisUsu.Email;
            nac.Text = thisUsu.Nac.ToString();

            intCells.Value = Properties.Settings.Default.CellsForPages;
            checkRecord.IsChecked = Properties.Settings.Default.RecordUser;

            ibPhoto.ImageSource = new BitmapImage(thisUsu.IPhoto);

            //---------------------Data de bool---------------------------//
            btnInven.IsEnabled = thisUsu._Departamento.AccessInv;
            btnSupplier.IsEnabled = thisUsu._Departamento.AccessPro;
            btnVentas.IsEnabled = thisUsu._Departamento.AccessVen;
            btnEmp.IsEnabled = thisUsu._Departamento.AccessEmp;
            btnDep.IsEnabled = thisUsu._Departamento.AccessDep;
            btnLog.IsEnabled = thisUsu._Departamento.AccessReS;
            Update();
        }

        public void Update()
        {
            viewEmp.Value = AccessSystem.CountInTable("Empleados", true).ToString();
            txtValue.Text = AccessSystem.CountInTable("Productos", true, "AND (Stock BETWEEN 0 AND 25)").ToString();
            GetAnioTotal();
            GetMesTotal();
        }

        private void GetAnioTotal()
        {
            int var_anio = DateTime.Now.Year; // obtengo el año actual
            DateTime dateIni = new DateTime(var_anio, 1, 1, 0, 0, 0);
            DateTime dateFin = new DateTime(var_anio, 12, 31, 23, 59, 59);

            decimal value = AccessSystem.SumTotalDate(dateIni, dateFin);
            viewAno.Value = string.Format("{0:C}", value);
        }

        private void GetMesTotal()
        {
            DateTime thisNow = DateTime.Now;
            DateTime primerDiaMes = new DateTime(thisNow.Year, thisNow.Month, 1, 0, 0, 0);
            DateTime ultimoDiaMes = new DateTime(thisNow.Year, thisNow.Month, 1, 23, 59, 59).AddMonths(1).AddDays(-1);

            decimal value = AccessSystem.SumTotalDate(primerDiaMes, ultimoDiaMes);
            viewTot.Value = string.Format("{0:C}", value);
        }

        private void BtnInven_Click(object sender, RoutedEventArgs e)
        {
            Inventario inventario = Helpers.ReturnWindowOpen<Inventario>() as Inventario;

            if (inventario != null)
            {
                if (inventario.WindowState == WindowState.Minimized) { inventario.WindowState = WindowState.Normal; }
                inventario.Topmost = true;
                inventario.Topmost = false;
                inventario.Focus();
                return;
            }

            inventario = new Inventario();
            inventario.Show();
        }

        private void BtnSupplier_Click(object sender, RoutedEventArgs e)
        {
            Proveedores proveedores = Helpers.ReturnWindowOpen<Proveedores>() as Proveedores;

            if (proveedores != null)
            {
                if (proveedores.WindowState == WindowState.Minimized) { proveedores.WindowState = WindowState.Normal; }
                proveedores.Topmost = true;
                proveedores.Topmost = false;
                proveedores.Focus();
                return;
            }

            proveedores = new Proveedores();
            proveedores.Show();
        }

        public void BtnVentas_Click(object sender, RoutedEventArgs e)
        {
            Ventas ventas = Helpers.ReturnWindowOpen<Ventas>() as Ventas;

            if (ventas != null)
            {
                if (ventas.WindowState == WindowState.Minimized) { ventas.WindowState = WindowState.Normal; }
                ventas.Topmost = true;
                ventas.Topmost = false;
                ventas.Focus();
                return;
            }

            ventas = new Ventas(thisUsu.IdUser, thisUsu.Nombre);
            ventas.Show();
        }

        private void BtnEmp_Click(object sender, RoutedEventArgs e)
        {
            Empleados empleados = Helpers.ReturnWindowOpen<Empleados>() as Empleados;

            if (empleados != null) {
                if (empleados.WindowState == WindowState.Minimized) { empleados.WindowState = WindowState.Normal; }
                empleados.Topmost = true;
                empleados.Topmost = false;
                empleados.Focus();
                return;
            }

            empleados = new Empleados(thisUsu.IdUser);
            empleados.Show();
        }

        private void BtnLog_Click(object sender, RoutedEventArgs e)
        {
            Reg_Sesiones regSesiones = Helpers.ReturnWindowOpen<Reg_Sesiones>() as Reg_Sesiones;

            if (regSesiones != null)
            {
                if (regSesiones.WindowState == WindowState.Minimized) { regSesiones.WindowState = WindowState.Normal; }
                regSesiones.Topmost = true;
                regSesiones.Topmost = false;
                regSesiones.Focus();
                return;
            }

            regSesiones = new Reg_Sesiones();
            regSesiones.Show();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!logOut) {
                MessageBoxResult result = MessageBox.Show("¿Seguro desea salir?", "A punto de salir...", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    if (Usuario.UsuarioExit(thisUsu))
                    {
                        CloseChildren();
                        e.Cancel = false;
                    }
                    else
                        e.Cancel = true;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            logOut = true;
            CloseChildren();
            if (Usuario.UsuarioExit(thisUsu))
            {
                AccessSystem.CloseSession(this);
            }
        }

        private void CloseChildren()
        {
            Reg_Sesiones regSesiones = Helpers.ReturnWindowOpen<Reg_Sesiones>() as Reg_Sesiones;
            if (regSesiones != null)
                regSesiones.Close();

            Empleados empleados = Helpers.ReturnWindowOpen<Empleados>() as Empleados;
            if (empleados != null)
                empleados.Close();

            Ventas ventas = Helpers.ReturnWindowOpen<Ventas>() as Ventas;
            if (ventas != null)
                ventas.Close();

            Proveedores proveedores = Helpers.ReturnWindowOpen<Proveedores>() as Proveedores;
            if (proveedores != null)
                proveedores.Close();

            Inventario inventario = Helpers.ReturnWindowOpen<Inventario>() as Inventario;
            if (inventario != null)
                inventario.Close();
        }

        private void BtnDep_Click(object sender, RoutedEventArgs e)
        {
            Cargos cargos = Helpers.ReturnWindowOpen<Cargos>() as Cargos;

            if (cargos != null)
            {
                if (cargos.WindowState == WindowState.Minimized) { cargos.WindowState = WindowState.Normal; }
                cargos.Topmost = true;
                cargos.Topmost = false;
                cargos.Focus();
                return;
            }

            cargos = new Cargos(txtCargo.Text);
            cargos.Show();
        }

        private void BtnReport_Click(object sender, RoutedEventArgs e)
        {
            Reporte reporte = Helpers.ReturnWindowOpen<Reporte>() as Reporte;

            if (reporte != null)
            {
                if (reporte.WindowState == WindowState.Minimized) { reporte.WindowState = WindowState.Normal; }
                reporte.Topmost = true;
                reporte.Topmost = false;
                reporte.Focus();
                return;
            }

            reporte = new Reporte();
            reporte.Show();
        }

        private void SavePass_Click(object sender, RoutedEventArgs e)
        {
            if (apass.Password == thisUsu.PassUser)
            {
                if (npass.Password == cpass.Password)
                {
                    AccessSystem.ChangePass(thisUsu.IdUser, npass.Password);
                    MessageBox.Show("Contraseña cambiada correctamente.", "Ha cambiado la contraseña");
                }
                else
                {
                    MessageBox.Show("La confirmacion de contraseña no es identica.", "Error de usuario...", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Contraseña incorrecta.", "Error de usuario...", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void IntCells_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            Properties.Settings.Default.CellsForPages = (int)intCells.Value;
            Properties.Settings.Default.Save();
        }

        private void CheckRecord_Checked(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.RecordUser = (bool)checkRecord.IsChecked;         

            if ((bool)checkRecord.IsChecked)
                Properties.Settings.Default.UserSave = idU.Text;
            else
                Properties.Settings.Default.UserSave = "";

            Properties.Settings.Default.Save();
        }
    }
}
