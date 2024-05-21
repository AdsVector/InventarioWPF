using System;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using InventarioWPF.PageCS;
using Microsoft.Win32;

namespace InventarioWPF.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Inventario.xaml
    /// </summary>    
    public partial class Inventario : Window
    {
        private string normalPath = @"C:\Inventario\Productos\";
        private string pathImage, antImage;
        
        bool _Oculto = false;

        BackgroundWorker worker = new BackgroundWorker();
        OleDbDataAdapter dbDataAdapter;
        DataSet dataSet = new DataSet();
        int cellsforpages = Properties.Settings.Default.CellsForPages;
        int total, indice, idEdit = 0;

        public Inventario()
        {
            InitializeComponent();           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            worker.DoWork += worker_DoWork;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted;

            Update(true);
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] dataTables = new object[2];          
          
            DataTable tableCat = new DataTable("Categorias");
            string cConsulta = "SELECT Id_Cat, Nombre FROM Categorias ORDER BY Nombre ASC";
            OleDbDataAdapter catDataReader = new OleDbDataAdapter(cConsulta, AccessSystem.dbConnection);
            catDataReader.Fill(tableCat);


            DataTable tablePro = new DataTable("Proveedores");
            string pConsulta = "SELECT Id_Sup, Nombre FROM Proveedores WHERE (Oculto=FALSE) ORDER BY Nombre ASC";
            OleDbDataAdapter proDataReader = new OleDbDataAdapter(pConsulta, AccessSystem.dbConnection);
            proDataReader.Fill(tablePro);


            dataTables[0] = tableCat;
            dataTables[1] = tablePro;

            e.Result = dataTables;
        }

        private void worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            object[] dataTables = e.Result as object[];

            dataSet.Tables.Add(dataTables[0] as DataTable);
            dataSet.Tables.Add(dataTables[1] as DataTable);
         
            //Cargar los datos en combobox
            cbCat.ItemsSource = dataSet.Tables["Categorias"].DefaultView;
            cbCat.DisplayMemberPath = "Nombre";
            cbCat.SelectedValuePath = "Id_Cat";
            cbCat.SelectedIndex = cbCat.Items.Count - 1;

            cbPro.ItemsSource = dataSet.Tables["Proveedores"].DefaultView;
            cbPro.DisplayMemberPath = "Nombre";
            cbPro.SelectedValuePath = "Id_Sup";
            cbPro.SelectedIndex = cbPro.Items.Count - 1;

            bussy.IsBusy = false;
        }

        private void BtnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            indice = 0;
            dataSet.Tables["Mercaderia"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            indice -= cellsforpages;
            if (indice < 0)
                indice = 0;

            dataSet.Tables["Mercaderia"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            indice += cellsforpages;

            if (indice > total - 1)
                indice -= cellsforpages;

            dataSet.Tables["Mercaderia"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnLast_Click(object sender, RoutedEventArgs e)
        {
            if (total > 1)
            {
                indice = total - cellsforpages;
                indice = indice < 0 ? 0 : indice;

                dataSet.Tables["Mercaderia"].Clear();
                dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            indice = 0;
            string salida_datos = "";
            if (txtSearch.Text.Length > 0) {
                switch (cbFiltro.SelectedIndex)
                {
                    case 0:
                        string[] palabra_Busqueda = txtSearch.Text.Split(' ');
                        foreach (string palabra in palabra_Busqueda)
                        {
                            if (salida_datos.Length == 0)
                                salida_datos = " (Productos.Code LIKE '%" + palabra + "%' OR  Productos.Descrip LIKE '%" + palabra + "%' OR Categorias.Nombre LIKE '%" + palabra + "%' OR Proveedores.Nombre LIKE '%" + palabra + "%') ";
                            else
                                salida_datos += "AND (Productos.Code LIKE '%" + palabra + "%' OR Productos.Descrip LIKE '%" + palabra + "%' OR Categorias.Nombre LIKE '%" + palabra + "%' OR Proveedores.Nombre LIKE '%" + palabra + "%')";
                        }
                        break;
                    case 1:
                        salida_datos = " (Productos.Code LIKE '%" + txtSearch.Text + "%')";
                        break;
                    case 2:
                        salida_datos = " (Productos.Descrip LIKE '%" + txtSearch.Text + "%')";
                        break;
                    case 3:
                        salida_datos = " (Categorias.Nombre LIKE '%" + txtSearch.Text + "%')";
                        break;
                    case 4:
                        salida_datos = " (Proveedores.Nombre LIKE '%" + txtSearch.Text + "%')";
                        break;
                    case 5:
                        int i = 0;
                        if (int.TryParse(txtSearch.Text, out i)) { salida_datos = " (Productos.Stock = " + txtSearch.Text + ")"; }
                        break;                   
                }               
            }

            if (salida_datos.Length > 0)
                Update(false, salida_datos);
            else
                Update();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            CloseStoryBoard.Begin();
            Show_OR_HideBackground(0.8, 0);        
        }

        private void BtnShow_Click(object sender, RoutedEventArgs e)
        {
            LimpiarPantalla();           
            Show_OR_HideBackground(0, 0.8);
            ShowStoryBoard.Begin();
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dwr = (DataRowView)(dgUsers).SelectedItem;
            if (dwr != null) {
                LimpiarPantalla();
                antImage = pathImage = dwr["Pict"].ToString();
                if (antImage.Length > 0) {
                    btnQuit.Visibility = Visibility.Visible;
                    btnImage.Background = Brushes.Transparent;
                    imgPhoto.Source = new BitmapImage(new Uri(pathImage));
                }
                                                                           
                idEdit = Convert.ToInt32(dwr["Id_Pro"]);
                txtCod.Text = dwr["Code"].ToString();
                txtDes.Text = dwr["Descrip"].ToString();
                cbCat.SelectedIndex = Helpers.FindIndexByText(cbCat, dwr["Categoria"].ToString());
                decCost.Value = Convert.ToDecimal(dwr["Cost"]);
                decPre.Value = Convert.ToDecimal(dwr["Price"]);
                intSt.Value = Convert.ToInt32(dwr["Stock"]);
                cbPro.SelectedIndex = Helpers.FindIndexByText(cbPro, dwr["Proveedor"].ToString());
                txtNote.Document.Blocks.Add(new Paragraph(new Run(dwr["Notes"].ToString())));

                Show_OR_HideBackground(0, 0.8);
                ShowStoryBoard.Begin();
            }
            else
            {
                MessageBox.Show("No ha seleccionado ningún producto para editar.", "Selección del producto...", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (txtCod.Text.Length == 0)
            {
                txtMessage.Text = "No puedes dejar el código vacio.";
                showError.Visibility = Visibility.Visible;
                txtCod.Focus();
            }

            if (txtDes.Text.Length == 0)
            {
                txtMessage.Text = "No puedes dejar la descripción vacia.";
                showError.Visibility = Visibility.Visible;
                txtDes.Focus();
            }

            if (cbPro.Items.Count == 0)
            {
                txtMessage.Text = "No puedes crear sin tener primero un proveedor.";
                showError.Visibility = Visibility.Visible;
            }


            try {
                string consulta = "";
                string picture = "";

                //Guardar imagen a la carpeta.
                if (imgPhoto.Source != null) {
                    picture = normalPath + txtCod.Text + System.IO.Path.GetExtension(pathImage);
                    if (pathImage != antImage) {
                        if (File.Exists(picture))
                            File.Delete(picture);

                        File.Copy(pathImage, picture);
                    }
                }

                string myNote = new TextRange(txtNote.Document.ContentStart, txtNote.Document.ContentEnd).Text;

                if (idEdit == 0)
                {
                    consulta = string.Format("INSERT INTO Productos (Code, Descrip, Id_Cat, Cost, Price, Stock, Id_Sup, Notes, Entry, Pict) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}')",
                                            txtCod.Text, txtDes.Text, cbCat.SelectedValue, decCost.Value, decPre.Value, intSt.Value, cbPro.SelectedValue, myNote, DateTime.Now, picture);
                }
                else
                {
                    consulta = string.Format("UPDATE Productos SET Code='{0}', Descrip='{1}', Id_Cat='{2}', Cost='{3}', Price='{4}', Stock='{5}', Id_Sup='{6}', Notes='{7}', Modify='{8}', Pict='{9}' WHERE Id_Pro={10}",
                                            txtCod.Text, txtDes.Text, cbCat.SelectedValue, decCost.Value, decPre.Value, intSt.Value, cbPro.SelectedValue, myNote, DateTime.Now, picture, idEdit);
                }

                OleDbCommand comando = new OleDbCommand(consulta, AccessSystem.dbConnection);
                comando.ExecuteNonQuery();
                
                Update();

                Show_OR_HideBackground(0.8, 0);
                CloseStoryBoard.Begin();
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("duplicados"))
                {
                    txtMessage.Text = "Codigo ya existente, por favor escriba uno nuevo.";
                    showError.Visibility = Visibility.Visible;
                    txtCod.SelectAll();
                    txtCod.Focus();
                }
                else
                {
                    txtMessage.Text = ex.Message;
                    showError.Visibility = Visibility.Visible;
                }
            }
        }

        private void LimpiarPantalla()
        {
            showError.Visibility = Visibility.Collapsed;
            pathImage = antImage = "";
            imgPhoto.Source = null;
            btnQuit.Visibility = Visibility.Hidden;
            txtCod.Text = "";
            txtDes.Text = "";
            cbCat.SelectedIndex = cbCat.Items.Count - 1;
            decCost.Value = decCost.Minimum;
            decPre.Value = decPre.Minimum;
            intSt.Value = intSt.Minimum;
            cbPro.SelectedIndex = cbPro.Items.Count - 1;
            txtNote.Document.Blocks.Clear();          
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

        private void CbCat_LostFocus(object sender, RoutedEventArgs e)
        {
            SaveCategory();
        }

        private void CbCat_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                SaveCategory();
            }
        }

        private void CbPro_KeyDown(object sender, KeyEventArgs e)
        {
            if (cbPro.SelectedIndex == -1)
            {
                MessageBox.Show("Lo sentimos debe elegir un proveedor ya existente.", "No existe proveedor");
            }
        }

        private void CbPro_LostFocus(object sender, RoutedEventArgs e)
        {
            if (cbPro.SelectedIndex == -1)
            {
                MessageBox.Show("Lo sentimos debe elegir un proveedor ya existente.", "No existe proveedor");
            }
        }

        public void BtnUpdate_Click(object sender, RoutedEventArgs e)
        {
            Update(true);
        }

        private void Update(bool othersTables = false, string wheres = "")
        {
            cellsforpages = Properties.Settings.Default.CellsForPages;
            string nConsulta = Properties.Settings.Default.ConsultaInv;
            try
            {
                bussy.IsBusy = true;
             
                if (wheres.Length > 0)
                {
                    nConsulta += " WHERE " + wheres;
                    if (!_Oculto)
                        nConsulta += " AND (Productos.Oculto=FALSE)";
                }
                else
                {
                    if (!_Oculto)
                        nConsulta += " WHERE (Productos.Oculto=FALSE)";
                }

                nConsulta += " ORDER BY Productos.Descrip ASC";

                dataSet.Tables.Clear();
                dbDataAdapter = new OleDbDataAdapter(nConsulta, AccessSystem.dbConnection);
                dataSet.Tables.Add("Mercaderia");

                //Cargar todos los datos 
                dbDataAdapter.Fill(dataSet.Tables["Mercaderia"]);
                total = dataSet.Tables["Mercaderia"].Rows.Count;
                dataSet.Clear();

                if (total == 0)
                    txtNumbers.Text = "No se encontraron productos registrados.";
                else if (total == 1)
                    txtNumbers.Text = "Se ha encontraron un producto registrado.";
                else
                    txtNumbers.Text = "Se ha encontraron " + total + " productos registrados.";

                //Realizamos la primera paginación.
                dbDataAdapter.Fill(dataSet, indice, cellsforpages, "Mercaderia");
                dgUsers.ItemsSource = dataSet.Tables["Mercaderia"].DefaultView;                                   
                
                if (othersTables)            
                    worker.RunWorkerAsync();
                else
                    bussy.IsBusy = false;

                MainWindow main = Helpers.ReturnWindowOpen<MainWindow>() as MainWindow;
                if (main != null) { main.Update(); }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + " - " + nConsulta);
                txtSearch.Text = "";
            }
        }

        private void BtnDelete_Click(object sender, RoutedEventArgs e)
        {
            try {
                DataRowView dwr = (DataRowView)(dgUsers).SelectedItem;
                if (dwr != null)
                {
                    bool sysOculto = Convert.ToBoolean(dwr["Oculto"]);
                    MessageBoxResult result = MessageBox.Show("¿Seguro desea " + (sysOculto ? "Restaurar" : "Eliminar") + " este producto?", 
                        "A punto de " + (sysOculto ? "Restaurar" : "Eliminar") + "...", MessageBoxButton.YesNo, MessageBoxImage.Question);              
                    if (result == MessageBoxResult.Yes) {
                        string consulta = string.Format("UPDATE Productos SET Oculto={0} WHERE (Id_Pro={1})", !sysOculto, dwr["Id_Pro"]);
                        OleDbCommand comando = new OleDbCommand(consulta, AccessSystem.dbConnection);
                        comando.ExecuteNonQuery();
                   
                        MessageBox.Show("Producto " + (sysOculto ? "Restaurado" : "Eliminado") +" Correctamente.");
                        Update();
                    }
                }
                else
                {
                    MessageBox.Show("No ha seleccionado ningún producto para eliminar.", "Selección del producto...", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            } catch (Exception ex){
                MessageBox.Show(ex.Message);
            }
        }

        private void BtnGenerar_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dwr = (DataRowView)(dgUsers).SelectedItem;
            if (dwr != null)
            {
                bool sysOculto = Convert.ToBoolean(dwr["Oculto"]);
                if (!sysOculto) {
                    MainWindow main = Helpers.ReturnWindowOpen<MainWindow>() as MainWindow;
                    main.BtnVentas_Click(sender, e);

                    Ventas ventas = Helpers.ReturnWindowOpen<Ventas>() as Ventas;

                    if (ventas != null)
                    {
                        ventas.id = Convert.ToInt32(dwr["Id_Pro"]);
                        ventas.code = dwr["Code"].ToString();
                        ventas.descrip = dwr["Descrip"].ToString();
                        ventas.price = Convert.ToDecimal(dwr["Price"]);
                        ventas.maxStock = Convert.ToInt32(dwr["Stock"]);
                        ventas.BtnShow_Click(sender, e);                        
                    }
                }
                else
                {
                    MessageBox.Show("No se puede seleccionar un producto eliminado para Generar Venta.", "Selección del producto...", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("No ha seleccionado ningún producto para Generar Venta.", "Selección del producto...", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
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

        public void SaveCategory()
        {
            if (cbCat.SelectedIndex == -1)
            {
                if (cbCat.Text.Length > 0)
                {
                    MessageBoxResult result = MessageBox.Show("Se ha detectado que ha escrito una nueva categoria.\n¿Desea guardar esta nueva categoria \"" +
                         cbCat.Text + "\" ?", "Categoria No existente", MessageBoxButton.YesNo, MessageBoxImage.Question);
                    if (result == MessageBoxResult.Yes)
                    {
                        try
                        {
                            string command = string.Format("INSERT INTO Categorias (Nombre, Descrip) VALUES ('{0}', 'Categoria agregada el {1}')",
                                cbCat.Text, DateTime.Now.ToShortDateString());

                            OleDbCommand comando = new OleDbCommand(command, AccessSystem.dbConnection);
                            comando.ExecuteNonQuery();

                            MessageBox.Show("Registro Guardado Correctamente.");
                            Update(true);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message);
                        }
                    }
                    else
                    {
                        if (cbCat.Items.Count > 0) { cbCat.SelectedIndex = cbCat.Items.Count - 1; }
                    }
                }
                else
                {
                    MessageBox.Show("Por favor cree o seleccione una categoria.", "Este campo no puede estar vacio..!", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    if (cbCat.Items.Count > 0) { cbCat.SelectedIndex = cbCat.Items.Count - 1; }
                }
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
    }
}
