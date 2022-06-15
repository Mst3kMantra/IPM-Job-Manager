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

        private Dictionary<string, bool> _completedOperations = new Dictionary<string, bool>();
        public Dictionary<string, bool> CompletedOperations
        {
            get { return _completedOperations; }
            set { _completedOperations = value; }
        }

        private ObservableCollection<Job> _CompletedJobs = new ObservableCollection<Job>();
        public ObservableCollection<Job> CompletedJobs
        {
            get { return _CompletedJobs; }
            set { _CompletedJobs = value; }
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

        private ObservableCollection<string> _attachedFileList = new ObservableCollection<string>();
        public ObservableCollection <string> AttachedFileList
        {
            get { return _attachedFileList; }
            set { _attachedFileList = value; }
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

        private Dictionary<string, int> _selectedTimes = new Dictionary<string, int>();
        public Dictionary<string, int> SelectedTimes
        {
            get { return _selectedTimes; }
            set { _selectedTimes = value; }
        }

        private ObservableCollection<Job> _jobNotes = new ObservableCollection<Job>();
        public ObservableCollection<Job> JobNotes
        {
            get { return _jobNotes; }
            set { _jobNotes = value; }
        }

        public AdminWindow adminWindow;

        public MainWindow mainWindow;

        public bool isAddingBoxes;

        public object LastSelectedUser;

        private object _lastSelectedJob;
        public object LastSelectedJob
        {
            get { return _lastSelectedJob; }
            set { _lastSelectedJob = value; }
        }
        public object SelectedOperation;
        public object SelectedAssignee;

        public object SelectedFileName;

        public bool isLogoutPressed = false;
        public bool isRefreshPressed = false;

        public AdminWindow()
        {
            InitializeComponent();
            DataContext = this;
            MainWin = Application.Current.MainWindow as MainWindow;
            JobList = MainWin.ReadJobsJson(MainWin.JobListPath);
            Root JsonUserList = MainWin.ReadUserJson(MainWin.UserListPath);
            foreach (User user in JsonUserList.Users)
            {
                UserList.Add(user);
            }
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
            CountJobs();
            CompletedJobs = MainWin.CompletedJobs;
        }

        public void CountJobs()
        {
            foreach (User user in UserList)
            {
                user.JobsAssigned = 0;
            }
            foreach (Job job in AssignedJobList)
            {
                foreach (User user in UserList)
                {
                    foreach (string employee in job.JobInfo["AssignedEmployees"])
                    {
                        if (employee == user.Username)
                        {
                            user.JobsAssigned++;
                        }
                    }
                }
            }
        }

        public void CalculateOpTime(Dictionary<string, int> times)
        {
            foreach (KeyValuePair<string, int> kvp in times)
            {
                if (kvp.Value > 0)
                {
                    int hours = 0;
                    int minutes = 0;
                    int seconds = 0;

                    if (kvp.Value >= 60)
                    {
                        minutes = kvp.Value / 60;
                        seconds += kvp.Value % 60;
                    }
                    else seconds = kvp.Value;

                    if (minutes >= 60)
                    {
                        hours = (minutes / 60);
                    }
                    if (hours > 0)
                    {
                        minutes -= hours * 60;
                    }

                    TextBlock Time = new TextBlock();
                    Time.Text = $"Hrs: {hours} | Mins: {minutes} | Secs: {seconds}";
                    Time.Tag = kvp.Key;
                    lstOperationTimes.Items.Add(Time);
                }
                else
                {
                    int hours = 0;
                    int minutes = 0;
                    int seconds = 0;
                    TextBlock Time = new TextBlock();
                    Time.Text = $"Hrs: {hours} | Mins: {minutes} | Secs: {seconds}";
                    Time.Tag = kvp.Key;
                    lstOperationTimes.Items.Add(Time);
                }
            }
        }

        public void RefreshWindow(ListView list)
        {
            AssignedJobList.Clear();
            AssignedJobList = MainWin.ReadJobsJson(MainWin.AssignedJobListPath);
            LastSelectedUser = lstUsers.SelectedItem;
            int LastJobIndex = list.SelectedIndex;
            isAddingBoxes = true;

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
                    lstOperationTimes.Items.Clear();
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

            CalculateTime();

            if (LastSelectedJob == null) { return; }

            if (SelectedOperations != null)
            {
                if (SelectedOperations.Count > 0)
                {
                    SelectedOperations.Clear();
                    OperationList.Clear();
                    AssignedEmployeeList.Clear();
                    lstOperationTimes.Items.Clear();
                }
            }

            if (lstStatus != null)
            {
                if (lstStatus.Items.Count > 0)
                {
                    lstStatus.Items.Clear();
                }
            }

            SelectedOperations = JsonConvert.DeserializeObject<Dictionary<string, string>>((LastSelectedJob as Job).JobInfo["Operations"].ToString());
            SelectedTimes = JsonConvert.DeserializeObject<Dictionary<string, int>>((LastSelectedJob as Job).JobInfo["OperationTime"].ToString());
            CompletedOperations = JsonConvert.DeserializeObject<Dictionary<string, bool>>((LastSelectedJob as Job).JobInfo["CompletedOperations"].ToString());

            try
            {
                foreach (string key in CompletedOperations.Keys)
                {
                    foreach (KeyValuePair<string, string> kvp in SelectedOperations)
                    {
                        if (kvp.Key == key)
                        {
                            CheckBox OpCheck = new CheckBox
                            {
                                Tag = key,
                                Content = "Complete",
                                Name = SelectedOperations[key].Replace(" ", ""),
                            };
                            OpCheck.Checked += new RoutedEventHandler(HandleOpChecked);
                            OpCheck.Unchecked += new RoutedEventHandler(HandleOpUnchecked);
                            if (CompletedOperations[key] == true)
                            {
                                OpCheck.IsChecked = true;
                            }
                            lstStatus.Items.Add(OpCheck);
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                return;
            }

            isAddingBoxes = false;

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

            CalculateOpTime(SelectedTimes);
        }

        public void AssignJob(User employee, int jobIndex)
        {

            if (AssignedJobList.Count == 0)
            {
                AssignedJobList.Add(JobList[jobIndex]);
            }

            if (LastSelectedJob != null)
            {
                foreach (Job assignedJob in AssignedJobList)
                {
                    if (JobList[jobIndex].JobInfo["JobNo"] == assignedJob.JobInfo["JobNo"])
                    {
                        SelectedOperations = JsonConvert.DeserializeObject<Dictionary<string, string>>(assignedJob.JobInfo["Operations"].ToString());
                        assignedJob.JobInfo["AssignedEmployees"].Add(employee.Username);
                        foreach (string key in SelectedOperations.Keys)
                        {
                            if (!assignedJob.JobInfo["CompletedOperations"].ContainsKey(key))
                            {
                                assignedJob.JobInfo["CompletedOperations"].Add(key, false);
                            }
                            if (!assignedJob.JobInfo["OperationTime"].ContainsKey(key))
                            {
                                assignedJob.JobInfo["OperationTime"].Add(key, 0);
                            }
                        }
                        CurEmployeeJobs.Add(assignedJob);
                        MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                        return;
                    }
                }

                AssignedJobList.Add(JobList[jobIndex]);
                var newJob = AssignedJobList[AssignedJobList.Count - 1];
                SelectedOperations = JsonConvert.DeserializeObject<Dictionary<string, string>>(newJob.JobInfo["Operations"].ToString());
                newJob.JobInfo["AssignedEmployees"].Add(employee.Username);
                foreach (string key in SelectedOperations.Keys)
                {
                    if (!newJob.JobInfo["CompletedOperations"].ContainsKey(key))
                    {
                        newJob.JobInfo["CompletedOperations"].Add(key, false);
                    }
                    if (!newJob.JobInfo["OperationTime"].ContainsKey(key))
                    {
                        newJob.JobInfo["OperationTime"].Add(key, 0);
                    }
                }
                CurEmployeeJobs.Add(newJob);
                MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                RefreshWindow(lstAssignedJobs);
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

                CompletedOperations = JsonConvert.DeserializeObject<Dictionary<string, bool>>(job.JobInfo["CompletedOperations"].ToString());

                foreach (Job assignedjob in AssignedJobList)
                {
                    if (assignedjob.JobInfo["PartNo"] == job.JobInfo["PartNo"])
                    {
                        foreach (string operation in OperationList)
                        {
                            assignedjob.JobInfo["Operations"][operation] = "";
                        }
                    }
                    if (assignedjob.JobInfo["JobNo"] == job.JobInfo["JobNo"])
                    {
                        foreach (string key in CompletedOperations.Keys)
                        {
                            assignedjob.JobInfo["CompletedOperations"][key] = false;
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
                    if (oldjob.JobInfo["JobNo"] == job.JobInfo["JobNo"])
                    {
                        foreach (string key in CompletedOperations.Keys)
                        {
                            oldjob.JobInfo["CompletedOperations"][key] = false;
                        }
                    }
                }
                MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                MainWin.WriteJobsJson(JobList, MainWin.JobListPath);
                MainWin.WriteJobsJson(JobNotes, MainWin.JobNotesPath);
            }
        }

        public void RefreshJobs()
        {
            LastSelectedUser = lstUsers.SelectedItem;
            lstStatus.Items.Clear();
            AttachedFileList.Clear();
            if (LastSelectedUser == null) { return; }
            if (CurEmployeeJobs.Count != 0)
            {
                CurEmployeeJobs.Clear();
            }

            try
            {
                AssignedJobList = MainWin.ReadJobsJson(MainWin.AssignedJobListPath);
                JobNotes = MainWin.ReadJobsJson(MainWin.JobNotesPath);
            }
            catch (FileNotFoundException)
            {
                return;
            }
            JobList = MainWin.ReadJobNotes(JobList);
            AssignedJobList = MainWin.ReadJobNotes(AssignedJobList);
            MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);

            User CurItem = LastSelectedUser as User;

            ObservableCollection<Job> jobs = new ObservableCollection<Job>();
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

            var SortedJobs = MainWin.SortJobs(CurEmployeeJobs);
            CurEmployeeJobs.Clear();
            foreach (Job sortedjob in SortedJobs)
            {
                CurEmployeeJobs.Add(sortedjob);
            }

            if (AttachedFileList != null)
            {
                if (AttachedFileList.Count > 0) { AttachedFileList.Clear(); }
            }

            if (SelectedOperations != null)
            {
                if (SelectedOperations.Count > 0)
                {
                    SelectedOperations.Clear();
                    OperationList.Clear();
                    AssignedEmployeeList.Clear();
                    lstOperationTimes.Items.Clear();
                }
            }
            txtNotes.Text = "";
        }

        public void CalculateTime()
        {
            foreach (Job job in CurEmployeeJobs)
            {
                int totalTime = 0;
                int hours = 0;
                int minutes = 0;
                int seconds = 0;
                SelectedTimes = JsonConvert.DeserializeObject<Dictionary<string, int>>(job.JobInfo["OperationTime"].ToString());

                foreach (int value in SelectedTimes.Values)
                {
                    totalTime += value;
                }
                if (totalTime > 0)
                {
                    totalTime *= int.Parse(job.JobInfo["QTY Due"]);
                    if (totalTime >= 60)
                    {
                        minutes = totalTime / 60;
                        seconds += totalTime % 60;
                    }
                    else seconds = totalTime;

                    if (minutes >= 60)
                    {
                        hours = (minutes / 60);
                    }
                    if (hours > 0)
                    {
                        minutes -= hours * 60;
                    }
                }
                job.JobInfo["EstTime"] = $"Hrs: {hours} | Mins: {minutes} | Secs: {seconds}";
            }
        }

        public void WriteFileToNotes(Job ChangedJob)
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
                            job.JobInfo["AttachedFiles"] = ChangedJob.JobInfo["AttachedFiles"];
                            MainWin.WriteJobsJson(JobNotes, MainWin.JobNotesPath);
                            isPartInJobNotes = true;
                            break;
                        }
                    }
                    if (isPartInJobNotes == false)
                    {
                        Job newNotes = new Job();
                        newNotes.JobInfo.Add("AttachedFiles", ChangedJob.JobInfo["AttachedFiles"]);
                        newNotes.JobInfo.Add("PartNo", ChangedJob.JobInfo["PartNo"]);
                        JobNotes.Add(newNotes);
                        MainWin.WriteJobsJson(JobNotes, MainWin.JobNotesPath);
                    }
                }
                else
                {
                    Job newNotes = new Job();
                    newNotes.JobInfo.Add("AttachedFiles", ChangedJob.JobInfo["AttachedFiles"]);
                    newNotes.JobInfo.Add("PartNo", ChangedJob.JobInfo["PartNo"]);
                    JobNotes.Add(newNotes);
                    MainWin.WriteJobsJson(JobNotes, MainWin.JobNotesPath);
                }
            }
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {
            isLogoutPressed = true;
            MainWin.CountJobs();
            MainWin.RefreshJobs();
            MainWin.Show();
            Close();
        }

        private void btnAssignJob_Click(object sender, RoutedEventArgs e)
        {
            MainWin.RefreshTimer.Stop();
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
            CountJobs();
            CalculateTime();
            MainWin.RefreshTimer.Start();
        }

        private void btnViewJob_Click(object sender, RoutedEventArgs e)
        {
            MainWin.RefreshTimer.Stop();
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
            MainWin.RefreshTimer.Start();
        }

        private void btnRemoveJob_Click(object sender, RoutedEventArgs e)
        {
            MainWin.RefreshTimer.Stop();
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
            CountJobs();
            lstStatus.Items.Clear();
            if (SelectedOperations != null)
            {
                if (SelectedOperations.Count > 0)
                {
                    SelectedOperations.Clear();
                    OperationList.Clear();
                    AssignedEmployeeList.Clear();
                    lstOperationTimes.Items.Clear();
                }
            }

            if (AttachedFileList != null)
            {
                if (AttachedFileList.Count > 0)
                {
                    AttachedFileList.Clear();
                }
            }
            txtNotes.Text = "";
            MainWin.RefreshTimer.Start();
        }

        private void btnAddPrio_Click(object sender, RoutedEventArgs e)
        {
            MainWin.RefreshTimer.Stop();
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
                    CalculateTime();
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
            MainWin.RefreshTimer.Start();
        }

        private void btnLowerPrio_Click(object sender, RoutedEventArgs e)
        {
            MainWin.RefreshTimer.Stop();
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
                    CalculateTime();
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
            MainWin.RefreshTimer.Start();
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

        private void btnEditOperations_Click(object sender, RoutedEventArgs e)
        {
            if (LastSelectedJob != null)
            {
                MainWin.RefreshTimer.Stop();
                Window OpWin = new OperationsWindow(LastSelectedJob as Job);
                OpWin.Owner = this;
                bool? DialogResult = OpWin.ShowDialog();
                if (DialogResult == true)
                {
                    try
                    {
                        RefreshWindow(lstAssignedJobs);
                    }
                    catch
                    {
                        return;
                    }
                }
                MainWin.RefreshTimer.Start();
            }
        }

        private void btnEditNotes_Click(object sender, RoutedEventArgs e)
        {
            if (LastSelectedJob != null)
            {
                MainWin.RefreshTimer.Stop();
                Window NotesWin = new NotesWindow(LastSelectedJob as Job);
                NotesWin.Owner = this;
                bool? DialogResult = NotesWin.ShowDialog();
                if (DialogResult == true)
                {
                    RefreshWindow(lstAssignedJobs);
                }
                MainWin.RefreshTimer.Start();
            }
        }

        private void btnAddUser_Click(object sender, RoutedEventArgs e)
        {
            MainWin.RefreshTimer.Stop();
            Window NewUserWindow = new NewUserWindow();
            NewUserWindow.Owner = this;
            NewUserWindow.ShowDialog();
            MainWin.RefreshTimer.Start();
        }

        private void btnViewCompletedJobs_Click(object sender, RoutedEventArgs e)
        {
            MainWin.RefreshTimer.Stop();
            try
            {
                Window CompleteJobsWindow = new CompletedJobsWindow();
                CompleteJobsWindow.Owner = this;
                CompleteJobsWindow.ShowDialog();
            }
            catch (InvalidOperationException)
            {
                MessageBox.Show("No completed jobs to display.", "View Completed Jobs Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            MainWin.RefreshTimer.Start();
        }

        private void btnAttachFile_Click(object sender, RoutedEventArgs e)
        {
            MainWin.RefreshTimer.Stop();
            if (LastSelectedJob != null)
            {
                Job SelectedJob = LastSelectedJob as Job;
                int JobIndex = 0;
                foreach (Job job in AssignedJobList)
                {
                    if (job.JobInfo["JobNo"] == SelectedJob.JobInfo["JobNo"])
                    {
                        JobIndex = AssignedJobList.IndexOf(job);
                    }
                }
                Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                dlg.Filter = "PDF Files|*.pdf|Word Documents|*.doc|Text Files|*.txt" +
                             "|Image files(*.png; *.jpeg)| *.png; *.jpeg" +
                             "|All Files|*.*";
                dlg.InitialDirectory = @"I:\";
                dlg.Multiselect = true;
                bool? DialogResult = dlg.ShowDialog();
                if (DialogResult == true)
                {
                    if (dlg.FileNames.Count() > 1)
                    {
                        foreach (string FileName in dlg.FileNames)
                        {
                            if (SelectedJob.JobInfo["AttachedFiles"].Contains(FileName) == false)
                            {
                                AssignedJobList[JobIndex].JobInfo["AttachedFiles"].Add(FileName);
                                AttachedFileList.Add(FileName);
                            }
                        }
                    }
                    else if (SelectedJob.JobInfo["AttachedFiles"].Contains(dlg.FileName) == false)
                    {
                        AssignedJobList[JobIndex].JobInfo["AttachedFiles"].Add(dlg.FileName);
                        AttachedFileList.Add(dlg.FileName);
                    }
                    MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                    WriteFileToNotes(AssignedJobList[JobIndex]);
                }
            }
            MainWin.RefreshTimer.Start();
        }


        private void btnDetachFile_Click(object sender, RoutedEventArgs e)
        {
            MainWin.RefreshTimer.Stop();
            if (SelectedFileName != null && LastSelectedJob != null)
            {
                if (lstAttachedFiles.SelectedItems.Count == 1)
                {
                    string FilePath = SelectedFileName.ToString();
                    Job SelectedJob = LastSelectedJob as Job;
                    AttachedFileList.Remove(FilePath);
                    foreach (Job job in AssignedJobList)
                    {
                        if (job.JobInfo["JobNo"] == SelectedJob.JobInfo["JobNo"])
                        {
                            for (int i = 0; i < job.JobInfo["AttachedFiles"].Count; i++)
                            {
                                if (job.JobInfo["AttachedFiles"][i] == FilePath)
                                {
                                    job.JobInfo["AttachedFiles"].RemoveAt(i);
                                    break;
                                }
                            }
                            MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                            break;
                        }
                    }
                    foreach (Job job in JobNotes)
                    {
                        if (job.JobInfo["PartNo"] == SelectedJob.JobInfo["PartNo"])
                        {
                            for (int i = 0; i < job.JobInfo["AttachedFiles"].Count; i++)
                            {
                                if (job.JobInfo["AttachedFiles"][i] == FilePath)
                                {
                                    job.JobInfo["AttachedFiles"].RemoveAt(i);
                                    break;
                                }
                            }
                            MainWin.WriteJobsJson(JobNotes, MainWin.JobNotesPath);
                            break;
                        }
                    }
                }
                else if (lstAttachedFiles.SelectedItems.Count > 1)
                {
                    Job SelectedJob = LastSelectedJob as Job;
                    foreach (Job job in AssignedJobList)
                    {
                        if (job.JobInfo["JobNo"] == SelectedJob.JobInfo["JobNo"])
                        {
                            for (int i = job.JobInfo["AttachedFiles"].Count - 1; i > -1; i--)
                            {
                                foreach (object item in lstAttachedFiles.SelectedItems)
                                {
                                    if (job.JobInfo["AttachedFiles"][i] == item.ToString())
                                    {
                                        job.JobInfo["AttachedFiles"].RemoveAt(i);
                                        i = job.JobInfo["AttachedFiles"].Count;
                                        break;
                                    }
                                }
                            }
                            MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                            break;
                        }
                    }
                    foreach (Job job in JobNotes)
                    {
                        if (job.JobInfo["PartNo"] == SelectedJob.JobInfo["PartNo"])
                        {
                            for (int i = job.JobInfo["AttachedFiles"].Count - 1; i > -1; i--)
                            {
                                foreach (object item in lstAttachedFiles.SelectedItems)
                                {
                                    if (job.JobInfo["AttachedFiles"][i] == item.ToString())
                                    {
                                        job.JobInfo["AttachedFiles"].RemoveAt(i);
                                        i = job.JobInfo["AttachedFiles"].Count;
                                        break;
                                    }
                                }
                            }
                            MainWin.WriteJobsJson(JobNotes, MainWin.JobNotesPath);
                            break;
                        }
                    }
                    for (int i = 0; i < lstAttachedFiles.SelectedItems.Count; i++)
                    {
                        AttachedFileList.Remove(lstAttachedFiles.SelectedItems[i].ToString());
                        i = -1;
                    }
                }
            }
            MainWin.RefreshTimer.Start();
        }

        private void HandleOpChecked(object sender, RoutedEventArgs e)
        {
            MainWin.RefreshTimer.Stop();
            if (isAddingBoxes == true) return;
            Job LastSelectedJob = lstAssignedJobs.SelectedItem as Job;
            User LastSelectedUser = lstUsers.SelectedItem as User;
            CheckBox checkBox = sender as CheckBox;
            bool AllAssignmentsDone = false;
            bool isJobInCompletedJobs = false;
            bool AllOpsDone = false;
            int AssignmentCheckedCounter = 0;
            List<CheckBox> AllAssignments = new List<CheckBox>();
            if (LastSelectedJob != null && LastSelectedUser != null && lstStatus.Items.Count > 0)
            {
                if (checkBox.IsChecked == true)
                {
                    foreach (Job job in AssignedJobList)
                    {
                        if (job.JobInfo["JobNo"] == LastSelectedJob.JobInfo["JobNo"])
                        {
                            job.JobInfo["CompletedOperations"][checkBox.Tag] = checkBox.IsChecked;
                            MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                        }
                    }
                }
                foreach (object item in lstStatus.Items)
                {
                    if (item.GetType() == typeof(CheckBox))
                    {
                        CheckBox OpCheckBox = item as CheckBox;
                        if (OpCheckBox.Name == LastSelectedUser.Username)
                        {
                            AllAssignments.Add(OpCheckBox);
                        }
                    }
                }
                if (AllAssignments.Count > 0)
                {
                    foreach (CheckBox OpCheckBox in AllAssignments)
                    {
                        if (OpCheckBox.IsChecked == true)
                        {
                            AssignmentCheckedCounter++;
                        }
                    }
                    if (AssignmentCheckedCounter == AllAssignments.Count)
                    {
                        AllAssignmentsDone = true;
                    }

                    if (AllAssignmentsDone)
                    {
                        foreach (Job job2 in AssignedJobList)
                        {
                            if (job2.JobInfo["JobNo"] == LastSelectedJob.JobInfo["JobNo"])
                            {
                                for (int i = 0; i < job2.JobInfo["AssignedEmployees"].Count; i++)
                                {
                                    if (job2.JobInfo["AssignedEmployees"][i] == checkBox.Name)
                                    {
                                        job2.JobInfo["AssignedEmployees"].RemoveAt(i);
                                        i = 0;
                                    }
                                }
                                if (job2.JobInfo["AssignedEmployees"].Count == 0)
                                {
                                    AllOpsDone = true;
                                    UnassignOperations(LastSelectedJob);
                                    AssignedJobList.Remove(job2);
                                }
                                break;
                            }
                        }
                        if (AllOpsDone)
                        {
                            foreach (Job job in CompletedJobs)
                            {
                                if (job.JobInfo["JobNo"] == LastSelectedJob.JobInfo["JobNo"])
                                {
                                    isJobInCompletedJobs = true;
                                    break;
                                }
                            }
                            if (!isJobInCompletedJobs)
                            {
                                string CompleteDate = DateTime.Now.ToString("MM/dd/yyyy h:mm tt");
                                LastSelectedJob.JobInfo.Add("CompletedDate", CompleteDate);
                                CompletedJobs.Add(LastSelectedJob);
                            }
                        }
                        MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                        MainWin.WriteJobsJson(CompletedJobs, MainWin.CompletedJobsPath);
                        RefreshJobs();
                        RefreshWindow(lstAssignedJobs);
                        CountJobs();
                    }
                }
            }
            MainWin.RefreshTimer.Start();
        }

        private void HandleOpUnchecked(object sender, RoutedEventArgs e)
        {
            MainWin.RefreshTimer.Stop();
            LastSelectedJob = lstAssignedJobs.SelectedItem;
            CheckBox checkBox = sender as CheckBox;
            Job SelectedJob = LastSelectedJob as Job;
            if (SelectedJob != null)
            {
                if (checkBox.IsChecked == false)
                {
                    foreach (Job job in AssignedJobList)
                    {
                        if (job.JobInfo["JobNo"] == SelectedJob.JobInfo["JobNo"])
                        {
                            job.JobInfo["CompletedOperations"][checkBox.Tag] = checkBox.IsChecked;
                            MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                        }
                    }
                }
            }
            MainWin.RefreshTimer.Start();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFileName != null)
            {
                string FilePath = SelectedFileName.ToString();
                System.Diagnostics.Process.Start(FilePath);
            }
        }

        private void btnNewJob_Click(object sender, RoutedEventArgs e)
        {

        }

        private void lstAssignedJobs_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LastSelectedJob = lstAssignedJobs.SelectedItem;
            isAddingBoxes = true;

            if (LastSelectedJob == null) { return; }

            if (SelectedOperations != null)
            {
                if (SelectedOperations.Count > 0)
                {
                    SelectedOperations.Clear();
                    OperationList.Clear();
                    AssignedEmployeeList.Clear();
                    lstOperationTimes.Items.Clear();
                }
            }

            if (lstStatus != null)
            {
                if (lstStatus.Items.Count > 0)
                {
                    lstStatus.Items.Clear();
                }
            }

            if (AttachedFileList != null)
            {
                if (AttachedFileList.Count > 0) { AttachedFileList.Clear(); }
            }

            if ((LastSelectedJob as Job).JobInfo.ContainsKey("AttachedFiles"))
            {

                foreach (string filename in (LastSelectedJob as Job).JobInfo["AttachedFiles"])
                {
                    AttachedFileList.Add(filename);
                }
            }

            CompletedOperations = JsonConvert.DeserializeObject<Dictionary<string, bool>>((LastSelectedJob as Job).JobInfo["CompletedOperations"].ToString());
            SelectedOperations = JsonConvert.DeserializeObject<Dictionary<string, string>>((LastSelectedJob as Job).JobInfo["Operations"].ToString());
            SelectedTimes = JsonConvert.DeserializeObject<Dictionary<string, int>>((LastSelectedJob as Job).JobInfo["OperationTime"].ToString());

            try
            {
                foreach (string key in CompletedOperations.Keys)
                {
                    foreach (KeyValuePair<string, string> kvp in SelectedOperations)
                    {
                        if (kvp.Key == key)
                        {
                            CheckBox OpCheck = new CheckBox
                            {
                                Tag = key,
                                Content = "Complete",
                                Name = SelectedOperations[key].Replace(" ", ""),
                            };
                            OpCheck.Checked += new RoutedEventHandler(HandleOpChecked);
                            OpCheck.Unchecked += new RoutedEventHandler(HandleOpUnchecked);
                            if (CompletedOperations[key] == true)
                            {
                                OpCheck.IsChecked = true;
                            }
                            lstStatus.Items.Add(OpCheck);
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                return;
            }

            isAddingBoxes = false;

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
            CalculateOpTime(SelectedTimes);
            txtNotes.Text = (LastSelectedJob as Job).JobInfo["Notes"].ToString();
        }

        private void lstJobs_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LastSelectedJob = lstJobs.SelectedItem;
            isAddingBoxes = true;

            if (LastSelectedJob == null) { return; }

            if (SelectedOperations != null)
            {
                if (SelectedOperations.Count > 0)
                {
                    SelectedOperations.Clear();
                    OperationList.Clear();
                    AssignedEmployeeList.Clear();
                    lstOperationTimes.Items.Clear();
                }
            }

            if (AttachedFileList != null)
            {
                if (AttachedFileList.Count > 0) { AttachedFileList.Clear(); }
            }

            if (lstStatus != null)
            {
                if (lstStatus.Items.Count > 0)
                {
                    lstStatus.Items.Clear();
                }
            }

            if ((LastSelectedJob as Job).JobInfo.ContainsKey("AttachedFiles"))
            {

                foreach (string filename in (LastSelectedJob as Job).JobInfo["AttachedFiles"])
                {
                    AttachedFileList.Add(filename);
                }
            }

            CompletedOperations = JsonConvert.DeserializeObject<Dictionary<string, bool>>((LastSelectedJob as Job).JobInfo["CompletedOperations"].ToString());
            SelectedOperations = JsonConvert.DeserializeObject<Dictionary<string, string>>((LastSelectedJob as Job).JobInfo["Operations"].ToString());
            SelectedTimes = JsonConvert.DeserializeObject<Dictionary<string, int>>((LastSelectedJob as Job).JobInfo["OperationTime"].ToString());

            try
            {
                foreach (string key in CompletedOperations.Keys)
                {
                    foreach (KeyValuePair<string, string> kvp in SelectedOperations)
                    {
                        if (kvp.Key == key)
                        {
                            CheckBox OpCheck = new CheckBox
                            {
                                Tag = key,
                                Content = "Complete",
                                Name = SelectedOperations[key].Replace(" ", ""),
                            };
                            OpCheck.Checked += new RoutedEventHandler(HandleOpChecked);
                            OpCheck.Unchecked += new RoutedEventHandler(HandleOpUnchecked);
                            if (CompletedOperations[key] == true)
                            {
                                OpCheck.IsChecked = true;
                            }
                            lstStatus.Items.Add(OpCheck);
                        }
                    }
                }
            }
            catch (InvalidOperationException)
            {
                return;
            }
            isAddingBoxes = false;

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
            CalculateOpTime(SelectedTimes);
            txtNotes.Text = (LastSelectedJob as Job).JobInfo["Notes"].ToString();
        }

        private void lstOperations_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectedOperation = lstOperations.SelectedItem;
        }

        private void lstUsers_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LastSelectedUser = lstUsers.SelectedItem;
            if (LastSelectedUser == null) return;
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
                    lstOperationTimes.Items.Clear();
                }
            }

            if (AttachedFileList != null)
            {
                if (AttachedFileList.Count > 0)
                {
                    AttachedFileList.Clear();
                }
            }

            if (lstStatus != null)
            {
                if (lstStatus.Items.Count > 0)
                {
                    lstStatus.Items.Clear();
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
            CalculateTime();
            if (lstJobs.SelectedItem != null)
            {
                LastSelectedJob = lstJobs.SelectedItem;
            }
        }

        private void lstAssigned_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectedAssignee = lstAssigned.SelectedItem;
        }

        private void lstAttachedFiles_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectedFileName = lstAttachedFiles.SelectedItem;
        }
    }
}
