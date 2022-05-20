using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace IPM_Job_Manager_net
{
    /// <summary>
    /// Interaction logic for JobDetailsWindow.xaml
    /// </summary>
    public partial class JobDetailsWindow : Window
    {
        private Job _displayedJob = new Job();
        public Job DisplayedJob
        {
            get { return _displayedJob; }
            set { _displayedJob = value; }
        }

        public AdminWindow adminWindow;

        public MainWindow mainWindow;


        public JobDetailsWindow(Job SelectedJob)
        {
            InitializeComponent();
            DisplayedJob = SelectedJob;
            DataContext = this;
        }

        
        private void btnEditOperations_Click(object sender, RoutedEventArgs e)
        {
            if (Owner.Title == "IPM Job Manager")
            {
                adminWindow = Owner as AdminWindow;
                Window OpWin = new OperationsWindow(DisplayedJob);
                OpWin.Owner = adminWindow;
                OpWin.ShowDialog();
            }
            else if (Owner.Title == "IPM Job Viewer")
            {
                mainWindow = Owner as MainWindow;
                Window OpWin = new OperationsWindow(DisplayedJob);
                OpWin.Owner = mainWindow;
                OpWin.ShowDialog();
            }
            
        }
    }
}
