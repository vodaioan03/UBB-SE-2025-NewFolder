using Hospital.Commands;
using Hospital.Exceptions;
using Hospital.Managers;
using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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

        public MedicalRecordDetailsViewModel(MedicalRecordJointModel medicalRecord, DocumentManagerModel documentManager)
        {
            MedicalRecord = medicalRecord;
            _documentManager = documentManager;
            _documentManager.LoadDocuments(MedicalRecord.MedicalRecordId).AsAsyncAction().Wait();
            Documents = new ObservableCollection<Document>(_documentManager.s_documentList);
            downloadDocuments = new RelayCommand(DownloadDocuments);
        }

        public void DownloadDocuments(Object obj)
        {
            Debug.WriteLine("Download button clicked");
            if (obj is List<Document> documents)
            {
                if (documents.Count == 0)
                {
                    throw new DocumentNotFoundException("No documents found");
                }
                foreach (Document document in documents)
                {
                    Debug.WriteLine($"\tDownloading document: {document.File}");
                }
            }
            else 
            {
                throw new FormatException("Invalid object type");
            }
        }

        public void OnDownloadButtonClicked()
        {
            downloadDocuments.Execute(Documents.ToList());
        }
    }
}
