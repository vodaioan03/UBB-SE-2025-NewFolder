using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.DatabaseServices;
using Hospital.Managers;
using CommunityToolkit.Mvvm.Input; // Correct namespace for RelayCommand
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Hospital.ViewModels
{
    class DoctorScheduleViewModel
    {
        private readonly AppointmentManagerModel _appointmentManager;
        private readonly ShiftManagerModel _shiftManager;

        public ObservableCollection<AppointmentJointModel> Appointments { get; set; }
        public ObservableCollection<Shift> Shifts { get; set; } // FIX: Changed to ObservableCollection

        public ICommand OpenDetailsCommand { get; set; }

        public DoctorScheduleViewModel(AppointmentManagerModel appointmentManager, ShiftManagerModel shiftManager)
        {
            _appointmentManager = appointmentManager;
            _shiftManager = shiftManager;
            Appointments = new ObservableCollection<AppointmentJointModel>();
            Shifts = new ObservableCollection<Shift>();

            // RelayCommand is now correctly set up
            OpenDetailsCommand = new RelayCommand<object>(OpenDetails);
        }

        private void OpenDetails(object obj)
        {
            if (obj is AppointmentJointModel appointment)
            {
                Console.WriteLine($"Opening details for Appointment ID: {appointment.AppointmentId}");
                // TODO: Implement UI navigation or details view
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
            }
        }

        public async Task LoadShiftsForDoctor(int doctorId)
        {
            try
            {
                var shifts = await _shiftManager.LoadShiftsAsync(doctorId); // FIX: Await the result
                Shifts.Clear();

                foreach (var shift in shifts)
                {
                    Shifts.Add(shift); // UI updates automatically
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading shifts: {ex.Message}");
            }
        }
    }
}
 