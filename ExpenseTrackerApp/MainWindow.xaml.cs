using System;
using System.Windows;

namespace ExpenseTrackerApp
{
    // Interface for Unit Text purposes
    public interface IMainWindow
    {
        ExpenseRepository db { get; }

        void RefreshData();

        void AddExpense(string category, Expense expense); 
    }
    public partial class MainWindow : Window
    {
        public ExpenseRepository db;
        public MainWindow()
        {
            InitializeComponent();
            db = new ExpenseRepository("ExpenseTracker");

            GlobalEvents.OnDataAdded += RefreshData;
        }

        // Method to refresh the data in ViewPersonsUserControl
        public void RefreshData()
        {
            // Here, navigate to the Frame again to refresh corresponding tab views
            viewDataFrame.Navigate(new Uri("Frontend\\ViewExpensesTab.xaml", UriKind.Relative));
            reportDataFrame.Navigate(new Uri("Frontend\\MonthlyReportTab.xaml", UriKind.Relative));
        }
    }
}
