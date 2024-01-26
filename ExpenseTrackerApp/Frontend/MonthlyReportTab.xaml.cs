using LiveCharts.Defaults;
using LiveCharts.Wpf;
using LiveCharts;
using System;
using System.Collections.Generic;
using System.Globalization;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;

namespace ExpenseTrackerApp.Frontend
{
    /// <summary>
    /// Interaction logic for MonthlyReportTab.xaml
    /// </summary>
    public partial class MonthlyReportTab : UserControl
    {

        public MonthlyReportTab()
        {
            InitializeComponent();
            LoadReportData();
        }

        public void LoadReportData()
        {
            var mainWindow = Application.Current.MainWindow as MainWindow;
            var expensesReport = mainWindow?.db.GetExpensesForChart("Expenses");

            var groupedExpenses = expensesReport
        .GroupBy(e => e.Type)
        .Select(group => new
        {
            Type = group.Key,
            TotalAmount = group.Sum(e => e.Amount)
        });

            SeriesCollection pieChartSeries = new SeriesCollection();

            foreach (var expense in groupedExpenses)
            {
                pieChartSeries.Add(new PieSeries
                {
                    Title = expense.Type,
                    Values = new ChartValues<decimal> { expense.TotalAmount },
                    DataLabels = true
                });
            }

            ReportPieChart.Series = pieChartSeries;
        }
    }
}