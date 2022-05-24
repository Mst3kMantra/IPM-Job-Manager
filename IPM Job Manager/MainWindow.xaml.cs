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
        public ObservableCollection <Job> JobNotes
        {
            get { return _jobNotes; }
            set { _jobNotes = value; }
        }

        private Dictionary<string, string> _selectedOperations = new Dictionary<string, string>();
        public Dictionary<string, string> SelectedOperations
        {
            get { return _selectedOperations; }
            set { _selectedOperations = value; }
        }

        public Root JsonUserList;

        public const int MaxPriority = 20;

        public object SelectedOperation;
        public object SelectedAssignee;

        public object LastSelectedUser;
        private object _lastSelectedJob;
        public object LastSelectedJob
        {
            get { return _lastSelectedJob; }
            set { _lastSelectedJob = value; }
        }

        public string AssignedJobListPath = @"I:\Program Files\IPM Job Manager\Data Files\assigned_job_list.json";
        public string JobListPath = @"I:\Program Files\IPM Job Manager\Data Files\job_list.json";
        public string UserListPath = @"I:\Program Files\IPM Job Manager\Data Files\Users.json";
        public string JobNotesPath = @"I:\Program Files\IPM Job Manager\Data Files\job_notes.json";
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
            this.DataContext = this;
            JsonUserList = ReadUserJson(UserListPath);
            foreach (User user in JsonUserList.Users)
            {
                UserList.Add(user);
            }
            GetData(QueryString, global::IPM_Job_Manager_net.Properties.Settings.Default.open_workConnectionString);
            JobList = ReadJobsJson(JobListPath);
            try
            {
                AssignedJobList = ReadJobsJson(AssignedJobListPath);
            }
            catch (FileNotFoundException)
            {
                return;
            }
            try
            {
                JobNotes = ReadJobsJson(JobNotesPath);
            }
            catch (FileNotFoundException)
            {
                return;
            }
            JobList = ReadJobNotes(JobList);
            AssignedJobList = ReadJobNotes(AssignedJobList);
            WriteJobsJson(AssignedJobList, AssignedJobListPath);
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

                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    JobList.Add(new Job());
                    JobList[i].JobInfo.Add("AssignedEmployees", AssignedEmployeeList);
                    JobList[i].JobInfo.Add("Operations", Operations);
                    JobList[i].JobInfo.Add("Notes", "");
                    JobList[i].JobInfo.Add("Priority", 0);
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
            IEnumerable<Job> SortedByPrio = Jobs.OrderBy(job => job.JobInfo["Priority"]);
            var SortedJobs = new ObservableCollection<Job>(SortedByPrio);
            return SortedJobs;
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

            lstAssignedJobs.SelectedItem = CurEmployeeJobs[LastJobIndex];
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

        private void btnEditNotes_Click(object sender, RoutedEventArgs e)
        {
            if (LastSelectedJob != null)
            {
                Window NotesWin = new NotesWindow(LastSelectedJob as Job);
                NotesWin.Owner = this;
                bool? DialogResult = NotesWin.ShowDialog();
                if (DialogResult == true)
                {
                    RefreshWindow();
                }
            }
        }

        private void ButtonAdminLogin_Click(object sender, RoutedEventArgs e)
        {
            var AdminWin = new AdminWindow();
            var LoginWin = new Login(this, JsonUserList, AdminWin);
            AdminWin.Owner = this;
            LoginWin.Owner = this;
            LoginWin.ShowDialog();
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

        private void lstUsers_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LastSelectedUser = lstUsers.SelectedItem;
            Job SelectedJob = LastSelectedJob as Job;

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

        private void lstOperations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedOperation = lstOperations.SelectedItem;
        }

        private void lstAssigned_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedAssignee = lstAssigned.SelectedItem;
        }

        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            if (LastSelectedUser != null && LastSelectedJob != null)
            {
                RefreshWindow();
            }
        }
    }
}
