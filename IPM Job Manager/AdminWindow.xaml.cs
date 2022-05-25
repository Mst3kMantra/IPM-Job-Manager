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
using System.Timers;

namespace IPM_Job_Manager_net
{
    /// <summary>
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public MainWindow MainWin;
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
        public ObservableCollection<Job> CurEmployeeJobs
        {
            get { return _curEmployeeJobs; }
            set { _curEmployeeJobs = value; }
        }

        private ObservableCollection<string> _operationList = new ObservableCollection<string>();
        public ObservableCollection<string> OperationList
        {
            get { return _operationList; }
            set { _operationList = value; }
        }

        private ObservableCollection<string> _assignedEmployeeList = new ObservableCollection<string>();
        public ObservableCollection<string> AssignedEmployeeList
        {
            get { return _assignedEmployeeList; }
            set { _assignedEmployeeList = value; }
        }

        private Dictionary<string, string> _selectedOperations = new Dictionary<string, string>();
        public Dictionary<string, string> SelectedOperations
        {
            get { return _selectedOperations; }
            set { _selectedOperations = value; }
        }

        private ObservableCollection<Job> _jobNotes = new ObservableCollection<Job>();
        public ObservableCollection<Job> JobNotes
        {
            get { return _jobNotes; }
            set { _jobNotes = value; }
        }

        public AdminWindow adminWindow;

        public MainWindow mainWindow;

        public object LastSelectedUser;

        private object _lastSelectedJob;
        public object LastSelectedJob
        {
            get { return _lastSelectedJob; }
            set { _lastSelectedJob = value; }
        }
        public object SelectedOperation;
        public object SelectedAssignee;

        public bool isLogoutPressed = false;
        public bool isRefreshPressed = false;

        public AdminWindow()
        {
            InitializeComponent();
            DataContext = this;
            MainWin = Application.Current.MainWindow as MainWindow;
            JobList = MainWin.ReadJobsJson(MainWin.JobListPath);
            UserList = MainWin.UserList;
            try
            {
                JobNotes = MainWin.ReadJobsJson(MainWin.JobNotesPath);
            }
            catch (FileNotFoundException)
            {
                return;
            }
            JobList = MainWin.ReadJobNotes(JobList);
            try
            {
                AssignedJobList = MainWin.ReadJobsJson(MainWin.AssignedJobListPath);
            }
            catch (FileNotFoundException)
            {
                return;
            }
        }

        private void OnTimedEvent()
        {

        }

        public void RefreshWindow(ListView list)
        {
            LastSelectedUser = lstUsers.SelectedItem;
            int LastJobIndex = list.SelectedIndex;

            if (CurEmployeeJobs.Count > 0)
            {
                CurEmployeeJobs.Clear();
            }

            if (SelectedOperations != null)
            {
                if (SelectedOperations.Count > 0)
                {
                    SelectedOperations.Clear();
                    OperationList.Clear();
                    AssignedEmployeeList.Clear();
                }
            }

            User CurItem = lstUsers.SelectedItem as User;

            ObservableCollection<Job> jobs = new ObservableCollection<Job>();
            try
            {
                jobs = MainWin.ReadJobsJson(MainWin.AssignedJobListPath);

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
            catch (FileNotFoundException)
            {
                return;
            }
            var SortedJobs = MainWin.SortJobs(CurEmployeeJobs);
            CurEmployeeJobs.Clear();
            foreach (Job sortedjob in SortedJobs)
            {
                CurEmployeeJobs.Add(sortedjob);
            }
            try
            {
                list.SelectedItem = CurEmployeeJobs[LastJobIndex];
            }
            catch (ArgumentOutOfRangeException)
            {
                list.SelectedItem = null;
            }
            LastSelectedJob = list.SelectedItem;

            if (LastSelectedJob == null) { return; }

            if (SelectedOperations != null)
            {
                if (SelectedOperations.Count > 0)
                {
                    SelectedOperations.Clear();
                    OperationList.Clear();
                    AssignedEmployeeList.Clear();
                }
            }

            SelectedOperations = JsonConvert.DeserializeObject<Dictionary<string, string>>((LastSelectedJob as Job).JobInfo["Operations"].ToString());
            foreach (string key in SelectedOperations.Keys)
            {
                OperationList.Add(key);
            }

            foreach (string value in SelectedOperations.Values)
            {
                AssignedEmployeeList.Add(value);
            }

            if (OperationList.Count > 0)
            {
                while (OperationList.Count > AssignedEmployeeList.Count)
                {
                    AssignedEmployeeList.Add("");
                }
            }
        }

        public void AssignJob(User employee, int jobIndex)
        {

            if (AssignedJobList.Count == 0)
            {
                AssignedJobList.Add(JobList[jobIndex]);
            }

            foreach (Job assignedJob in AssignedJobList)
            {
                if (JobList[jobIndex].JobInfo["JobNo"] == assignedJob.JobInfo["JobNo"])
                {
                    assignedJob.JobInfo["AssignedEmployees"].Add(employee.Username);
                    CurEmployeeJobs.Add(assignedJob);
                    MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                    return;
                }
            }

            AssignedJobList.Add(JobList[jobIndex]);
            var newJob = AssignedJobList[AssignedJobList.Count - 1];
            newJob.JobInfo["AssignedEmployees"].Add(employee.Username);
            CurEmployeeJobs.Add(newJob);
            MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
            RefreshWindow(lstAssignedJobs);
        }

        public void UnassignOperations(Job job)
        {
            JobNotes = MainWin.ReadJobsJson(MainWin.JobNotesPath);

            if (OperationList != null)
            {
                foreach (Job jobnote in JobNotes)
                {
                    if (jobnote.JobInfo["PartNo"] == job.JobInfo["PartNo"])
                    {
                        foreach (string operation in OperationList)
                        {
                            jobnote.JobInfo["Operations"][operation] = "";
                        }
                    }
                }
                foreach (Job assignedjob in AssignedJobList)
                {
                    if (assignedjob.JobInfo["PartNo"] == job.JobInfo["PartNo"])
                    {
                        foreach (string operation in OperationList)
                        {
                            assignedjob.JobInfo["Operations"][operation] = "";
                        }
                    }
                }
                foreach (Job oldjob in JobList)
                {
                    if (oldjob.JobInfo["PartNo"] == job.JobInfo["PartNo"])
                    {
                        foreach (string operation in OperationList)
                        {
                            oldjob.JobInfo["Operations"][operation] = "";
                        }
                    }
                }
                MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                MainWin.WriteJobsJson(JobList, MainWin.JobListPath);
                MainWin.WriteJobsJson(JobNotes, MainWin.JobNotesPath);
            }
        }

        public void RemoveJob(User employee, Job job)
        {
            foreach (Job job2 in AssignedJobList)
            {
                if (job2.JobInfo["JobNo"] == job.JobInfo["JobNo"])
                {
                    for (int i = 0; i < job2.JobInfo["AssignedEmployees"].Count; i++)
                    {
                        if (job2.JobInfo["AssignedEmployees"][i] == employee.Username)
                        {
                            job2.JobInfo["AssignedEmployees"].RemoveAt(i);
                            i = 0;
                        }
                    }
                    if (job2.JobInfo["AssignedEmployees"].Count == 0)
                    {
                        AssignedJobList.Remove(job2);
                    }
                    break;
                }
            }

            foreach (Job curJob in CurEmployeeJobs)
            {
                if (curJob.JobInfo["JobNo"] == job.JobInfo["JobNo"])
                {
                    for (int i = 0; i < curJob.JobInfo["AssignedEmployees"].Count; i++)
                    {
                        if (curJob.JobInfo["AssignedEmployees"][i] == employee.Username)
                        {
                            curJob.JobInfo["AssignedEmployees"].RemoveAt(i);
                            i = 0;
                        }
                    }
                    CurEmployeeJobs.Remove(curJob);
                    break;
                }
            }
            MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            isLogoutPressed = true;
            MainWin.Show();
            Close();
        }

        private void btnAssignJob_Click(object sender, RoutedEventArgs e)
        {
            User SelectedUser = LastSelectedUser as User;
            Job SelectedJob = LastSelectedJob as Job;
            int JobIndex;
            bool IsInList = false;

            if (SelectedJob == null)
            {
                MessageBox.Show("No job selected. Select a job from the job list.", "Assign Job Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedUser == null)
            {
                MessageBox.Show("No employee selected. Select a employee from the employee list.", "Assign Job Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (Job assignedjob in AssignedJobList)
            {
                if (assignedjob.JobInfo["JobNo"] == SelectedJob.JobInfo["JobNo"])
                {
                    foreach (string employee in assignedjob.JobInfo["AssignedEmployees"])
                    {
                        if (employee == SelectedUser.Username)
                        {
                            IsInList = true;
                        }
                    }

                }
            }

            if (IsInList)
            {
                MessageBox.Show("Job already assigned to this employee.", "Assign Job Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedJob == null || SelectedUser == null)
            {
                return;
            }

            foreach (Job job in JobList)
            {
                if (job.JobInfo["JobNo"] == SelectedJob.JobInfo["JobNo"])
                {
                    JobIndex = JobList.IndexOf(job);
                    AssignJob(SelectedUser, JobIndex);
                }
            }
            if (CurEmployeeJobs.Count > 0)
            {
                var SortedJobs = MainWin.SortJobs(CurEmployeeJobs);
                CurEmployeeJobs.Clear();
                foreach (Job sortedjob in SortedJobs)
                {
                    CurEmployeeJobs.Add(sortedjob);
                }
            }
        }

        private void btnViewJob_Click(object sender, RoutedEventArgs e)
        {
            Job SelectedJob = LastSelectedJob as Job;

            if (SelectedJob == null)
            {
                MessageBox.Show("No job selected. Select a job from the job list.", "View Job Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (Job job in AssignedJobList)
            {
                if (job.JobInfo["JobNo"] == SelectedJob.JobInfo["JobNo"])
                {
                    Window AssignedJobDetails = new JobDetailsWindow(job);
                    AssignedJobDetails.Owner = this;
                    AssignedJobDetails.Show();
                    return;
                }
            }
            Window JobDetails = new JobDetailsWindow(SelectedJob);
            JobDetails.Owner = this;
            JobDetails.Show();
        }

        private void btnRemoveJob_Click(object sender, RoutedEventArgs e)
        {
            User SelectedUser = LastSelectedUser as User;
            Job SelectedJob = LastSelectedJob as Job;
            bool isUserNotInList = true;

            if (SelectedJob == null)
            {
                MessageBox.Show("No job selected. Select a job from the job list.", "Remove Job Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (SelectedUser == null)
            {
                MessageBox.Show("No employee selected. Select a employee from the employee list.", "Remove Job Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                foreach (Job assignedjob in AssignedJobList)
                {
                    if (assignedjob.JobInfo["JobNo"] == SelectedJob.JobInfo["JobNo"])
                    {
                        foreach (string employee in assignedjob.JobInfo["AssignedEmployees"])
                        {
                            if (employee == SelectedUser.Username)
                            {
                                isUserNotInList = false;
                            }
                        }

                    }
                }

                if (isUserNotInList)
                {
                    MessageBox.Show("Employee not assigned to this job.", "Remove Job Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    RemoveJob(SelectedUser, SelectedJob);
                }
            }
            catch (InvalidOperationException)
            {
                return;
            }
            LastSelectedJob = lstJobs.SelectedItem;
            if (CurEmployeeJobs.Count > 0)
            {
                var SortedJobs = MainWin.SortJobs(CurEmployeeJobs);
                CurEmployeeJobs.Clear();
                foreach (Job sortedjob in SortedJobs)
                {
                    CurEmployeeJobs.Add(sortedjob);
                }
            }
        }

        private void btnAddPrio_Click(object sender, RoutedEventArgs e)
        {
            Job SelectedJob = LastSelectedJob as Job;
            if (SelectedJob != null)
            {
                int CurJobIndex;
                int AssignedJobIndex;
                string LastJobNo = SelectedJob.JobInfo["JobNo"];

                if (SelectedJob.JobInfo["Priority"] < MainWindow.MaxPriority)
                {
                    foreach (Job job in CurEmployeeJobs)
                    {
                        if (job.JobInfo["JobNo"] == SelectedJob.JobInfo["JobNo"])
                        {
                            CurJobIndex = CurEmployeeJobs.IndexOf(job);
                            CurEmployeeJobs[CurJobIndex].JobInfo["Priority"] += 1;
                        }
                    }
                    foreach (Job job in AssignedJobList)
                    {
                        if (job.JobInfo["JobNo"] == SelectedJob.JobInfo["JobNo"])
                        {
                            AssignedJobIndex = AssignedJobList.IndexOf(job);
                            AssignedJobList[AssignedJobIndex].JobInfo["Priority"] += 1;
                        }
                    }
                    MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                    var SortedJobs = MainWin.SortJobs(CurEmployeeJobs);
                    CurEmployeeJobs.Clear();
                    foreach (Job sortedjob in SortedJobs)
                    {
                        CurEmployeeJobs.Add(sortedjob);
                    }
                    foreach (Job orderedJob in CurEmployeeJobs)
                    {
                        if (orderedJob.JobInfo["JobNo"] == LastJobNo)
                        {
                            LastSelectedJob = orderedJob;
                        }
                    }
                    if (LastSelectedJob != null)
                    {
                        foreach (object item in lstAssignedJobs.Items)
                        {
                            if ((item as Job).JobInfo["JobNo"] == (LastSelectedJob as Job).JobInfo["JobNo"])
                            {
                                lstAssignedJobs.SelectedItem = item;
                                LastSelectedJob = lstAssignedJobs.SelectedItem;
                            }
                        }
                    }
                }
            }
        }

        private void btnLowerPrio_Click(object sender, RoutedEventArgs e)
        {
            Job SelectedJob = LastSelectedJob as Job;
            int CurJobIndex;
            int AssignedJobIndex;

            if (SelectedJob != null)
            {
                string LastJobNo = SelectedJob.JobInfo["JobNo"];
                if (SelectedJob.JobInfo["Priority"] > 0)
                {
                    foreach (Job job in CurEmployeeJobs)
                    {
                        if (job.JobInfo["JobNo"] == SelectedJob.JobInfo["JobNo"])
                        {
                            CurJobIndex = CurEmployeeJobs.IndexOf(job);
                            CurEmployeeJobs[CurJobIndex].JobInfo["Priority"] -= 1;
                        }
                    }
                    foreach (Job job in AssignedJobList)
                    {
                        if (job.JobInfo["JobNo"] == SelectedJob.JobInfo["JobNo"])
                        {
                            AssignedJobIndex = AssignedJobList.IndexOf(job);
                            AssignedJobList[AssignedJobIndex].JobInfo["Priority"] -= 1;
                        }
                    }
                    MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                    var SortedJobs = MainWin.SortJobs(CurEmployeeJobs);
                    CurEmployeeJobs.Clear();
                    foreach (Job sortedjob in SortedJobs)
                    {
                         CurEmployeeJobs.Add(sortedjob);
                    }
                    foreach (Job orderedJob in CurEmployeeJobs)
                    {
                        if (orderedJob.JobInfo["JobNo"] == LastJobNo)
                        {
                            LastSelectedJob = orderedJob;
                        }
                    }
                    if (LastSelectedJob != null)
                    {
                        foreach (object item in lstAssignedJobs.Items)
                        {
                            if ((item as Job).JobInfo["JobNo"] == (LastSelectedJob as Job).JobInfo["JobNo"])
                            {
                                lstAssignedJobs.SelectedItem = item;
                                LastSelectedJob = lstAssignedJobs.SelectedItem;
                            }
                        }
                    }
                }
            }
        }

        private void lstUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LastSelectedUser = lstUsers.SelectedItem;

            if (CurEmployeeJobs.Count > 0)
            {
                CurEmployeeJobs.Clear();
            }

            if (SelectedOperations != null)
            {
                if (SelectedOperations.Count > 0)
                {
                    SelectedOperations.Clear();
                    OperationList.Clear();
                    AssignedEmployeeList.Clear();
                }
            }

            txtNotes.Text = "";

            User CurItem = lstUsers.SelectedItem as User;

            ObservableCollection<Job> jobs = new ObservableCollection<Job>();
            try
            {
                jobs = MainWin.ReadJobsJson(MainWin.AssignedJobListPath);

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
            var SortedJobs = MainWin.SortJobs(CurEmployeeJobs);
            CurEmployeeJobs.Clear();
            foreach (Job sortedjob in SortedJobs)
            {
                CurEmployeeJobs.Add(sortedjob);
            }
        }

        private void lstJobs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LastSelectedJob = lstJobs.SelectedItem;

            if (LastSelectedJob == null) { return; }

            if (SelectedOperations != null)
            {
                if (SelectedOperations.Count > 0)
                {
                    SelectedOperations.Clear();
                    OperationList.Clear();
                    AssignedEmployeeList.Clear();
                }
            }
            try
            {
                SelectedOperations = JsonConvert.DeserializeObject<Dictionary<string, string>>((LastSelectedJob as Job).JobInfo["Operations"].ToString());
            }
            catch (JsonReaderException)
            {
                SelectedOperations = (LastSelectedJob as Job).JobInfo["Operations"];
            }
            
            foreach (string key in SelectedOperations.Keys)
            {
                OperationList.Add(key);
            }

            foreach (string value in SelectedOperations.Values)
            {
                AssignedEmployeeList.Add(value);
            }

            if (OperationList.Count > 0)
            {
                while (OperationList.Count > AssignedEmployeeList.Count)
                {
                    AssignedEmployeeList.Add("");
                }
            }
            txtNotes.Text = (LastSelectedJob as Job).JobInfo["Notes"].ToString();
        }
        
        private void AdminWindow_Closing(object sender, CancelEventArgs e)
        {
            if (isLogoutPressed == false && isRefreshPressed == false)
            {
                 try
                 {
                     MainWin.Close();
                 }
                 catch (InvalidOperationException)
                 {
                     return;
                 }
            }
        }

        private void lstAssignedJobs_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LastSelectedJob = lstAssignedJobs.SelectedItem;

            if (LastSelectedJob == null) { return; }

            if (SelectedOperations != null)
            {
                if (SelectedOperations.Count > 0)
                {
                    SelectedOperations.Clear();
                    OperationList.Clear();
                    AssignedEmployeeList.Clear();
                }
            }

            SelectedOperations = JsonConvert.DeserializeObject<Dictionary<string, string>>((LastSelectedJob as Job).JobInfo["Operations"].ToString());
            foreach (string key in SelectedOperations.Keys)
            {
                OperationList.Add(key);
            }

            foreach (string value in SelectedOperations.Values)
            {
                AssignedEmployeeList.Add(value);
            }

            if (OperationList.Count > 0)
            {
                while (OperationList.Count > AssignedEmployeeList.Count)
                {
                    AssignedEmployeeList.Add("");
                }
            }
            txtNotes.Text = (LastSelectedJob as Job).JobInfo["Notes"].ToString();
        }

        private void btnEditOperations_Click(object sender, RoutedEventArgs e)
        {
            if (LastSelectedJob != null)
            {
                Window OpWin = new OperationsWindow(LastSelectedJob as Job);
                OpWin.Owner = this;
                bool? DialogResult = OpWin.ShowDialog();
                if (DialogResult == true)
                {
                    RefreshWindow(lstAssignedJobs);
                }
            }
        }

        private void lstOperations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedOperation = lstOperations.SelectedItem;
        }

        private void lstAssigned_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedAssignee = lstAssigned.SelectedItem;
        }

        private void btnEditNotes_Click(object sender, RoutedEventArgs e)
        {
            if (LastSelectedJob != null)
            {
                Window NotesWin = new NotesWindow(LastSelectedJob as Job);
                NotesWin.Owner = this;
                bool? DialogResult = NotesWin.ShowDialog();
                if (DialogResult == true)
                {
                    RefreshWindow(lstAssignedJobs);
                }
            }
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            Window NewUserWindow = new NewUserWindow();
            NewUserWindow.Owner = this;
            NewUserWindow.ShowDialog();
        }
    }
}
