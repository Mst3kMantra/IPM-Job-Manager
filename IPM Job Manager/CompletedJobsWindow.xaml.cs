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
using System.IO;

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
        public MainWindow MainWin;
        public Job SelectedJob;
        public CompletedJobsWindow()
        {
            InitializeComponent();
            MainWin = Application.Current.MainWindow as MainWindow;
            this.DataContext = this;
            try
            {
                CompletedJobs = MainWin.ReadJobsJson(MainWin.CompletedJobsPath);
            }
            catch (FileNotFoundException)
            {
                this.Close();
                return;
            }
        }

        private void btnRemoveJob_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedJob != null)
            {
                foreach (Job job in CompletedJobs)
                {
                    if (job.JobInfo["JobNo"] == SelectedJob.JobInfo["JobNo"])
                    {
                        CompletedJobs.Remove(job);
                        break;
                    }
                }
                MainWin.WriteJobsJson(CompletedJobs, MainWin.CompletedJobsPath);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void lstCompletedJobs_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectedJob = lstCompletedJobs.SelectedItem as Job;
        }
    }
}
