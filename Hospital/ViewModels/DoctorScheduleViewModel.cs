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

namespace Hospital.ViewModels
{
    public class DoctorScheduleViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //Mangers
        private readonly AppointmentManagerModel _appointmentManager;
        private readonly ShiftManagerModel _shiftManager;

        //Observable Collections
        public ObservableCollection<TimeSlotModel> DailySchedule { get; set; } = new();
        public ObservableCollection<DateTimeOffset> ShiftDates { get; set; }

        public List<AppointmentJointModel> Appointments { get; set; }
        public List<Shift> Shifts { get; set; }

        public ICommand OpenDetailsCommand { get; set; }

        public int DoctorId { get; set; } = 1; // default for testing

        //Setting the calendar to show only the current month
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

        //Setting the calendar to not allow navigation to other months
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


        public DoctorScheduleViewModel(AppointmentManagerModel appointmentManager, ShiftManagerModel shiftManager)
        {
            _appointmentManager = appointmentManager;
            _shiftManager = shiftManager;
            Appointments = new List<AppointmentJointModel>();
            Shifts = new List<Shift>();
            ShiftDates = new ObservableCollection<DateTimeOffset>();

            OpenDetailsCommand = new RelayCommand(OpenDetails);
        }


        private void OpenDetails(object obj)
        {
            if (obj is AppointmentJointModel appointment)
            {
                Console.WriteLine($"Opening details for Appointment ID: {appointment.AppointmentId}");
            }
        }

        public async Task LoadAppointmentsForDoctor(int doctorId)
        {
            try
            {
                await _appointmentManager.LoadAppointmentsForDoctor(doctorId);
                var appointments = _appointmentManager.s_appointmentList;

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
                    var shiftDate = shift.DateTime.Date;
                    ShiftDates.Add(new DateTimeOffset(shiftDate, TimeSpan.Zero));
                }


                OnPropertyChanged(nameof(ShiftDates));

            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error loading shifts: {ex.Message}");
                throw new Exception($"Error loading shifts: {ex.Message}");
            }
        }


        public async Task OnDateSelected(DateTime date)
        {
            try { 
            await _appointmentManager.LoadDoctorAppointmentsOnDate(DoctorId, date);
            await _shiftManager.LoadShifts(DoctorId);

            DailySchedule.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading appointments: {ex.Message}");
                throw new Exception($"Error loading appointments: {ex.Message}");
            }

        }

    }
}