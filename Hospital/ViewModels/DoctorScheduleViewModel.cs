using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Managers;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Hospital.ViewModels
{
    class DoctorScheduleViewModel
    {
        private readonly AppointmentManagerModel _appointmentManager;
        private readonly ShiftManagerModel _shiftManager;

        public ObservableCollection<AppointmentJointModel> Appointments { get; set; }
        public List<Shift> Shifts { get; set; }  // Changed to List<Shift>

        public ICommand OpenDetailsCommand { get; set; }

        public DoctorScheduleViewModel(AppointmentManagerModel appointmentManager, ShiftManagerModel shiftManager)
        {
            _appointmentManager = appointmentManager;
            _shiftManager = shiftManager;
            Appointments = new ObservableCollection<AppointmentJointModel>();
            Shifts = new List<Shift>();

            OpenDetailsCommand = new RelayCommand<object>(OpenDetails);
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
            }
        }

        public async Task LoadShiftsForDoctor(int doctorId)
        {
            try
            {
                Shifts = await _shiftManager.LoadShifts(doctorId); 
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading shifts: {ex.Message}");
            }
        }
    }
}
