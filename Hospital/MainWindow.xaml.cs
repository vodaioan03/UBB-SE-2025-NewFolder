using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using Hospital.Views;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace Hospital
{
  /// <summary>
  /// An empty window that can be used on its own or navigated to within a Frame.
  /// </summary>
      public sealed partial class MainWindow : Window
      {
            public MainWindow()
            {
              this.InitializeComponent();
            }

            private void Patient1_Click(object sender, RoutedEventArgs e)
            {
                AppointmentCreationForm appointmentCreationForm = new AppointmentCreationForm();
                appointmentCreationForm.Activate();
            }

        private void Patient2_Click(object sender, RoutedEventArgs e)
        {
            //test ui of feature Patient2 here
        }

        private void Patient3_Click(object sender, RoutedEventArgs e)
        {
            //test ui of feature Patient3 here
        }

        private void Doctor1_Click(object sender, RoutedEventArgs e)
        {
            //test ui of feature Patient3 here
        }

        private void Doctor2_Click(object sender, RoutedEventArgs e)
        {
            //test ui of feature Patient3 here
        }
    }
}