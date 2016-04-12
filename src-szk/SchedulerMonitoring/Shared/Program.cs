using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using EventScheduler;
using Microsoft.Owin.Hosting;
using SCHEMON.WebServer;
using System.Threading;
using System.ServiceProcess;
using MySMSSVC;
using System.Reflection;
using System.IO;
using System.Xml;
using System.Data.Entity.Infrastructure;
using TracerX;

namespace SCHEMON
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>

        static void Main()
        {
            string[] args = Environment.GetCommandLineArgs();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            WindowsServiceManager SM = new WindowsServiceManager();
            //MyShared.AppName = ("App Name: " + Application.ExecutablePath.Replace(Environment.CurrentDirectory + @"\",""));
            

            string arg0 = string.Empty;

            if (args.Length > 1)
                arg0 = (args[1] ?? string.Empty).ToLower();

             if (arg0 == "-install" || arg0 == "-i")
            {
                if (!SM.InstallService(Application.ExecutablePath + " -service",
                        MyShared.ServiceName, MyShared.ServiceDesc, "SENS"))
                {
                    MessageBox.Show("Service install failed.");
                }
                else
                {
                    
                    MyShared.DebugInfo( MyShared.ServiceName + " service installed successfully");
                    MessageBox.Show(MyShared.ServiceName + " service installed successfully");
                }

                Application.Exit();
                return;
            }
            else if (arg0 == "-uninstall" || arg0 == "-u")
            {
                if (!SM.UnInstallService(MyShared.ServiceName))
                {
                    MessageBox.Show("Service failed to uninstall.");

                }
                else
                {
                    MyShared.DebugInfo(MyShared.ServiceName + " service was removed successfully");
                    MessageBox.Show(MyShared.ServiceName + " service was removed successfully");
                }

                Application.Exit();
                return;
            }else
            if (arg0 == "-service")
            {
                RunService();
                return;
            }

            FakeRunService();

        }

        static void RunService()
        {
            var ServicesToRun = new ServiceBase[] { new Svc() };
            MyShared.DebugInfo(MyShared.ServiceName + " Service started as a Windows Service.");
            ServiceBase.Run(ServicesToRun);
        }

        static void FakeRunService()
        {
            MyShared.SvcState = true;
            var service = new Svc();
            service.DebugStart();

            MyShared.DebugInfo(MyShared.ServiceName + " Service started as FakeService for debugging.");

            while (MyShared.SvcState)
            {
                Thread.Sleep(3000);
            }
        }                
    }
}
