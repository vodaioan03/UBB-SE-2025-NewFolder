using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Hospital.Managers;
using Hospital.ViewModels;
using System;

namespace Hospital.Views
{
    public sealed partial class DoctorScheduleView : UserControl
    {
        private readonly AppointmentManagerModel _appointmentManager;
        private readonly ShiftManagerModel _shiftManager;
        private DoctorScheduleViewModel _viewModel;
        private CalendarView ScheduleCalendar;

        public DoctorScheduleView(AppointmentManagerModel appointmentManager, ShiftManagerModel shiftManager)
        {
            this.InitializeComponent();

            _appointmentManager = appointmentManager;
            _shiftManager = shiftManager;
            _viewModel = new DoctorScheduleViewModel(_appointmentManager, _shiftManager);

            SetupCalendar();
        }

        private void SetupCalendar()
        {
            ScheduleCalendar = new CalendarView
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                SelectionMode = CalendarViewSelectionMode.Multiple,
                IsGroupLabelVisible = false,
                IsOutOfScopeEnabled = false,
                IsTodayHighlighted = true
            };

            var today = DateTime.Today;
            ScheduleCalendar.MinDate = new DateTimeOffset(new DateTime(today.Year, today.Month, 1));
            ScheduleCalendar.MaxDate = new DateTimeOffset(new DateTime(today.Year, today.Month, DateTime.DaysInMonth(today.Year, today.Month)));

            var rootGrid = new Grid();
            rootGrid.Children.Add(ScheduleCalendar);
            this.Content = rootGrid;
        }

    }
}
