using Hospital.Configs;
using Hospital.Models;
using Microsoft.Data.SqlClient;
using System;
using System.Threading.Tasks;

namespace Hospital.DatabaseServices
{
	class DocumentDatabaseService
	{
		private readonly Config _config;

		public DocumentDatabaseService(Config config)
		{
			_config = config;
		}

		public async Task<bool> UploadDocumentToDB(Document document )
		{
			const string queryUploadDocument =
			  "INSERT INTO Documents (MedicalRecordId, File) " +
			  "VALUES (@MedicalRecordId, @File)";
			
			try
			{
				using var connection = new SqlConnection(_config.DatabaseConnection);

				// Open the database connection asynchronously
				await connection.OpenAsync().ConfigureAwait(false);
				Console.WriteLine("Connection established successfully.");

				// Create a command to execute the SQL query
				using var command = new SqlCommand(queryUploadDocument, connection);

				// Add the parameters to the query with values from the appointment object
				command.Parameters.AddWithValue("@MedicalRecordId", document.MedicalRecordId);
				command.Parameters.AddWithValue("@File", document.File);

				// Execute the query asynchronously and check how many rows were affected
				int rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);

				// Close DB Connection
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
	}
}
