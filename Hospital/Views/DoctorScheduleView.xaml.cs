using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Hospital.Managers;
using Hospital.ViewModels;
using System;
using Microsoft.UI;
using System.Collections.ObjectModel;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml.Media;
using System.Collections.Generic;

namespace Hospital.Views
{
    public sealed partial class DoctorScheduleView : Window
    {
        private readonly AppointmentManagerModel _appointmentManager;
        private readonly ShiftManagerModel _shiftManager;
        private ObservableCollection<DateTimeOffset> _shiftsDates;
        public ObservableCollection<TimeSlotModel> DailySchedule { get; private set; } 
        private int _doctorId = 1;//Just for testing

        public DoctorScheduleView(AppointmentManagerModel appointmentManagerModel, ShiftManagerModel shiftManagerModel)
        {
            this.InitializeComponent();

            _appointmentManager = appointmentManagerModel;
            _shiftManager = shiftManagerModel;
            _shiftsDates = new ObservableCollection<DateTimeOffset>();
            DailySchedule = new ObservableCollection<TimeSlotModel>();
            DailyScheduleList.ItemsSource = DailySchedule;

            DateTime today = DateTime.Today;
            DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            DoctorSchedule.MinDate = firstDayOfMonth;
            DoctorSchedule.MaxDate = lastDayOfMonth;

            DoctorSchedule.CalendarViewDayItemChanging += CalendarView_DayItemChanging;
            LoadShiftsForDoctor(_doctorId);
        }


        private async void LoadShiftsForDoctor(int doctorID)
        {
            await _shiftManager.LoadShifts(doctorID);
            _shiftsDates.Clear();
            foreach (var shift in _shiftManager.GetShifts())
            {
                _shiftsDates.Add(new DateTimeOffset(shift.DateTime.Date));
            }
            DoctorSchedule.InvalidateMeasure();
        }

        private void CalendarView_DayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            DateTime date = args.Item.Date.Date;

            if (_shiftsDates.Contains(new DateTimeOffset(date)))
            {
                args.Item.SetDensityColors(new List<Windows.UI.Color> { Microsoft.UI.Colors.Green });
            }
        }

        private void DoctorSchedule_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (args.AddedDates.Count > 0)
            {
                DateTime selectedDate = args.AddedDates[0].DateTime.Date;
                _appointmentManager.LoadDoctorAppointmentsOnDate(_doctorId, selectedDate);
                _shiftManager.LoadShifts(_doctorId);
                DailySchedule.Clear();

                List<TimeSlotModel> timeSlots = GenerateTimeSlots(selectedDate);
                foreach (var slot in timeSlots)
                {
                    DailySchedule.Add(slot);
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




    }
}
