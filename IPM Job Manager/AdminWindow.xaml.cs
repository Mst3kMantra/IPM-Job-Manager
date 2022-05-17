using IPM_Job_Manager_net;
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
    /// Interaction logic for AdminWindow.xaml
    /// </summary>
    public partial class AdminWindow : Window
    {
        public Window MainWin;
        private ObservableCollection<string> _userList = new ObservableCollection<string>();
        public ObservableCollection<string> UserList
        {
            get { return _userList; }
            set { _userList = value; }
        }

        public AdminWindow(ObservableCollection<string> Usernames, Window Win)
        {
            InitializeComponent();
            this.DataContext = this;
            UserList = Usernames;
            MainWin = Win;
        }

        private void btnLogout_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAssignJob_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnViewJob_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
