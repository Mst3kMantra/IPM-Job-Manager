using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private O

        public AdminWindow AdminWindow;

        public OperationsWindow(ObservableCollection<Job> jobs)
        {
            InitializeComponent();
            AssignedJobList = jobs;
            foreach (Window window in Application.Current.Windows.OfType<AdminWindow>())
            {
                AdminWindow = window as AdminWindow;
            }
        }

        private void btnAddOperation_Click(object sender, RoutedEventArgs e)
        {
            AdminWindow.AssignedJobList.Add 
        }

        private void btnRemoveOperation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAssignOperation_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnUnassignOperation_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
