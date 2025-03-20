using Hospital.Commands;
using Hospital.DatabaseServices;
using Hospital.Exceptions;
using Hospital.Managers;
using Hospital.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Windows.UI.Popups;

namespace Hospital.ViewModels
{
    class AppointmentCreationFormViewModel
    {
        // List Properties
        public ObservableCollection<Department>? DepartmentsList { get; set; }
        public ObservableCollection<Procedure>? ProceduresList { get; set; }
        public ObservableCollection<DoctorJointModel>? DoctorsList { get; set; }
        private List<Shift>? shiftsList { get; set; }
        private List<AppointmentJointModel>? AppointmentsList { get; set; }
        public ObservableCollection<string> HoursList { get; set; } = new ObservableCollection<string>();

        //Calendar Dates
        public DateTimeOffset MinDate { get; set; }
        public DateTimeOffset MaxDate { get; set; }

        //Manager Models
        private DepartmentManagerModel _departmentManager;
        private MedicalProcedureManagerModel _procedureManager;
        private DoctorManagerModel _doctorManager;
        private ShiftManagerModel _shiftManager;
        private AppointmentManagerModel _appointmentManager;

        //public event
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //selected fields
        public Department? SelectedDepartment { get; set; }
        public Procedure? SelectedProcedure { get; set; }
        public DoctorJointModel? SelectedDoctor { get; set; }
        public DateTime? SelectedDate { get; set; }
        public TimeSpan? SelectedTime { get; set; }

        private DateTimeOffset? _selectedCalendarDate;
        public DateTimeOffset? SelectedCalendarDate
        {
            get => _selectedCalendarDate;
            set
            {
                _selectedCalendarDate = value;
                OnPropertyChanged(nameof(SelectedCalendarDate));
            }
        }

        private string _selectedHour;
        public string SelectedHour
        {
            get => _selectedHour;
            set
            {
                _selectedHour = value;
                OnPropertyChanged(nameof(SelectedHour));
                // Optionally update SelectedTime when the hour changes:
                if (!string.IsNullOrWhiteSpace(_selectedHour))
                {
                    // Parse the string to a TimeSpan. Ensure the format matches (HH:mm)
                    SelectedTime = TimeSpan.Parse(_selectedHour);
                }
            }
        }

        // Feedback to the UI
        private string _bookingStatusMessage;
        public string BookingStatusMessage
        {
            get => _bookingStatusMessage;
            set
            {
                _bookingStatusMessage = value;
                OnPropertyChanged(nameof(BookingStatusMessage));
            }
        }

        //XAML Root
        public XamlRoot? Root { get; set; }

        public AppointmentCreationFormViewModel(DepartmentManagerModel departmentManagerModel, MedicalProcedureManagerModel procedureManagerModel, DoctorManagerModel doctorManagerModel, ShiftManagerModel shiftManagerModel, AppointmentManagerModel appointmentManagerModel)
        {
            _departmentManager = departmentManagerModel;
            _procedureManager = procedureManagerModel;
            _doctorManager = doctorManagerModel;
            _shiftManager = shiftManagerModel;
            _appointmentManager = appointmentManagerModel;

            //initialize lists
            DepartmentsList = new ObservableCollection<Department>();
            ProceduresList = new ObservableCollection<Procedure>();
            DoctorsList = new ObservableCollection<DoctorJointModel>();

            //load data
            LoadDepartments();

            //set calendar dates
            MinDate = DateTimeOffset.Now;
            MaxDate = MinDate.AddMonths(1);


            // HERE YOU WILL POPULATE WITH DOCTOR'S SHIFTS
            /*DateTime start = DateTime.Today.AddHours(8);
            DateTime end = DateTime.Today.AddHours(20);
            while (start <= end)
            {
                HoursList.Add(start.ToString("HH:mm"));
                start = start.AddMinutes(30);
            }*/
        }

        private async void LoadDepartments()
        {
            DepartmentsList.Clear();
            await _departmentManager.LoadDepartments();
            foreach(Department dept in _departmentManager.GetDepartments())
            {
                DepartmentsList?.Add(dept);
            }
        }

        public async void LoadProceduresAndDoctorsOfSelectedDepartment()
        {
            //clear the list
            ProceduresList.Clear();
            DoctorsList.Clear();

            //load the procedures
            await _procedureManager.LoadProceduresByDepartmentId(SelectedDepartment.DepartmentId);
            foreach (Procedure proc in _procedureManager.GetProcedures())
            {
                ProceduresList?.Add(proc);
            }

            //load the doctors
            await _doctorManager.LoadDoctors(SelectedDepartment.DepartmentId);
            foreach (DoctorJointModel doc in _doctorManager.GetDoctorsWithRatings())
            {
                DoctorsList?.Add(doc);
            }
        }

        public async void LoadAvailableTimeSlots()
        {
            //checl for all necessary fields
            if (SelectedDoctor == null || SelectedCalendarDate == null || SelectedProcedure == null)
            {
                return;
            }

            //load the shifts
            await _shiftManager.LoadShifts(SelectedDoctor.DoctorId);
            shiftsList = _shiftManager.GetShifts();

            //if there are no shifts return
            if (shiftsList == null)
            {
                HoursList.Clear();
                return;
            }

            //get shift for the selected date
            Shift shift;
            try
            {
                shift = _shiftManager.GetShiftByDay(SelectedCalendarDate.Value.Date);
            }
            catch (ShiftNotFoundException ex)
            {
                //if there is no shift for the selected date return
                HoursList.Clear();
                return;
            }

            //get appointments for the selected doctor on the selected date
            await _appointmentManager.LoadDoctorAppointmentsOnDate(SelectedDoctor.DoctorId, SelectedCalendarDate.Value.Date);

            //get the appointments
            AppointmentsList = _appointmentManager.GetAppointments();

            //compute available time slots
            List<string> availableTimeSlots = new List<string>();

            //get the start time
            TimeSpan start = shift.StartTime;
            TimeSpan end = shift.EndTime;

            // Round procedure duration to the nearest 30-minute multiple
            TimeSpan procedureDuration = SelectedProcedure.Duration;
            int totalMinutes = (int)procedureDuration.TotalMinutes;
            int roundedMinutes = (int)Math.Round(totalMinutes / 30.0) * 30; // Round to nearest multiple of 30
            procedureDuration = TimeSpan.FromMinutes(roundedMinutes); // Convert back to TimeSpan

            //generate the time slots
            TimeSpan currentTime = start;

            foreach (var appointment in AppointmentsList)
            {
                TimeSpan appointmentStartTime = appointment.Date.TimeOfDay;
                TimeSpan appointmentEndTime = appointmentStartTime.Add(appointment.ProcedureDuration);

                // Check for available slots before the next appointment starts
                while (currentTime + procedureDuration <= appointmentStartTime)
                {
                    availableTimeSlots.Add(currentTime.ToString(@"hh\:mm")); // Format as HH:mm
                    currentTime = currentTime.Add(procedureDuration); // Move to the next possible slot
                }

                // Move past the current appointment
                currentTime = appointmentEndTime;
            }

            // Check remaining time after the last appointment
            while (currentTime + procedureDuration <= end)
            {
                availableTimeSlots.Add(currentTime.ToString(@"hh\:mm"));
                currentTime = currentTime.Add(procedureDuration);
            }

            // Update the list of available time slots
            HoursList.Clear();
            foreach (string timeSlot in availableTimeSlots)
            {
                HoursList.Add(timeSlot);
            }
        }

        public async Task<bool> BookAppointment()
        {
            var date = SelectedCalendarDate.Value.Date; // e.g. 2025-04-01
            var time = TimeSpan.Parse(SelectedHour); // e.g. 14:00:00
            DateTime actualDateTime = date + time; // e.g. 2025-04-01 14:00:00

            // Create the new appointment
            var newAppointment = new Models.Appointment(
                0,                           // Appointment ID (0 so SQL Server auto-generates it)
                SelectedDoctor.DoctorId,
                1,                           // Patient ID (adjust as needed)
                actualDateTime,       
                false,                       // Finished (initially false)
                SelectedProcedure.ProcedureId
            );

            bool isCreated = await _appointmentManager.CreateAppointment(newAppointment);
            BookingStatusMessage = isCreated
                ? "Appointment created successfully!"
                : "Failed to create appointment. Please try again later.";

            return isCreated;
        }

        // Validate user input
        public bool ValidateAppointment()
        {
            bool isValid = !(SelectedDepartment == null || SelectedProcedure == null || SelectedDoctor == null ||
                SelectedCalendarDate == null || SelectedTime == null);

            return isValid;
        }
    }
}
