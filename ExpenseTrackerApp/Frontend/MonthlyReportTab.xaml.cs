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
            //LoadReportData();

            // Initialize the ComboBoxes with default values if needed
            InitializeComboBoxes();
        }

        private void InitializeComboBoxes()
        {
            // Populate the yearComboBox with a range of years including the current year
            yearComboBox.Items.Clear();
            int currentYear = DateTime.Now.Year;
            for (int year = currentYear - 10; year <= currentYear; year++)
            {
                yearComboBox.Items.Add(year.ToString());
            }

            // Set the current month and year as default
            monthComboBox.SelectedIndex = DateTime.Now.Month - 1; // Months are zero-indexed
            yearComboBox.SelectedItem = currentYear.ToString();

            // Get the default month and year
            int defaultMonth = monthComboBox.SelectedIndex + 1; // Adding 1 because months are zero-indexed in the ComboBox
            int defaultYear = int.Parse(yearComboBox.SelectedItem.ToString());

            LoadReportData(defaultMonth, defaultYear);

        }

        private void GoButton_Click(object sender, RoutedEventArgs e)
        {
            if (monthComboBox.SelectedItem != null && yearComboBox.SelectedItem != null)
            {
                string monthName = monthComboBox.SelectedItem.ToString();
                int year = int.Parse(yearComboBox.SelectedItem.ToString());

                // Extract only the month name part if the string contains additional text
                monthName = monthName.Replace("System.Windows.Controls.ComboBoxItem: ", "").Trim();

                int month = DateTime.ParseExact(monthName, "MMMM", CultureInfo.InvariantCulture).Month;

                LoadReportData(month, year);
            }
        }

        private void monthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optionally, you can call LoadReportData here as well if you want the chart to update 
            // automatically when the month or year selection changes.
        }

        private void yearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optionally, you can call LoadReportData here as well if you want the chart to update 
            // automatically when the month or year selection changes.
        }

        public void LoadReportData(int month, int year)
        {
            //var mainWindow = Application.Current.MainWindow as MainWindow;
            //var expensesReport = mainWindow?.db.GetExpensesForChart("Expenses");

            var mainWindow = Application.Current.MainWindow as MainWindow;
            var expensesReport = mainWindow?.db.GetExpensesByMonthYear("Expenses", month, year);

            var groupedExpenses = expensesReport
        .GroupBy(e => e.Type)
        .Select(group => new
        {
            Type = group.Key,
            TotalAmount = group.Sum(e => e.Amount)
        });

            SeriesCollection pieChartSeries = new SeriesCollection();

            // Clear existing series and its palette
            ReportPieChart.Series.Clear();

            var blueBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1e90ff"));
            var redBrush = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#cd5c5c"));

            foreach (var expense in groupedExpenses.OrderByDescending(e => e.Type))
            {
                pieChartSeries.Add(new PieSeries
                {
                    Title = expense.Type,
                    Values = new ChartValues<decimal> { expense.TotalAmount },
                    DataLabels = true,
                    Fill = expense.Type == "Income" ? blueBrush : redBrush
                });
            }

            ReportPieChart.Series = pieChartSeries;

            decimal totalIncome = expensesReport.Where(e => e.Type == "Income").Sum(e => e.Amount);
            decimal totalExpense = expensesReport.Where(e => e.Type == "Expense").Sum(e => e.Amount);
            decimal balance = totalIncome - totalExpense;

            BalanceTextBlock.Text = (balance >= 0 ? "+" : "-") + balance.ToString("C"); // Format as currency
            BalanceTextBlock.Foreground = balance < 0 ? Brushes.Red : Brushes.Green;

        }

    }
}