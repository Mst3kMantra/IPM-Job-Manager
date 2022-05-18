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


        public Root JsonUserList;

        #region DataSet, DataAdapter, DataTable
        internal DataSet dataSet;
        internal OleDbDataAdapter dataAdapter;
        internal DataTable dataTable;
        private OleDbConnection connection;
        #endregion

        public Root ReadUserJson()
        {
            using (StreamReader sr = new StreamReader("I:/GTMT/test/Users.json"))
            {
                string json = sr.ReadToEnd();
                Root JsonUserList = JsonConvert.DeserializeObject<Root>(json);
                return JsonUserList;
            }

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

                for (int i = 0; i < dataTable.Rows.Count - 1; i++)
                {
                    JobList.Add(new Job());
                    JobList[i].JobInfo.Add("AssignedEmployees", AssignedEmployeeList);
                    foreach (DataColumn column in dataTable.Columns)
                    {
                        JobList[i].JobInfo.Add(column.ColumnName.ToString(), dataTable.Rows[i][column].ToString());
                    }
                }

                WriteJobsJson(JobList, @"I:\GTMT\test\job_list.json");


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
            JsonUserList = ReadUserJson();
            foreach (User user in JsonUserList.Users)
            {
                UserList.Add(user);
            }
            GetData(QueryString, global::IPM_Job_Manager_net.Properties.Settings.Default.open_workConnectionString);
            JobList = ReadJobsJson("I:/GTMT/test/job_list.json");
        }

        private void lstUsers_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            string CurItem = lstUsers.SelectedItem.ToString();

            switch (CurItem)
            {
                case "Bill":
                    break;
                case "Chau":
                    break;
                case "Michael":
                    break;
                case "Evelyn":
                    break;
                case "Eduardo":
                    break;
                case "Toan":
                    break;
                case "Victor":
                    break;
                case "Martie":
                    break;
                default:
                    break;
            }
        }

        private void ButtonViewJob_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonAdminLogin_Click(object sender, RoutedEventArgs e)
        {
            var AdminWin = new AdminWindow(UserList, this, JobList);
            var LoginWin = new Login(this, JsonUserList, UserList, AdminWin);
            AdminWin.Owner = this;
            LoginWin.Owner = this;
            LoginWin.Show();
        }
    }
}
