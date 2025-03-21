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
using System.Linq;

namespace Hospital.Views
{
    public sealed partial class DoctorScheduleView : Window
    {
        public DoctorScheduleViewModel ViewModel => _viewModel;

        private readonly DoctorScheduleViewModel _viewModel;

        public DoctorScheduleView(AppointmentManagerModel appointmentManagerModel, ShiftManagerModel shiftManagerModel)
        {
            this.InitializeComponent();

            _viewModel = new DoctorScheduleViewModel(appointmentManagerModel, shiftManagerModel);
            _viewModel.DoctorId = 1;
            ((FrameworkElement)this.Content).DataContext = _viewModel;


            LoadInitialCalendarRange();
            _viewModel.LoadShiftsForDoctor(_viewModel.DoctorId);
        }

        private void LoadInitialCalendarRange()
        {
            var today = DateTime.Today;
            _viewModel.MinDate = new DateTimeOffset(new DateTime(today.Year, today.Month, 1));
            _viewModel.MaxDate = _viewModel.MinDate.AddMonths(1).AddDays(-1);
        }

        private async void CalendarView_SelectedDatesChanged(CalendarView sender, CalendarViewSelectedDatesChangedEventArgs args)
        {
            if (args.AddedDates.Count > 0)
            {
                var selectedDate = args.AddedDates[0].DateTime.Date;
                await _viewModel.OnDateSelected(selectedDate);
            }
        }

        private void CalendarView_DayItemChanging(CalendarView sender, CalendarViewDayItemChangingEventArgs args)
        {
            if (_viewModel.ShiftDates.Any(d => d.Date == args.Item.Date.Date))
            {
                args.Item.SetDensityColors(new List<Windows.UI.Color> { Microsoft.UI.Colors.Green });
            }
        }
    }

}
