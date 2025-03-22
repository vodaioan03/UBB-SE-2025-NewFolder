using Hospital.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hospital.Managers;
using Hospital.Commands;
using System.ComponentModel;
using System.Diagnostics;

namespace Hospital.ViewModels
{
    public class MedicalRecordCreationFormViewModel
    {
        private readonly MedicalRecordManagerModel _medicalRecordManager;
        private readonly DocumentManagerModel _documentManager;

        private string _patientName;
        private string _doctorName;
        //private DateTime _appointmentDate;
        private string _appointmentTime;
        private string _department;
        private string _conclusion;
        public ObservableCollection<string> DocumentPaths { get; private set; } = new ObservableCollection<string>();

        public void AddDocument(string path)
        {
            DocumentPaths.Add(path);
        }

        private DateTime _appointmentDate;
        public DateTimeOffset? AppointmentDateOffset
        {
            get => new DateTimeOffset(_appointmentDate);
            set
            {
                if (value.HasValue)
                {
                    _appointmentDate = value.Value.DateTime;
                    OnPropertyChanged(nameof(AppointmentDateOffset));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        int maxDocs = 10;
        public ObservableCollection<string> Documents { get; private set; }


        public MedicalRecordCreationFormViewModel(MedicalRecordManagerModel medicalRecordManager, DocumentManagerModel documentManagerModel)
        {
            _medicalRecordManager = medicalRecordManager;
            _documentManager = documentManagerModel;
            Documents = new ObservableCollection<string>();
        }

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public async Task<int> CreateMedicalRecord(AppointmentJointModel detailedAppointment, string conclusion)
        {
           
            try
            {
                detailedAppointment.Finished = true;
                detailedAppointment.Date = DateTime.Now;
                

                int medicalRecordId = await _medicalRecordManager.CreateMedicalRecord(detailedAppointment, conclusion);
               
                return medicalRecordId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating medical record: {ex.Message}");
                return -1;
            }
        }

        public void AddDocument(int medicalRecordId, string path)
        {
            Document doc = new Document(0, medicalRecordId, path);
            if (Documents.Count < maxDocs)
            {
                Documents.Add(path);
                _ = _documentManager.AddDocumentToMedicalRecord(doc);
            }
        }


        public string PatientName
        {
            get => _patientName;
            set { _patientName = value; OnPropertyChanged(nameof(PatientName)); }
        }

        public string DoctorName
        {
            get => _doctorName;
            set { _doctorName = value; OnPropertyChanged(nameof(DoctorName)); }
        }

        public DateTime AppointmentDate
        {
            get => _appointmentDate;
            set { _appointmentDate = value; OnPropertyChanged(nameof(AppointmentDate)); }
        }

        public string AppointmentTime
        {
            get => _appointmentTime;
            set { _appointmentTime = value; OnPropertyChanged(nameof(AppointmentTime)); }
        }

        public string Department
        {
            get => _department;
            set { _department = value; OnPropertyChanged(nameof(Department)); }
        }

        public string Conclusion
        {
            get => _conclusion;
            set { _conclusion = value; OnPropertyChanged(nameof(Conclusion)); }
        }
    }
}
