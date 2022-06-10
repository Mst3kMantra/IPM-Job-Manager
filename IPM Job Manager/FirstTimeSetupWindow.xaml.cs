using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Ookii.Dialogs;

namespace IPM_Job_Manager_net
{
    /// <summary>
    /// Interaction logic for FirstTimeSetupWindow.xaml
    /// </summary>
    public partial class FirstTimeSetupWindow : Window
    {
        public FirstTimeSetupWindow()
        {
            InitializeComponent();
        }

        private void btnFindFolder_Click(object sender, RoutedEventArgs e)
        {
            Ookii.Dialogs.Wpf.VistaFolderBrowserDialog dlg = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            bool? DialogResult = dlg.ShowDialog();
            if (DialogResult == true)
            {
                txtPath.Text = dlg.SelectedPath;
            }
        }

        private void btnFinish_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtPath.Text))
            {
                if (chkDemoMode.IsChecked == true)
                {
                    Properties.Settings.Default.isInDemoMode = true;
                }
                Properties.Settings.Default.DataFileDirectory = txtPath.Text;
                Properties.Settings.Default.Save();
                DialogResult = true;
            }
        }
    }
}
