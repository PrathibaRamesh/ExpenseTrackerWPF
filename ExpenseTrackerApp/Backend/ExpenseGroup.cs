using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTrackerApp.Backend
{
    /// <summary>
    /// Represents a group of expenses belonging to a specific category and it's total amount.
    /// </summary>
    public class ExpenseGroup
    {
        public string Category { get; set; }
        public decimal TotalAmount { get; set; }
    }

}
