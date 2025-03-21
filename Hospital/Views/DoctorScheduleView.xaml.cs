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
            _viewModel = new DoctorScheduleViewModel(appointmentManagerModel, shiftManagerModel);

            LoadInitialCalendarRange();
            this.InitializeComponent();
            ((FrameworkElement)this.Content).DataContext = _viewModel;
            DailyScheduleList.ItemsSource = _viewModel.DailySchedule;
            DoctorSchedule.CalendarViewDayItemChanging += CalendarView_DayItemChanging;
            LoadShiftsAndRefreshCalendar();
        }

        private async void LoadShiftsAndRefreshCalendar()
        {
            await _viewModel.LoadShiftsForDoctor();

            if (_viewModel.Shifts == null || !_viewModel.Shifts.Any()) return;

            DoctorSchedule.SelectedDates.Clear();
            DoctorSchedule.InvalidateArrange();
            DoctorSchedule.InvalidateMeasure();
            DoctorSchedule.UpdateLayout();

            RecreateCalendarView();
        }

        private void RecreateCalendarView()
        {
            CalendarContainer.Children.Clear();

            var newCalendar = new CalendarView
            {
                MinDate = _viewModel.MinDate.DateTime,
                MaxDate = _viewModel.MaxDate.DateTime,
                SelectionMode = CalendarViewSelectionMode.Single,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Top,
                BorderBrush = new SolidColorBrush(Colors.Green),
                BorderThickness = new Thickness(2)
            };

            newCalendar.CalendarViewDayItemChanging += CalendarView_DayItemChanging;
            newCalendar.SelectedDatesChanged += CalendarView_SelectedDatesChanged;

            DoctorSchedule = newCalendar;
            CalendarContainer.Children.Insert(0, newCalendar); 
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
            if (_viewModel.ShiftDates == null || !_viewModel.ShiftDates.Any()) return;
            var date = args.Item.Date.Date;

            if (_viewModel.ShiftDates.Any(d => d.Date == date.Date))
            {
                args.Item.Background = new SolidColorBrush(Colors.LightGreen);
            }
        }
    }

}