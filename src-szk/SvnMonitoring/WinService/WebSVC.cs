using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.ServiceProcess;
//using MySMSSVC;
using System.Threading;
using SVNMON.WebServer.Configs;
using Microsoft.Owin.Hosting;
using System.Diagnostics;
using EventScheduler;
using MySMSSVC;
using SQLWatcher;
using System.Data.SqlClient;
using System.Data;


namespace SVNMON.WebServer
{

    public class Svc : ServiceBase
    {
        //private SensAdvisor m_Advisor = null;
        private frmConfig m_Dlg = null;
        IDisposable WebSvc { get; set; }

        //private static SqlWatcher SqlQueueWatcher;

        public Svc()
        {
            
        }

        //internal void StartWaching()
        //{
        //    //Build the command object we want to monitor (don't include a SqlConnection)
        //    SqlCommand cmd = new SqlCommand();
        //    cmd = new SqlCommand("dbo.uspfn_SysDealerHistGet3");
        //    cmd.CommandType = CommandType.StoredProcedure;

        //    //Setup the SQLWatcher
        //    SqlQueueWatcher = new SqlWatcher((System.Configuration.ConfigurationManager.ConnectionStrings["SimDMS"].ToString()), cmd, SqlWatcherNotificationType.Threaded);
        //    SqlQueueWatcher.OnChange += new SqlWatcher.SqlWatcherEventHandler(QueueSQLWatcher_OnChange);
        //    SqlQueueWatcher.Start();
        //}

        //private static void QueueSQLWatcher_OnChange(DataSet Result)
        //{
        //    //Do something with the updated DataSet object
        //    MyShared.DebugInfo("Changed Detected: " + Result.Tables[0].Rows.Count.ToString());
        //}

        internal void DebugStart()
        {
            MyShared.DebugInfo("Run service on debug mode");
            string Url = System.Configuration.ConfigurationManager.AppSettings["url"].ToString();
            MyShared.DebugInfo("Url: " + Url);
            WebSvc = WebApp.Start<Startup>(Url);
            m_Dlg = frmConfig.StartUIThread();           
            m_Dlg.mNotifyTgr += new MyServiceHandler(MyServiceEventNotifyHandle);
            MyShared.DebugInfo("Service has been running...");

            //StartWaching();
        }

        private void MyServiceEventNotifyHandle(int code)
        {
            MyShared.DebugInfo("Stoping service...");
            OnStop();
        }

        protected override void OnStart(string[] args)
        {
            // if the service started after the windows logon
            //CloseDlg();

            string Url = System.Configuration.ConfigurationManager.AppSettings["url"].ToString();
            MyShared.DebugInfo("Url: " + Url);
            WebSvc = WebApp.Start<Startup>(Url);
            MyShared.DebugInfo("Service has been running...");
            //m_Dlg = frmConfig.StartUIThread();
            //m_Dlg.mNotifyTgr += new MyServiceHandler(MyServiceEventNotifyHandle);

            //StartWaching();
            //m_Advisor = new SensAdvisor();
            //m_Advisor.OnShellStarted += this.PostShell;
        }

        protected override void OnStop()
        {
            MyShared.SvcState = false;
            CloseDlg();
            Thread.Sleep(1000);

            WebSvc.Dispose();
            MyShared.DebugInfo("Service shutdown");
            //m_Advisor = null;
            //Thread.Sleep(2000);
            //SqlQueueWatcher.Stop();

            GC.Collect();

            base.OnStop();
        }

        /// <summary>
        /// Called when the shell is ready
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PostShell(object sender, SensLogon2EventArgs e)
        {
            CloseDlg();
            MyShared.DebugInfo("Shell is ready");
            //m_Dlg = frmConfig.StartUIThread();
            //m_Dlg.mNotifyTgr += new MyServiceHandler(MyServiceEventNotifyHandle);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (WebSvc != null)
            {
                WebSvc.Dispose();
                WebSvc = null;
            }
        }

        protected void CloseDlg()
        {
            //if (m_Dlg != null)
            //{
            //    try
            //    {
            //        m_Dlg.mNotifyTgr -= new MyServiceHandler(MyServiceEventNotifyHandle);
            //        m_Dlg.Close();
            //        m_Dlg.Dispose();
            //    }
            //    catch (Exception eX)
            //    {
            //        MyShared.DebugInfo("Error on closing dialog: " + eX.Message);
            //    }
            //    m_Dlg = null;
            //}
        }
    }
    
 

}
