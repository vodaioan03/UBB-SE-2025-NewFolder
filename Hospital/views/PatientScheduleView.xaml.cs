using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Hospital.Managers;
using Hospital.Models;
using System.Diagnostics;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using System.Collections.Generic;
using Microsoft.UI;
using Hospital.ViewModels;


namespace Hospital.Views
{
    public sealed partial class PatientScheduleView : Window
    {
        private readonly AppointmentManagerModel _appointmentManager;
        public ObservableCollection<TimeSlotModel> DailyAppointments { get; private set; }
        private ObservableCollection<DateTimeOffset> HighlightedDates;

        public PatientScheduleView()
        {
            this.ExtendsContentIntoTitleBar = false;
            Debug.WriteLine("PatientScheduleView initialized.");
            this.InitializeComponent();

            _appointmentManager = new AppointmentManagerModel(new DatabaseServices.AppointmentsDatabaseService());
            DailyAppointments = new ObservableCollection<TimeSlotModel>();
            HighlightedDates = new ObservableCollection<DateTimeOffset>();

            DailyScheduleList.ItemsSource = DailyAppointments;
            AppointmentsCalendar.CalendarViewDayItemChanging += CalendarView_DayItemChanging;

            LoadAppointmentsForPatient(1); // Replace with actual patient ID
        }

        private async void LoadAppointmentsForPatient(int patientId)
        {
            Debug.WriteLine($"Fetching appointments for patient ID: {patientId}...");

            await _appointmentManager.LoadAppointmentsForPatient(patientId);

            HighlightedDates.Clear();
            foreach (var appointment in _appointmentManager.s_appointmentList)
            {
                HighlightedDates.Add(new DateTimeOffset(appointment.Date.Date)); // Highlight days
            }

            AppointmentsCalendar.InvalidateMeasure(); // Refresh calendar UI
        }

        private void AppointmentsCalendar_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (args.AddedDates.Count > 0)
            {
                DateTime selectedDate = args.AddedDates[0].DateTime.Date;
                Debug.WriteLine($"Selected Date: {selectedDate}");

                DailyAppointments.Clear();
                List<TimeSlotModel> timeSlots = GenerateTimeSlots(selectedDate);

                var selectedAppointments = _appointmentManager.s_appointmentList
                    .Where(a => a.Date.Date == selectedDate)
                    .OrderBy(a => a.Date.TimeOfDay)
                    .ToList();

                foreach (var appointment in selectedAppointments)
                {
                    DateTime appointmentStart = appointment.Date;
                    DateTime appointmentEnd = appointmentStart.Add(appointment.ProcedureDuration);

                    foreach (var slot in timeSlots)
                    {
                        if (slot.TimeSlot >= appointmentStart && slot.TimeSlot < appointmentEnd)
                        {
                            slot.Appointment = appointment.ProcedureName;
                            slot.HighlightColor = new SolidColorBrush(Colors.Green);
                        }
                    }
                }

                foreach (var slot in timeSlots)
                {
                    DailyAppointments.Add(slot);
                }
            }
        }

        private List<TimeSlotModel> GenerateTimeSlots(DateTime date)
        {
            List<TimeSlotModel> slots = new List<TimeSlotModel>();
            DateTime startTime = date.Date;
            DateTime endTime = startTime.AddHours(24);

            while (startTime < endTime)
            {
                slots.Add(new TimeSlotModel
                {
                    TimeSlot = startTime,
                    Time = startTime.ToString("hh:mm tt"),
                    Appointment = "",
                    HighlightColor = new SolidColorBrush(Colors.Transparent)
                });

                startTime = startTime.AddMinutes(30);
            }

            return slots;
        }

        private void CalendarView_DayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            var date = args.Item.Date.Date;
            if (HighlightedDates.Any(a => a.Date == date))
            {
                args.Item.Background = new SolidColorBrush(Colors.LightGreen);
            }
        }
        private void DailyScheduleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedSlot = (TimeSlotModel)e.AddedItems[0];

                if (!string.IsNullOrEmpty(selectedSlot.Appointment))
                {
                    var selectedDate = AppointmentsCalendar.SelectedDates.Any()
                        ? AppointmentsCalendar.SelectedDates.First().DateTime.Date
                        : DateTime.MinValue;  // Default to MinValue if no date is selected

                    var selectedAppointment = _appointmentManager.s_appointmentList
                        .FirstOrDefault(a =>
                            a.ProcedureName == selectedSlot.Appointment &&
                            a.Date.Date == selectedDate);

                    if (selectedAppointment != null)
                    {
                        AppointmentDetailsView detailsView = new AppointmentDetailsView(selectedAppointment, _appointmentManager);
                        detailsView.Activate();
                    }
                }
            }
        }

    }

}
