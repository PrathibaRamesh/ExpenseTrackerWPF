using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ExpenseTrackerApp.Frontend
{
    /// <summary>
    /// Interaction logic for ViewExpensesTab.xaml
    /// </summary>
    public partial class ViewExpensesTab : UserControl
    {
        // Define categories for each type
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

                var mainWindow = Application.Current.MainWindow as MainWindow;
                ExpensesDataGrid.ItemsSource = mainWindow?.db.GetExpenses(filter, "Expenses");
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
        private void DatePicker1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadExpenses();
        }

        private void CategoryComboBox1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadExpenses();
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
