// Updated by XamlIntelliSenseFileGenerator 5/25/2022 5:01:24 PM
#pragma checksum "..\..\MainWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "EFB1844959490F15805C3513263B3B9B8F1D45534503969F9A7883BF95B172DA"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using IPM_Job_Manager_net;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace IPM_Job_Manager_net
{


    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector
    {


#line 23 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView lstAssignedJobs;

#line default
#line hidden


#line 92 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lstOperations;

#line default
#line hidden


#line 99 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lstAssigned;

#line default
#line hidden


#line 107 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock txtNotes;

#line default
#line hidden


#line 113 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListBox lstUsers;

#line default
#line hidden


#line 125 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnEditNotes;

#line default
#line hidden


#line 126 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button btnAdmin;

#line default
#line hidden

        private bool _contentLoaded;

        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent()
        {
            if (_contentLoaded)
            {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/IPM Job Manager;component/mainwindow.xaml", System.UriKind.Relative);

#line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);

#line default
#line hidden
        }

        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
        {
            switch (connectionId)
            {
                case 1:
                    this.lstAssignedJobs = ((System.Windows.Controls.ListView)(target));

#line 23 "..\..\MainWindow.xaml"
                    this.lstAssignedJobs.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.lstAssignedJobs_SelectionChanged);

#line default
#line hidden
                    return;
                case 2:
                    this.lstOperations = ((System.Windows.Controls.ListBox)(target));

#line 92 "..\..\MainWindow.xaml"
                    this.lstOperations.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.lstOperations_SelectionChanged);

#line default
#line hidden
                    return;
                case 3:
                    this.lstAssigned = ((System.Windows.Controls.ListBox)(target));

#line 99 "..\..\MainWindow.xaml"
                    this.lstAssigned.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.lstAssigned_SelectionChanged);

#line default
#line hidden
                    return;
                case 4:
                    this.txtNotes = ((System.Windows.Controls.TextBlock)(target));
                    return;
                case 5:
                    this.lstUsers = ((System.Windows.Controls.ListBox)(target));

#line 113 "..\..\MainWindow.xaml"
                    this.lstUsers.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.lstUsers_SelectionChanged);

#line default
#line hidden
                    return;
                case 6:
                    this.btnRefresh = ((System.Windows.Controls.Button)(target));

#line 124 "..\..\MainWindow.xaml"
                    this.btnRefresh.Click += new System.Windows.RoutedEventHandler(this.btnRefresh_Click);

#line default
#line hidden
                    return;
                case 7:
                    this.btnEditNotes = ((System.Windows.Controls.Button)(target));

#line 125 "..\..\MainWindow.xaml"
                    this.btnEditNotes.Click += new System.Windows.RoutedEventHandler(this.btnEditNotes_Click);

#line default
#line hidden
                    return;
                case 8:
                    this.btnAdmin = ((System.Windows.Controls.Button)(target));

#line 126 "..\..\MainWindow.xaml"
                    this.btnAdmin.Click += new System.Windows.RoutedEventHandler(this.ButtonAdminLogin_Click);

#line default
#line hidden
                    return;
            }
            this._contentLoaded = true;
        }

        internal System.Windows.Controls.Grid OperationsGrid;
    }
}

