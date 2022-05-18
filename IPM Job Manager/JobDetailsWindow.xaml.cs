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
    /// Interaction logic for JobDetailsWindow.xaml
    /// </summary>
    public partial class JobDetailsWindow : Window
    {
        private Job _displayedJob = new Job();
        public Job DisplayedJob
        {
            get { return _displayedJob; }
            set { _displayedJob = value; }
        }
        public JobDetailsWindow(Job SelectedJob)
        {
            InitializeComponent();
            this.DataContext = this;
            DisplayedJob = SelectedJob;
            Console.WriteLine(SelectedJob.JobInfo["JobNo"]);
        }
    }
}
