using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace IPM_Job_Manager_net
{
    public class LogWriter
    {
        private string m_exePath = string.Empty;
        public LogWriter(string logMessage)
        {
            LogWrite(logMessage);
        }
        public void LogWrite(string logMessage)
        {
            m_exePath = Properties.Settings.Default.DataFileDirectory + @"\Logs";
            try
            {
                using (StreamWriter w = File.AppendText(m_exePath + "\\" + DateTime.Now.ToString("MM-dd-yyyy") + ".txt"))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.Write("\r\nLog Entry : ");
                txtWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(),
                    DateTime.Now.ToLongDateString());
                txtWriter.WriteLine("");
                txtWriter.WriteLine("  :{0}", logMessage);
                txtWriter.WriteLine("-------------------------------");
            }
            catch (Exception ex)
            {
                return;
            }
        }
    }
}
