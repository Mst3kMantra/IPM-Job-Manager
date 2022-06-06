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
    /// Interaction logic for OpTimeWindow.xaml
    /// </summary>
    public partial class OpTimeWindow : Window
    {
        public MainWindow MainWin;
        public Job SelectedJob;
        public TextBlock SelectedTime;
        public ObservableCollection<Job> AssignedJobList;

        public OpTimeWindow(Job selectedJob, TextBlock Time)
        {
            InitializeComponent();
            MainWin = Owner as MainWindow;
            SelectedJob = selectedJob;
            SelectedTime = Time;
            txtHours.Text = "0";
            txtMinutes.Text = "0";
            txtSeconds.Text = "0";
            AssignedJobList = MainWin.AssignedJobList;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if ((!string.IsNullOrWhiteSpace(txtHours.Text)) && (!string.IsNullOrWhiteSpace(txtMinutes.Text)) && (!string.IsNullOrWhiteSpace(txtSeconds.Text))
                && txtHours.Text.All(c => char.IsDigit(c)) && txtMinutes.Text.All(c => char.IsDigit(c)) && txtSeconds.Text.All(c => char.IsDigit(c)))
            {
                foreach (Job job in AssignedJobList)
                {
                    if (job.JobInfo["JobNo"] == SelectedJob.JobInfo["JobNo"])
                    {
                        int NewHrs = int.Parse(txtHours.Text, System.Globalization.NumberStyles.AllowLeadingWhite | System.Globalization.NumberStyles.AllowTrailingWhite);
                        int NewMins = int.Parse(txtMinutes.Text, System.Globalization.NumberStyles.AllowLeadingWhite | System.Globalization.NumberStyles.AllowTrailingWhite);
                        int NewSecs = int.Parse(txtSeconds.Text, System.Globalization.NumberStyles.AllowLeadingWhite | System.Globalization.NumberStyles.AllowTrailingWhite);

                        int TotalTime = 60 * (NewHrs * 60 + NewMins) + NewSecs;
                        
                        job.JobInfo["OperationTime"][SelectedTime.Tag.ToString()] = TotalTime;
                        break;
                    }
                }
                MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
            }
        }
    }
}
