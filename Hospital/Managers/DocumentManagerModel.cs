//using ABI.System;
using Hospital.DatabaseServices;
using Hospital.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Document = Hospital.Models.Document;


namespace Hospital.Managers
{
    public class DocumentManagerModel
    {
        public List<Document> s_documentList { get; private set; }
        private readonly DocumentDatabaseService _documentDBService;

        public DocumentManagerModel(DocumentDatabaseService dbService)
        {
            _documentDBService = dbService;
            s_documentList = new List<Document>();
        }

        public async Task<List<Document>> GetDocuments()
        {
            return s_documentList;
        }

        public async Task AddDocumentToMedicalRecord(Document document)
        {
            try
            {
                bool success = await _documentDBService.UploadDocumentToDB(document).ConfigureAwait(false);
                if (success)
                {
                    s_documentList.Add(document);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error adding document: {ex.Message}");
            }
        }

        public async Task LoadDocuments(int MedicalRecordId)
        {
            try
            {
                s_documentList = _documentDBService.GetDocumentsByMedicalRecordId(MedicalRecordId).Result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading documents: {ex.Message}");
            }
        }

    }
}
