using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Hospital.Managers;
using Hospital.Models;
using System.Threading.Tasks;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using Microsoft.UI;
using System.Diagnostics;

namespace Hospital.Views
{
    public sealed partial class PatientScheduleView : Window
    {
        private readonly AppointmentManagerModel _appointmentManager;
        public ObservableCollection<DateTimeOffset> Appointments { get; private set; }
        private CalendarView AppointmentsCalendar;

        public PatientScheduleView()
        {
            this.ExtendsContentIntoTitleBar = false;

            //Log that the window is being initialized
            Debug.WriteLine("PatientScheduleView initialized.");

            // Create the root layout
            var rootGrid = new Grid();
            AppointmentsCalendar = new CalendarView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                SelectionMode = CalendarViewSelectionMode.Multiple
            };

            rootGrid.Children.Add(AppointmentsCalendar);
            this.Content = rootGrid; // Set window content

            // Initialize Data & Events
            _appointmentManager = new AppointmentManagerModel(new DatabaseServices.AppointmentsDatabaseService());
            Appointments = new ObservableCollection<DateTimeOffset>();

            AppointmentsCalendar.CalendarViewDayItemChanging += CalendarView_DayItemChanging;

            LoadAppointmentsForPatient(1); // Replace with actual patient ID
        }

        private async void LoadAppointmentsForPatient(int patientId)
        {
            Debug.WriteLine($"Fetching appointments for patient ID: {patientId}...");

            await _appointmentManager.LoadAppointmentsForPatient(patientId);
            Appointments.Clear();

            if (_appointmentManager.s_appointmentList.Count == 0)
            {
                Debug.WriteLine("No appointments found.");
            }

            foreach (var appointment in _appointmentManager.s_appointmentList)
            {
                Debug.WriteLine($"Appointment found: {appointment.Date}");
                Appointments.Add(new DateTimeOffset(appointment.Date));
            }

            Debug.WriteLine($"Total Appointments Loaded: {Appointments.Count}");
        }

        private void CalendarView_DayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            var date = args.Item.Date;
            Debug.WriteLine($"Checking date: {date}");

            // Highlight dates with appointments
            if (Appointments.Any(a => a.Date == date.Date))
            {
                Debug.WriteLine($"Highlighting appointment date: {date}");
                args.Item.Background = new SolidColorBrush(Colors.LightPink);
            }
        }
    }
}