using InventarioWPF.PageCS;
using System;
using System.Data;
using System.Data.OleDb;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace InventarioWPF.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Cargos.xaml
    /// </summary>

    public partial class Cargos : Window
    {
        string dep = "Ninguno";
        OleDbDataAdapter dbDataAdapter;
        DataSet dataSet = new DataSet();
        int cellsforpages = Properties.Settings.Default.CellsForPages;
        int total, indice, idEdit = 0;

        public Cargos(string sDep)
        {
            InitializeComponent();
            dep = sDep;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void BtnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            indice = 0;
            dataSet.Tables["Cargos"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            indice -= cellsforpages;
            if (indice < 0)
                indice = 0;

            dataSet.Tables["Cargos"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            indice += cellsforpages;

            if (indice > total - 1)
                indice -= cellsforpages;

            dataSet.Tables["Cargos"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnLast_Click(object sender, RoutedEventArgs e)
        {
            if (total > 1)
            {
                indice = total - cellsforpages;
                indice = indice < 0 ? 0 : indice;

                dataSet.Tables["Cargos"].Clear();
                dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
            }
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            indice = 0;

            string salida_datos = "";

            if (txtSearch.Text.Length > 0)
            {
                salida_datos = "(Nombre LIKE '%" + txtSearch.Text + "%')";
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
            if (txtNom.Text.Length == 0)
            {
                MessageBox.Show("Esta obligado a llenar el campo Nombre.");
                return;
            }

            if (!cbDep.IsChecked.Value && !cbEmp.IsChecked.Value && !cbInv.IsChecked.Value &&
                !cbPro.IsChecked.Value && !cbReg.IsChecked.Value && !cbVen.IsChecked.Value)
            {
                MessageBox.Show("Esta obligado a llenar minimo un Departamento autorizado.");
                return;
            }

            try
            {
                string consulta = "";

                if (idEdit == 0)
                {
                    consulta = string.Format("INSERT INTO Departamento (Nombre, Inventario, Proveedores, Ventas, Empleados, Departamentos, RegSesiones) VALUES ('{0}', {1}, {2}, {3}, {4}, {5}, {6})",
                        txtNom.Text, cbInv.IsChecked, cbPro.IsChecked, cbVen.IsChecked, cbEmp.IsChecked, cbDep.IsChecked, cbReg.IsChecked);
                }
                else
                {
                    consulta = string.Format("UPDATE Departamento SET Nombre='{0}', Inventario={1}, Proveedores={2}, Ventas={3}, Empleados={4}, Departamentos={5}, RegSesiones={6} WHERE Id_Dep={7}",
                        txtNom.Text, cbInv.IsChecked, cbPro.IsChecked, cbVen.IsChecked, cbEmp.IsChecked, cbDep.IsChecked, cbReg.IsChecked, idEdit);                  
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
                    MessageBox.Show("Nombre duplicado, por favor ingrese otro nombre.");
                    txtNom.SelectAll();
                    txtNom.Focus();
                }
                else
                    MessageBox.Show(ex.Message);
            }
        }

        private void LimpiarPantalla()
        {
            idEdit = 0;         
            txtNom.Text = "";
            cbDep.IsChecked = false;
            cbEmp.IsChecked = false;
            cbInv.IsChecked = false;
            cbPro.IsChecked = false;
            cbReg.IsChecked = false;
            cbVen.IsChecked = false;
        }

        private void Update(string wheres = "")
        {
            try
            {
                bussy.IsBusy = true;

                cellsforpages = Properties.Settings.Default.CellsForPages;

                string nConsulta = Properties.Settings.Default.ConsultaDep;

                if (wheres.Length > 0)
                   nConsulta += " WHERE " + wheres;

                nConsulta += " ORDER BY Nombre ASC";

                dataSet.Tables.Clear();
                dbDataAdapter = new OleDbDataAdapter(nConsulta, AccessSystem.dbConnection);
                dataSet.Tables.Add("Cargos");

                //Cargar todos los datos 
                dbDataAdapter.Fill(dataSet.Tables["Cargos"]);
                total = dataSet.Tables["Cargos"].Rows.Count;
                dataSet.Clear();

                if (total == 0)
                    txtNumbers.Text = "No se encontraron cargos registradas.";
                else if (total == 1)
                    txtNumbers.Text = "Se ha encontrado un cargo registrado.";
                else
                    txtNumbers.Text = "Se ha encontraron " + total + " cargos registradas.";

                //Realizamos la primera paginación.
                dbDataAdapter.Fill(dataSet, indice, cellsforpages, "Cargos");
                dgEmp.ItemsSource = dataSet.Tables["Cargos"].DefaultView;

                bussy.IsBusy = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                txtSearch.Text = "";
            }
        }

        private void BtnEdit_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dwr = (DataRowView)(dgEmp).SelectedItem;
            if (dwr != null)
            {
                LimpiarPantalla();
                idEdit = Convert.ToInt32(dwr["Id_Dep"]);
                txtNom.Text = dwr["Nombre"].ToString();
                cbDep.IsChecked = Convert.ToBoolean(dwr["Departamentos"]);
                cbEmp.IsChecked = Convert.ToBoolean(dwr["Empleados"]);
                cbInv.IsChecked = Convert.ToBoolean(dwr["Ventas"]);
                cbPro.IsChecked = Convert.ToBoolean(dwr["Proveedores"]);
                cbReg.IsChecked = Convert.ToBoolean(dwr["RegSesiones"]);
                cbVen.IsChecked = Convert.ToBoolean(dwr["Ventas"]);

                Show_OR_HideBackground(0, 0.8);
                ShowStoryBoard.Begin();
            }
            else
            {
                MessageBox.Show("No ha seleccionado ningún empleado para editar.", "Selección del empleado...", MessageBoxButton.OK, MessageBoxImage.Warning);
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
    }
}
