using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using Hospital.Managers;
using Hospital.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;

namespace Hospital.Views
{
    class DoctorScheduleView : Window
    {
        private readonly AppointmentManagerModel _appointmentManager;
        private readonly ShiftManagerModel _shiftManager;

        private DoctorScheduleViewModel _viewModel;

        private CalendarView ScheduleCalendar;

        public DoctorScheduleView(AppointmentManagerModel appointmentManager, ShiftManagerModel shiftManager)
        {
            _appointmentManager = appointmentManager;
            _shiftManager = shiftManager;
            _viewModel = new DoctorScheduleViewModel(_appointmentManager, _shiftManager);

            var rootGrid = new Grid();
            this.Content = rootGrid;

            ScheduleCalendar = new CalendarView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                SelectionMode = CalendarViewSelectionMode.Multiple,

                IsGroupLabelVisible = false,
                IsOutOfScopeEnabled = false,

                MinDate = new DateTimeOffset(new DateTime(DateTime.Now.Year,DateTime.Now.Month,1)),
                MaxDate = new DateTimeOffset(new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month))),

                IsTodayHighlighted = true
            };
            

            rootGrid.Children.Add(ScheduleCalendar);
            this.Content = rootGrid;



        }


    }
}
