using LiveCharts.Defaults;
using System.Windows.Media;

namespace ExpenseTrackerApp
{
    public class ObservableValueWithColor : ObservableValue
    {
        public SolidColorBrush Color { get; set; }

        public ObservableValueWithColor(double value, SolidColorBrush color)
            : base(value)
        {
            Color = color;
        }
    }
}
