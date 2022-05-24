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
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

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
        public bool isLoginSuccessful = false;
        public string ProvidedPassword;
        public Root UserList { get; private set; }
        public Login(Window LastWindow, Root JsonUserList, Window NewWindow)
        {
            InitializeComponent();
            Userlist = JsonUserList;
            MainWin = LastWindow;
            AdminWin = NewWindow;

        }


        public string HashPassword(string password, byte[] salt)
        {

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: password,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 100000,
                numBytesRequested: 256 / 8));

            return hashed;
        }

        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            isLoginSuccessful = false;

            foreach (User user in Userlist.Users)
            {
                if (!string.IsNullOrWhiteSpace(PwBox.Password) && !string.IsNullOrWhiteSpace(UserBox.Text) && user.Username == UserBox.Text && user.IsAdmin == true)
                {   
                    ProvidedPassword = PwBox.Password;
                    string HashedPassword = HashPassword(ProvidedPassword, user.Salt);
                    if (user.Password == HashedPassword)
                    {
                        Console.WriteLine(HashedPassword);
                        Console.WriteLine(user.Password);
                        AdminWin.Show();
                        this.Close();
                        MainWin.Hide();
                        isLoginSuccessful = true;
                    }
                }
            }
            if (isLoginSuccessful == false)
            {
                MessageBox.Show("Invalid Username or Password. Try Again.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
