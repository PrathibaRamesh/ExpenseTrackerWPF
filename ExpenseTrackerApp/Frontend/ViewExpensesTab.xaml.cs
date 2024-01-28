using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ExpenseTrackerApp.Frontend
{
    /// <summary>
    /// Interaction logic for ViewExpensesTab.xaml
    /// </summary>
    public partial class ViewExpensesTab : UserControl
    {
        // Define categories for each type for dynamic load
        private Dictionary<string, List<string>> categoryOptions = new Dictionary<string, List<string>>
        {
            { "Expense", new List<string> { "", "Food", "Utilities", "Transportation", "Others" } },
            { "Income", new List<string> { "", "Salary", "Bonus", "Gift", "Others" } }
        };

        public ViewExpensesTab()
        {
            InitializeComponent();

            // Wire up the event handler
            TypeComboBox1.SelectionChanged += TypeComboBox_SelectionChanged;

            // Load expenses from MongoDB and populate the DataGrid
            LoadExpenses();
        }

        private long ConvertToMongoDBTicks(DateTime dateTime)
        {
            // MongoDB tick is 100-nanosecond intervals since 1/1/0001, which is the same as .NET DateTime ticks
            return dateTime.Ticks;
        }

        private int GetTimezoneOffset(DateTime dateTime)
        {
            // Get timezone offset in minutes
            return (int)TimeZoneInfo.Local.GetUtcOffset(dateTime).TotalMinutes;
        }

        // Method: LoadExpenses
        // Purpose: Method to Load all expenses and income in grid from MongoDB based on filter criteria.
        private void LoadExpenses()
        {
            try
            {
                // Retrieve filter criteria from UI
                string selectedType = (TypeComboBox1.SelectedItem as ComboBoxItem)?.Content?.ToString();
                string selectedCategory = CategoryComboBox1.SelectedItem?.ToString();
                DateTime? selectedDate = DatePicker1.SelectedDate;

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
                    // Adjust filter to match the MongoDB structure to match Date time array.
                    long selectedDateTicks = ConvertToMongoDBTicks(selectedDate.Value.Date);
                    int timezoneOffset = GetTimezoneOffset(selectedDate.Value.Date);

                    filter &= filterBuilder.Gte("Date.0", selectedDateTicks) &
                              filterBuilder.Lt("Date.0", ConvertToMongoDBTicks(selectedDate.Value.Date.AddDays(1)));
                }

                var mainWindow = Application.Current.MainWindow as MainWindow;
                ExpensesDataGrid.ItemsSource = mainWindow?.db.GetExpenses(filter, "Expenses");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method: TypeComboBox_SelectionChanged
        // Purpose: Method to handle Type combo box to load the data dynamically based on Category.
        private void TypeComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            ComboBox selectedComboBox = sender as ComboBox;
            // Handle the selection change event of the TypeComboBox
            if (selectedComboBox != null)
            {
                string selectedType;

                if (selectedComboBox == TypeComboBox1)
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

                    if (selectedComboBox == TypeComboBox1)
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

        // Method: DatePicker1_SelectionChanged
        // Purpose: Method to handle Date picker calendar.
        private void DatePicker1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadExpenses();
        }

        // Method: CategoryComboBox1_SelectionChanged
        // Purpose: Method to handle Category combo box.
        private void CategoryComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadExpenses();
        }

        // Method: ClearFiltersButton_Click
        // Purpose: Method to clear the filter data id needed.
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
