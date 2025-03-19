using Hospital.DatabaseServices;
using Hospital.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Hospital.Exceptions;


namespace Hospital.Managers
{
    public class AppointmentManagerModel
    {
        public ObservableCollection<AppointmentJointModel> s_appointmentList { get; private set; }

        private readonly AppointmentsDatabaseService _appointmentsDBService;

        public AppointmentManagerModel(AppointmentsDatabaseService dbService)
        {
            _appointmentsDBService = dbService;
            s_appointmentList = new ObservableCollection<AppointmentJointModel>();
        }

        public async Task<ObservableCollection<AppointmentJointModel>> GetAppointments()
        {
          return s_appointmentList;
        }

        public async Task LoadDoctorAppointmentsOnDate(int doctorId, DateTime date)
        {
            try
            {
                List<AppointmentJointModel> appointments = await _appointmentsDBService
                    .GetAppointmentsByDoctorAndDate(doctorId, date)
                    .ConfigureAwait(false);
                s_appointmentList = new ObservableCollection<AppointmentJointModel>(appointments);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading doctor appointments: {ex.Message}");
                return;
            }
        }

        public async Task LoadAppointmentsForPatient(int patientId)
        {
            try
            {
                List<AppointmentJointModel> appointments = await _appointmentsDBService
                    .GetAppointmentsForPatient(patientId)
                    .ConfigureAwait(false);

                s_appointmentList = new ObservableCollection<AppointmentJointModel>(
                    appointments.Where(a => a.Date > DateTime.Now && !a.Finished)
                );
                

                s_appointmentList.Clear();
                foreach (AppointmentJointModel appointment in appointments)
                { 
                    s_appointmentList.Add(appointment);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading patient appointments: {ex.Message}");
                return;
            }
        }

        public async Task<bool> RemoveAppointment(int appointmentId)
        {
            try
            {
                AppointmentJointModel appointment = await _appointmentsDBService.GetAppointment(appointmentId);
                if (appointment == null)
                {
                    throw new AppointmentNotFoundException($"Appointment with ID {appointmentId} not found.");
                }

                if ((appointment.Date - DateTime.Now).TotalHours < 24)
                {
                    throw new CancellationNotAllowedException($"Appointment {appointmentId} is within 24 hours and cannot be canceled.");
                }

                if (!await _appointmentsDBService.RemoveAppointmentFromDB(appointmentId))
                {
                    throw new DatabaseOperationException($"Failed to cancel appointment {appointmentId} due to a database error.");
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task LoadAppointmentsForDoctor(int doctorId)
        {
            try
            {
                List<AppointmentJointModel> appointments =
                    await _appointmentsDBService.GetAppointmentsForDoctor(doctorId).ConfigureAwait(false);
                s_appointmentList.Clear();
                foreach (AppointmentJointModel appointment in appointments)
                {
                    s_appointmentList.Add(appointment);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error loading appointments for doctor {doctorId}: {ex.Message}");
            }
        }

        public async Task LoadAppointmentsByDoctorAndDate(int doctorId, DateTime date)
        {
            try
            {
                List<AppointmentJointModel> appointments = await _appointmentsDBService
                    .GetAppointmentsByDoctorAndDate(doctorId, date)
                    .ConfigureAwait(false);
                s_appointmentList.Clear();
                foreach (AppointmentJointModel appointment in appointments)
                {
                    s_appointmentList.Add(appointment);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading appointments: {ex.Message}");
                return;
            }
        }
        
        public async Task<bool> CreateAppointment(AppointmentJointModel detailedAppointment)
        {
            try
            {
                // Validate the doctor is available for the given time slot
                int doctorId = detailedAppointment.DoctorId;
                DateTime date = detailedAppointment.Date;
                List<AppointmentJointModel> existingAppointments = await _appointmentsDBService
                                                                    .GetAppointmentsByDoctorAndDate(doctorId, date)
                                                                    .ConfigureAwait(false);

                bool isSlotTaken = existingAppointments.Any(a => a.Date == detailedAppointment.Date);
                if(isSlotTaken)
                {
                    throw new AppointmentConflictException($"The selected time slot is already booked for doctor with id {doctorId}");
                }

                // Validate the patient doesn't have another appointment at the same time
                int patientId = detailedAppointment.PatientId;
                List<AppointmentJointModel> patientAppointments = await _appointmentsDBService.GetAppointmentsForPatient(patientId).ConfigureAwait(false);

                bool isPatientBusy = patientAppointments.Any(a => a.Date == detailedAppointment.Date);
                if(isPatientBusy)
                {
                    throw new AppointmentConflictException($"The patient with id {patientId} already has an appointment at the this time {detailedAppointment.Date}");
                }

                Appointment newAppointment = new Appointment(
                    detailedAppointment.AppointmentId, 
                    detailedAppointment.DoctorId, 
                    detailedAppointment.PatientId,
                    detailedAppointment.Date, 
                    detailedAppointment.Finished, 
                    detailedAppointment.ProcedureId
                );

                bool isInserted = await _appointmentsDBService.AddAppointmentToDB(newAppointment).ConfigureAwait(false);

                if(!isInserted)
                {
                    throw new DatabaseOperationException("Failed to save the appointment in the database");
                }

                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}