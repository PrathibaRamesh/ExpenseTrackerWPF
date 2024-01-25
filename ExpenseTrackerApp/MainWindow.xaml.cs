using ExpenseTrackerApp;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace YourExpenseTrackerNamespace
{
    public partial class MainWindow : Window
    {
        private ExpenseRepository _expenseRepository;

        // Define categories for each type
        private Dictionary<string, List<string>> categoryOptions = new Dictionary<string, List<string>>
        {
            { "Expense", new List<string> { "Food", "Utilities", "Transportation", "Others" } },
            { "Income", new List<string> { "Salary", "Bonus", "Gift", "Others" } }
        };

        public MainWindow()
        {
            InitializeComponent();
            _expenseRepository = new ExpenseRepository();

            // Set up the initial categories
            SetCategoryOptions("Expense");

            // Wire up the event handler
            TypeComboBox.SelectionChanged += TypeComboBox_SelectionChanged;
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
            // Handle the selection change event of the TypeComboBox
            if (TypeComboBox.SelectedItem != null)
            {
                // Use the Content property to get the selected type as a string
                string selectedType = (TypeComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

                if (!string.IsNullOrEmpty(selectedType) && categoryOptions.ContainsKey(selectedType))
                {
                    // Check if CategoryComboBox is not null
                    if (CategoryComboBox != null)
                    {
                        // Check if Items is not null before clearing
                        if (CategoryComboBox.Items != null)
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
                }
            }
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
    }
}
