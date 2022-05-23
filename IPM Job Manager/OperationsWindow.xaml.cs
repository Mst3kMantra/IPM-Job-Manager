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
    /// Interaction logic for OperationsWindow.xaml
    /// </summary>
    public partial class OperationsWindow : Window
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

        private ObservableCollection<User> _userlist = new ObservableCollection<User>();
        public ObservableCollection<User> UserList
        {
            get { return _userlist; }
            set { _userlist = value; }
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

        public MainWindow MainWin;

        public User SelectedUser;

        public int JobIndex;

        public object SelectedOperation;
        public object SelectedAssignee;

        public bool isDataChanged = false;

        private Dictionary<string, string> _selectedOperations = new Dictionary<string, string>();
        public Dictionary<string, string> SelectedOperations
        {
            get { return _selectedOperations; }
            set { _selectedOperations = value; }
        }

        public OperationsWindow(Job job)
        {
            InitializeComponent();
            MainWin = Application.Current.MainWindow as MainWindow;
            UserList = MainWin.UserList;
            DataContext = this;
            txtOperations.Focus();

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

            SelectedOperations = JsonConvert.DeserializeObject<Dictionary<string, string>>(job.JobInfo["Operations"].ToString());

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

            try
            {
                JobNotes = MainWin.ReadJobsJson(MainWin.JobNotesPath);
            }
            catch (FileNotFoundException)
            {
                return;
            }
        }

        public void WriteToNotes(Job SelectedJob)
        {
            if (JobNotes != null)
            {
                if (JobNotes.Count > 0)
                {
                    foreach (Job job in JobNotes)
                    {
                        if (job.JobInfo["PartNo"] == SelectedJob.JobInfo["PartNo"])
                        {
                            job.JobInfo["Operations"] = SelectedJob.JobInfo["Operations"];
                            MainWin.WriteJobsJson(JobNotes, MainWin.JobNotesPath);
                            break;
                        }
                        else
                        {
                            Job newNotes = new Job();
                            newNotes.JobInfo.Add("Operations", SelectedJob.JobInfo["Operations"]);
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
                    newNotes.JobInfo.Add("Operations", SelectedJob.JobInfo["Operations"]);
                    newNotes.JobInfo.Add("PartNo", SelectedJob.JobInfo["PartNo"]);
                    JobNotes.Add(newNotes);
                    MainWin.WriteJobsJson(JobNotes, MainWin.JobNotesPath);
                }
            }
        }
        private void btnAddOperation_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtOperations.Text) == false && txtOperations.Text.Length != 0)
            {
                AssignedJobList[JobIndex].JobInfo["Operations"].Add(txtOperations.Text, "");
                OperationList.Add(txtOperations.Text);
                AssignedEmployeeList.Add("");
                MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                WriteToNotes(AssignedJobList[JobIndex]);
                isDataChanged = true;
            }
            else MessageBox.Show("No operation text detected.", "Add Operation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnRemoveOperation_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedOperation != null)
            {
                AssignedJobList[JobIndex].JobInfo["Operations"].Remove(SelectedOperation.ToString());
                int OpIndex = OperationList.IndexOf(SelectedOperation.ToString());
                OperationList.Remove(SelectedOperation.ToString());
                AssignedEmployeeList.RemoveAt(OpIndex);
                MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
                WriteToNotes(AssignedJobList[JobIndex]);
                isDataChanged=true;
            }
            else MessageBox.Show("No operation selected from list.", "Remove Operation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void btnAssignOperation_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedUser == null)
            {
                MessageBox.Show("Please select a employee.", "Assign Operation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (SelectedOperation == null)
            {
                MessageBox.Show("Please select an operation", "Assign Operation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            AssignedJobList[JobIndex].JobInfo["Operations"][SelectedOperation.ToString()] = SelectedUser.Username;
            int OpIndex = OperationList.IndexOf(SelectedOperation.ToString());
            AssignedEmployeeList[OpIndex] = SelectedUser.Username;
            MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
            WriteToNotes(AssignedJobList[JobIndex]);
            isDataChanged =true;
        }

        private void btnUnassignOperation_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedAssignee == null || SelectedAssignee.ToString() == "")
            {
                MessageBox.Show("Please select a employee from assigned list.", "Unassign Operation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int AssignedIndex = AssignedEmployeeList.IndexOf(SelectedAssignee.ToString());

            AssignedJobList[JobIndex].JobInfo["Operations"][OperationList[AssignedIndex].ToString()] = "";
            AssignedEmployeeList[AssignedIndex] = "";
            MainWin.WriteJobsJson(AssignedJobList, MainWin.AssignedJobListPath);
            WriteToNotes(AssignedJobList[JobIndex]);
            isDataChanged=true;
        }

        private void lstEmployees_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedUser = lstEmployees.SelectedItem as User;
        }

        private void lstOperations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedOperation = lstOperations.SelectedItem;
        }

        private void lstAssigned_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            SelectedAssignee = lstAssigned.SelectedItem;
        }

        private void OperationsWindow_Closing(object sender, CancelEventArgs e)
        {
            if (isDataChanged)
            {
                DialogResult = true;
            }
        }
    }
}
