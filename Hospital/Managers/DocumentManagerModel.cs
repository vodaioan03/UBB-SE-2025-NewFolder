//using ABI.System;
using Hospital.DatabaseServices;
using Hospital.Exceptions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Document = Hospital.Models.Document;


namespace Hospital.Managers
{
    class DocumentManagerModel
    {
        public static ObservableCollection<Document> s_documentList { get; private set; }
        private readonly DocumentDatabaseService _documentDBService;

        public DocumentManagerModel(DocumentDatabaseService dbService)
        {
            _documentDBService = dbService;
            s_documentList = new ObservableCollection<Document>();
        }

        public async Task<ObservableCollection<Document>> GetDocuments()
        {
            if (s_documentList.Count == 0)
            {
                throw new DocumentNotFoundException("No documents found.");
            }
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
                List<Document> documents = await _documentDBService.GetDocumentsByMedicalRecordId(MedicalRecordId).ConfigureAwait(false);
                s_documentList = new ObservableCollection<Document>(documents);
            }
            catch (DocumentNotFoundException)
            {
                throw new DocumentNotFoundException("No documents found for the medical record with the given ID.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading documents: {ex.Message}");
            }
        }

    }
}
