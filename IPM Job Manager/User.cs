using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace IPM_Job_Manager_net
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class User : INotifyPropertyChanged
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }

        private int _jobsAssigned = 0;
        public int JobsAssigned
        {
            get { return _jobsAssigned; }
            set
            {
                if (value != _jobsAssigned)
                {
                    this._jobsAssigned = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public byte[] Salt { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class Root
    {
        public List<User> Users { get; set; }
    }
}
