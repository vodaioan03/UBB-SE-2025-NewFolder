using Hospital.ViewModels;
using Microsoft.UI;
using Microsoft.UI.Windowing;
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
            this.AppWindow.Resize(new(800, 600));
            this.StyleTitleBar();

            _viewModel = medicalRecordsHistoryViewModel;
            this.MedicalRecordsPanel.DataContext = _viewModel;
        }

        private void StyleTitleBar()
        {

            // Get the title bar of the app window.
            AppWindow m_Window = this.AppWindow;
            AppWindowTitleBar m_TitleBar = m_Window.TitleBar;

            // Set title bar colors.
            m_TitleBar.ForegroundColor = Colors.White;
            m_TitleBar.BackgroundColor = Colors.Green;

            // Set button colors.
            m_TitleBar.ButtonForegroundColor = Colors.White;
            m_TitleBar.ButtonBackgroundColor = Colors.SeaGreen;

            // Set button hover colors.
            m_TitleBar.ButtonHoverForegroundColor = Colors.Gainsboro;
            m_TitleBar.ButtonHoverBackgroundColor = Colors.DarkSeaGreen;
            m_TitleBar.ButtonPressedForegroundColor = Colors.Gray;
            m_TitleBar.ButtonPressedBackgroundColor = Colors.LightGreen;

            // Set inactive window colors.
            // Note: No effect when app is running on Windows 10
            // because color customization is not supported.
            m_TitleBar.InactiveForegroundColor = Colors.Gainsboro;
            m_TitleBar.InactiveBackgroundColor = Colors.SeaGreen;
            m_TitleBar.ButtonInactiveForegroundColor = Colors.Gainsboro;
            m_TitleBar.ButtonInactiveBackgroundColor = Colors.SeaGreen;


            /*arrivalCalendarDatePicker.MinDate = DateTime.Today;
            arrivalCalendarDatePicker.Date = DateTime.Today;
            arrivalCalendarDatePicker.MaxDate = DateTime.Today.AddMonths(1);*/

        }
    }
}