using Hospital.Commands;
using Hospital.Managers;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Hospital.ViewModels
{
    class MedicalRecordDetailsViewModel
    {
        private DocumentManagerModel _documentManager;
        public MedicalRecordJointModel MedicalRecord { get; private set; }
        public ICommand downloadDocuments { get; private set; }
        public ObservableCollection<Document> Documents { get; private set; }

        // list of document.file
        public string DocumentsDisplay => string.Join(", ", ListDocuments());

        private List<String> ListDocuments()
        {
            List<String> documentList = new List<String>();
            foreach (Document document in Documents)
            {
                documentList.Add(document.File);
            }
            return documentList;
        }

        public MedicalRecordDetailsViewModel(MedicalRecordJointModel medicalRecord, DocumentManagerModel documentManager)
        {
            MedicalRecord = medicalRecord;
            _documentManager = documentManager;
            _documentManager.LoadDocuments(MedicalRecord.MedicalRecordId);
            Documents = new ObservableCollection<Document>(_documentManager.s_documentList);
            downloadDocuments = new RelayCommand(DownloadDocuments);
        }

        public void DownloadDocuments(Object obj)
        {
            if (obj is List<Document> documents)
            {
                foreach (Document document in documents)
                {
                    Console.WriteLine($"Downloading document: {document.File}");
                }
            }
        }

        public void OnDownloadButtonClicked()
        {
            // Download the documents of the medical record
        }
    }
}
