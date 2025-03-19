using Hospital.ViewModels;
using Microsoft.UI.Xaml;

namespace Hospital.Views
{
    /// <summary>
    /// A window where past medical records are listed for the logged in user.
    /// </summary>
    public sealed partial class MedicalRecordsHistoryView : Window
    {
        private MedicalRecordsHistoryViewModel _viewModel;

        public MedicalRecordsHistoryView(MedicalRecordsHistoryViewModel medicalRecordsHistoryViewModel)
        {
            this.InitializeComponent();
            _viewModel = medicalRecordsHistoryViewModel;
            this.MedicalRecordsPanel.DataContext = _viewModel;

            this.AppWindow.Resize(new(800, 600));
        }
    }
}