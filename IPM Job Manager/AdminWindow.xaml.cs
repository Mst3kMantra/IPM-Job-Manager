using IPM_Job_Manager_net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public Window MainWin;
        private ObservableCollection<User> _userList = new ObservableCollection<User>();
        public ObservableCollection<User> UserList
        {
            get { return _userList; }
            set { _userList = value; }
        }

        private ObservableCollection<Job> _jobList = new ObservableCollection<Job>();
        public ObservableCollection<Job> JobList
        {
            get { return _jobList; }
            set { _jobList = value; }
        }

        private ObservableCollection<Job> _assignedJobList = new ObservableCollection<Job>();
        public ObservableCollection<Job> AssignedJobList
        {
            get { return _assignedJobList; }
            set { _assignedJobList = value; }
        }

        private ObservableCollection<Job> _curEmployeeJobs = new ObservableCollection<Job>();
        public ObservableCollection <Job> CurEmployeeJobs
        {
            get { return _curEmployeeJobs; }
            set { _curEmployeeJobs = value; }
        }

        public object LastSelectedUser;
        public object LastSelectedJob;

        public bool isLogoutPressed;

        public ObservableCollection<Job> ReadJobsJson(string JsonPath)
        {
            using (StreamReader sr = new StreamReader(JsonPath))
            {
                string json = sr.ReadToEnd();
                ObservableCollection<Job> Jobs = new ObservableCollection<Job>();
                Jobs = JsonConvert.DeserializeObject<ObservableCollection<Job>>(json);
                return Jobs;
            }
        }

        public void WriteJobsJson(ObservableCollection<Job> Jobs, string JsonPath)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.TypeNameHandling = TypeNameHandling.All;

            using (StreamWriter sw = new StreamWriter(JsonPath))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                writer.Formatting = Formatting.Indented;
                serializer.Serialize(writer, Jobs);
            }
        }

        public void AssignJob(User employee, int jobIndex)
        {
            AssignedJobList.Add(JobList[jobIndex]);
            foreach (Job job in AssignedJobList)
            {
                if (job.Equals(JobList[jobIndex]))
                {
                    job.JobInfo["AssignedEmployees"].Add(employee.Username);
                }
            }
            CurEmployeeJobs.Add(JobList[jobIndex]);
            WriteJobsJson(AssignedJobList, @"I:\GTMT\test\assigned_job_list.json");
        }

        public AdminWindow(ObservableCollection<User> Usernames, Window Win, ObservableCollection<Job> jobs)
        {
            InitializeComponent();
            this.DataContext = this;
            UserList = Usernames;
            MainWin = Win;
            JobList = jobs;
            try
            {
                AssignedJobList = ReadJobsJson(@"I:\GTMT\test\assigned_job_list.json");
            } 
            catch (FileNotFoundException)
            {
                return;
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            isLogoutPressed = true;
            MainWin.Show();
            this.Close();
        }

        private void btnAssignJob_Click(object sender, RoutedEventArgs e)
        {
            User SelectedUser = LastSelectedUser as User;
            Job SelectedJob = LastSelectedJob as Job;
            int JobIndex;

            foreach (Job assignedjob in AssignedJobList)
            {
                if (assignedjob.JobInfo["JobNo"] == SelectedJob.JobInfo["JobNo"])
                {
                    foreach (string employee in assignedjob.JobInfo["AssignedEmployees"])
                    {
                        if (employee == SelectedUser.Username)
                        {
                            MessageBox.Show("Job already assigned to this employee.", "Assign Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                }
            }

            foreach (Job job in JobList)
            {
                if (job == SelectedJob)
                {
                    JobIndex = JobList.IndexOf(job);
                    AssignJob(SelectedUser, JobIndex);
                }
            }
        }

        private void btnViewJob_Click(object sender, RoutedEventArgs e)
        {
            var JobDetails = new JobDetailsWindow(LastSelectedJob as Job);
            JobDetails.Owner = this;
            JobDetails.Show();
        }

        private void btnRemoveJob_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lstUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LastSelectedUser = lstUsers.SelectedItem;

            if (CurEmployeeJobs.Count != 0)
            {
                CurEmployeeJobs.Clear();
            }

            User CurItem = lstUsers.SelectedItem as User;

            ObservableCollection<Job> jobs = new ObservableCollection<Job>();
            try
            {
                jobs = ReadJobsJson(@"I:\GTMT\test\assigned_job_list.json");

                foreach (Job job in jobs)
                {
                    foreach (string employee in job.JobInfo["AssignedEmployees"])
                    {
                        if (employee == CurItem.Username)
                        {
                            CurEmployeeJobs.Add(job);
                        }
                    }

                }
            } 
            catch (System.IO.FileNotFoundException)
            {
                return;
            }

        }

        private void lstJobs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LastSelectedJob = lstJobs.SelectedItem;
        }
        
        private void AdminWindow_Closing(object sender, CancelEventArgs e)
        {
            if (isLogoutPressed != true)
            {
                isLogoutPressed = false;
                MainWin.Close();
            }
        }

        private void lstAssignedJobs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LastSelectedJob = lstAssignedJobs.SelectedItem;
        }
    }
}
