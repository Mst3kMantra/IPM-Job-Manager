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
        private ObservableCollection<Job> _assignedJobList = new ObservableCollection<Job>();
        public ObservableCollection<Job> AssignedJobList
        {
            get { return _assignedJobList; }
            set { _assignedJobList = value; }
        }
        private ObservableCollection<Job> _jobNotes = new ObservableCollection<Job>();
        public ObservableCollection<Job> JobNotes
        {
            get { return _jobNotes; }
            set { _jobNotes = value; }
        }

        public OpTimeWindow(Job selectedJob, TextBlock Time)
        {
            InitializeComponent();
            MainWin = Application.Current.MainWindow as MainWindow;
            SelectedJob = selectedJob;
            SelectedTime = Time;
            txtHours.Text = "0";
            txtMinutes.Text = "0";
            txtSeconds.Text = "0";
            AssignedJobList = MainWin.ReadJobsJson(MainWin.AssignedJobListPath);
            JobNotes = MainWin.ReadJobsJson(MainWin.JobNotesPath);
        }

        public void WriteToNotes(Job ChangedJob)
        {
            bool isPartInJobNotes = false;
            if (JobNotes != null)
            {
                if (JobNotes.Count > 0)
                {
                    foreach (Job job in JobNotes)
                    {
                        if (job.JobInfo["PartNo"] == ChangedJob.JobInfo["PartNo"])
                        {
                            job.JobInfo["OperationTime"] = ChangedJob.JobInfo["OperationTime"];
                            MainWin.WriteJobsJson(JobNotes, MainWin.JobNotesPath);
                            isPartInJobNotes = true;
                            break;
                        }
                    }
                    if (isPartInJobNotes == false)
                    {
                        Job newNotes = new Job();
                        newNotes.JobInfo.Add("Operations", ChangedJob.JobInfo["Operations"]);
                        newNotes.JobInfo.Add("OperationTime", ChangedJob.JobInfo["OperationTime"]);
                        newNotes.JobInfo.Add("PartNo", ChangedJob.JobInfo["PartNo"]);
                        JobNotes.Add(newNotes);
                        MainWin.WriteJobsJson(JobNotes, MainWin.JobNotesPath);
                    }
                }
                else
                {
                    Job newNotes = new Job();
                    newNotes.JobInfo.Add("Operations", ChangedJob.JobInfo["Operations"]);
                    newNotes.JobInfo.Add("OperationTime", ChangedJob.JobInfo["OperationTime"]);
                    newNotes.JobInfo.Add("PartNo", ChangedJob.JobInfo["PartNo"]);
                    JobNotes.Add(newNotes);
                    MainWin.WriteJobsJson(JobNotes, MainWin.JobNotesPath);
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if ((!string.IsNullOrWhiteSpace(txtHours.Text)) && (!string.IsNullOrWhiteSpace(txtMinutes.Text)) && (!string.IsNullOrWhiteSpace(txtSeconds.Text))
                && txtHours.Text.All(c => char.IsDigit(c)) && txtMinutes.Text.All(c => char.IsDigit(c)) && txtSeconds.Text.All(c => char.IsDigit(c)))
            {
                int JobIndex = 0;
                foreach (Job job in AssignedJobList)
                {
                    if (job.JobInfo["JobNo"] == SelectedJob.JobInfo["JobNo"])
                    {
                        int NewHrs = int.Parse(txtHours.Text, System.Globalization.NumberStyles.AllowLeadingWhite | System.Globalization.NumberStyles.AllowTrailingWhite);
                        int NewMins = int.Parse(txtMinutes.Text, System.Globalization.NumberStyles.AllowLeadingWhite | System.Globalization.NumberStyles.AllowTrailingWhite);
                        int NewSecs = int.Parse(txtSeconds.Text, System.Globalization.NumberStyles.AllowLeadingWhite | System.Globalization.NumberStyles.AllowTrailingWhite);

                        int TotalTime = 60 * (NewHrs * 60 + NewMins) + NewSecs;
                        
                        job.JobInfo["OperationTime"][SelectedTime.Tag.ToString()] = TotalTime;
                        JobIndex = AssignedJobList.IndexOf(job);
                        break;
                    }
                }
                MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                WriteToNotes(AssignedJobList[JobIndex]);
                DialogResult = true;
                //Add jobnotes writing also.
            }
        }
    }
}
