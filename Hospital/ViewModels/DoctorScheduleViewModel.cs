using Hospital.Models;
using Hospital.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Managers;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Hospital.Commands;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Hospital.Views;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Reflection.Metadata;

namespace Hospital.ViewModels
{
    public class DoctorScheduleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        // Managers
        private readonly AppointmentManagerModel _appointmentManager;
        private readonly ShiftManagerModel _shiftManager;

        // Observable Collections
        public ObservableCollection<TimeSlotModel> DailySchedule { get; set; } = new();
        public ObservableCollection<DateTimeOffset> ShiftDates { get; set; }

        public List<AppointmentJointModel> Appointments { get; set; }
        public List<Shift> Shifts { get; set; }

        public ICommand OpenDetailsCommand { get; set; }

        public int DoctorId { get; set; } = 1; // default for testing

        private DateTimeOffset _minDate;
        public DateTimeOffset MinDate
        {
            get => _minDate;
            set
            {
                _minDate = value;
                OnPropertyChanged();
            }
        }

        private DateTimeOffset _maxDate;
        public DateTimeOffset MaxDate
        {
            get => _maxDate;
            set
            {
                _maxDate = value;
                OnPropertyChanged();
            }
        }

        private TimeSlotModel _selectedSlot;
        public TimeSlotModel SelectedSlot
        {
            get => _selectedSlot;
            set
            {
                _selectedSlot = value;
                OnPropertyChanged();
            }
        }

        public DoctorScheduleViewModel(AppointmentManagerModel appointmentManager, ShiftManagerModel shiftManager)
        {
            _appointmentManager = appointmentManager;
            _shiftManager = shiftManager;
            Appointments = new List<AppointmentJointModel>();
            Shifts = new List<Shift>();
            ShiftDates = new ObservableCollection<DateTimeOffset>();

            OpenDetailsCommand = new RelayCommand(OpenAppointmentForDoctor);
        }

        private void OpenAppointmentForDoctor(object obj)
        {
            if (obj is not TimeSlotModel selectedSlot) return;

            if (string.IsNullOrEmpty(selectedSlot.Appointment) && selectedSlot.HighlightColor.Color == Colors.Transparent)
                return;

            SelectedSlot = selectedSlot;
        }



        public async Task LoadAppointmentsForDoctor()
        {
            try
            {
                await _appointmentManager.LoadAppointmentsForDoctor(DoctorId);
                var appointments = _appointmentManager.appointmentList;

                Appointments.Clear();
                foreach (var appointment in appointments)
                {
                    Appointments.Add(appointment);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading appointments: {ex.Message}");
                throw new Exception($"Error loading appointments: {ex.Message}");
            }
        }

        public async Task LoadShiftsForDoctor()
        {
            try
            {
                await _shiftManager.LoadShifts(this.DoctorId);
                Shifts = _shiftManager.GetShifts();

                ShiftDates.Clear();
                foreach (var shift in Shifts)
                {
                    var shiftStartDate = shift.DateTime.Date;
                    var shiftEndDate = shift.DateTime.Date;

                    if (shift.EndTime <= shift.StartTime)
                    {
                        shiftEndDate = shiftEndDate.AddDays(1); 
                    }

                    ShiftDates.Add(new DateTimeOffset(shiftStartDate, TimeSpan.Zero));

                    if (shiftEndDate > shiftStartDate)
                    {
                        ShiftDates.Add(new DateTimeOffset(shiftEndDate, TimeSpan.Zero));
                    }
                }

                OnPropertyChanged(nameof(ShiftDates));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading shifts: {ex.Message}");
                throw new Exception($"Error loading shifts: {ex.Message}");
            }
        }

        public async Task OnDateSelected(DateTime date)
        {
            DailySchedule.Clear();
            try
            {
                await _appointmentManager.LoadDoctorAppointmentsOnDate(DoctorId, date);
                Appointments = _appointmentManager.appointmentList;
                await _shiftManager.LoadShifts(DoctorId);
                Shifts = _shiftManager.GetShifts();

                var slots = GenerateTimeSlots(date);
                foreach (var slot in slots)
                {
                    DailySchedule.Add(slot);
                }
                OnPropertyChanged(nameof(DailySchedule));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Database access failed: {ex.Message}");
            }
        }

        private List<TimeSlotModel> GenerateTimeSlots(DateTime date)
        {
            List<TimeSlotModel> slots = new();
            DateTime startTime = date.Date;
            DateTime endTime = startTime.AddDays(1); 

            var selectedAppointments = Appointments
                .Where(a => a.Date.Date == date.Date)
                .ToList();

            var selectedShifts = Shifts
                .Where(s =>
                {
                    var shiftStart = s.DateTime.Date + s.StartTime;
                    var shiftEnd = s.DateTime.Date + s.EndTime;

                    if (s.EndTime <= s.StartTime)
                        shiftEnd = shiftEnd.AddDays(1); 

                    return shiftStart < endTime && shiftEnd > startTime;
                })
                .ToList();

            while (startTime < endTime)
            {
                var slot = new TimeSlotModel
                {
                    TimeSlot = startTime,
                    Time = startTime.ToString("hh:mm tt"),
                    Appointment = "",
                    HighlightColor = new SolidColorBrush(Colors.Transparent)
                };

                SolidColorBrush brush = new SolidColorBrush(Colors.Transparent);

                bool isInShift = selectedShifts.Any(s =>
                {
                    DateTime shiftStart = s.DateTime.Date + s.StartTime;
                    DateTime shiftEnd = s.DateTime.Date + s.EndTime;
                    if (s.EndTime <= s.StartTime)
                        shiftEnd = shiftEnd.AddDays(1);

                    return startTime >= shiftStart && startTime < shiftEnd;
                });

                var matchingAppointment = selectedAppointments.FirstOrDefault(a =>
                    a.Date == startTime && isInShift);

                if (matchingAppointment != null)
                {
                    slot.Appointment = matchingAppointment.ProcedureName;
                    brush = new SolidColorBrush(Colors.Orange);
                }
                else if (isInShift)
                {
                    brush = new SolidColorBrush(Colors.Green);
                }

                slot.HighlightColor = brush;
                slots.Add(slot);
                startTime = startTime.AddMinutes(30);
            }

            return slots;
        }

        

    }
}
