using InventarioWPF.PageCS;
using Microsoft.Win32;
using System;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace InventarioWPF.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Empleados.xaml
    /// </summary>

    public partial class Empleados : Window
    {     
        private string normalPath = @"C:\Inventario\Empleados\";
        private string pathImage, antImage;

        bool _Oculto = false;

        OleDbDataAdapter dbDataAdapter;
        DataSet dataSet = new DataSet();
        int cellsforpages = Properties.Settings.Default.CellsForPages;
        int total, indice, idEdit = 0;
        int myIdEmp;

        public Empleados(int idEmp)
        {
            InitializeComponent();
            myIdEmp = idEmp;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Update(true);
        }

        private void BtnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            indice = 0;
            dataSet.Tables["Empleados"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            indice -= cellsforpages;
            if (indice < 0)
                indice = 0;

            dataSet.Tables["Empleados"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            indice += cellsforpages;

            if (indice > total - 1)
                indice -= cellsforpages;

            dataSet.Tables["Empleados"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnLast_Click(object sender, RoutedEventArgs e)
        {
            if (total > 1)
            {
                indice = total - cellsforpages;
                indice = indice < 0 ? 0 : indice;

                dataSet.Tables["Empleados"].Clear();
                dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            indice = 0;
            string salida_datos = "";
            switch (cbFiltro.SelectedIndex)
            {
                case 0:
                    string[] palabra_Busqueda = txtSearch.Text.Split(' ');
                    foreach (string palabra in palabra_Busqueda)
                    {
                        if (salida_datos.Length == 0)
                        {
                            salida_datos = "(Empleados.C_R LIKE '%" + palabra + "%' OR Empleados.Nombre LIKE '%" + palabra + "%' OR Departamento.Nombre LIKE '%" + palabra +
                                "%' OR Empleados.Dir LIKE '%" + palabra + "%' OR Empleados.Email LIKE '%" + palabra + "%')";
                        }
                        else
                        {
                            salida_datos += " AND (Empleados.C_R LIKE '%" + palabra + "%' OR Empleados.Nombre LIKE '%" + palabra + "%' OR Departamento.Nombre LIKE '%" + palabra +
                                "%' OR Empleados.Dir LIKE '%" + palabra + "%' OR Empleados.Email LIKE '%" + palabra + "%')";
                        }
                    }                    
                    break;
                case 1:
                    salida_datos = "(Empleados.C_R LIKE '%" + txtSearch.Text + "%')";
                    break;
                case 2:
                    salida_datos = "(Empleados.Nombre LIKE '%" + txtSearch.Text + "%')";
                    break;
                case 3:
                    salida_datos = "(Departamento.Cargo LIKE '%" + txtSearch.Text + "%')";
                    break;
                case 4:
                    salida_datos = "(Empleados.Dir LIKE '%" + txtSearch.Text + "%')";
                    break;
                case 5:
                    salida_datos = "(Empleados.Email LIKE '%" + txtSearch.Text + "%')";
                    break;
            }

            if (txtSearch.Text.Length > 0)
                Update();
            else
                Update(false, salida_datos);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dwr = (DataRowView)(dgEmp).SelectedItem;
            if (dwr != null)
            {
                if (dwr["Dir"].ToString().Length > 0)
                {
                    Process.Start("https:maps.google.com/maps?q=" + dwr["Dir"].ToString());
                }
                else
                {
                    MessageBox.Show("Este proveedor tiene el campo Dirección vacio.", "Dirección vacia..", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("No ha seleccionado ningún proveedor para editar.", "Selección del proveedor...", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseStoryBoard.Begin();
            Show_OR_HideBackground(0.8, 0);
        }

        public void BtnShow_Click(object sender, RoutedEventArgs e)
        {
            LimpiarPantalla();
            Show_OR_HideBackground(0, 0.8);
            ShowStoryBoard.Begin();
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtCR.Text.Length == 0 || txtNom.Text.Length == 0 || txtDir.Text.Length == 0 || txtMovil.Text.Length == 0 || dp.Value == null)
            {
                txtMessage.Text = "Por favor llene los campos que contienen ateriscos.";
                showError.Visibility = Visibility.Visible;
                return;
            }

            if (cbDep.Items.Count == 0)
            {
                txtMessage.Text = "Por favor cree un Departamento antes de crear un empleado.";
                showError.Visibility = Visibility.Visible;
                return;
            }

            if (!VerificadorCI_RUC.VerificaIdentificacion(txtCR.Text)) {
                txtCR.SelectAll();
                txtCR.Focus();
                txtMessage.Text = "La Cédula o RUC no corresponde a una valida.\nEj.: C.I - 0911309714/R.U.C - 0911309714001";
                showError.Visibility = Visibility.Visible;               
                return;
            }

            if (txtMovil.Text.Length < 10 || txtMovil.Text.Substring(0, 2) != "09")
            {
                txtMovil.SelectAll();
                txtMovil.Focus();
                txtMessage.Text = "El Número de Teléfono Movil no tiene el formato correcto. Ej.: 099-999-9999\n" +
                    "*Recuerde: debe tener minimo 10 caracteres y empezar con 09.";
                showError.Visibility = Visibility.Visible;
                return;
            }

            if (!Helpers.validarEmail(txtMail.Text))
            {
                txtMail.SelectAll();
                txtMail.Focus();
                txtMessage.Text = "El Correo electronico no tiene el formato correcto. Ej.: usuario@dominio.ext";
                showError.Visibility = Visibility.Visible;
                return;
            }

            try
            {
                string consulta = "";
                string picture = "";

                //Guardar imagen a la carpeta.
                if (imgPhoto.Source != null)
                {
                    picture = normalPath + txtCR.Text + System.IO.Path.GetExtension(pathImage);
                    if (pathImage != antImage)
                    {
                        if (File.Exists(picture))
                            File.Delete(picture);

                        File.Copy(pathImage, picture);
                    }
                }

                if (idEdit == 0)
                {
                    consulta = string.Format("INSERT INTO Empleados (C_R, Nombre, Gener, Id_Dep, Dir, Telf, Movil, Email, F_Nac, Entry, Pict) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}')",
                        txtCR.Text, txtNom.Text, cbGener.Text, cbDep.SelectedValue, txtDir.Text, txtConv.Text, txtMovil.Text, txtMail.Text, dp.Value, DateTime.Now, pathImage);
                }
                else
                {
                    consulta = string.Format("UPDATE Empleados SET C_R='{0}', Pass='12345' Nombre='{1}', Gener='{2}', Id_Dep='{3}', Dir='{4}', Telf='{5}', Movil='{6}', Email='{7}', F_Nac='{8}', Modify='{9}', Pict='{10}' WHERE Id_Emp={11}", 
                        txtCR.Text, txtNom.Text, cbGener.Text, cbDep.SelectedValue, txtDir.Text, txtConv.Text, txtMovil.Text, txtMail.Text, dp.Value, DateTime.Now, pathImage, idEdit);                  
                }

                OleDbCommand cdUpdate = new OleDbCommand(consulta, AccessSystem.dbConnection);
                cdUpdate.ExecuteNonQuery();

                Update();

                Show_OR_HideBackground(0.8, 0);
                CloseStoryBoard.Begin();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("duplicados"))
                {
                    txtMessage.Text = "Por favor no repita números de Cédula o R.U.C!";
                    showError.Visibility = Visibility.Visible;
                    txtCR.SelectAll();
                    txtCR.Focus();
                }
                else
                    MessageBox.Show(ex.Message);
            }
        }

        private void LimpiarPantalla()
        {
            pathImage = antImage = "";
            imgPhoto.Source = null;
            btnQuit.Visibility = Visibility.Hidden;

            idEdit = 0;         
            txtCR.Text = "";
            txtNom.Text = "";
            cbGener.SelectedIndex = 0;
            cbDep.SelectedIndex = cbDep.Items.Count - 1;
            txtDir.Text = "";
            txtConv.Text = "";
            txtMovil.Text = "";
            txtMail.Text = "";
            showError.Visibility = Visibility.Collapsed;
        }

        private void Update(bool otherTables = false, string wheres = "")
        {
            try
            {
                bussy.IsBusy = true;

                cellsforpages = Properties.Settings.Default.CellsForPages;

                string nConsulta = Properties.Settings.Default.ConsultaEmp;

                if (wheres.Length > 0)
                {
                   nConsulta += " WHERE " + wheres;
                    if (!_Oculto)
                        nConsulta += " AND (Oculto = FALSE)";
                }
                else
                {
                    if (!_Oculto)
                        nConsulta += " WHERE (Oculto = FALSE)";
                }

                nConsulta += " ORDER BY Empleados.Nombre ASC";


                dataSet.Tables.Clear();
                dbDataAdapter = new OleDbDataAdapter(nConsulta, AccessSystem.dbConnection);
                dataSet.Tables.Add("Empleados");

                //Cargar todos los datos 
                dbDataAdapter.Fill(dataSet.Tables["Empleados"]);
                total = dataSet.Tables["Empleados"].Rows.Count;
                dataSet.Clear();

                if (total == 0)
                    txtNumbers.Text = "No se encontraron empleados registradas.";
                else if (total == 1)
                    txtNumbers.Text = "Se ha encontrado una venta registrada.";
                else
                    txtNumbers.Text = "Se ha encontraron " + total + " empleados registradas.";

                //Realizamos la primera paginación.
                dbDataAdapter.Fill(dataSet, indice, cellsforpages, "Empleados");
                dgEmp.ItemsSource = dataSet.Tables["Empleados"].DefaultView;

                if (otherTables) {
                    string sConsulta = "SELECT Id_Dep, Nombre FROM Departamento ORDER BY Nombre ASC";
                    OleDbDataAdapter depDataAdapter = new OleDbDataAdapter(sConsulta, AccessSystem.dbConnection);

                    //Cargar todos los datos 
                    dataSet.Tables.Add("Departamento");
                    depDataAdapter.Fill(dataSet.Tables["Departamento"]);

                    cbDep.ItemsSource = dataSet.Tables["Departamento"].DefaultView;
                    cbDep.DisplayMemberPath = "Nombre";
                    cbDep.SelectedValuePath = "Id_Dep";
                    cbDep.SelectedIndex = cbDep.Items.Count - 1;
                }

                bussy.IsBusy = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                txtSearch.Text = "";
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView dwr = (DataRowView)(dgEmp).SelectedItem;
                if (dwr != null)
                {
                    bool sysOculto = Convert.ToBoolean(dwr["Oculto"]);
                    if (myIdEmp != Convert.ToInt32(dwr["Id_Emp"])) {                      
                        MessageBoxResult result = MessageBox.Show("¿Seguro desea " + (sysOculto ? "Restaurar" : "Eliminar") + " este empleado?",
                            "A punto de " + (sysOculto ? "Restaurar" : "Eliminar") + "...", MessageBoxButton.YesNo, MessageBoxImage.Question);
                        if (result == MessageBoxResult.Yes)
                        {
                            string consulta = string.Format("UPDATE Empleados SET Oculto={0} WHERE (Id_Emp={1})", !sysOculto, dwr["Id_Emp"]);
                            OleDbCommand comando = new OleDbCommand(consulta, AccessSystem.dbConnection);
                            comando.ExecuteNonQuery();

                            MessageBox.Show("Empleado " + (sysOculto ? "Restaurado" : "Eliminado") + " Correctamente.");
                            Update();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Usted no puede " + (sysOculto ? "restaurarce" : "eliminarse"));
                    }
                }
                else
                {
                    MessageBox.Show("No ha seleccionado ningún empleado para eliminar.", "Selección del producto...", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dwr = (DataRowView)(dgEmp).SelectedItem;
            if (dwr != null)
            {
                LimpiarPantalla();
                antImage = pathImage = dwr["Pict"].ToString();
                if (antImage.Length > 0)
                {
                    btnQuit.Visibility = Visibility.Visible;
                    btnImage.Background = Brushes.Transparent;
                    imgPhoto.Source = new BitmapImage(new Uri(pathImage));
                }

                idEdit = Convert.ToInt32(dwr["Id_Emp"]);
                txtCR.Text = dwr["C_R"].ToString();
                txtNom.Text = dwr["Nombre"].ToString();
                cbGener.SelectedIndex = (dwr["Gener"].ToString() == "Masculino" ? 0 : 1);
                cbDep.SelectedIndex = Helpers.FindIndexByText(cbDep, dwr["Dep"].ToString());
                txtDir.Text = dwr["Dir"].ToString();
                txtConv.Text = dwr["Telf"].ToString();
                txtMovil.Text = dwr["Movil"].ToString();
                txtMail.Text = dwr["Email"].ToString();
                dp.Text = dwr["F_Nac"].ToString();

                Show_OR_HideBackground(0, 0.8);
                ShowStoryBoard.Begin();
            }
            else
            {
                MessageBox.Show("No ha seleccionado ningún empleado para editar.", "Selección del empleado...", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }   

        private void BtnImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.Title = "Select a picture";
            op.Filter = "All supported graphics|*.jpg;*.jpeg;*.png|" +
              "JPEG (*.jpg;*.jpeg)|*.jpg;*.jpeg|" +
              "Portable Network Graphic (*.png)|*.png";
            if (op.ShowDialog() == true)
            {
                pathImage = op.FileName;
                imgPhoto.Source = new BitmapImage(new Uri(pathImage));
                btnImage.Background = Brushes.Transparent;
                btnQuit.Visibility = Visibility.Visible;
            }
        }

        private void BtnQuit_Click(object sender, RoutedEventArgs e)
        {
            pathImage = "";
            imgPhoto.Source = null;
            btnImage.Background = Brushes.Gray;
            btnQuit.Visibility = Visibility.Hidden;
        }

        private void TxtCR_PreviewKeyDown(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Show_OR_HideBackground(double getValue, double setValue)
        {
            if (getValue == 0) { backColor.Visibility = Visibility.Visible; }
            var showAnimation = new DoubleAnimation();
            showAnimation.From = getValue;
            showAnimation.To = setValue;
            showAnimation.AutoReverse = false;

            backColor.BeginAnimation(Border.OpacityProperty, showAnimation);
            if (setValue == 0) { backColor.Visibility = Visibility.Hidden; }
        }

        private void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Update(true);
        }

        private void BtnSDelete_Click(object sender, RoutedEventArgs e)
        {
            _Oculto = !_Oculto;

            if (_Oculto)
            {
                ((MenuItem)sender).Header = "Ocultar Eliminados";
                hideDelete.Visibility = Visibility.Visible;
            }
            else
            {
                ((MenuItem)sender).Header = "Mostrar Eliminados";
                hideDelete.Visibility = Visibility.Hidden;
            }

            Update();
        }
    }
}
