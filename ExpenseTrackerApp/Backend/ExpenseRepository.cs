using ExpenseTrackerApp;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
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
}
