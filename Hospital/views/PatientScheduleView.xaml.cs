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

            RefreshAppointments();
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

                bool anyAppointments = false;

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
                            anyAppointments = true;
                        }
                    }
                }

                foreach (var slot in timeSlots)
                {
                    DailyAppointments.Add(slot);
                }

                NoAppointmentsText.Visibility = anyAppointments ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private List<TimeSlotModel> GenerateTimeSlots(DateTime date)
        {
            List<TimeSlotModel> slots = new List<TimeSlotModel>();

            // Start at 8:00 AM
            DateTime startTime = date.Date.AddHours(8);

            // End at 6:00 PM (18:00)
            DateTime endTime = date.Date.AddHours(18);

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

        private async void DailyScheduleList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                var selectedSlot = (TimeSlotModel)e.AddedItems[0];

                // Clear selection immediately to allow re-selection later
                DailyScheduleList.SelectedItem = null;

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
                        await ShowAppointmentDetailsDialog(selectedAppointment);
                    }
                }
            }
        }


        private async Task ShowAppointmentDetailsDialog(AppointmentJointModel appointment)
        {
            string message = $"Date and Time: {appointment.Date:f}\n" +
                             $"Doctor: {appointment.DoctorName}\n" +
                             $"Department: {appointment.DepartmentName}\n" +
                             $"Procedure: {appointment.ProcedureName}\n" +
                             $"Procedure Duration: {appointment.ProcedureDuration.TotalMinutes} minutes";

            ContentDialog dialog = new ContentDialog
            {
                Title = "Appointment Details",
                CloseButtonText = "Close",
                XamlRoot = this.Content.XamlRoot,
                RequestedTheme = ElementTheme.Default
            };

            StackPanel dialogContent = new StackPanel
            {
                Spacing = 10
            };


            dialogContent.Children.Add(new TextBlock
            {
                Text = message,
                TextWrapping = TextWrapping.Wrap
            });

            // Determine if cancellation is allowed
            bool canCancel = (appointment.Date.ToLocalTime() - DateTime.Now).TotalHours >= 24;

            StackPanel buttonRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                Spacing = 10,
                Margin = new Thickness(0, 10, 0, 0)
            };

            // Go Back button

            // Cancel button (enabled or disabled)
            if (canCancel)
            {
                Button cancelBtn = new Button
                {
                    Content = "Cancel Appointment",
                    Width = 160,
                    Height = 40,
                    Background = new SolidColorBrush(Colors.Red),
                    Foreground = new SolidColorBrush(Colors.White)
                };
                cancelBtn.Click += async (s, e) =>
                {
                    dialog.Hide();
                    ContentDialog confirmDialog = new ContentDialog
                    {
                        Title = "Confirm Cancellation",
                        Content = "Are you sure you want to cancel this appointment?",
                        PrimaryButtonText = "Yes",
                        CloseButtonText = "No",
                        DefaultButton = ContentDialogButton.Close,
                        XamlRoot = this.Content.XamlRoot
                    };

                    var result = await confirmDialog.ShowAsync();
                    if (result == ContentDialogResult.Primary)
                    {
                        try
                        {
                            _appointmentManager.RemoveAppointment(appointment.AppointmentId);


                            // Force a full refresh of the DailyScheduleList (destroy and recreate)
                            if (AppointmentsCalendar.SelectedDates.Any())
                            {
                                var selectedDate = AppointmentsCalendar.SelectedDates.First().DateTime.Date;

                                DailyAppointments.Clear();

                                List<TimeSlotModel> timeSlots = GenerateTimeSlots(selectedDate);

                                var selectedAppointments = _appointmentManager.s_appointmentList
                                    .Where(a => a.Date.Date == selectedDate)
                                    .OrderBy(a => a.Date.TimeOfDay)
                                    .ToList();

                                foreach (var appointment in selectedAppointments)
                                {
                                    DateTime start = appointment.Date;
                                    DateTime end = start.Add(appointment.ProcedureDuration);

                                    foreach (var slot in timeSlots)
                                    {
                                        if (slot.TimeSlot >= start && slot.TimeSlot < end)
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

                                NoAppointmentsText.Visibility = selectedAppointments.Any() ? Visibility.Collapsed : Visibility.Visible;
                            }

                        }
                        catch (Exception ex)
                        {
                            ContentDialog errorDialog = new ContentDialog
                            {
                                Title = "Cancellation Failed",
                                Content = ex.Message,
                                CloseButtonText = "OK",
                                XamlRoot = this.Content.XamlRoot
                            };
                            await errorDialog.ShowAsync();
                        }
                    }
                };

                buttonRow.Children.Add(cancelBtn);
            }
            else
            {
                Button disabledBtn = new Button
                {
                    Content = "Cancel Appointment",
                    Width = 160,
                    Height = 40,
                    IsEnabled = false,
                    Background = new SolidColorBrush(Color.FromArgb(255, 255, 102, 102)),
                    Foreground = new SolidColorBrush(Colors.White)
                };
                ToolTipService.SetToolTip(disabledBtn, "You can only cancel appointments more than 24 hours in advance.");
                buttonRow.Children.Add(disabledBtn);
            }

            // Add the full row of buttons to the dialog
            dialogContent.Children.Add(buttonRow);


            dialog.Content = dialogContent;

            await dialog.ShowAsync();

            DailyScheduleList.SelectedItem = null;
        }


        private async void RefreshAppointments()
        {
            try
            {
                // Detach old event handlers to avoid duplicate calls
                AppointmentsCalendar.CalendarViewDayItemChanging -= CalendarView_DayItemChanging;
                AppointmentsCalendar.SelectedDatesChanged -= AppointmentsCalendar_SelectedDatesChanged;

                // Reset visual and functional properties
                AppointmentsCalendar.MinDate = DateTimeOffset.Now.Date;
                AppointmentsCalendar.MaxDate = DateTimeOffset.Now.Date.AddMonths(1).AddDays(-1);
                AppointmentsCalendar.SelectionMode = CalendarViewSelectionMode.Single;
                AppointmentsCalendar.BorderBrush = new SolidColorBrush(Colors.Green);
                AppointmentsCalendar.BorderThickness = new Thickness(2);

                // Re-attach event handlers
                AppointmentsCalendar.CalendarViewDayItemChanging += CalendarView_DayItemChanging;
                AppointmentsCalendar.SelectedDatesChanged += AppointmentsCalendar_SelectedDatesChanged;

                await LoadAppointmentsForPatient(1);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error refreshing calendar: " + ex.Message);
            }
        }


    }
}
