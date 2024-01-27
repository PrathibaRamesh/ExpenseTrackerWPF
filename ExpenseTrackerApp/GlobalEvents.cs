namespace ExpenseTrackerApp
{
    /// <summary>
    /// Provides a mechanism for handling global data added events.
    /// </summary>
    public static class GlobalEvents
    {
        // Represents the delegate for the DataAdded event.
        public delegate void DataAddedEventHandler();
        // Event that is triggered when new data is added globally.
        public static event DataAddedEventHandler OnDataAdded;

        public static void RaiseDataAdded()
        {
            // Invoke the event, calling all subscribed event handlers.
            OnDataAdded?.Invoke();
        }
    }
}
