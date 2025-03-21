using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Hospital.Managers;
using Hospital.Models;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using System.Collections.Generic;
using Microsoft.UI;
using Hospital.ViewModels;
using System.Threading.Tasks;
using Microsoft.UI.Dispatching;

namespace Hospital.Views
{
    public sealed partial class PatientScheduleView : Window
    {
        private readonly AppointmentManagerModel _appointmentManager;
        public ObservableCollection<TimeSlotModel> DailyAppointments { get; private set; }
        private ObservableCollection<DateTimeOffset> HighlightedDates;
        private readonly DispatcherQueue _dispatcherQueue;

        public PatientScheduleView()
        {
            this.ExtendsContentIntoTitleBar = false;
            this.InitializeComponent();

            _dispatcherQueue = DispatcherQueue.GetForCurrentThread();

            _appointmentManager = new AppointmentManagerModel(new DatabaseServices.AppointmentsDatabaseService());
            DailyAppointments = new ObservableCollection<TimeSlotModel>();
            HighlightedDates = new ObservableCollection<DateTimeOffset>();

            DailyScheduleList.ItemsSource = DailyAppointments;
            AppointmentsCalendar.CalendarViewDayItemChanging += CalendarView_DayItemChanging;

            DateTime now = DateTime.Now;
            DateTime firstDay = new DateTime(now.Year, now.Month, 1);
            DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);

            AppointmentsCalendar.MinDate = firstDay;
            AppointmentsCalendar.MaxDate = lastDay;

            LoadAppointmentsAndUpdateUI();
        }

        private async void LoadAppointmentsAndUpdateUI()
        {
            await LoadAppointmentsForPatient(1); // can be changed to the current patient
            ForceCalendarUIRefresh();
            await Task.Delay(150);
            ForceCalendarUIRefresh();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            RefreshAppointments();
        }


        private async Task LoadAppointmentsForPatient(int patientId)
        {
            await _appointmentManager.LoadAppointmentsForPatient(patientId);

            HighlightedDates.Clear();
            foreach (var appointment in _appointmentManager.s_appointmentList)
            {
                HighlightedDates.Add(new DateTimeOffset(appointment.Date.Date));
            }

            ForceCalendarUIRefresh();
        }

        private void ForceCalendarUIRefresh()
        {
            _dispatcherQueue.TryEnqueue(() =>
            {
                AppointmentsCalendar.SelectedDates.Clear();
                AppointmentsCalendar.InvalidateMeasure();
                AppointmentsCalendar.UpdateLayout();
            });
        }

        private void AppointmentsCalendar_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (args.AddedDates.Count > 0)
            {
                DateTime selectedDate = args.AddedDates[0].DateTime.Date;

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
                        : DateTime.MinValue;

                    var selectedAppointment = _appointmentManager.s_appointmentList
                        .FirstOrDefault(a =>
                            a.ProcedureName == selectedSlot.Appointment &&
                            a.Date.Date == selectedDate);

                    if (selectedAppointment != null)
                    {
                        AppointmentDetailsView detailsView = new AppointmentDetailsView(selectedAppointment, _appointmentManager, RefreshAppointments);
                        detailsView.Activate();
                    }
                }
            }
        }

        private async void RefreshAppointments()
        {
            await LoadAppointmentsForPatient(1);
            await Task.Delay(100);
            ForceCalendarUIRefresh();
        }
    }
}