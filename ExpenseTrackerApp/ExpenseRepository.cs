using ExpenseTrackerApp;
using MongoDB.Driver;
using System.Collections.Generic;

public class ExpenseRepository
{
    private readonly IMongoCollection<Expense> _expenseCollection;

    public ExpenseRepository()
    {
        _expenseCollection = App.Database.GetCollection<Expense>("Expenses");
    }

    public void AddExpense(Expense expense)
    {
        // Insert expense data into MongoDB
        _expenseCollection.InsertOne(expense);
    }

    public List<Expense> GetExpenses(FilterDefinition<Expense> filter)
    {
        // Retrieve expenses from the MongoDB collection based on filter
        return _expenseCollection.Find(filter).ToList();
    }
}
