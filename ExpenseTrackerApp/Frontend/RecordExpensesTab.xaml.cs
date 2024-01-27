using System;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ExpenseTrackerApp.Frontend
{    public partial class RecordExpensesTab : UserControl
    {
        //private ExpenseRepository _expenseRepository;

        // Define categories for each type
        private Dictionary<string, List<string>> categoryOptions = new Dictionary<string, List<string>>
        {
            { "Expense", new List<string> { "", "Food", "Utilities", "Transportation", "Others" } },
            { "Income", new List<string> { "", "Salary", "Bonus", "Gift", "Others" } }
        };

        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                //this.DragEnter();
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

                //_expenseRepository.AddExpense(expense);

                var mainWindow = Application.Current.MainWindow as MainWindow;
                mainWindow?.db.AddExpense("Expenses", expense);

                // Clear all inputs after adding to the database
                DescriptionTextBox.Text = string.Empty;
                AmountTextBox.Text = "$";
                TypeComboBox.SelectedIndex = 0; // Assuming the default index represents "Expense"
                CategoryComboBox.Items.Clear();
                DatePicker.SelectedDate = DateTime.Now;

                // Inside AddPersonUserControl, after inserting a person...
                GlobalEvents.RaiseDataAdded();

                // Refresh the persons data grid in the other tab
                mainWindow?.RefreshData();

                MessageBox.Show("Expense added successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            }
        }
    }
}
