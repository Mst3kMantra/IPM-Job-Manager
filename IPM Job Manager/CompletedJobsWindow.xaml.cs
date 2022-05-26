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
using System.Collections.ObjectModel;

namespace IPM_Job_Manager_net
{
    /// <summary>
    /// Interaction logic for CompletedJobsWindow.xaml
    /// </summary>
    public partial class CompletedJobsWindow : Window
    {
        private ObservableCollection<Job> _completedJobs = new ObservableCollection<Job>();
        public ObservableCollection<Job> CompletedJobs
        {
            get { return _completedJobs; }
            set { _completedJobs = value; }
        }
        MainWindow MainWin;
        public CompletedJobsWindow()
        {
            InitializeComponent();
            MainWin = Application.Current.MainWindow as MainWindow;
            CompletedJobs = MainWin.ReadJobsJson(MainWin.CompletedJobsPath);
        }
    }
}
