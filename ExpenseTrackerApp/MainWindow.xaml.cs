using ExpenseTrackerApp;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace YourExpenseTrackerNamespace
{
    public partial class MainWindow : Window
    {
        private ExpenseRepository _expenseRepository;

        // Collection to store expenses for binding to DataGrid
        private ObservableCollection<Expense> expensesCollection;

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

            // Set up the initial categories
            //SetCategoryOptions("Expense");

            // Wire up the event handler
            TypeComboBox.SelectionChanged += TypeComboBox_SelectionChanged;

            // Initialize the expenses collection
            expensesCollection = new ObservableCollection<Expense>();

            // Set the DataGrid's item source to the expenses collection
            ExpensesDataGrid.ItemsSource = expensesCollection;

            // Load expenses from MongoDB and populate the DataGrid
            LoadExpenses();
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
                    filter &= filterBuilder.Gte("Date", selectedDate.Value.Date) &
                              filterBuilder.Lt("Date", selectedDate.Value.Date.AddDays(1));
                }

                // Retrieve expenses from MongoDB based on filter
                List<Expense> expenses = _expenseRepository.GetExpenses(filter);

                // Clear existing data in the collection
                expensesCollection.Clear();

                // Add the retrieved expenses to the collection
                foreach (var expense in expenses)
                {
                    expensesCollection.Add(expense);
                }
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
