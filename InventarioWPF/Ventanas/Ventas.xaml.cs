using System;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using InventarioWPF.PageCS;

namespace InventarioWPF.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Venta.xaml
    /// </summary>
    public partial class Ventas : Window
    {
        private int idEmp;
        public int id, maxStock;
        public string code, descrip;
        public decimal price, tot;

        OleDbDataAdapter dbDataAdapter;
        DataSet dataSet = new DataSet();
        int cellsforpages = Properties.Settings.Default.CellsForPages;
        int total, indice = 0;

        public Ventas(int id, string name)
        {
            InitializeComponent();
            idEmp = id;
            txtName.Text = name;
            txtDate.Text = DateTime.Now.ToLongDateString();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void BtnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            indice = 0;
            dataSet.Tables["Ventas"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            indice -= cellsforpages;
            if (indice < 0)
                indice = 0;

            dataSet.Tables["Ventas"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            indice += cellsforpages;

            if (indice > total - 1)
                indice -= cellsforpages;

            dataSet.Tables["Ventas"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnLast_Click(object sender, RoutedEventArgs e)
        {
            if (total > 1)
            {
                indice = total - cellsforpages;
                indice = indice < 0 ? 0 : indice;

                dataSet.Tables[0].Clear();
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
                            salida_datos = "(Productos.Descrip LIKE '%" + palabra + "%' OR Empleados.Nombre LIKE '%" + palabra + "%')";
                        else
                            salida_datos += " AND (Productos.Descrip LIKE '%" + palabra + "%' OR Empleados.Nombre LIKE '%" + palabra + "%')";
                    }
                    break;
                case 1:
                    salida_datos = "(Productos.Descrip LIKE '%" + txtSearch.Text + "%')";
                    break;
                case 2:
                    salida_datos = "(Empleados.Nombre LIKE '%" + txtSearch.Text + "%')";
                    break;
            }

            if (salida_datos.Length > 0)
                Update(salida_datos);
            else
                Update();
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

            txtCode.Text = code;
            txtDes.Text = descrip;
            txtSal.Text = string.Format("{0:C}", price);
            numCant.Maximum = maxStock;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            try {
                string consulta = "";

                consulta = string.Format("INSERT INTO Ventas (Id_Pro, Total, Stock, Id_Emp, Fecha) VALUES ('{0}', '{1}', '{2}', '{3}', '{4}')",
                                        id, txtTotal.Text, numCant.Value, idEmp, DateTime.Now);
                OleDbCommand comando = new OleDbCommand(consulta, AccessSystem.dbConnection);
                comando.ExecuteNonQuery();

                string cUpdate = string.Format("Update Productos SET Stock={0} WHERE Id_Pro = {1}", (numCant.Maximum - numCant.Value), id);
                OleDbCommand cdUpdate = new OleDbCommand(cUpdate, AccessSystem.dbConnection);
                cdUpdate.ExecuteNonQuery();

                Update();

                Inventario inventario = Helpers.ReturnWindowOpen<Inventario>() as Inventario;
                if (inventario != null) { inventario.BtnUpdate_Click(sender, e); }

                MainWindow main = Helpers.ReturnWindowOpen<MainWindow>() as MainWindow;
                if (main != null) { main.Update(); }

                Show_OR_HideBackground(0.8, 0);
                CloseStoryBoard.Begin();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void NumCant_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            try
            {
                tot = price * Convert.ToInt32(numCant.Value);
                txtTotal.Text = string.Format("{0:C}", tot);
            }
            catch { }
        }

        private void LimpiarPantalla()
        {
            txtCode.Text = "";
            txtDes.Text = "";            
            numCant.Value = numCant.Minimum;
            txtTotal.Text = txtSal.Text = string.Format("{0:C}", (decimal)0.00);
        }

        private void Update (string wheres = "")
        {
            try {
                bussy.IsBusy = true;

                cellsforpages = Properties.Settings.Default.CellsForPages;

                string nConsulta = Properties.Settings.Default.ConsultaVen;

                if (wheres.Length > 0)
                    nConsulta += " WHERE " + wheres;

                nConsulta += " ORDER BY Ventas.Id_Ven DESC";

                dataSet.Tables.Clear();
                dbDataAdapter = new OleDbDataAdapter(nConsulta, AccessSystem.dbConnection);
                dataSet.Tables.Add("Ventas");

                //Cargar todos los datos 
                dbDataAdapter.Fill(dataSet.Tables["Ventas"]);
                total = dataSet.Tables["Ventas"].Rows.Count;
                dataSet.Clear();

                if (total == 0)
                    txtNumbers.Text = "No se encontraron ventas registradas.";
                else if (total == 1)
                    txtNumbers.Text = "Se ha encontrado una venta registrada.";
                else
                    txtNumbers.Text = "Se ha encontraron " + total + " ventas registradas.";

                //Realizamos la primera paginación.
                dbDataAdapter.Fill(dataSet, indice, cellsforpages, "Ventas");
                dgStore.ItemsSource = dataSet.Tables["Ventas"].DefaultView;

                bussy.IsBusy = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                txtSearch.Text = "";
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
