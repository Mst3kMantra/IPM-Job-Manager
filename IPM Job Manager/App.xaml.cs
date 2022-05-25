using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace IPM_Job_Manager_net
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            MessageBox.Show(args.Exception.ToString());

            MessageBox.Show("An unexpected exception has occurred. Shutting down the application. Please check the log file for more details.");

            // Prevent default unhandled exception processing
            args.Handled = true;

            Environment.Exit(0);
        }
    }
}
