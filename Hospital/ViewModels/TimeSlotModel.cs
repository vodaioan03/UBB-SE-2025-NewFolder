using System;
using Microsoft.UI;
using Microsoft.UI.Xaml.Media;
using Windows.UI;

namespace Hospital.ViewModels
{
    public class TimeSlotModel
    {
        public DateTime TimeSlot { get; set; }
        public string Time { get; set; }
        public string Appointment { get; set; }
        public SolidColorBrush HighlightColor { get; set; }

        public TimeSlotModel()
        {
            HighlightColor = new SolidColorBrush(Colors.Transparent);
        }
    }
}
