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

namespace IPM_Job_Manager_net
{
    /// <summary>
    /// Interaction logic for QuantityWindow.xaml
    /// </summary>
    public partial class QuantityWindow : Window
    {
        public int PartsFinished;
        public string JobNo;
        public QuantityWindow(string job)
        {
            InitializeComponent();
            txtParts.Focus();
            DataContext = this;
            JobNo = job;
            txtJobNo.Content = $"Job: {JobNo}";
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(txtParts.Text) && txtParts.Text.All(c => char.IsDigit(c)))
            {
                PartsFinished = int.Parse(txtParts.Text, System.Globalization.NumberStyles.AllowLeadingWhite | System.Globalization.NumberStyles.AllowTrailingWhite);
                DialogResult = true;
            }
        }
    }
}
