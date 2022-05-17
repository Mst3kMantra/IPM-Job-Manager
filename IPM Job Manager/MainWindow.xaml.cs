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

namespace IPM_Job_Manager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ObservableCollection<string> _jobList = new ObservableCollection<string>();
        public ObservableCollection<string> jobList
        {
            get { return _jobList; }
            set { _jobList = value; }
        }
        #region DataSet, DataAdapter, DataTable
        internal DataSet dataSet;
        internal OleDbDataAdapter dataAdapter;
        internal DataTable dataTable;
        private OleDbConnection connection;
        #endregion


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
                
                foreach (DataRow row in dataTable.Rows)
                {
                    jobList.Add(row["JobNo"].ToString());
                }

               


                // OPTIONAL: use OleDbCommandBuilder to build a complete set of CRUD commands
                //OleDbCommandBuilder builder = new OleDbCommandBuilder(dataAdapter);
                // Update, Insert and Delete Commands
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
            string queryString = "SELECT * FROM [Job List]";
            InitializeComponent();
            this.DataContext = this;
            jobList = new ObservableCollection<string>();
            GetData(queryString, global::IPM_Job_Manager_net.Properties.Settings.Default.open_workConnectionString);
            foreach (string job in jobList)
            {
                Debug.WriteLine(job);
            }
        }

        private void ButtonAddName_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
