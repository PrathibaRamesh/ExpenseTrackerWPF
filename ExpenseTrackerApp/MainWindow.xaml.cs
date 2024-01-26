using ExpenseTrackerApp;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using System.Linq;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Globalization;

namespace YourExpenseTrackerNamespace
{
    public partial class MainWindow : Window
    {
        private ExpenseRepository _expenseRepository;

        // Collection to store expenses for binding to DataGrid
        private ObservableCollection<Expense> expensesCollection;

        // Class-level field to store expenses
        private List<Expense> expenses;

        // Add these properties for chart data
        public SeriesCollection ExpenseSeries { get; set; }
        public ChartValues<ObservableValue> IncomeValues { get; set; }
        public ChartValues<ObservableValue> ExpenseValues { get; set; }
        public ChartValues<ObservableValue> RemainingValues { get; set; }

        // Define categories for each type
        private Dictionary<string, List<string>> categoryOptions = new Dictionary<string, List<string>>
        {
            { "Expense", new List<string> { "", "Food", "Utilities", "Transportation", "Others" } },
            { "Income", new List<string> { "", "Salary", "Bonus", "Gift", "Others" } }
        };

        public MainWindow()
        {
            InitializeComponent();
            _expenseRepository = new ExpenseRepository();

            // Initialize chart data
            ExpenseSeries = new SeriesCollection();
            IncomeValues = new ChartValues<ObservableValue>();
            ExpenseValues = new ChartValues<ObservableValue>();
            RemainingValues = new ChartValues<ObservableValue>();

            // Set chart data context
            DataContext = this;

            // Set up the initial categories
            //SetCategoryOptions("Expense");

            // Wire up the event handler
            TypeComboBox.SelectionChanged += TypeComboBox_SelectionChanged;

            // Load months into MonthComboBox
            LoadMonths();

            // Load years into YearComboBox
            LoadYears();

            // Initialize the expenses collection
            expensesCollection = new ObservableCollection<Expense>();

            // Set the DataGrid's item source to the expenses collection
            ExpensesDataGrid.ItemsSource = expensesCollection;

            // Load expenses from MongoDB and populate the DataGrid
            LoadExpenses();
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

        private void LoadExpenses()
        {
            try
            {
                // Retrieve filter criteria from UI
                string selectedType = (TypeComboBox1.SelectedItem as ComboBoxItem)?.Content?.ToString();
                string selectedCategory = CategoryComboBox1.SelectedItem?.ToString();
                DateTime? selectedDate = DatePicker1.SelectedDate;

                string selectedMonth = MonthComboBox.SelectedItem?.ToString();
                string selectedYear = YearComboBox.SelectedItem?.ToString();

                // Build filter definition based on selected criteria
                var filterBuilder = Builders<Expense>.Filter;
                var filter = filterBuilder.Empty;

                if (!string.IsNullOrEmpty(selectedType))
                {
                    filter &= filterBuilder.Eq("Type", selectedType);
                }

                if (!string.IsNullOrEmpty(selectedCategory))
                {
                    filter &= filterBuilder.Eq("Category", selectedCategory);
                }

                if (selectedDate.HasValue)
                {
                    filter &= filterBuilder.Gte("Date", selectedDate.Value.Date) &
                              filterBuilder.Lt("Date", selectedDate.Value.Date.AddDays(1));
                }

                // Retrieve expenses from MongoDB based on filter
                expenses = _expenseRepository.GetExpenses(filter);

                // Clear existing data in the collection
                expensesCollection.Clear();

                // Add the retrieved expenses to the collection
                foreach (var expense in expenses)
                {
                    expensesCollection.Add(expense);
                }

                // Update chart data
                //UpdateChartData(expenses);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Check if the selected tab is the "Monthly Report" tab
            if (myTabControl.SelectedItem == tabItem3)
            {
                // Call your charts method here
                UpdateChartData();
            }
        }

        private void SetCategoryOptions(string selectedType)
        {
            if (categoryOptions.ContainsKey(selectedType))
            {
                // Clear existing items and add new categories
                CategoryComboBox.Items.Clear();
                foreach (var category in categoryOptions[selectedType])
                {
                    CategoryComboBox.Items.Add(category);
                }
                CategoryComboBox.SelectedIndex = 0; // Optionally select the first item
            }
        }

        private void TypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox selectedComboBox = sender as ComboBox;
            // Handle the selection change event of the TypeComboBox
            if (selectedComboBox != null)
            {
                string selectedType;

                // Determine the selected ComboBox and retrieve its selected type
                if (selectedComboBox == TypeComboBox)
                {
                    selectedType = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                }
                else if (selectedComboBox == TypeComboBox1)
                {
                    selectedType = (TypeComboBox1.SelectedItem as ComboBoxItem)?.Content?.ToString();
                }
                else
                {
                    // Handle unexpected ComboBox
                    return;
                }

                if (!string.IsNullOrEmpty(selectedType) && categoryOptions.ContainsKey(selectedType))
                {
                    ComboBox relatedCategoryComboBox;

                    // Determine the related Category ComboBox
                    if (selectedComboBox == TypeComboBox)
                    {
                        relatedCategoryComboBox = CategoryComboBox;
                    }
                    else if (selectedComboBox == TypeComboBox1)
                    {
                        relatedCategoryComboBox = CategoryComboBox1;
                    }
                    else
                    {
                        // Handle unexpected ComboBox
                        return;
                    }

                    // Check if the related Category ComboBox is not null
                    if (relatedCategoryComboBox != null)
                    {
                        // Check if Items is not null before clearing
                        if (relatedCategoryComboBox.Items != null)
                        {
                            // Clear existing items and add new categories
                            relatedCategoryComboBox.Items.Clear();
                            foreach (var category in categoryOptions[selectedType])
                            {
                                relatedCategoryComboBox.Items.Add(category);
                            }
                            relatedCategoryComboBox.SelectedIndex = 0; // Optionally select the first item
                        }
                    }
                }
                LoadExpenses();
            }
        }

        private void DatePicker1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadExpenses();
        }

        private void CategoryComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadExpenses();
        }

        private void AddExpenseButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(DescriptionTextBox.Text) ||
                    !decimal.TryParse(AmountTextBox.Text.Replace("$", ""), out decimal amount) ||
                    DatePicker.SelectedDate == null)
                {
                    MessageBox.Show("Invalid input. Please enter valid values.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                string selectedType = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();
                string selectedCategory = CategoryComboBox.SelectedItem?.ToString();
                DateTime selectedDate = DatePicker.SelectedDate ?? DateTime.Now;

                Expense expense = new Expense
                {
                    Description = DescriptionTextBox.Text,
                    Amount = amount,
                    Type = selectedType,
                    Category = selectedCategory,
                    Date = selectedDate
                    // Add other properties as needed
                };

                _expenseRepository.AddExpense(expense);

                // Clear all inputs after adding to the database
                DescriptionTextBox.Text = string.Empty;
                AmountTextBox.Text = "$";
                TypeComboBox.SelectedIndex = 0; // Assuming the default index represents "Expense"
                CategoryComboBox.Items.Clear();
                DatePicker.SelectedDate = DateTime.Now;

                MessageBox.Show("Expense added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearFiltersButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear filter options and reload expenses
            TypeComboBox1.SelectedIndex = 0; // Set to default value or empty
            DatePicker1.SelectedDate = null;
            CategoryComboBox1.SelectedIndex = -1; // Clear selection

            LoadExpenses();
        }
    }
}
