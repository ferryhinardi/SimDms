using System;
using System.Collections.Generic;
using System.Web;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using System.ComponentModel;

namespace SQLWatcher
{

    public enum SqlWatcherNotificationType
    {
        Blocking,
        Threaded // Launch in another thread so SqlWatcher can immediately start monitoring again.
    }

    public class SqlWatcher : IDisposable
    {
        private string ConnectionString;
        private SqlConnection Connection;
        private SqlCommand Command;
        private SqlDataAdapter Adapter;
        private DataSet Result;
        private SqlWatcherNotificationType NotificationType;

        public SqlWatcher(string ConnectionString, SqlCommand Command, SqlWatcherNotificationType NotificationType)
        {
            this.NotificationType = NotificationType;
            this.ConnectionString = ConnectionString;
            SqlDependency.Start(this.ConnectionString);
            this.Connection = new SqlConnection(this.ConnectionString);
            this.Connection.Open();
            this.Command = Command;
            this.Command.Connection = this.Connection;
            Adapter = new SqlDataAdapter(this.Command);
        }

        public void Start()
        {
            RegisterForChanges();
        }

        public void Stop()
        {
            SqlDependency.Stop(this.ConnectionString);
        }

        public delegate void SqlWatcherEventHandler(DataSet Result);

        public event SqlWatcherEventHandler OnChange;

        public DataSet DataSet
        {
            get { return Result; }
        }

        private void RegisterForChanges()
        {
            //Remove old dependency object
            this.Command.Notification = null;
            //Create new dependency object
            SqlDependency dep = new SqlDependency(this.Command);
            dep.OnChange += new OnChangeEventHandler(Handle_OnChange);
            //Save data
            Result = new DataSet();
            Adapter.Fill(Result);
            //Notify client of change to DataSet
            switch (NotificationType)
            {
                case SqlWatcherNotificationType.Blocking:
                    OnChange(Result);
                    break;
                case SqlWatcherNotificationType.Threaded:
                    ThreadPool.QueueUserWorkItem(ChangeEventWrapper, Result);
                    break;
            }
        }

        public void ChangeEventWrapper(object state)
        {
            DataSet Result = (DataSet)state;
            OnChange(Result);
        }

        private void Handle_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type != SqlNotificationType.Change)
                throw new ApplicationException("Failed to create queue notification subscription!");

            //Clean up the old notification
            SqlDependency dep = (SqlDependency)sender;
            dep.OnChange -= Handle_OnChange;

            //Register for the new notification
            RegisterForChanges();
        }

        public void Dispose()
        {
            Stop();
        }
    }

    /*
    public class SqlWatcher : IDisposable
    {
        # region Private Members
        private SqlConnection watcherConn = null;
        private SqlDependency watcherDependency = null;

        private string _commandText = string.Empty;
        private string _resultsName = string.Empty;
        private bool _serviceBrokerBeganEnabled = false;
        private bool _subscribeSuccessful = false;
        //private int _numNotifications = 0;
        private int _maxNotifications = int.MaxValue;

        /// <summary>
        /// Attempts to enable/disable service broker in a database
        /// </summary>
        /// <param name="connectionString">Connection string to the SQL Server 2005/2008 database</param>
        /// <param name="enableServiceBroker">True to enable Service Broker, false to disable</param>
        /// <returns>True if the command to enable/disable Service Broker completed successfully</returns>
        /// <remarks>EnableServiceBroker will fail if the connection string does not contain admin-level credentials</remarks>
        private bool EnableServiceBroker(string connectionString, bool enableServiceBroker)
        {
            try
            {
                using (SqlConnection tempConn = new SqlConnection(connectionString))
                {
                    SqlCommand serviceBrokerCmd = new SqlCommand(string.Format("ALTER DATABASE {0} SET {1}", tempConn.Database, enableServiceBroker ? "ENABLE_BROKER" : "DISABLE_BROKER"), tempConn);
                    serviceBrokerCmd.ExecuteNonQuery();
                    tempConn.Close();
                    if (enableServiceBroker)
                        SqlDependency.Start(connectionString);
                    else
                        SqlDependency.Stop(connectionString);
                }
                return true;
            }
            catch (Exception ex)
            {
                // Connection string is bad, user doesn't have permissions, etc.
                if (OnNotificationError != null)
                    OnNotificationError(ex.Message);
                return false;
            }
        }
        # endregion Private Members

        # region Delegates and events
        public delegate void DataChangeDetected();
        public event DataChangeDetected OnDataChangeDetected;
        public delegate void NotificationError(string reason);
        public event NotificationError OnNotificationError;
        public ManualResetEvent processingComplete;

        /// <summary>
        /// Internal event handler for SqlDependency notifications, including subscription failures
        /// </summary>
        /// <param name="sender">SqlDependency object that raised the error</param>
        /// <param name="e">Information on the event type</param>
        void OnChange(object sender, SqlNotificationEventArgs e)
        {
            SqlDependency dependency = (SqlDependency)sender;
            dependency.OnChange -= OnChange;

            switch (e.Type)
            {
                case SqlNotificationType.Change:
                    // Raise external OnDataChangeDetected event
                    if (OnDataChangeDetected != null)
                        OnDataChangeDetected();
                    //MyLogger.Info("CHange detected");
                    break;
                case SqlNotificationType.Subscribe:
                    // The only Subscribe messages are of subscription failure
                    if (OnNotificationError != null)
                        OnNotificationError(string.Format("The subscription for this query was rejected for the following reason: {0}. Make sure your query follows the guidelines at http://msdn.microsoft.com/en-us/library/aewzkxxh.aspx", e.Info));
                    break;
                case SqlNotificationType.Unknown:
                    // .NET doesn't know what this is, so neither do I
                    throw new ApplicationException("Unknown notification event type received from SQL Server");
            }

            // If we've reached the maximum number of desired notifications, fire
            // processingComplete.Set() so the calling app knows it can end.
            //_numNotifications++;
            //if (_numNotifications >= _maxNotifications && processingComplete != null)
            //    processingComplete.Set();
        }
        # endregion Delegates and events

        # region Constructors
        /// <summary>
        /// SqlWatcher class to ad-hoc database change monitoring.
        /// 
        /// Technical underpinnings of Service Broker and SqlDependency instructed 
        /// and inspired by http://www.codeproject.com/KB/database/chatter.aspx
        /// </summary>
        /// <param name="connectionString">SQL Server 2005/2008 connection string</param>
        /// <param name="commandText">SQL command to execute</param>
        /// <param name="resultsName">Name to give the results table. This is used as the root node name when serializing the results DataTable to XML</param>
        /// <param name="maxNotifications">Number of notifications to receive before SqlWatcher terminates itself. Zero for unlimited.</param>
        /// <see cref="http://msdn.microsoft.com/en-us/library/aewzkxxh.aspx"/>
        public SqlWatcher(string connectionString, string commandText, string resultsName, int maxNotifications)
        {
            // If a dependency already exists, stop it
            SqlDependency.Stop(connectionString);
            try
            {
                // Start a SqlDependency against this connection string
                SqlDependency.Start(connectionString);
                _serviceBrokerBeganEnabled = true;
            }
            catch (InvalidOperationException)
            {
                // It failed, probably because Service Broker is not running.
                // Try to start it automatically
                if (!EnableServiceBroker(connectionString, true))
                    throw new ApplicationException("Service Broker is not enabled on this database and SqlWatcher was unable to activate it automatically.\nPlease try again with administrative credentials, or ask your DBA to do it for you.");
            }

            // Set up our database connection and get ready for the user to call GetData()
            watcherConn = new SqlConnection(connectionString);
            _commandText = commandText;
            _resultsName = resultsName;
            _maxNotifications = maxNotifications <= 0 ? int.MaxValue : maxNotifications;

            // The public ManualResetEvent property "processingComplete" allows a
            // main program thread to wait while background threads handle
            // notifications. When we call processingComplete.Set(), the main
            // thread will resume and the program will end.
            processingComplete = new ManualResetEvent(false);
        }
        # endregion Constructors

        # region Properties
        /// <summary>
        /// The text of the command being watched
        /// </summary>
        public string CommandText
        {
            get
            {
                return _commandText;
            }
        }

        /// <summary>
        /// Indicates whether SqlWatcher subscribed to the connection string & query successfully
        /// </summary>
        public bool IsSubscribed
        {
            get
            {
                return _subscribeSuccessful;
            }
        }
        # endregion Properties

        # region Public methods

        /// <summary>
        /// Executes the query statement against the database, 
        /// registers to receive notifications if the results change,
        /// and returns the results as a DataTable
        /// </summary>
        /// <returns>DataTable containing the current query results</returns>
        public DataTable GetData()
        {
            DataTable resultsTable = new DataTable();

            try
            {
                // Create SqlCommand and attach the OnChange event to it
                SqlCommand watcherCmd = new SqlCommand(_commandText, watcherConn);
                watcherCmd.CommandType = CommandType.Text;
                watcherCmd.Notification = null;

                // This is where we actually set up the SqlDependency to watch the query and raise the event!
                watcherDependency = new SqlDependency(watcherCmd);
                watcherDependency.OnChange += new OnChangeEventHandler(OnChange);

                // Open database connection
                if (watcherConn.State == ConnectionState.Closed)
                    watcherConn.Open();

                SqlDataAdapter adapter = new SqlDataAdapter(watcherCmd);
                DataTable dt = new DataTable();
                adapter.Fill(resultsTable);

                _subscribeSuccessful = true;
            }
            finally
            {
                // Always make sure we close the connection
                //if (watcherConn.State == ConnectionState.Open)
                //    watcherConn.Close();
            }

            return resultsTable;
        }
        # endregion Public methods

        # region Destructor/IDisposable Members
        ~SqlWatcher()
        {
            Dispose();
        }

        public void Dispose()
        {
            // Detach events
            OnDataChangeDetected -= OnDataChangeDetected;
            OnNotificationError -= OnNotificationError;
            watcherDependency.OnChange -= OnChange;
            watcherDependency = null;

            // If we turned Service Broker on, turn it back off
            if (_serviceBrokerBeganEnabled == false)
                EnableServiceBroker(watcherConn.ConnectionString, false);

            // Stop SqlDependency on this connection
            SqlDependency.Stop(watcherConn.ConnectionString);

            // Clean up our database connection
            if (watcherConn.State == ConnectionState.Open)
                watcherConn.Close();
            watcherConn.Dispose();
            watcherConn = null;

            // Fire our "all clear" event to any client waiting on it   
            if (processingComplete != null)
                processingComplete.Set();
        }

        # endregion Destructor/IDisposable Members
    }//*/
}
