using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IPM_Job_Manager_net
{
    public class Job : INotifyPropertyChanged
    {
        private Dictionary<string, dynamic> _jobInfo = new Dictionary<string, dynamic>();
        public Dictionary<string, dynamic> JobInfo
        {
            get { return _jobInfo; }
            set
            {
                if (value != _jobInfo)
                {
                    this._jobInfo = value;
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
