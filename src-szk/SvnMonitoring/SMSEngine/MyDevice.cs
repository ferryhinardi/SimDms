using EventScheduler;
using MySMSSVC;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCHEMON.Models
{
    public partial class Device
    {

        private MySMS mydevice = null;
        private bool IsDisposed = false;
        private bool IsRegister = false;
        private bool IsSendingMsg = false;

        ~Device()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool Diposing)
        {
            if (!IsDisposed)
            {
                if (mydevice != null)
                {
                    mydevice.ClosePort();
                    mydevice = null;
                }
            }
            IsDisposed = true;
        }

        public async Task Disconnect()
        {
            try
            {
                if (mydevice != null)
                {
                    MyGlobalVar.DebugInfo(string.Format("Disconnecting from {0}...", mydevice.PortName));
                    await mydevice.CloseAsync();
                    mydevice = null;
                    MyGlobalVar.DebugInfo("Connection closed");
                }
            }
            catch (Exception ex)
            {
                MyGlobalVar.DebugInfo(ex.Message);
            }
        }

        public async Task Connect()
        {
            mydevice = new MySMS();

            if (this.PortName == null)
            {
                this.PortName = "COM5";
            }
            if (this.BaudRate == null)
            {
                this.BaudRate = 115200;
            }

            mydevice.BaudRate = (int)this.BaudRate;
            mydevice.PortName = this.PortName;
            mydevice.AllowDebug = false;

            try
            {
                bool bConnect = await mydevice.OpenAsync();

                if (bConnect)
                {
                    MyGlobalVar.DebugInfo(string.Format("Connect to {0}", mydevice.PortName));
                }
                else
                {
                    MyGlobalVar.DebugInfo("Invalid port settings");
                }

                Observable.Interval(TimeSpan.FromSeconds(5))
                    .StartWith(-1L)
                    .ObserveOn(ThreadPoolScheduler.Instance)
                    .Where(i => IsRegister)     // IsProcessing is the bool value
                    .Subscribe(_ => CheckInbox());

            }
            catch (Exception ex)
            {
                MyGlobalVar.DebugInfo(ex.Message);
            }

        }

        public async Task<int> SendSMSAsync(string PhoneNo, string Msg)
        {
            if (mydevice == null)
            {
                await Connect();
            }
            if (mydevice != null)
            {
                var refid = await mydevice.SendSMSAsync(PhoneNo, Msg);
                return refid;
            }

            return 0;
        }

        public async Task<string> CheckPulsa(string CUSD)
        {
            if (mydevice == null)
            {
                await Connect();
            }
            if (mydevice != null)
            {
                var refid = await mydevice.CheckPulsaAsync(CUSD);
                return refid;
            }

            return "Tidak dapat melakukan pengecekan pulsa";
        }

        private async Task SendMessageAsync()
        {
            if (IsSendingMsg) return;

            IsSendingMsg = true;
            
            try
            {
                using (SMSDB db = new SMSDB())
                {
                    string sql = string.Format("select * from ISmsOutbox where DeviceId={0} and refno=0", Id); ;
                    List<Outbox> listMsg = await db.Database.SqlQuery<Outbox>(sql).ToListAsync();

                    if ( listMsg.Count > 0)
                    {
                        int n = listMsg.Count, i = 0;
                        //MyGlobalVar.DebugInfo("Starting sending message ... WIP: " + n.ToString());

                        foreach (Outbox outMsg in listMsg)
                        {
                            if (!string.IsNullOrEmpty(outMsg.Message))
                            {
                                var refid = await mydevice.SendSMSAsync(outMsg.PhoneNo, outMsg.Message);
                                if (refid > 0) // success
                                {
                                    sql = "UPDATE ISmsOutbox set SentOn=getdate(), RefNo={0}, [status]=1 where id={1}; "
                                          + string.Format("UPDATE IConfigDevices set ModifiedOn=getdate(), SMSOUT = SMSOUT + 1 where id={0}", Id);

                                    await db.Database.ExecuteSqlCommandAsync(sql, refid, outMsg.Id);
                                    i++;
                                }
                            }

                        }

                        

                        sql = "select * from INewsReminder where Active=1 and InProgress=0 and NextOn > getdate()";
                        var reminders = db.Database.SqlQuery<Reminder>(sql).ToList();

                        foreach (var reminder in reminders)
                        {
                            var NewsId = reminder.NewsId.ToString();
                            DateTime nextOn = reminder.NextOn;
                            reminder.InProgress = 1;
                            reminder.ModifiedOn = DateTime.Now;
                            db.Entry(reminder).State = EntityState.Modified;
                            db.SaveChanges();

                            Observable.Timer(nextOn, NewThreadScheduler.Default).Take(1)
                                      .Subscribe(_ => MyGlobalVar.DebugInfo("Reminder #" + NewsId + " was executed"), () => ExecuteReminder(NewsId));
                        }

                        MyGlobalVar.DebugInfo("Message Sent (" + i.ToString() + "/" + n.ToString() + ") => Reminder: " + reminders.Count.ToString());

                    }


                }
                
            }
            catch (Exception eX)
            {
                MyGlobalVar.DebugInfo("Error when sending message: " + eX.Message);
            }
            finally
            {
                IsSendingMsg = false;
            }
        }

        private async Task RunScheduleReminder(string AutoNo)
        {
            using (SMSDB db = new SMSDB())
            {
                try
                {
                    string SQL = "EXEC CHECK_REMINDER {0}";
                    await db.Database.ExecuteSqlCommandAsync(SQL, AutoNo);
                } catch (Exception eX)
                {
                    MyGlobalVar.DebugInfo("Error On Running Schedule: " + eX.Message);
                }
            }
        }


        private async Task<bool> CheckIncomingMessage()
        {
            IsRegister = false;
            bool iResult = false;
            DateTime StartCheckin = DateTime.Now;
            //MyGlobalVar.DebugInfo("Start checking");
            int new_sms = 0;
            SMSDB db = new SMSDB();
            string SQL = "select * from ISmsDummy where [status]=0 order by multipart, sender, partref, partno";
            
            try
            {
                ShortMessageCollection lsM = await mydevice.ListUnReadSMSAsync();

                if (lsM != null)
                {
                    await mydevice.DeleteReadMsgAsync();

                   
                    Repository<Inbox> repoInbox = new Repository<Inbox>(db);
                    Repository<Dummy> repoTemp = new Repository<Dummy>(db);

                    foreach (ShortMessage sms in lsM)
                    {
                        Dummy tmp = new Dummy();
                        tmp.Message = sms._message;
                        tmp.Multipart = sms.MultiPart;
                        tmp.PartCount = sms.PartCount;
                        tmp.PartNo = sms.PartNumber;
                        tmp.Sender = sms._phoneNumber;
                        tmp.SmscDate = sms._serviceCenterTimeStamp;
                        tmp.PartRef = sms.PartRefefence;
                        tmp.Status = 0;                        
                        await repoTemp.AddAsync(tmp);                      
                    }

                    new_sms = lsM.Count;

                    await db.Database.ExecuteSqlCommandAsync("update IConfigDevices set ModifiedOn=getdate(), SMSIN = SMSIN + {0} where id={1}", new_sms, Id);
                
                
                
                var tmpSMS = await db.Database.SqlQuery<Dummy>(SQL).ToListAsync();

                if (tmpSMS.Count > 0)
                {
                    SQL = "delete from ISmsDummy where id in (0";

                    int nRows = tmpSMS.Count;
                    int LastPart = 0;
                    int LastRef = -1;
                    bool GoodPart = false;
                    string LastPhone = "";
                    List<Dummy> lstTemp = new List<Dummy>();

                    if (nRows > 0)
                    {

                        #region looping in collection

                        string sqlSenderName = "select * from IContactList where Id=(select ContactID from IContactPhone where PhoneNo = '{0}' and [Active] = 1)";

                        for (int i = 0; i < nRows; i++)
                        {
                            Dummy tmp = tmpSMS[i];
                            if (tmp != null)
                            {
                                if (tmp.Multipart == false)
                                {
                                    Inbox inbox = new Inbox();
                                    inbox.PhoneNo = tmp.Sender;
                                    inbox.SmscDate = tmp.SmscDate;
                                    inbox.Message = tmp.Message;
                                    inbox.EmployeeId = 0;
                                    inbox.DeviceId = Id;
                                    inbox.Status = 0;
                                    inbox.Deleted = 0;

                                    Contact senderName = await db.Database.SqlQuery<Contact>(string.Format(sqlSenderName, tmp.Sender)).FirstOrDefaultAsync();
                                    if (senderName != null)
                                    {
                                        inbox.ContactName = senderName.Name;
                                        inbox.EmployeeId = senderName.EmployeeId;
                                    }

                                    await repoInbox.AddAsync(inbox);
                                    SQL += "," + tmp.Id.ToString();
                                }
                                else
                                {
                                    if (tmp.PartNo == 1)
                                    {
                                        if (lstTemp.Count > 0)
                                        {
                                            lstTemp.Clear();
                                        }
                                        GoodPart = true;
                                        LastPhone = tmp.Sender;
                                        LastPart = tmp.PartNo;
                                        LastRef = tmp.PartRef;
                                        lstTemp.Add(tmp);
                                    }
                                    else if (tmp.PartNo == tmp.PartCount)
                                    {
                                        if (GoodPart && (lstTemp.Count == (tmp.PartCount - 1)))
                                        {
                                            Inbox inbox = new Inbox();
                                            inbox.PhoneNo = tmp.Sender;
                                            inbox.SmscDate = tmp.SmscDate;
                                            inbox.Status = 0;
                                            inbox.Deleted = 0;

                                            for (int x = 0; x < lstTemp.Count; x++)
                                            {
                                                inbox.Message += lstTemp[x].Message;
                                                SQL += "," + lstTemp[x].Id.ToString();

                                                if (inbox.SmscDate < lstTemp[x].SmscDate)
                                                {
                                                    inbox.SmscDate = lstTemp[x].SmscDate;
                                                }
                                            }

                                            inbox.Message += tmp.Message;
                                            inbox.DeviceId = Id;

                                            Contact senderName = await db.Database.SqlQuery<Contact>(string.Format(sqlSenderName, tmp.Sender)).FirstOrDefaultAsync();
                                            if (senderName != null)
                                            {
                                                inbox.ContactName = senderName.Name;
                                                inbox.EmployeeId = senderName.EmployeeId;
                                            }       

                                            await repoInbox.AddAsync(inbox);

                                            lstTemp.Clear();
                                            SQL += "," + tmp.Id.ToString();

                                        }
                                    }
                                    else
                                    {
                                        if (LastRef != tmp.PartRef) GoodPart = false;   
                                        if (LastPhone != tmp.Sender) GoodPart = false;
                                        if (tmp.PartNo - LastPart > 1) GoodPart = false;
                                        
                                        if (tmp.PartNo - LastPart == 1)
                                        {
                                            lstTemp.Add(tmp);
                                            LastPart = tmp.PartNo;
                                            LastRef = tmp.PartRef;
                                        }
                                    }
                                }
                            }
                        }
                        #endregion

                        // remove temporary message
                        SQL += ")";
                        await db.Database.ExecuteSqlCommandAsync(SQL);

                    }
                }

                    // clean up sms temp
                    iResult = true;
                }
            }
            catch (Exception eX)
            {
                MyGlobalVar.DebugInfo(eX.Message);
            }
            finally
            {
                
                //MyGlobalVar.DebugInfo(string.Format("Check Incoming SMS: {0} sms, Duration: {1} ms", new_sms, DateTime.Now.Subtract(StartCheckin).TotalMilliseconds));

                // Starting to send auto response / auto forwarding
                Task.Run(() => SendMessageAsync());

                IsRegister = true;
            }

            return iResult;
        }

        private void ExecuteReminder(string Id)
        {
            try
            {
                SMSDB db = new SMSDB();
                string SQL = "EXEC CHECK_REMINDER " + Id;
                db.Database.ExecuteSqlCommand(SQL);

            } catch (Exception Ex)
            {
                MyGlobalVar.DebugInfo("Error on Executing Reminder: " + Ex.Message);
            }

        }

        async void CheckInbox()
        {
            if (mydevice == null)
            {
                await Connect();
            }
            await CheckIncomingMessage();
        }

        public void Subscribe()
        {
            IsRegister = true;
        }

        public void Unsubscribe()
        {
            IsRegister = false;
        }

        [NotMapped]
        public MySMS ActiveDevice
        {
            get
            {
                return mydevice;
            }
        }

        public async Task GetDeviceInfo()
        {
            Network = await mydevice.NetworkInfoAsync();
            Manufacture = await mydevice.ManufactureAsync();
            Model = await mydevice.ModelAsync();
            IMEI = await mydevice.IMEIAsync();
            Signal = await mydevice.SignalStrengthAsync();
            ServiceCenter = await mydevice.SMSCentreAsync();
        }

    }

}
