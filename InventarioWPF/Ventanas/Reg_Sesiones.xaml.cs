using InventarioWPF.PageCS;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
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

namespace InventarioWPF.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Reg_Sesiones.xaml
    /// </summary>
    public partial class Reg_Sesiones : Window
    {
        OleDbDataAdapter dbDataAdapter;
        DataSet dataSet = new DataSet();
        int cellsforpages = Properties.Settings.Default.CellsForPages;
        int total, indice;

        public Reg_Sesiones()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }

        private void TxtSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            indice = 0;

            if (txtSearch.Text.Length > 0)
                Update("(Empleados.Nombre LIKE '%" + txtSearch.Text + "%')");
            else
                Update();
        }

        private void BtnFirstPage_Click(object sender, RoutedEventArgs e)
        {
            indice = 0;
            dataSet.Tables["Sesiones"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnPrevious_Click(object sender, RoutedEventArgs e)
        {
            indice -= cellsforpages;
            if (indice < 0)
                indice = 0;

            dataSet.Tables["Sesiones"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnNext_Click(object sender, RoutedEventArgs e)
        {
            indice += cellsforpages;

            if (indice > total - 1)
                indice -= cellsforpages;

            dataSet.Tables["Sesiones"].Clear();
            dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
        }

        private void BtnLast_Click(object sender, RoutedEventArgs e)
        {
            if (total > 1)
            {
                indice = total - cellsforpages;
                indice = indice < 0 ? 0 : indice;

                dataSet.Tables["Sesiones"].Clear();
                dbDataAdapter.Fill(dataSet, indice, cellsforpages, dataSet.Tables[0].TableName);
            }
        }

        private void Update(string wheres = "")
        {
            try
            {
                bussy.IsBusy = true;

                cellsforpages = Properties.Settings.Default.CellsForPages;

                string nConsulta = Properties.Settings.Default.ConsultaReg;

                if (wheres.Length > 0)
                    nConsulta += " WHERE " + wheres;

                nConsulta += " ORDER BY Exit DESC";

                dataSet.Tables.Clear();
                dbDataAdapter = new OleDbDataAdapter(nConsulta, AccessSystem.dbConnection);
                dataSet.Tables.Add("Sesiones");

                //Cargar todos los datos 
                dbDataAdapter.Fill(dataSet.Tables["Sesiones"]);
                total = dataSet.Tables["Sesiones"].Rows.Count;
                dataSet.Clear();

                if (total == 0)
                    txtNumbers.Text = "No se encontraron Sesiones registradas.";
                else if (total == 1)
                    txtNumbers.Text = "Se ha encontrado una venta registrada.";
                else
                    txtNumbers.Text = "Se ha encontraron " + total + " Sesiones registradas.";

                //Realizamos la primera paginación.
                dbDataAdapter.Fill(dataSet, indice, cellsforpages, "Sesiones");
                dgReg.ItemsSource = dataSet.Tables["Sesiones"].DefaultView;

                bussy.IsBusy = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                txtSearch.Text = "";
            }
        }
    }
}
