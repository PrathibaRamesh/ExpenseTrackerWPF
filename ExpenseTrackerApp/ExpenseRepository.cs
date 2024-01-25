using ExpenseTrackerApp;
using MongoDB.Driver;

public class ExpenseRepository
{
    private readonly IMongoCollection<Expense> _expenseCollection;

    public ExpenseRepository()
    {
        _expenseCollection = App.Database.GetCollection<Expense>("Expenses");
    }

    public void AddExpense(Expense expense)
    {
        _expenseCollection.InsertOne(expense);
    }

    // Implement other CRUD operations as needed
}
