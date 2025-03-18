using Hospital.DatabaseServices;
using Hospital.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


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
                s_documentList.Clear();
                foreach (var document in documents)
                {
                    s_documentList.Add(document);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading documents: {ex.Message}");
            }
        }

    }
}
