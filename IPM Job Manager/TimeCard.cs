using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IPM_Job_Manager_net
{
    public class TimeCard : INotifyPropertyChanged
    {
        private Dictionary<string, DateTime> _clockedInJobs = new Dictionary<string, DateTime>();
        public Dictionary<string, DateTime> ClockedInJobs
        {
            get { return _clockedInJobs; }
            set
            {
                if (value != _clockedInJobs)
                {
                    this._clockedInJobs = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private Dictionary<string, string> _trackedOperations = new Dictionary<string, string>();
        public Dictionary<string, string> TrackedOperations
        {
            get { return _trackedOperations; }
            set { _trackedOperations = value; }
        }

        private string _allClockedInJobs = string.Empty;
        public string AllClockedInJobs
        {
            get
            {
                return _allClockedInJobs;
            }
            set
            {
                if (_allClockedInJobs != value)
                {
                    _allClockedInJobs = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
