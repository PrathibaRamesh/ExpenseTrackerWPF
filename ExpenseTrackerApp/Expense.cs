using MongoDB.Bson;
using System;

namespace ExpenseTrackerApp
{
    public class Expense
    {
        public ObjectId Id { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }

        // Add other properties as needed

        public Expense()
        {
            // Ensure Date is initialized with a default value if needed
            Date = DateTime.Now;
        }
    }
}
