using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

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
            LoadExpensesByCategory(defaultMonth, defaultYear);

        }

        // Method: GoButton_Click
        // Purpose: Method to handle button click that will get the filter options and load grid accordingly from DB.
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
                LoadExpensesByCategory(month, year);
            }
        }

        // Method: monthComboBox_SelectionChanged
        // Purpose: Method to handle month combo box.
        private void monthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        // Method: yearComboBox_SelectionChanged
        // Purpose: Method to handle year combo box.
        private void yearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        // Method: LoadReportData
        // Purpose: Method to load the pie chart with total income, expense, remaining amount from DB based on month and year filter.
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


            // For Data table Category Vs Amount Spent


        }

        // Method: LoadExpensesByCategory
        // Purpose: Method to load the Data Table with expenses total grouped by category from DB based on month and year filter.
        public void LoadExpensesByCategory(int month, int year)
        {
            // Set the ItemsSource to null
            ReportDataTable.ItemsSource = null;

            // Clear the Items collection
            ReportDataTable.Items.Clear();

            var mainWindow = Application.Current.MainWindow as MainWindow;
            var expensesByCategory = mainWindow?.db.GetExpensesGroupedByCategory(month, year, "Expenses");

            // Update the ItemsSource with the new data
            ReportDataTable.ItemsSource = expensesByCategory;
        }
    }
}