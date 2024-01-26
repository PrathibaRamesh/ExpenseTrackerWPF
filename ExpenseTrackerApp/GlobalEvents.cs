using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTrackerApp
{
    public static class GlobalEvents
    {
        public delegate void DataAddedEventHandler();
        public static event DataAddedEventHandler OnDataAdded;

        public static void RaiseDataAdded()
        {
            OnDataAdded?.Invoke();
        }
    }
}
