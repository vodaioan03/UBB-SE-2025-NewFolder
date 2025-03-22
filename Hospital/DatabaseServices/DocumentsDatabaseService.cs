using Hospital.Configs;
using Hospital.Exceptions;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Document = Hospital.Models.Document;

namespace Hospital.DatabaseServices
{
	public class DocumentDatabaseService
	{
		private readonly Config _config;

		public DocumentDatabaseService()
		{
			_config = Config.GetInstance();
		}

		public async Task<bool> UploadDocumentToDB(Document document)
		{
			const string queryUploadDocument =
			  "INSERT INTO Documents (MedicalRecordId, Files) " +
			  "VALUES (@MedicalRecordId, @Files)";
			
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
				command.Parameters.AddWithValue("@Files", document.Files);

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

        public async Task<List<Document>> GetDocumentsByMedicalRecordId(int medicalRecordId)
        {
            const string queryGetDocumentByMedicalRecordId =
                "SELECT * FROM Documents WHERE MedicalRecordId = @MedicalRecordId";
            try
            {
                using var connection = new SqlConnection(_config.DatabaseConnection);
                // Open the database connection asynchronously
                await connection.OpenAsync().ConfigureAwait(false);
                Console.WriteLine("Connection established successfully.");
                // Create a command to execute the SQL query
                using var command = new SqlCommand(queryGetDocumentByMedicalRecordId, connection);
                // Add the parameters to the query with values from the appointment object
                command.Parameters.AddWithValue("@MedicalRecordId", medicalRecordId);
                // Execute the query asynchronously and read the result
                using var reader = await command.ExecuteReaderAsync().ConfigureAwait(false);
                // Create a list to hold the documents
                List<Document> documents = new List<Document>();
                // Read all rows
                while (await reader.ReadAsync().ConfigureAwait(false))
                {
                    // Create a new Document object with the values from the row
                    Document document = new Document(
                        reader.GetInt32(0),
                        reader.GetInt32(1),
                        reader.GetString(2)
                    );
                    // Add the document to the list
                    documents.Add(document);
                }
                // Close DB Connection
                connection.Close();

                // Return the list of Document objects
                return documents;
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
        }
    }
}
