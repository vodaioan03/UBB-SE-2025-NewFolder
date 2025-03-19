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

namespace Hospital.ViewModels
{
    public class DoctorScheduleViewModel
    {
        private readonly AppointmentManagerModel _appointmentManager;
        private readonly ShiftManagerModel _shiftManager;

        public List<AppointmentJointModel> Appointments { get; set; }
        public List<Shift> Shifts { get; set; }  

        public ICommand OpenDetailsCommand { get; set; }

        public DoctorScheduleViewModel(AppointmentManagerModel appointmentManager, ShiftManagerModel shiftManager)
        {
            _appointmentManager = appointmentManager;
            _shiftManager = shiftManager;
            Appointments = new List<AppointmentJointModel>();
            Shifts = new List<Shift>();

            OpenDetailsCommand = new RelayCommand(OpenDetails);
        }

        public DoctorScheduleViewModel()
        {
            AppointmentsDatabaseService appointmentDatabaseService = new AppointmentsDatabaseService();
            _appointmentManager = new AppointmentManagerModel(appointmentDatabaseService);
            ShiftsDatabaseService shiftDatabaseService = new ShiftsDatabaseService();
            _shiftManager = new ShiftManagerModel(shiftDatabaseService);
            Appointments = new List<AppointmentJointModel>();
            Shifts = new List<Shift>();
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
