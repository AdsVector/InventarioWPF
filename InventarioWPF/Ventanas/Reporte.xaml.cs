using InventarioWPF.PageCS;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.OleDb;
using System.Drawing;
using System.Windows;

namespace InventarioWPF.Ventanas
{
    /// <summary>
    /// Lógica de interacción para Reporte.xaml
    /// </summary>
    public partial class Reporte : Window
    {
        string[] meses = new string[] { "Enero", "Febrero", "Marzo", "Abril", "Mayo", "Junio", "Julio", "Agosto", "Septiembre", "Octubre", "Noviembre", "Diciembre" };

        BackgroundWorker yearWorker = new BackgroundWorker();
        BackgroundWorker mothReceiver = new BackgroundWorker();

        public Reporte()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            cbMoths.ItemsSource = meses;
            cbMoths.SelectedIndex = DateTime.Now.Month - 1;

            int FirstYear = AccessSystem.GetFirstOrLastDate("Ventas").Year;
            int LastYear = AccessSystem.GetFirstOrLastDate("Ventas", true).Year;

            for(int i = LastYear; i < (FirstYear + 1); i++)
            { 
                cbAnio.Items.Add(i.ToString());
                cbAnioXMes.Items.Add(i.ToString());
                if (i == DateTime.Now.Year)
                {
                    cbAnio.SelectedIndex = cbAnio.Items.Count - 1;
                    cbAnioXMes.SelectedIndex = cbAnioXMes.Items.Count - 1;
                }
            }

            mothReceiver.DoWork += MothsWorker_DoWork;
            mothReceiver.RunWorkerCompleted += MothsWorker_RunWorkerCompleted;

            yearWorker.DoWork += YearWorker_DoWork;
            yearWorker.RunWorkerCompleted += YearWorker_RunWorkerCompleted;
            
            Update();
        }

        private void MothsWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            object[] valuesResult = new object[4];
            
            DateTime primerDiaMes = Convert.ToDateTime(e.Argument);
            DateTime ultimoDiaMes = new DateTime(primerDiaMes.Year, primerDiaMes.Month, primerDiaMes.Day, 23, 59, 59).AddMonths(1).AddDays(-1);

            string sConsulta = "SELECT TOP 20 Productos.Code, SUM(Ventas.Total) AS TMoney, SUM(Ventas.Stock) AS TStock FROM (Ventas INNER JOIN Productos " +
                "ON Ventas.Id_Pro=Productos.Id_Pro) WHERE Fecha BETWEEN @dtIni AND @dtFin GROUP BY Productos.Code ORDER BY SUM(Ventas.Total) DESC, SUM(Ventas.Stock)" +
                " DESC";
            OleDbCommand sumDataReader = new OleDbCommand(sConsulta, AccessSystem.dbConnection);
            sumDataReader.Parameters.AddWithValue("@dtIni", primerDiaMes);
            sumDataReader.Parameters.AddWithValue("@dtFin", ultimoDiaMes);
            OleDbDataReader dataReader = sumDataReader.ExecuteReader();

            List<string> labelsD = new List<string>();
            ChartValues<decimal> valuesD = new ChartValues<decimal>();
            ChartValues<int> valuesS = new ChartValues<int>();

            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    labelsD.Add(dataReader["Code"].ToString());
                    valuesD.Add(Convert.ToDecimal(dataReader["TMoney"]));
                    valuesS.Add(Convert.ToInt32(dataReader["TStock"]));
                }
            }

            valuesResult[0] = valuesD;
            valuesResult[1] = labelsD;
            valuesResult[2] = valuesS;
         
            e.Result = valuesResult;
        }

        private void MothsWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            object[] results = e.Result as object[];

            //***************************************************************//
            Pro.Series.Clear();
            Pro.AxisX.Clear();
            Pro.AxisY.Clear();

            Pro.Series = new SeriesCollection
            {
                new ColumnSeries {
                    Title = "Dinero", //Title of series
                    Values = results[0] as ChartValues<decimal>, //list of values (age)
                    DataLabels = true, //display bar value on top of bar
                },
                new ColumnSeries {
                    Title = "Stock", //Title of series
                    Values = results[2] as ChartValues<int>, //list of values (age)
                    DataLabels = true, //display bar value on top of bar
                }
            };
         
            Pro.AxisX.Add(new Axis
            {
                Title = "Códigos de productos",
                Labels = results[1] as List<string>,
                Unit = 1
            });

            List<Axis> axes = new List<Axis>() {
                new Axis
                {
                    Title = "Dinero",
                    LabelFormatter = value => value.ToString()
                },
                new Axis
                {
                    Title = "Stock",
                    LabelFormatter = value => value.ToString()
                } };

            Pro.AxisY.AddRange(axes);

            bussy.IsBusy = false;
        }

        private void YearWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            ChartValues<decimal> values2 = new ChartValues<decimal>();

            for (int i = 1; i < 13; i++)
            {
                int _Year = Convert.ToInt32(e.Argument);
                DateTime thisPrimerMes = new DateTime(_Year, i, 1, 0, 0, 0);
                DateTime thisUltimoMes = new DateTime(_Year, thisPrimerMes.Month, thisPrimerMes.Day, 23, 59, 59).AddMonths(1).AddDays(-1);
                decimal value = AccessSystem.SumTotalDate(thisPrimerMes, thisUltimoMes);
                values2.Add(value);
            }

            e.Result = values2;
        }

        private void YearWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ChartValues<decimal> values2 = e.Result as ChartValues<decimal>;

            Moth.Series.Clear();
            Moth.AxisX.Clear();
            Moth.AxisY.Clear();

            Moth.Series = new SeriesCollection
            {
                new LineSeries {
                    Title = "Dinero", //Title of series
                    Values = values2, //list of values (age)
                    DataLabels = true, //display bar value on top of bar
                    
                },
            };

           Moth.AxisX.Add(new Axis
            {
                Title = "Mes",
                Labels = meses,
                Unit = 1
            });

            Moth.AxisY.Add(new Axis
            {
                Title = "Dinero",
                LabelFormatter = value => value.ToString()
            });

            bussy.IsBusy = false;
        }

        private void CbMoths_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cbAnio.Text.Length > 0 && cbMoths.Text.Length > 0)
            {            
                int year;
                if (!int.TryParse(cbAnio.SelectedItem.ToString(), out year))
                    year = DateTime.Now.Year;
            
                DateTime dt = new DateTime(year, (cbMoths.SelectedIndex + 1), 1, 0, 0, 0);

                bussy.IsBusy = true;
                mothReceiver.RunWorkerAsync(dt);
            }
        }

        private void Update(bool all = false)
        {
            bussy.IsBusy = true;
            int yearM = Convert.ToInt32(cbAnio.SelectedItem.ToString());
            DateTime dt = new DateTime(yearM, (cbMoths.SelectedIndex + 1), 1, 0, 0, 0);
            mothReceiver.RunWorkerAsync(dt);

            bussy.IsBusy = true;
            int yearY = Convert.ToInt32(cbAnioXMes.Text);
            yearWorker.RunWorkerAsync(yearY);

            bussy.IsBusy = true;
            txtSB.Text = AccessSystem.CountInTable("Productos", true, "AND (Stock BETWEEN 0 AND 25)").ToString();
            txtTC.Text = AccessSystem.CountInTable("Categorias").ToString();
            txtTD.Text = AccessSystem.CountInTable("Departamento").ToString();
            txtTE.Text = AccessSystem.CountInTable("Empleados", true).ToString();
            txtTP.Text = AccessSystem.CountInTable("Productos", true).ToString();
            txtTS.Text = AccessSystem.CountInTable("Proveedores", true).ToString();
            txtTV.Text = AccessSystem.CountInTable("Ventas").ToString();
            bussy.IsBusy = false;
        }

        private void CbAnioXMes_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (cbAnioXMes.Text.Length > 0)
            {              
                int year;
                if (!int.TryParse(cbAnioXMes.SelectedItem.ToString(), out year))
                    year = DateTime.Now.Year;

                bussy.IsBusy = true;
                yearWorker.RunWorkerAsync(year);
            }
        }
    }
}
