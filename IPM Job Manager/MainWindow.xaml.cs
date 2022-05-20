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

        private Dictionary<string, string> _selectedOperations = new Dictionary<string, string>();
        public Dictionary<string, string> SelectedOperations
        {
            get { return _selectedOperations; }
            set { _selectedOperations = value; }
        }

        public Root JsonUserList;

        public object SelectedOperation;
        public object SelectedAssignee;

        public object LastSelectedUser;
        public object LastSelectedJob;

        public string AssignedJobListPath = @"I:\GTMT\test\assigned_job_list.json";
        public string JobListPath = @"I:\GTMT\test\job_list.json";
        public string UserListPath = @"I:\GTMT\test\Users.json";

        #region DataSet, DataAdapter, DataTable
        internal DataSet dataSet;
        internal OleDbDataAdapter dataAdapter;
        internal DataTable dataTable;
        private OleDbConnection connection;
        #endregion

        private ObservableCollection<Job> _assignedJobList = new ObservableCollection<Job>();
        public ObservableCollection<Job> AssignedJobList
        {
            get { return _assignedJobList; }
            set { _assignedJobList = value; }
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

        internal void GetData(string SelectQuery, string ConnectionString)
        {
            try
            {

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

        public MainWindow()
        {
            string QueryString = "SELECT * FROM [Job List]";
            InitializeComponent();
            this.DataContext = this;
            JsonUserList = ReadUserJson("I:/GTMT/test/Users.json");
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
        }

        private void btnViewJob_Click(object sender, RoutedEventArgs e)
        {
            Job SelectedJob = LastSelectedJob as Job;

            if (SelectedJob == null)
            {
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

        private void ButtonAdminLogin_Click(object sender, RoutedEventArgs e)
        {
            var AdminWin = new AdminWindow();
            var LoginWin = new Login(this, JsonUserList, UserList, AdminWin);
            AdminWin.Owner = this;
            LoginWin.Owner = this;
            LoginWin.Show();
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
            catch (System.IO.FileNotFoundException)
            {
                return;
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
    }
}
