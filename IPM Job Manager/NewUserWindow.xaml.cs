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
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.IO;

namespace IPM_Job_Manager_net
{
    /// <summary>
    /// Interaction logic for NewUserWindow.xaml
    /// </summary>
    public partial class NewUserWindow : Window
    {
        public MainWindow MainWin;
        public string Password;
        public string Username;
        public Root Userlist;
        public byte[] PasswordSalt;
        public NewUserWindow()
        {
            InitializeComponent();
            MainWin = Application.Current.MainWindow as MainWindow;
            Userlist = MainWin.ReadUserJson(MainWin.UserListPath);
        }

        public byte[] GetSalt()
        {
            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetNonZeroBytes(salt);
            }
            return salt;
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

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(PwBox.Password) && !string.IsNullOrWhiteSpace(UserBox.Text))
            {
                User NewUser = new User();
                AdminWindow adminWindow = Owner as AdminWindow;
                Password = PwBox.Password;
                Username = UserBox.Text;
                PasswordSalt = GetSalt();
                Password = HashPassword(Password, PasswordSalt);
                NewUser.Salt = PasswordSalt;
                NewUser.Password = Password;
                NewUser.Username = Username;
                if (isAdmin.IsChecked == true)
                {
                    NewUser.IsAdmin = true;
                }
                Userlist.Users.Add(NewUser);
                MainWin.UserList.Add(NewUser);
                MainWin.WriteUserJson(Userlist, MainWin.UserListPath);
            }
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
