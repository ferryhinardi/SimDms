using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.ServiceProcess;
using System.Configuration;
using System.Configuration.Install;

using Microsoft.Win32;
using SVNMON;


namespace SystemTrayIconInSvc
{
    [RunInstaller(true)]
    public class SvcInstaller : Installer
    {

        public SvcInstaller()
        {
            Installers.Clear();

            ServiceInstaller serviceInstaller = new ServiceInstaller();
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.ServiceName = MyShared.ServiceName;
            serviceInstaller.DisplayName = MyShared.ServiceName;
            serviceInstaller.Description = MyShared.ServiceDesc;

            serviceInstaller.ServicesDependedOn = new string[] { "SENS", "COMSysApp" };

            Installers.Add(serviceInstaller);

            ServiceProcessInstaller processInstaller = new ServiceProcessInstaller();
            processInstaller.Account = ServiceAccount.LocalSystem;
            processInstaller.Password = null;
            processInstaller.Username = null;

            Installers.Add(processInstaller);
        }


        protected override void OnAfterInstall(IDictionary savedState)
        {
            ServiceController controller = null;
            ServiceController[] controllers = ServiceController.GetServices();
            for (int i = 0; i < controllers.Length; i++)
            {
                if (controllers[i].ServiceName == MyShared.ServiceName)
                {
                    controller = controllers[i];
                    break;
                }
            }
            if (controller == null)
            {
                return;
            }

            // if the service is not active, start it
            if (controller.Status != ServiceControllerStatus.Running)
            {
                string[] args = { "-install" };
                controller.Start(args);
            }
        }
    }
}
