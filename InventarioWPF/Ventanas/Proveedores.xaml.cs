using InventarioWPF.PageCS;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace InventarioWPF.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Proveedores.xaml
    /// </summary>

    public partial class Proveedores : Window
    {
        private string normalPath = @"C:\Inventario\Proveedores\";
        private string pathImage, antImage;

        bool _Oculto = false;

        OleDbDataAdapter dbDataAdapter;
        DataSet dataSet = new DataSet();
        int cellsforpages = Properties.Settings.Default.CellsForPages;
        int total, indice, idEdit = 0;

        public Proveedores()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void BtnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            indice = 0;
            dataSet.Tables["Proveedores"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            indice -= cellsforpages;
            if (indice < 0)
                indice = 0;

            dataSet.Tables["Proveedores"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            indice += cellsforpages;

            if (indice > total - 1)
                indice -= cellsforpages;

            dataSet.Tables["Proveedores"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnLast_Click(object sender, RoutedEventArgs e)
        {
            if (total > 1)
            {
                indice = total - cellsforpages;
                indice = indice < 0 ? 0 : indice;

                dataSet.Tables["Proveedores"].Clear();
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
                            salida_datos = "(C_R LIKE '%" + palabra + "%' OR Nombre LIKE '%" + palabra + "%' OR Dir LIKE '%" + palabra + "%' OR RorG LIKE '%" + palabra + "%' OR Email LIKE '%" + palabra + "%')";
                        }
                        else
                        {
                            salida_datos += " AND (C_R LIKE '%" + palabra + "%' OR Nombre LIKE '%" + palabra + "%' OR Dir LIKE '%" + palabra + "%' OR RorG LIKE '%" + palabra + "%' OR Email LIKE '%" + palabra + "%')";
                        }
                    }
                    break;
                case 1:
                    salida_datos = "(C_R LIKE '%" + txtSearch.Text + "%')";
                    break;
                case 2:
                    salida_datos = "(Nombre LIKE '%" + txtSearch.Text + "%')";
                    break;
                case 3:
                    salida_datos = "(RorG LIKE '%" + txtSearch.Text + "%')";
                    break;
                case 4:
                    salida_datos = "(Dir LIKE '%" + txtSearch.Text + "%')";
                    break;
                case 5:
                    salida_datos = "(Email LIKE '%" + txtSearch.Text + "%')";
                    break;
            }

            Update(salida_datos);
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
            if (txtCR.Text.Length == 0 || txtNom.Text.Length == 0 || txtRorG.Text.Length == 0 || txtDir.Text.Length == 0 || txtMovil.Text.Length == 0)
            {
                txtMessage.Text = "Por favor llene los campos que contienen ateriscos.";
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
                string myNote = new TextRange(txtNotes.Document.ContentStart, txtNotes.Document.ContentEnd).Text;
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
                    consulta = "INSERT INTO Proveedores (C_R, Nombre, RorG, Dir, Telf, Movil, Email, Web, Notes, Pict, Entry) VALUES (@CR, @No, @RG, @Di, @Co, @Mo, @Ma, @We, @My, @Pi, @Fe)";
                }
                else
                {
                    consulta = "UPDATE Proveedores SET C_R=@Cr, Nombre=@No, RorG=@Rg, Dir=@Di, Telf=@Co, Movil=@Mo, Email=@Ma, Web=@We, Notes=@My, Pict=@Pi, Modify=@Fe WHERE Id_Sup = " + idEdit;                  
                }

                OleDbCommand cdUpdate = new OleDbCommand(consulta, AccessSystem.dbConnection);
                cdUpdate.Parameters.AddWithValue("@Cr", txtCR.Text);
                cdUpdate.Parameters.AddWithValue("@No", txtNom.Text);
                cdUpdate.Parameters.AddWithValue("@Rg", txtRorG.Text);
                cdUpdate.Parameters.AddWithValue("@Di", txtDir.Text);
                cdUpdate.Parameters.AddWithValue("@Co", txtConv.Text);
                cdUpdate.Parameters.AddWithValue("@Mo", txtMovil.Text);
                cdUpdate.Parameters.AddWithValue("@Ma", txtMail.Text);
                cdUpdate.Parameters.AddWithValue("@We", txtWeb.Text);
                cdUpdate.Parameters.AddWithValue("@My", myNote);
                cdUpdate.Parameters.AddWithValue("@Pi", pathImage);
                cdUpdate.Parameters.AddWithValue("@Fe", DateTime.Now.ToString()); // Fecha de hoy.                      
                cdUpdate.ExecuteNonQuery();

                Update();

                Inventario inventario = Helpers.ReturnWindowOpen<Inventario>() as Inventario;
                if (inventario != null) { inventario.BtnUpdate_Click(sender, e); }

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
                    MessageBox.Show(ex.Message );
            }
        }

        private void LimpiarPantalla()
        {
            idEdit = 0;
            txtCR.Text = "";
            txtNom.Text = "";
            txtRorG.Text = "";
            txtDir.Text = "";
            txtConv.Text = "";
            txtMovil.Text = "";
            txtMail.Text = "";
            txtWeb.Text = "";
            txtNotes.Document.Blocks.Clear();
            pathImage = antImage = "";
            imgPhoto.Source = null;
            btnQuit.Visibility = Visibility.Hidden;
            showError.Visibility = Visibility.Collapsed;
        }

        private void Update(string wheres = "")
        {
            try
            {
                bussy.IsBusy = true;

                cellsforpages = Properties.Settings.Default.CellsForPages;

                string nConsulta = Properties.Settings.Default.ConsultaPro;

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

                nConsulta += " ORDER BY Nombre ASC";


                dataSet.Tables.Clear();
                dbDataAdapter = new OleDbDataAdapter(nConsulta, AccessSystem.dbConnection);
                dataSet.Tables.Add("Proveedores");

                //Cargar todos los datos 
                dbDataAdapter.Fill(dataSet.Tables["Proveedores"]);
                total = dataSet.Tables["Proveedores"].Rows.Count;
                dataSet.Clear();

                if (total == 0)
                    txtNumbers.Text = "No se encontraron Proveedores registradas.";
                else if (total == 1)
                    txtNumbers.Text = "Se ha encontrado una venta registrada.";
                else
                    txtNumbers.Text = "Se ha encontraron " + total + " Proveedores registradas.";

                //Realizamos la primera paginación.
                dbDataAdapter.Fill(dataSet, indice, cellsforpages, "Proveedores");
                dgEmp.ItemsSource = dataSet.Tables["Proveedores"].DefaultView;

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
                    MessageBoxResult result = MessageBox.Show("¿Seguro desea " + (sysOculto ? "Restaurar" : "Eliminar") + " este proveedor?",
                        "A punto de " + (sysOculto ? "Restaurar" : "Eliminar") + "...", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        string consulta = string.Format("UPDATE Proveedores SET Oculto={0} WHERE (Id_Sup={1})", !sysOculto, dwr["Id_Sup"]);
                        OleDbCommand comando = new OleDbCommand(consulta, AccessSystem.dbConnection);
                        comando.ExecuteNonQuery();

                        Update();
                    }
                }
                else
                {
                    MessageBox.Show("No ha seleccionado ningún proveedor para eliminar.", "Selección del proveedor...", MessageBoxButton.OK, MessageBoxImage.Warning);
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

                idEdit = Convert.ToInt32(dwr["Id_Sup"]);
                txtCR.Text = dwr["C_R"].ToString();
                txtNom.Text = dwr["Nombre"].ToString();
                txtRorG.Text = dwr["RorG"].ToString();
                txtDir.Text = dwr["Dir"].ToString();
                txtConv.Text = dwr["Telf"].ToString();
                txtMovil.Text = dwr["Movil"].ToString();
                txtMail.Text = dwr["Email"].ToString();
                txtWeb.Text = dwr["Web"].ToString();
                txtNotes.Document.Blocks.Add(new Paragraph(new Run(dwr["Notes"].ToString())));

                Show_OR_HideBackground(0, 0.8);
                ShowStoryBoard.Begin();
            }
            else
            {
                MessageBox.Show("No ha seleccionado ningún proveedor para editar.", "Selección del proveedor...", MessageBoxButton.OK, MessageBoxImage.Warning);
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dwr = (DataRowView)(dgEmp).SelectedItem;
            if (dwr != null)
            {
                if (dwr["Dir"].ToString().Length > 0) {
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

        private void ShowWeb_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dwr = (DataRowView)(dgEmp).SelectedItem;
            if (dwr != null)
            {
                if (dwr["Web"].ToString().Length > 0)
                {
                    Process.Start(dwr["Web"].ToString());
                }
                else
                {
                    MessageBox.Show("Este proveedor tiene el campo Web vacio.", "Sitio Web vacia..", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                MessageBox.Show("No ha seleccionado ningún proveedor para editar.", "Selección del proveedor...", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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
            Update();
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
