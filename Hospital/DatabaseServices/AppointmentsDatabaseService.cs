using Hospital.Configs;
using Hospital.Models;
using Hospital.Exceptions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
    class AppointmentsDatabaseService
    {
        private readonly Config _config;

        public AppointmentsDatabaseService()
        {
            _config = Config.GetInstance();
        }

        public async Task<bool> AddAppointmentToDB(Appointment appointment)
        {
            const string query =
              "INSERT INTO Appointments (PatientId, DoctorId, DateAndTime, ProcedureId, Finished) " +
              "VALUES (@PatientId, @DoctorId, @DateAndTime, @ProcedureId, @Finished)";

            try
            {
                using SqlConnection connection = new SqlConnection(_config.DatabaseConnection);

                // Open the database connection asynchronously
                await connection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                // Create a command to execute the SQL query
                using SqlCommand command = new SqlCommand(query, connection);

                // Add the parameters to the query with values from the appointment object
                command.Parameters.AddWithValue("@PatientId", appointment.PatientId);
                command.Parameters.AddWithValue("@DoctorId", appointment.DoctorId);
                command.Parameters.AddWithValue("@DateAndTime", appointment.DateAndTime);
                command.Parameters.AddWithValue("@ProcedureId", appointment.ProcedureId);
                command.Parameters.AddWithValue("@Finished", appointment.Finished);

                // Execute the query asynchronously and check how many rows were affected
                int rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

                connection.Close();

                // If at least one row was affected, the insert was successful
                return rowsAffected > 0;
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"SQL Error: {sqlException.Message}");
                return false;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"General Error: {exception.Message}");
                return false;
            }
        }

        public async Task<List<AppointmentJointModel>> GetAppointments()
        {
            const string query = @"SELECT 
                    a.AppointmentId,
                    a.Finished,
                    a.DateAndTime,
                    d.DepartmentId,
                    d.DepartmentName,
                    doc.DoctorId,
                    u1.Name as DoctorName,
                    p.PatientId,
                    u2.Name as PatientName,
                    pr.ProcedureId,
                    pr.ProcedureName,
                    pr.ProcedureDuration
                FROM Appointments a
                JOIN Doctors doc ON a.DoctorId = doc.DoctorId
                JOIN Users u1 ON doc.UserId = u1.UserId
                JOIN Departments d ON doc.DepartmentId = d.DepartmentId
                JOIN Patients p ON a.PatientId = p.PatientId
                JOIN Users u2 ON p.UserId = u2.UserId
                JOIN Procedures pr ON a.ProcedureId = pr.ProcedureId
                ORDER BY a.AppointmentId;";

            using DataTable dt = new DataTable();

            try
            {
                using SqlConnection connection = new SqlConnection(_config.DatabaseConnection);
                // Open the database connection asynchronously.
                await connection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                // Create a command to execute the SQL query.
                using SqlCommand command = new SqlCommand(query, connection);

                // Execute the command and obtain a SqlDataReader.
                using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                // Load the results into a DataTable.
                await Task.Run(() => dt.Load(reader)).ConfigureAwait(false);

                // Create a list to store the AppointmentJointModel objects.
                List<AppointmentJointModel> appointments = new List<AppointmentJointModel>();
                foreach (DataRow row in dt.Rows)
                {
                    appointments.Add(new AppointmentJointModel(
                      (int)row["AppointmentId"],
                      (bool)row["Finished"],
                      (DateTime)row["DateAndTime"],
                      (int)row["DepartmentId"],
                      (string)row["DepartmentName"],
                      (int)row["DoctorId"],
                      (string)row["DoctorName"],
                      (int)row["PatientId"],
                      (string)row["PatientName"],
                      (int)row["ProcedureId"],
                      (string)row["ProcedureName"],
                      (TimeSpan)row["ProcedureDuration"]
                    ));
                }

                connection.Close();

                return appointments;
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"SQL Error: {sqlException.Message}");
                return new List<AppointmentJointModel>();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"General Error: {exception.Message}");
                return new List<AppointmentJointModel>();
            }
        }

        public async Task<List<AppointmentJointModel>> GetAppointmentsForPatient(int patientId)
        {
            const string query = @"SELECT 
                    a.AppointmentId,
                    a.Finished,
                    a.DateAndTime,
                    d.DepartmentId,
                    d.DepartmentName,
                    doc.DoctorId,
                    u1.Name as DoctorName,
                    p.PatientId,
                    u2.Name as PatientName,
                    pr.ProcedureId,
                    pr.ProcedureName,
                    pr.ProcedureDuration
                FROM Appointments a
                JOIN Doctors doc ON a.DoctorId = doc.DoctorId
                JOIN Users u1 ON doc.UserId = u1.UserId
                JOIN Departments d ON doc.DepartmentId = d.DepartmentId
                JOIN Patients p ON a.PatientId = p.PatientId
                JOIN Users u2 ON p.UserId = u2.UserId
                JOIN Procedures pr ON a.ProcedureId = pr.ProcedureId
                WHERE p.PatientId = @PatientId
                ORDER BY a.DateAndTime;";

            using DataTable dt = new DataTable();

            try
            {
                using SqlConnection connection = new SqlConnection(_config.DatabaseConnection);
                await connection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine($"Connected to DB. Fetching appointments for Patient ID: {patientId}");

                using SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@PatientId", patientId);

                using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                await Task.Run(() => dt.Load(reader)).ConfigureAwait(false);

                List<AppointmentJointModel> appointments = new List<AppointmentJointModel>();
                Console.WriteLine($"Rows returned: {dt.Rows.Count}");

                foreach (DataRow row in dt.Rows)
                {
                    var appointment = new AppointmentJointModel(
                        (int)row["AppointmentId"],
                        (bool)row["Finished"],
                        (DateTime)row["DateAndTime"],
                        (int)row["DepartmentId"],
                        (string)row["DepartmentName"],
                        (int)row["DoctorId"],
                        (string)row["DoctorName"],
                        (int)row["PatientId"],
                        (string)row["PatientName"],
                        (int)row["ProcedureId"],
                        (string)row["ProcedureName"],
                        (TimeSpan)row["ProcedureDuration"]
                    );

                    Console.WriteLine($"Appointment found: {appointment.AppointmentId} - {appointment.Date}");
                    appointments.Add(appointment);
                }

                return appointments;
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"SQL Error: {sqlException.Message}");
                return new List<AppointmentJointModel>();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"General Error: {exception.Message}");
                return new List<AppointmentJointModel>();
            }
        }

        public async Task<List<AppointmentJointModel>> GetAppointmentsByDoctorAndDate(int doctorId, DateTime date)
        {
            const string query = @"SELECT 
                    a.AppointmentId,
                    a.Finished,
                    a.DateAndTime,
                    d.DepartmentId,
                    d.DepartmentName,
                    doc.DoctorId,
                    u1.Name as DoctorName,
                    p.PatientId,
                    u2.Name as PatientName,
                    pr.ProcedureId,
                    pr.ProcedureName,
                    pr.ProcedureDuration
                FROM Appointments a
                JOIN Doctors doc ON a.DoctorId = doc.DoctorId
                JOIN Users u1 ON doc.UserId = u1.UserId
                JOIN Departments d ON doc.DepartmentId = d.DepartmentId
                JOIN Patients p ON a.PatientId = p.PatientId
                JOIN Users u2 ON p.UserId = u2.UserId
                JOIN Procedures pr ON a.ProcedureId = pr.ProcedureId
                WHERE a.DoctorId = @DoctorId
                  AND CONVERT(DATE, a.DateAndTime) = @Date
                ORDER BY a.DateAndTime;";

            using DataTable dt = new DataTable();

            try
            {
                using SqlConnection connection = new SqlConnection(_config.DatabaseConnection);

                // Open the database connection asynchronously
                await connection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                // Create a command to execute the SQL query
                using SqlCommand command = new SqlCommand(query, connection);

                // Add parameters for filtering by doctor and date
                command.Parameters.AddWithValue("@DoctorId", doctorId);
                command.Parameters.AddWithValue("@Date", date.Date);

                // Get the results from running the command
                using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                // Load the results into the DataTable
                await Task.Run(() => dt.Load(reader)).ConfigureAwait(false);

                // Create a list to store the AppointmentJointModel objects
                List<AppointmentJointModel> appointments = new List<AppointmentJointModel>();
                foreach (DataRow row in dt.Rows)
                {
                    appointments.Add(new AppointmentJointModel(
                      (int)row["AppointmentId"],
                      (bool)row["Finished"],
                      (DateTime)row["DateAndTime"],
                      (int)row["DepartmentId"],
                      (string)row["DepartmentName"],
                      (int)row["DoctorId"],
                      (string)row["DoctorName"],
                      (int)row["PatientId"],
                      (string)row["PatientName"],
                      (int)row["ProcedureId"],
                      (string)row["ProcedureName"],
                      (TimeSpan)row["ProcedureDuration"]
                    ));
                }

                connection.Close();

                return appointments;
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"SQL Error: {sqlException.Message}");
                return new List<AppointmentJointModel>();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"General Error: {exception.Message}");
                return new List<AppointmentJointModel>();
            }
        }

        public async Task<List<AppointmentJointModel>> GetAppointmentsForDoctor(int doctorId)
        {
            const string query = @"SELECT 
                    a.AppointmentId,
                    a.Finished,
                    a.DateAndTime,
                    d.DepartmentId,
                    d.DepartmentName,
                    doc.DoctorId,
                    u1.Name as DoctorName,
                    p.PatientId,
                    u2.Name as PatientName,
                    pr.ProcedureId,
                    pr.ProcedureName,
                    pr.ProcedureDuration
                FROM Appointments a
                JOIN Doctors doc ON a.DoctorId = doc.DoctorId
                JOIN Users u1 ON doc.UserId = u1.UserId
                JOIN Departments d ON doc.DepartmentId = d.DepartmentId
                JOIN Patients p ON a.PatientId = p.PatientId
                JOIN Users u2 ON p.UserId = u2.UserId
                JOIN Procedures pr ON a.ProcedureId = pr.ProcedureId
                WHERE a.DoctorId = @DoctorId
                ORDER BY a.DateAndTime;";

            using DataTable dt = new DataTable();

            try
            {
                using SqlConnection connection = new SqlConnection(_config.DatabaseConnection);

                // Open the database connection asynchronously
                await connection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");

                // Create a command to execute the SQL query
                using SqlCommand command = new SqlCommand(query, connection);

                // Add parameters for filtering by doctor
                command.Parameters.AddWithValue("@DoctorId", doctorId);

                // Get the results from running the command
                using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);

                // Load the results into the DataTable
                await Task.Run(() => dt.Load(reader)).ConfigureAwait(false);

                // Create a list to store the AppointmentJointModel objects
                List<AppointmentJointModel> appointments = new List<AppointmentJointModel>();
                foreach (DataRow row in dt.Rows)
                {
                    appointments.Add(new AppointmentJointModel(
                      (int)row["AppointmentId"],
                      (bool)row["Finished"],
                      (DateTime)row["DateAndTime"],
                      (int)row["DepartmentId"],
                      (string)row["DepartmentName"],
                      (int)row["DoctorId"],
                      (string)row["DoctorName"],
                      (int)row["PatientId"],
                      (string)row["PatientName"],
                      (int)row["ProcedureId"],
                      (string)row["ProcedureName"],
                      (TimeSpan)row["ProcedureDuration"]
                    ));
                }

                connection.Close();

                return appointments;
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"SQL Error: {sqlException.Message}");
                return new List<AppointmentJointModel>();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"General Error: {exception.Message}");
                return new List<AppointmentJointModel>();
            }
        }

        public async Task<AppointmentJointModel> GetAppointment(int appointmentId)
        {
            string GetAppointmentByAppointmentIdQuery = @"SELECT 
                    a.AppointmentId,
                    a.Finished,
                    a.DateAndTime,
                    d.DepartmentId,
                    d.DepartmentName,
                    doc.DoctorId,
                    u1.Name as DoctorName,
                    p.PatientId,
                    u2.Name as PatientName,
                    pr.ProcedureId,
                    pr.ProcedureName,
                    pr.ProcedureDuration
                FROM Appointments a
                JOIN Doctors doc ON a.DoctorId = doc.DoctorId
                JOIN Users u1 ON doc.UserId = u1.UserId
                JOIN Departments d ON doc.DepartmentId = d.DepartmentId
                JOIN Patients p ON a.PatientId = p.PatientId
                JOIN Users u2 ON p.UserId = u2.UserId
                JOIN Procedures pr ON a.ProcedureId = pr.ProcedureId
                WHERE a.AppointmentId = @AppointmentId;";

            using DataTable dt = new DataTable();
            try
            {
                using SqlConnection connection = new SqlConnection(_config.DatabaseConnection);
                await connection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");
                using SqlCommand command = new SqlCommand(GetAppointmentByAppointmentIdQuery, connection);
                command.Parameters.AddWithValue("@AppointmentId", appointmentId);
                using SqlDataReader reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                await Task.Run(() => dt.Load(reader)).ConfigureAwait(false);
                AppointmentJointModel appointment = new AppointmentJointModel(
                    (int)dt.Rows[0]["AppointmentId"],
                    (bool)dt.Rows[0]["Finished"],
                    (DateTime)dt.Rows[0]["DateAndTime"],
                    (int)dt.Rows[0]["DepartmentId"],
                    (string)dt.Rows[0]["DepartmentName"],
                    (int)dt.Rows[0]["DoctorId"],
                    (string)dt.Rows[0]["DoctorName"],
                    (int)dt.Rows[0]["PatientId"],
                    (string)dt.Rows[0]["PatientName"],
                    (int)dt.Rows[0]["ProcedureId"],
                    (string)dt.Rows[0]["ProcedureName"],
                    (TimeSpan)dt.Rows[0]["ProcedureDuration"]
                );
                connection.Close();
                return appointment;
            }
            catch (SqlException sqlException)
            {
                Console.WriteLine($"SQL Error: {sqlException.Message}");
                return null;
            }
            catch (Exception exception)
            {
                Console.WriteLine($"General Error: {exception.Message}");
                return null;

            }

            throw new AppointmentNotFoundException($"Appointment not found for id {appointmentId}");
        }
        public async Task<bool> RemoveAppointmentFromDB(int appointmentId)
        {
            try
            {
                Console.WriteLine($"Checking if appointment ID {appointmentId} exists before deletion...");

                const string checkQuery = "SELECT COUNT(*) FROM Appointments WHERE AppointmentId = @AppointmentId";
                using SqlConnection connection = new SqlConnection(_config.DatabaseConnection);
                await connection.OpenAsync().ConfigureAwait(false);

                using SqlCommand checkCommand = new SqlCommand(checkQuery, connection);
                checkCommand.Parameters.AddWithValue("@AppointmentId", appointmentId);

                int count = (int)await checkCommand.ExecuteScalarAsync().ConfigureAwait(false);
                if (count == 0)
                {
                    Console.WriteLine($"Appointment ID {appointmentId} does NOT exist in DB. Throwing exception.");
                    throw new AppointmentNotFoundException($"Appointment {appointmentId} not found.");
                }

                Console.WriteLine($"Appointment ID {appointmentId} exists. Proceeding with deletion.");

                const string deleteQuery = "DELETE FROM Appointments WHERE AppointmentId = @AppointmentId";
                using SqlCommand deleteCommand = new SqlCommand(deleteQuery, connection);
                deleteCommand.Parameters.AddWithValue("@AppointmentId", appointmentId);

                int rowsAffected = await deleteCommand.ExecuteNonQueryAsync().ConfigureAwait(false);

                if (rowsAffected > 0)
                {
                    Console.WriteLine($"Successfully deleted appointment ID {appointmentId}.");
                    return true;
                }
                else
                {
                    Console.WriteLine($"Deletion failed for appointment ID {appointmentId}. No rows affected.");
                    return false;
                }
            }
            catch (SqlException ex)
            {
                Console.WriteLine($"SQL Error while deleting appointment: {ex.Message}");
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"General Error while deleting appointment: {ex.Message}");
                return false;
            }
        }



    }
}
