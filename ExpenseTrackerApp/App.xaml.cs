using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ExpenseTrackerApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IMongoClient MongoClient { get; private set; }
        public static IMongoDatabase Database { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            MongoClient = new MongoClient("mongodb+srv://prathibaramesh2120:prathibaramesh2120@cluster0.2rc3s01.mongodb.net/?retryWrites=true&w=majority");
            Database = MongoClient.GetDatabase("ExpenseTracker");
        }
    }
}
