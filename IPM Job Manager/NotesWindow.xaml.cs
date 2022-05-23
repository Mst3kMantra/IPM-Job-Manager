using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    /// Interaction logic for NotesWindow.xaml
    /// </summary>
    public partial class NotesWindow : Window
    {
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

        private Job _selectedJob = new Job();
        public Job SelectedJob
        {
            get { return _selectedJob; }
            set { _selectedJob = value; }
        }

        public int JobIndex;
        public MainWindow MainWin;
        public NotesWindow(Job job)
        {
            InitializeComponent();
            txtNotes.Text = job.JobInfo["Notes"];
            txtNotes.Focus();
            DataContext = this;
            MainWin = Application.Current.MainWindow as MainWindow;
            SelectedJob = job;

            try
            {
                AssignedJobList = MainWin.ReadJobsJson(MainWin.AssignedJobListPath);
            }
            catch (FileNotFoundException)
            {
                return;
            }

            foreach (Job assignedJob in AssignedJobList)
            {
                if (job.JobInfo["JobNo"] == assignedJob.JobInfo["JobNo"])
                {
                    JobIndex = AssignedJobList.IndexOf(assignedJob);
                }
            }

            try
            {
                JobNotes = MainWin.ReadJobsJson(MainWin.JobNotesPath);
            }
            catch (FileNotFoundException)
            {
                return;
            }

        }

        private void btnSaveNotes_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNotes.Text) == false && txtNotes.Text.Length != 0)
            {
                if (JobNotes != null)
                {
                    if (JobNotes.Count > 0)
                    {
                        foreach (Job job in JobNotes)
                        {
                            if (job.JobInfo["PartNo"] == SelectedJob.JobInfo["PartNo"])
                            {
                                job.JobInfo["Notes"] = txtNotes.Text;
                                MainWin.WriteJobsJson(JobNotes, MainWin.JobNotesPath);
                                break;
                            }
                            else
                            {
                                Job newNotes = new Job();
                                newNotes.JobInfo.Add("Notes", txtNotes.Text);
                                newNotes.JobInfo.Add("PartNo", SelectedJob.JobInfo["PartNo"]);
                                JobNotes.Add(newNotes);
                                MainWin.WriteJobsJson(JobNotes, MainWin.JobNotesPath);
                                break;
                            }
                        }
                    }
                    else
                    {
                        Job newNotes = new Job();
                        newNotes.JobInfo.Add("Notes", txtNotes.Text);
                        newNotes.JobInfo.Add("PartNo", SelectedJob.JobInfo["PartNo"]);
                        JobNotes.Add(newNotes);
                        MainWin.WriteJobsJson(JobNotes, MainWin.JobNotesPath);
                    }
                }
                AssignedJobList[JobIndex].JobInfo["Notes"] = txtNotes.Text;
                MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                DialogResult = true;
            }
        }
    }
}
