using System;
using System.Collections.Generic;
using System.Text;

using COMAdmin;
using SensEvents;

namespace MyServiceNotification
{
    /// <summary>
    /// ISensLogon2 Event Args
    /// </summary>
    public class SensLogonToEventArgs : EventArgs
    {
        public string Username;
        public uint SessionId;
    }

    /// <summary>
    /// subscribe SENS notification
    /// Ref MSDN:Accessing System Power and Network Status Using SENS
    /// ms-help://MS.MSDN.vAug06.en/dntablet/html/tbconFFFSENS.htm
    /// </summary>
    public sealed class SensAdvisor2 : ISensLogon2
    {
        public const string ISensLogon2_ID = "{d5978650-5b9f-11d1-8dd2-00aa004abd5e}";
        public string UserName { get; set; }

        public SensAdvisor2()
        {
            COMAdminCatalogClass comAdmin = new COMAdminCatalogClass();
            ICatalogCollection subCollection = (ICatalogCollection)comAdmin.GetCollection("TransientSubscriptions");

            SubscribeToEvent(subCollection, "PostShell", ISensLogon2_ID);
            SubscribeToEvent(subCollection, "Logon", ISensLogon2_ID);
            SubscribeToEvent(subCollection, "Logoff", ISensLogon2_ID);
            SubscribeToEvent(subCollection, "SessionReconnect", ISensLogon2_ID);
            SubscribeToEvent(subCollection, "SessionDisconnect", ISensLogon2_ID);
        }
            
        private void SubscribeToEvent(ICatalogCollection subCollection, string methodName, string guidString)
        {
           ICatalogObject catalogObject = (ICatalogObject)subCollection.Add();

           // Specify the parameters of the desired subscription.
           catalogObject.set_Value("EventCLSID", guidString);
           catalogObject.set_Value("Name", "Subscription to " + methodName + " event");
           catalogObject.set_Value("MethodName", methodName);
           catalogObject.set_Value("SubscriberInterface", this);
           catalogObject.set_Value("Enabled", true);
           // This setting allows subscriptions to work for non-Administrator users.
           catalogObject.set_Value("PerUser", true);  

           // Save the changes made to the transient subscription collection.
           subCollection.SaveChanges();
        }


        public delegate void PostShellEventHandler(object sender, SensLogonToEventArgs e);
        public delegate void SessionReconnectEventHandler(object sender, SensLogonToEventArgs e);
        public delegate void SessionDisconnectEventHandler(object sender, SensLogonToEventArgs e);
        public delegate void LogonEventHandler(object sender, SensLogonToEventArgs e);
        public delegate void LogoffEventHandler(object sender, SensLogonToEventArgs e);

        public event PostShellEventHandler OnShellStarted;
        public event SessionReconnectEventHandler OnSessionReconnected;
        public event SessionDisconnectEventHandler OnSessionDisconnected;
        public event LogonEventHandler OnLogon;
        public event LogoffEventHandler OnLogoff;


        public void PostShell(string bstrUserName, uint dwSessionId)
        {
            if (OnShellStarted != null)
            {
                SensLogonToEventArgs args = new SensLogonToEventArgs();
                UserName = bstrUserName;
                args.Username = bstrUserName;
                args.SessionId = dwSessionId;
                OnShellStarted(this, args);
            }
        }


        public void SessionReconnect(string bstrUserName, uint dwSessionId)
        {
            if (OnSessionReconnected != null)
            {
                SensLogonToEventArgs args = new SensLogonToEventArgs();
                UserName = bstrUserName;
                args.Username = bstrUserName;
                args.SessionId = dwSessionId;
                OnSessionReconnected(this, args);
            }
        }

        public void SessionDisconnect(string bstrUserName, uint dwSessionId)
        {
            if (OnSessionDisconnected != null)
            {
                SensLogonToEventArgs args = new SensLogonToEventArgs();
                UserName = bstrUserName;
                args.Username = bstrUserName;
                args.SessionId = dwSessionId;
                OnSessionDisconnected(this, args);
            }
        }

        public void Logoff(string bstrUserName, uint dwSessionId)
        {
            if (OnLogoff != null)
            {
                SensLogonToEventArgs args = new SensLogonToEventArgs();
                UserName = bstrUserName;
                args.Username = bstrUserName;
                args.SessionId = dwSessionId;
                OnLogoff(this, args);
            }
        }

        public void Logon(string bstrUserName, uint dwSessionId)
        {
            if (OnLogon != null)
            {
                SensLogonToEventArgs args = new SensLogonToEventArgs();
                UserName = bstrUserName;
                args.Username = bstrUserName;
                args.SessionId = dwSessionId;
                OnLogon(this, args);
            }
        }
    }

}
