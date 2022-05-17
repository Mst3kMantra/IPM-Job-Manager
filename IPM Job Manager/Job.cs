using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IPM_Job_Manager
{
    public class Job
    {
        private string _jobNo;
        public string JobNo { get { return _jobNo; } set { _jobNo = value; } }

        private string _partNo;
        public string PartNo { get { return _partNo; } set { _partNo = value; } }

        private string _status;
        public string Status { get { return _status; } set { _status = value; } }

        private string _partDesc;
        public string PartDesc { get { return _partDesc; } set { _partDesc = value;} }

        private List<string> _operations;
        public List<string> Operations { get { return _operations; } set { _operations = value;} }

        private string _notes;
        public string Notes { get { return _notes; } set { _notes = value; } }

        private string _assignedEmployee;
        public string AssignedEmployee { get { return _assignedEmployee; } set { _assignedEmployee = value;} }

        private string _custDesc;
        public string CustDesc { get { return _custDesc; } set { _custDesc = value;} }

        private string _dueDate;
        public string DueDate { get { return _dueDate; } set { _dueDate = value;} }

        private string _poNum;
        public string PoNum { get { return _poNum; } set { _poNum = value;} }

        private string _qtyOrdered;
        public string QtyOrdered { get { return _qtyOrdered; } set { _qtyOrdered = value;} }

        private string _qtyDue;
        public string QtyDue { get { return _qtyDue; } set { _qtyDue = value;} }




        public Job(string jobNumber, string partNumber, string partDescription, string partNotes, string employee)
        {
            _jobNo = jobNumber;
            _partNo = partNumber;
            _partDesc = partDescription;
            _notes = partNotes;
            _assignedEmployee = employee;
        }
    }
}
