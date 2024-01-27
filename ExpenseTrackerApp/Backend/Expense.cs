using MongoDB.Bson;
using System;

namespace ExpenseTrackerApp
{
    /// <summary>
    /// Represents an expense entry class and attributes for the expense tracker application.
    /// </summary>
    public class Expense
    {
        // Gets or sets the unique identifier for the expense.
        public ObjectId Id { get; set; }
        // Gets or sets the description of the expense.
        public string Description { get; set; }
        // Gets or sets the amount of the expense.
        public decimal Amount { get; set; }
        // Gets or sets the type of the expense (e.g., 'Food', 'Transportation', 'Entertainment', etc.).
        public string Type { get; set; }
        // Gets or sets the category of the expense (e.g., 'Groceries', 'Gas', 'Movie', etc.).
        public string Category { get; set; }
        // Gets or sets the date and time when the expense was recorded.
        public DateTimeOffset Date { get; set; }

        // Initializes a new instance of the <see cref="Expense"/> class with default value as current date.
        public Expense()
        {
            Date = DateTimeOffset.Now;
        }
    }
}
