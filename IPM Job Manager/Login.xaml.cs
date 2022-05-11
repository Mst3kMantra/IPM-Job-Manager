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

namespace IPM_Job_Manager
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        
        public Root Userlist;
        public string CurrentLogin;

        public Login()
        {
            InitializeComponent();
            Userlist = ReadUserJson();
            //todo add password decrypter and make dictionary of users, passwords, and admin status
        }

        public Root ReadUserJson()
        {
            using (StreamReader sr = new StreamReader("I:/GTMT/test/Users.json"))
            {
                string json = sr.ReadToEnd();
                Root UserList = JsonConvert.DeserializeObject<Root>(json);
                foreach (User user in UserList.Users)
                {
                    Debug.WriteLine(user.Username);
                }
                return UserList;
            }

        }


        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            foreach (User user in Userlist.Users)
            {
                if (!string.IsNullOrWhiteSpace(PwBox.Password) && !string.IsNullOrWhiteSpace(UserBox.Text) && user.Username == UserBox.Text && user.Password == PwBox.Password)
                {
                    CurrentLogin = user.Username;
                    var Main = new MainWindow();
                    Main.Show();
                    this.Close();
                }
            }
        }
    }
}
