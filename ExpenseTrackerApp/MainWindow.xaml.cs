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
            //var viewPersonsControl = MyTabControl.Items[1] as ViewPersonsUserControl;
            //viewPersonsControl?.LoadPersonsData();
            // Here, navigate to the ViewPersonsUserControl Frame again to refresh it
            viewDataFrame.Navigate(new Uri("Frontend\\ViewExpensesTab.xaml", UriKind.Relative));
            reportDataFrame.Navigate(new Uri("Frontend\\MonthlyReportTab.xaml", UriKind.Relative));
        }
    }
}
