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

namespace ExpenseTrackerApp
{
    /// <summary>
    /// Interaction logic for MonthlyReportTab.xaml
    /// </summary>
    public partial class MonthlyReportTab : UserControl
    {
        private ExpenseRepository _expenseRepository;

        // Collection to store expenses for binding to DataGrid
        private ObservableCollection<Expense> expensesCollection;

        // Add these properties for chart data
        public SeriesCollection ExpenseSeries { get; set; }
        public ChartValues<ObservableValue> IncomeValues { get; set; }
        public ChartValues<ObservableValue> ExpenseValues { get; set; }
        public ChartValues<ObservableValue> RemainingValues { get; set; }

        public MonthlyReportTab()
        {
            InitializeComponent();

            // Load months into MonthComboBox
            LoadMonths();

            // Load years into YearComboBox
            LoadYears();

            _expenseRepository = new ExpenseRepository();

            // Set chart data context
            DataContext = this;

            // Initialize the expenses collection
            expensesCollection = new ObservableCollection<Expense>();

            // Initialize chart data
            ExpenseSeries = new SeriesCollection();
            IncomeValues = new ChartValues<ObservableValue>();
            ExpenseValues = new ChartValues<ObservableValue>();
            RemainingValues = new ChartValues<ObservableValue>();

            UpdateChartData();
        }

        private void LoadMonths()
        {
            // Get the names of the months using CultureInfo
            string[] monthNames = DateTimeFormatInfo.CurrentInfo.MonthNames;

            // Add months to the MonthComboBox
            foreach (string month in monthNames)
            {
                if (!string.IsNullOrEmpty(month))
                {
                    MonthComboBox.Items.Add(month);
                }
            }
        }

        private void LoadYears()
        {
            // Define a range of years (adjust as needed)
            int startYear = 2020;
            int endYear = DateTime.Now.Year;

            // Add years to the YearComboBox
            for (int year = startYear; year <= endYear; year++)
            {
                YearComboBox.Items.Add(year.ToString());
            }
        }

        private void MonthComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle month selection change if needed
        }

        private void YearComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Handle year selection change if needed
        }

        private void ClearFiltersButton1_Click(object sender, RoutedEventArgs e)
        {
            // Handle clear filters button click if needed
        }

        private void UpdateChartData()
        {
            // Clear existing chart data
            IncomeValues.Clear();
            ExpenseValues.Clear();
            RemainingValues.Clear();
            if (ExpenseSeries != null)
            {
                ExpenseSeries = new SeriesCollection();
                // Clear existing chart data
                ExpenseSeries.Clear();
            }
            else
            {
                // Initialize if not already initialized
                ExpenseSeries = new SeriesCollection();
            }

            // Retrieve expenses from MongoDB (replace this with your MongoDB code)
            List<Expense> expensesChart = _expenseRepository.GetExpensesForChart();

            // Clear existing data in the collection
            expensesCollection.Clear();

            // Add the retrieved expenses to the collection
            foreach (var expense in expensesChart)
            {
                expensesCollection.Add(expense);
            }

            // Group expenses by type
            var groupedExpenses = expensesChart.GroupBy(e => e.Type);

            foreach (var group in groupedExpenses)
            {
                string type = group.Key;
                decimal totalAmount = group.Sum(e => e.Amount);

                if (type == "Income")
                {
                    IncomeValues.Add(new ObservableValue((double)totalAmount));
                }
                else if (type == "Expense")
                {
                    ExpenseValues.Add(new ObservableValue((double)totalAmount));
                }
            }

            // Calculate Remaining
            double remainingAmount = IncomeValues.Sum(iv => iv.Value) - ExpenseValues.Sum(ev => ev.Value);
            SolidColorBrush remainingColor = remainingAmount >= 0 ? Brushes.Green : Brushes.Red;

            RemainingValues.Add(new ObservableValueWithColor(remainingAmount, remainingColor));

            // Add data to series
            ExpenseSeries.Add(new PieSeries
            {
                Values = IncomeValues,
                Title = "Total Income",
                DataLabels = true,
                Fill = Brushes.LimeGreen
            });

            ExpenseSeries.Add(new PieSeries
            {
                Values = ExpenseValues,
                Title = "Total Expense",
                DataLabels = true,
                Fill = Brushes.OrangeRed
            });

            ExpenseSeries.Add(new PieSeries
            {
                Values = RemainingValues,
                Title = "Remaining",
                DataLabels = true,
                Fill = remainingColor
            });
        }
    }
}
