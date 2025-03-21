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
        private int _doctorId = 1;//Just for testing

        public DoctorScheduleView(AppointmentManagerModel appointmentManagerModel, ShiftManagerModel shiftManagerModel)
        {
            this.InitializeComponent();

            _appointmentManager = appointmentManagerModel;
            _shiftManager = shiftManagerModel;
            _shiftsDates = new ObservableCollection<DateTimeOffset>();

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
                
            }
        }


    }
}
