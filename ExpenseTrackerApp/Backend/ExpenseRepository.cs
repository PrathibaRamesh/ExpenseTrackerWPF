using ExpenseTrackerApp;
using ExpenseTrackerApp.Backend;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

public class ExpenseRepository
{
    // private readonly IMongoCollection<Expense> _expenseCollection;
    private IMongoDatabase db;

    public ExpenseRepository(string database)
    {
        //_expenseCollection = App.Database.GetCollection<Expense>("Expenses");
        // MongoDB connection string
        var connectionString = "mongodb+srv://prathibaramesh2120:prathibaramesh2120@cluster0.2rc3s01.mongodb.net/?retryWrites=true&w=majority";

        var client = new MongoClient(connectionString);
        db = client.GetDatabase(database);
    }

    public void AddExpense(string table, Expense expense)
    {
        // Insert expense data into MongoDB
        //_expenseCollection.InsertOne(expense);

        var collection = db.GetCollection<Expense>(table);
        collection.InsertOne(expense);
    }

    public List<Expense> GetExpenses(FilterDefinition<Expense> filter, string table)
    {
        // Retrieve expenses from the MongoDB collection based on filter
        //return _expenseCollection.Find(filter).ToList();

        var collection = db.GetCollection<Expense>(table);
        return collection.Find(filter).ToList();
    }

    public List<Expense> GetExpensesForChart(string table)
    {
        // Retrieve all expenses from the MongoDB collection
        //return _expenseCollection.Find(FilterDefinition<Expense>.Empty).ToList();

        var collection = db.GetCollection<Expense>(table);
        return collection.Find(FilterDefinition<Expense>.Empty).ToList();
    }

    public List<Expense> GetExpensesByMonthYear(string table, int month, int year)
    {
        var collection = db.GetCollection<Expense>(table);
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1); // Adjusted to include the entire last day

        var filterBuilder = Builders<Expense>.Filter;
        var filter = filterBuilder.Gte(e => e.Date, new DateTimeOffset(startOfMonth)) &
                     filterBuilder.Lte(e => e.Date, new DateTimeOffset(endOfMonth.AddHours(23).AddMinutes(59).AddSeconds(59))); // Include the entire last day

        return collection.Find(filter).ToList();
    }

    public List<ExpenseGroup> GetExpensesGroupedByCategory(int month, int year, string table)
    {
        var collection = db.GetCollection<Expense>(table);
        var startOfMonth = new DateTime(year, month, 1);
        var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

        var filterBuilder = Builders<Expense>.Filter;
        var filter = filterBuilder.Gte(e => e.Date, new DateTimeOffset(startOfMonth)) &
                     filterBuilder.Lte(e => e.Date, new DateTimeOffset(endOfMonth.AddHours(23).AddMinutes(59).AddSeconds(59))) &
                     filterBuilder.Eq(e => e.Type, "Expense");

        //var groupedData = collection
        //    .Aggregate()
        //    .Match(filter)
        //    .Group(
        //        e => e.Category,
        //        g => new ExpenseGroup
        //        {
        //            Category = g.Key,
        //            TotalAmount = g.Sum(e => e.Amount)
        //        }
        //    )
        //    .ToList();

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
