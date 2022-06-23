using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.Data.OleDb;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.IO;
using Newtonsoft.Json;
using System.Timers;

namespace IPM_Job_Manager_net
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
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

        private ObservableCollection<Job> _jobNotes = new ObservableCollection<Job>();
        public ObservableCollection<Job> JobNotes
        {
            get { return _jobNotes; }
            set { _jobNotes = value; }
        }

        private ObservableCollection<Job> _CompletedJobs = new ObservableCollection<Job>();
        public ObservableCollection<Job> CompletedJobs
        {
            get { return _CompletedJobs; }
            set { _CompletedJobs = value; }
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

        private Dictionary<string, bool> _completedOperations = new Dictionary<string, bool>();
        public Dictionary<string, bool> CompletedOperations
        {
            get { return _completedOperations; }
            set { _completedOperations = value; }
        }

        private ObservableCollection<string> _attachedFileList = new ObservableCollection<string>();
        public ObservableCollection<string> AttachedFileList
        {
            get { return _attachedFileList; }
            set { _attachedFileList = value; }
        }

        private Root _jsonUserList = new Root();
        public Root JsonUserList
        {
            get { return _jsonUserList; }
            set { _jsonUserList = value; }
        }

        public const int MaxPriority = 20;

        public bool isAddingBoxes;

        public object SelectedOperation;
        public object SelectedFileName;

        public object LastSelectedUser;
        private object _lastSelectedJob;
        public object LastSelectedJob
        {
            get { return _lastSelectedJob; }
            set { _lastSelectedJob = value; }
        }

        public Timer RefreshTimer;

        public string AssignedJobListPath;
        public string JobListPath;
        public string UserListPath;
        public string JobNotesPath;
        public string CompletedJobsPath;
        public string QueryString = "SELECT * FROM [Job List]";

        #region DataSet, DataAdapter, DataTable
        public DataSet dataSet;
        public OleDbDataAdapter dataAdapter;
        public DataTable dataTable;
        public OleDbConnection connection;
        #endregion

        private ObservableCollection<Job> _assignedJobList = new ObservableCollection<Job>();
        public ObservableCollection<Job> AssignedJobList
        {
            get { return _assignedJobList; }
            set { _assignedJobList = value; }
        }

        public MainWindow()
        {
            InitializeComponent();
            this.LocationChanged += new EventHandler(SaveWindowPos);
            this.SizeChanged += new SizeChangedEventHandler(SaveWindowSize);
            Left = Properties.Settings.Default.SavedLeft;
            Top = Properties.Settings.Default.SavedTop;
            this.DataContext = this;
            this.Show();
            if (Properties.Settings.Default.isFirstTimeSetupDone != true)
            {
                FirstTimeSetupWindow SetupWin = new FirstTimeSetupWindow();
                SetupWin.Owner = this;
                bool? DialogResult = SetupWin.ShowDialog();
                if (DialogResult == true)
                {
                    Properties.Settings.Default.isFirstTimeSetupDone = true;
                    Properties.Settings.Default.Save();
                }
            }
            AssignedJobListPath = Properties.Settings.Default.DataFileDirectory + @"\assigned_job_list.json";
            JobListPath = Properties.Settings.Default.DataFileDirectory + @"\job_list.json";
            UserListPath = Properties.Settings.Default.DataFileDirectory + @"\Users.json";
            JobNotesPath = Properties.Settings.Default.DataFileDirectory + @"\job_notes.json";
            CompletedJobsPath = Properties.Settings.Default.DataFileDirectory + @"\completed_jobs.json";
            if (!File.Exists(UserListPath))
            {
                User admin = new User();
                admin.IsAdmin = true;
                admin.Username = "admin";
                JsonUserList.Users.Add(admin);
                WriteUserJson(JsonUserList, UserListPath);
            }
            try
            {
                JsonUserList = ReadUserJson(UserListPath);
                foreach (User user in JsonUserList.Users)
                {
                    UserList.Add(user);
                }
            }
            catch (FileNotFoundException)
            {
                return;
            }
            if (!File.Exists(AssignedJobListPath))
            {
                WriteJobsJson(AssignedJobList, AssignedJobListPath);
            }
            if (!File.Exists(JobListPath))
            {
                WriteJobsJson(JobList, JobListPath);
            }
            if (!File.Exists(JobNotesPath))
            {
                WriteJobsJson(JobNotes, JobNotesPath);
            }
            if (Properties.Settings.Default.isInDemoMode != true)
            {
                GetData(QueryString, global::IPM_Job_Manager_net.Properties.Settings.Default.open_workConnectionString);
            }
            try
            {
                JobList = ReadJobsJson(JobListPath);
                AssignedJobList = ReadJobsJson(AssignedJobListPath);
                JobNotes = ReadJobsJson(JobNotesPath);
            }
            catch (FileNotFoundException)
            {
                return;
            }
            SetTimer();
            JobList = ReadJobNotes(JobList);
            AssignedJobList = ReadJobNotes(AssignedJobList);
            WriteJobsJson(AssignedJobList, AssignedJobListPath);
            try
            {
                CompletedJobs = ReadJobsJson(CompletedJobsPath);
            }
            catch (FileNotFoundException)
            {
                return;
            }
            foreach (Job job in AssignedJobList)
            {
                job.JobInfo["EstDays"] = CalculateDays(job);
            }
            WriteJobsJson(AssignedJobList, AssignedJobListPath);
            CountJobs();
        }

        public void SetTimer()
        {
            // Create a timer with a two second interval.
            RefreshTimer = new Timer(300000);
            // Hook up the Elapsed event for the timer. 
            RefreshTimer.Elapsed += OnTimedEvent;
            RefreshTimer.AutoReset = true;
            RefreshTimer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            JobList.Clear();
            if (Properties.Settings.Default.isInDemoMode != true)
            {
                GetData(QueryString, global::IPM_Job_Manager_net.Properties.Settings.Default.open_workConnectionString);
            }
            try
            {
                JobList = ReadJobsJson(JobListPath);
                AssignedJobList = ReadJobsJson(AssignedJobListPath);
                JobNotes = ReadJobsJson(JobNotesPath);
            }
            catch (FileNotFoundException)
            {
                return;
            }
            JobList = ReadJobNotes(JobList);
            AssignedJobList = ReadJobNotes(AssignedJobList);
            foreach (Job job in AssignedJobList)
            {
                job.JobInfo["EstDays"] = CalculateDays(job);
            }
            WriteJobsJson(AssignedJobList, AssignedJobListPath);
            CompletedJobs.Clear();
            CompletedJobs = ReadJobsJson(CompletedJobsPath);
            if (LastSelectedUser != null && LastSelectedJob != null)
            {
                this.Dispatcher.Invoke(() => RefreshJobs());
            }
            CountJobs();
            this.Dispatcher.Invoke(() => RefreshAdminJobs());
        }

        public void RefreshAdminJobs()
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.Title == "IPM Job Manager")
                {
                    AdminWindow adminWindow = (AdminWindow)window;
                    adminWindow.JobList.Clear();
                    adminWindow.AssignedJobList.Clear();
                    adminWindow.JobNotes.Clear();
                    adminWindow.UserList.Clear();
                    JsonUserList.Users.Clear();
                    JsonUserList = ReadUserJson(UserListPath);
                    foreach (User user in JsonUserList.Users)
                    {
                        adminWindow.UserList.Add(user);
                    }
                    foreach (Job job1 in JobList)
                    {
                        adminWindow.JobList.Add(job1);
                    }
                    foreach (Job job2 in JobNotes)
                    {
                        adminWindow.JobNotes.Add(job2);
                    }
                    foreach (Job job3 in AssignedJobList)
                    {
                        adminWindow.AssignedJobList.Add(job3);
                    }
                    adminWindow.CountJobs();
                }
            }
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

        public ObservableCollection<Job> ReadJobNotes(ObservableCollection<Job> jobs)
        {
            foreach (Job job in jobs)
            {
                foreach (Job jobnotes in JobNotes)
                {
                    if (jobnotes.JobInfo["PartNo"] == job.JobInfo["PartNo"])
                    {
                        if (jobnotes.JobInfo.ContainsKey("Notes"))
                        {
                            job.JobInfo["Notes"] = jobnotes.JobInfo["Notes"];
                        }
                        if (jobnotes.JobInfo.ContainsKey("Operations"))
                        {
                            job.JobInfo["Operations"] = jobnotes.JobInfo["Operations"];
                        }
                        if (jobnotes.JobInfo.ContainsKey("AttachedFiles"))
                        {
                            job.JobInfo["AttachedFiles"] = jobnotes.JobInfo["AttachedFiles"];
                        }
                    }
                }
            }
            return jobs;
        }

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

        public Root ReadUserJson(string JsonPath)
        {
            using (StreamReader sr = new StreamReader(JsonPath))
            {
                string json = sr.ReadToEnd();
                Root JsonUserList = JsonConvert.DeserializeObject<Root>(json);
                return JsonUserList;
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

        public void WriteUserJson(Root users, string JsonPath)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.TypeNameHandling = TypeNameHandling.All;

            using (StreamWriter sw = new StreamWriter(JsonPath))
            using (JsonWriter jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                serializer.Serialize(jsonWriter, users);
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

        public int CalculateTime(int time, int totalQty)
        {
            int TotalTime = (time) * totalQty;
            return TotalTime;
        }

        public void GetData(string SelectQuery, string ConnectionString)
        {
            try
            {
                JobList.Clear();
                #region Create Data Objects: Connection, DataAdapter, DataSet, DataTable
                // use OleDb Connection to MS Access DB
                connection = new OleDbConnection(ConnectionString);
                connection.Open();

                // create new DataAdapter on OleDb Connection and Select Query text
                dataAdapter = new OleDbDataAdapter();
                dataAdapter.SelectCommand = new OleDbCommand(SelectQuery, connection);


                // create DataSet
                dataSet = new DataSet("OpenWork");
                dataTable = new DataTable("JobList");
                dataSet.Clear();
                dataTable.Clear();
                // use DataAdapter to Fill Dataset
                dataAdapter.Fill(dataSet);
                dataTable = dataSet.Tables[0];

                List<string> AssignedEmployeeList = new List<string>();
                Dictionary<string, string> Operations = new Dictionary<string, string>();
                Dictionary<string, bool> CompletedOperations = new Dictionary<string, bool>();
                List<string> AttachedFileList = new List<string>();
                Dictionary<string, int> OperationTime = new Dictionary<string, int>();

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    JobList.Add(new Job());
                    JobList[i].JobInfo.Add("AssignedEmployees", AssignedEmployeeList);
                    JobList[i].JobInfo.Add("Operations", Operations);
                    JobList[i].JobInfo.Add("CompletedOperations", CompletedOperations);
                    JobList[i].JobInfo.Add("Notes", "");
                    JobList[i].JobInfo.Add("EstDays", 0);
                    JobList[i].JobInfo.Add("Priority", 0);
                    JobList[i].JobInfo.Add("EstTime", "");
                    JobList[i].JobInfo.Add("AttachedFiles", AttachedFileList);
                    JobList[i].JobInfo.Add("OperationTime", OperationTime);
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        JobList[i].JobInfo.Add(column.ColumnName.ToString(), dataTable.Rows[i][column].ToString());
                    }
                }

                WriteJobsJson(JobList, JobListPath);


                // OPTIONAL: use OleDbCommandBuilder to build a complete set of CRUD commands
                //OleDbCommandBuilder builder = new OleDbCommandBuilder(dataAdapter);
                //Update, Insert and Delete Commands
                //dataAdapter.UpdateCommand = builder.GetUpdateCommand();
                //dataAdapter.InsertCommand = builder.GetInsertCommand();
                //dataAdapter.DeleteCommand = builder.GetDeleteCommand();
                #endregion

                connection.Close();
            }
            catch { throw; }
        }

        public ObservableCollection<Job> SortJobs(ObservableCollection<Job> Jobs)
        {
            IEnumerable<Job> SortedByDays = Jobs.OrderBy(job => job.JobInfo["EstDays"]);
            var SortedJobs = new ObservableCollection<Job>(SortedByDays);
            IEnumerable<Job> SortedByPrio = SortedJobs.OrderBy(job => job.JobInfo["Priority"]);
            SortedJobs = new ObservableCollection<Job>(SortedByPrio);
            return SortedJobs;
        }

        public void UnassignOperations(Job job)
        {
            JobNotes = ReadJobsJson(JobNotesPath);

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
                WriteJobsJson(AssignedJobList, AssignedJobListPath);
                WriteJobsJson(JobList, JobListPath);
                WriteJobsJson(JobNotes, JobNotesPath);
            }
        }

        public void RefreshJobs()
        {
            LastSelectedUser = lstUsers.SelectedItem;
            lstStatus.Items.Clear();
            if (LastSelectedUser == null) { return; }
            if (CurEmployeeJobs.Count != 0)
            {
                CurEmployeeJobs.Clear();
            }

            try
            {
                AssignedJobList = ReadJobsJson(AssignedJobListPath);
                JobNotes = ReadJobsJson(JobNotesPath);
            }
            catch (FileNotFoundException)
            {
                return;
            }
            JobList = ReadJobNotes(JobList);
            AssignedJobList = ReadJobNotes(AssignedJobList);
            WriteJobsJson(AssignedJobList, AssignedJobListPath);

            User CurItem = LastSelectedUser as User;

            ObservableCollection<Job> jobs = new ObservableCollection<Job>();
            jobs = ReadJobsJson(AssignedJobListPath);

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

            var SortedJobs = SortJobs(CurEmployeeJobs);
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

        public void RefreshWindow()
        {
            LastSelectedUser = lstUsers.SelectedItem;
            int LastJobIndex = lstAssignedJobs.SelectedIndex;

            if (CurEmployeeJobs.Count != 0)
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
                jobs = ReadJobsJson(AssignedJobListPath);

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
            var SortedJobs = SortJobs(CurEmployeeJobs);
            CurEmployeeJobs.Clear();
            foreach (Job sortedjob in SortedJobs)
            {
                CurEmployeeJobs.Add(sortedjob);
            }

            if (CurEmployeeJobs.Count > 0)
            {
                try
                {
                    lstAssignedJobs.SelectedItem = CurEmployeeJobs[LastJobIndex];
                }
                catch (ArgumentOutOfRangeException)
                {
                    lstAssignedJobs.SelectedItem = CurEmployeeJobs[0];
                }
                LastSelectedJob = lstAssignedJobs.SelectedItem;
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

            if (LastSelectedJob == null) { return; }

            SelectedOperations = JsonConvert.DeserializeObject<Dictionary<string, string>>((LastSelectedJob as Job).JobInfo["Operations"].ToString());
            SelectedTimes = JsonConvert.DeserializeObject<Dictionary<string, int>>((LastSelectedJob as Job).JobInfo["OperationTime"].ToString());
            foreach (string key in SelectedOperations.Keys)
            {
                OperationList.Add(key);
            }

            foreach (string value in SelectedOperations.Values)
            {
                AssignedEmployeeList.Add(value);
            }

            CalculateOpTime(SelectedTimes);

            if (OperationList.Count > 0)
            {
                while (OperationList.Count > AssignedEmployeeList.Count)
                {
                    AssignedEmployeeList.Add("");
                }
            }
            txtNotes.Text = (LastSelectedJob as Job).JobInfo["Notes"].ToString();
        }

        private void btnEditNotes_Click(object sender, RoutedEventArgs e)
        {
            Job LastSelectedJob = lstAssignedJobs.SelectedItem as Job;
            if (LastSelectedJob != null)
            {
                RefreshTimer.Stop();
                Window NotesWin = new NotesWindow(LastSelectedJob);
                NotesWin.Owner = this;
                bool? DialogResult = NotesWin.ShowDialog();
                if (DialogResult == true)
                {
                    RefreshWindow();
                }
            }
        }

        private void btnAdminLogin_Click(object sender, RoutedEventArgs e)
        {
            RefreshTimer.Stop();
            var AdminWin = new AdminWindow();
            var LoginWin = new Login(this, JsonUserList, AdminWin);
            AdminWin.Owner = this;
            LoginWin.Owner = this;
            LoginWin.ShowDialog();
            RefreshTimer.Start();
        }

        private void HandleOpChecked(object sender, RoutedEventArgs e)
        {
            if (isAddingBoxes == true) return;
            Job LastSelectedJob = lstAssignedJobs.SelectedItem as Job;
            User LastSelectedUser = lstUsers.SelectedItem as User;
            CheckBox checkBox = sender as CheckBox;
            bool AllAssignmentsDone = false;
            bool isJobInCompletedJobs = false;
            bool AllOpsDone = false;
            int AssignmentCheckedCounter = 0;
            List<CheckBox> AllAssignments = new List<CheckBox>();
            if (checkBox.Name != LastSelectedUser.Username) { return; }
            if (LastSelectedJob != null && LastSelectedUser != null && lstStatus.Items.Count > 0)
            {
                if (checkBox.IsChecked == true)
                {
                    foreach (Job job in AssignedJobList)
                    {
                        if (job.JobInfo["JobNo"] == LastSelectedJob.JobInfo["JobNo"])
                        {
                            job.JobInfo["CompletedOperations"][checkBox.Tag] = checkBox.IsChecked;
                            WriteJobsJson(AssignedJobList, AssignedJobListPath);
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
                        WriteJobsJson(AssignedJobList, AssignedJobListPath);
                        WriteJobsJson(CompletedJobs, CompletedJobsPath);
                        RefreshJobs();
                        CountJobs();
                    }
                }
            }
        }

        private void HandleOpUnchecked(object sender, RoutedEventArgs e)
        {
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
                            WriteJobsJson(AssignedJobList, AssignedJobListPath);
                        }
                    }
                }
            }
        }

        private void SaveWindowPos(object sender, EventArgs e)
        {
            Properties.Settings.Default.SavedLeft = Left;
            Properties.Settings.Default.SavedTop = Top;
            Properties.Settings.Default.Save();
        }

        private void SaveWindowSize(object sender, SizeChangedEventArgs e)
        {
            Properties.Settings.Default.SavedHeight = Height;
            Properties.Settings.Default.SavedWidth = Width;
            Properties.Settings.Default.Save();
        }

        private void btnOpenFile_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedFileName != null)
            {
                string FilePath = SelectedFileName.ToString();
                System.Diagnostics.Process.Start(FilePath);
            }
        }

        private void lstAssignedJobs_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Job LastSelectedJob = lstAssignedJobs.SelectedItem as Job;
            User LastSelectedUser = lstUsers.SelectedItem as User;
            isAddingBoxes = true;

            if (LastSelectedJob == null) { return; }
            if (LastSelectedUser == null) { return; }
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

                foreach (string filename in LastSelectedJob.JobInfo["AttachedFiles"])
                {
                    AttachedFileList.Add(filename);
                }
            }

            CompletedOperations = JsonConvert.DeserializeObject<Dictionary<string, bool>>(LastSelectedJob.JobInfo["CompletedOperations"].ToString());
            SelectedOperations = JsonConvert.DeserializeObject<Dictionary<string, string>>(LastSelectedJob.JobInfo["Operations"].ToString());
            SelectedTimes = JsonConvert.DeserializeObject<Dictionary<string, int>>(LastSelectedJob.JobInfo["OperationTime"].ToString());

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

            CalculateOpTime(SelectedTimes);

            if (OperationList.Count > 0)
            {
                while (OperationList.Count > AssignedEmployeeList.Count)
                {
                    AssignedEmployeeList.Add("");
                }
            }
            txtNotes.Text = LastSelectedJob.JobInfo["Notes"].ToString();
        }

        private void lstAttachedFiles_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectedFileName = lstAttachedFiles.SelectedItem;
        }

        private void lstUsers_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LastSelectedUser = lstUsers.SelectedItem;

            if (LastSelectedUser == null) return;
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

            if (CurEmployeeJobs.Count != 0)
            {
                CurEmployeeJobs.Clear();
            }

            User CurItem = lstUsers.SelectedItem as User;

            ObservableCollection<Job> jobs = new ObservableCollection<Job>();
            try
            {
                jobs = ReadJobsJson(AssignedJobListPath);

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
            var SortedJobs = SortJobs(CurEmployeeJobs);
            CurEmployeeJobs.Clear();
            foreach (Job sortedjob in SortedJobs)
            {
                CurEmployeeJobs.Add(sortedjob);
            }
        }

        private void lstOperations_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectedOperation = lstOperations.SelectedItem;
        }

        private void btnClockIn_Click(object sender, RoutedEventArgs e)
        {
            if (lstOperations.SelectedItem != null && lstUsers.SelectedItem != null && lstAssignedJobs.SelectedItem != null)
            {
                User SelectedUser = lstUsers.SelectedItem as User;
                Job SelectedJob = lstAssignedJobs.SelectedItem as Job;
                string SelectedOperation = lstOperations.SelectedItem.ToString();
                if (SelectedUser != null && SelectedJob != null && SelectedOperation != null)
                {
                    foreach (User user in JsonUserList.Users)
                    {
                        if (user.Username == SelectedUser.Username)
                        {
                            user.TimeCard.ClockedInJobs.Add(SelectedJob.JobInfo["JobNo"], DateTime.Now);
                            user.TimeCard.TrackedOperations.Add(SelectedJob.JobInfo["JobNo"], SelectedOperation);
                            if (user.TimeCard.ClockedInJobs.Keys.Count > 0)
                            {
                                user.isPunchedIn = true;
                            }
                            WriteUserJson(JsonUserList, UserListPath);
                            return;
                        }
                    }
                }
                else if (SelectedJob == null)
                {
                    MessageBox.Show("Clock in error, please select a job to clock into.", "Clock In Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (SelectedUser == null)
                {
                    MessageBox.Show("Clock in error, please select a employee to clock in as.", "Clock In Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else if (SelectedOperation == null)
                {
                    MessageBox.Show("Clock in error, please select a operation to clock into.", "Clock In Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void btnClockOut_Click(object sender, RoutedEventArgs e)
        {
            User SelectedUser = lstUsers.SelectedItem as User;
            int PartsDone;
            int CycleTime;
            if (SelectedUser != null)
            {
                foreach (User user in JsonUserList.Users)
                {
                    if (user.Username == SelectedUser.Username)
                    {
                        foreach (KeyValuePair<string, DateTime> kvp in user.TimeCard.ClockedInJobs)
                        {
                            QuantityWindow QuantityWin = new QuantityWindow();
                            QuantityWin.Owner = this;
                            bool? DialogResult = QuantityWin.ShowDialog();
                            if (DialogResult == true)
                            {
                                PartsDone = QuantityWin.PartsFinished;
                                CycleTime = CalculateClock(kvp.Value, PartsDone);
                                foreach (Job job in AssignedJobList)
                                {
                                    if (job.JobInfo["JobNo"] == kvp.Key)
                                    {
                                        foreach (KeyValuePair<string, string> kvp2 in user.TimeCard.TrackedOperations)
                                        {
                                            if (kvp.Key == kvp2.Key)
                                            {
                                                job.JobInfo["OperationTime"][kvp2.Value] = CycleTime;
                                            }
                                        }
                                        break;
                                    }
                                }
                            }
                        }
                        user.TimeCard.ClockedInJobs.Clear();
                        user.TimeCard.TrackedOperations.Clear();
                        user.isPunchedIn = false;
                        WriteUserJson(JsonUserList, UserListPath);
                        WriteJobsJson(AssignedJobList, AssignedJobListPath);
                        RefreshWindow();
                        break;
                    }
                }
            }
        }

        public int CalculateClock(DateTime time, int parts)
        {
            TimeSpan TotalTime = new TimeSpan();
            TotalTime = DateTime.Now - time;
            double TotalSeconds = TotalTime.TotalSeconds;
            double CycleTime = TotalSeconds / parts;
            Math.Round(CycleTime, 0, MidpointRounding.AwayFromZero);
            return (int)CycleTime;
        }

        public int CalculateDays(Job job)
        {
            TimeSpan DaysLeft = new TimeSpan();
            DateTime CurrentDate = DateTime.Now;
            DaysLeft = DateTime.Parse(job.JobInfo["DueDate"]) - CurrentDate;
            return DaysLeft.Days;
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
