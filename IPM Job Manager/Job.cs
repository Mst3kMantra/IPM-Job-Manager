using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPM_Job_Manager_net
{
    public class Job
    {
        private Dictionary<string, dynamic> _jobInfo = new Dictionary<string, dynamic>();
        public Dictionary<string, dynamic> JobInfo
        {
            get { return _jobInfo; }
            set { _jobInfo = value; }
        }
    }
}
