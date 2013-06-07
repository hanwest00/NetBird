using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using NetBird.Util;
using System.Diagnostics;

namespace NetBird
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            Process[] procs = Process.GetProcessesByName("NetBird");
            if (procs.Length > 0)
            {
                MessageBox.Show("Is running!");
                Application.Current.Shutdown();
            }
            base.OnStartup(e);
        }

        private void Application_DispatcherUnhandledException_1(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
           AppLog.SysLog(e.Exception.ToString());
           MessageBox.Show("error!");
        }
    }
}
