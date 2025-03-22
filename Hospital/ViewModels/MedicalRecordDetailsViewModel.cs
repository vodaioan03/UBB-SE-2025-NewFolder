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
        public ObservableCollection<Document> Documents { get; private set; }

        public MedicalRecordDetailsViewModel(MedicalRecordJointModel medicalRecord, DocumentManagerModel documentManager)
        {
            MedicalRecord = medicalRecord;
            _documentManager = documentManager;
            _documentManager.LoadDocuments(MedicalRecord.MedicalRecordId);
            Documents = new ObservableCollection<Document>(_documentManager.s_documentList);
        }

        public async Task OnDownloadButtonClicked()
        {
            await _documentManager.DownloadDocuments(MedicalRecord.PatientId);
        }

        public bool getDownloadButtonIsEnabled()
        {
            return Documents.Count > 0;
        }
    }
}
