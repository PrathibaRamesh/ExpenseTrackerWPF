using System;
using System.Windows;

namespace ExpenseTrackerApp
{
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
            // Here, navigate to the Frame again to refresh it
            viewDataFrame.Navigate(new Uri("Frontend\\ViewExpensesTab.xaml", UriKind.Relative));
            reportDataFrame.Navigate(new Uri("Frontend\\MonthlyReportTab.xaml", UriKind.Relative));
        }
    }
}
