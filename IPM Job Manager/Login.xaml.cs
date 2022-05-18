using System;
using System.Collections.Generic;
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
using System.IO;
using Newtonsoft.Json;
using System.Diagnostics;
using IPM_Job_Manager_net;
using System.Collections.ObjectModel;

namespace IPM_Job_Manager_net
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        

        public string CurrentLogin;
        public Window MainWin;
        public Window AdminWin;
        public Root Userlist;
        public ObservableCollection<User> Usernames;
        public Root UserList { get; private set; }
        public Login(Window LastWindow, Root JsonUserList, ObservableCollection<User> UsernameList, Window NewWindow)
        {
            InitializeComponent();
            Userlist = JsonUserList;
            MainWin = LastWindow;
            AdminWin = NewWindow;
            Usernames = UsernameList;

            //todo add password decrypter and make dictionary of users, passwords, and admin status
        }



        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            foreach (User user in Userlist.Users)
            {
                if (!string.IsNullOrWhiteSpace(PwBox.Password) && !string.IsNullOrWhiteSpace(UserBox.Text) && user.Username == UserBox.Text && user.Password == PwBox.Password && user.IsAdmin == true)
                {
                    CurrentLogin = user.Username;
                    AdminWin.Show();
                    this.Close();
                    MainWin.Hide();
                }
            }
        }
    }
}
