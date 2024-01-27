using ExpenseTrackerApp;
using ExpenseTrackerApp.Backend;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

public class ExpenseRepository
{
    private IMongoDatabase db;

    // Constructor: Initializes the ExpenseRepository with MongoDb connection and database name.
    public ExpenseRepository(string database)
    {        
        var connectionString = ConfigurationManager.AppSettings["MongoDbConnectionString"];
        var client = new MongoClient(connectionString);
        db = client.GetDatabase(database);
    }

    // Method: AddExpense
    // Purpose: Adds a new expense entry to the specified collection (table) in the database.
    public void AddExpense(string table, Expense expense)
    {
        var collection = db.GetCollection<Expense>(table);
        collection.InsertOne(expense);
    }

    // Method: GetExpenses
    // Purpose: Retrieves a list of expenses from the specified collection (table) in the database based on the provided filter.
    public List<Expense> GetExpenses(FilterDefinition<Expense> filter, string table)
    {
        var collection = db.GetCollection<Expense>(table);
        return collection.Find(filter).ToList();
    }

    public List<Expense> GetExpensesForChart(string table)
    {
        var collection = db.GetCollection<Expense>(table);
        return collection.Find(FilterDefinition<Expense>.Empty).ToList();
    }

    // Method: GetExpensesForChart
    // Purpose: Retrieves a list of expenses from the specified collection (table) in the database for a specific month and year.
    // Filters expenses based on the provided month and year.
    public List<Expense> GetExpensesByMonthYear(string table, int month, int year)
    {
        var collection = db.GetCollection<Expense>(table);
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        var filterBuilder = Builders<Expense>.Filter;
        var filter = filterBuilder.Gte(e => e.Date, new DateTimeOffset(startOfMonth)) &
                     filterBuilder.Lte(e => e.Date, new DateTimeOffset(endOfMonth.AddHours(23).AddMinutes(59).AddSeconds(59)));

        return collection.Find(filter).ToList();
    }

    // Method: GetExpensesByMonthYear
    // Method: GetExpensesGroupedByCategory
    // Purpose: Retrieves a list of expenses from the specified collection (table) in the database for a specific month and year,
    // and groups them by category. 
    public List<ExpenseGroup> GetExpensesGroupedByCategory(int month, int year, string table)
    {
        var collection = db.GetCollection<Expense>(table);
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        var filterBuilder = Builders<Expense>.Filter;
        var filter = filterBuilder.Gte(e => e.Date, new DateTimeOffset(startOfMonth)) &
                     filterBuilder.Lte(e => e.Date, new DateTimeOffset(endOfMonth.AddHours(23).AddMinutes(59).AddSeconds(59))) &
                     filterBuilder.Eq(e => e.Type, "Expense");

        var filteredExpenses = collection.Find(filter).ToList();

        var groupedData = filteredExpenses
            .GroupBy(e => e.Category)
            .Select(group => new ExpenseGroup
            {
                Category = group.Key,
                TotalAmount = group.Sum(e => e.Amount)
            })
            .ToList();

        return groupedData;
    }

}
