using EventScheduler;
using SVNMON.Panels;
using SVNMON.WebServer;
using MySMSSVC;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SVNMON
{
    public partial class frmConfig : Form
    {

        private Desktop m_Desktop = new Desktop();
        private IContainer m_Container = null;
        private NotifyIcon m_NotifyIcon = null;

        private bool IsMustExit = false;

        //private Button btnHide;
        private ContextMenu m_ContextMenu = null;

        public event MyServiceHandler mNotifyTgr = null;

        public static frmConfig StartUIThread()
        {
            frmConfig dlg = new frmConfig();
            dlg.Visible = false;
            Thread thread = new Thread(new ThreadStart(dlg.UIThread));
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
            MyShared.DebugInfo("New thread created: Loading frmConfig...");
            return dlg;
        }

        public void UIThread()
        {
            if (!m_Desktop.BeginInteraction())
                return;
            this.Visible = false;
            this.Hide();
            Application.Run(this);
        }

        //SmsDB db = new SmsDB();
        public frmConfig()
        {
            InitializeComponent();
            //var test = db.Database.ExecuteSqlCommand("select getdate()");
        }

        List<BasePanel> myPanels = new List<BasePanel>();
        string lastNode = "";


        private void LoadPanel(string pnlName)
        {

            if (lastNode == pnlName) return;

            UserControl control = null;
            if (myPanels.Count == 0 )
            {
                control = (UserControl)Activator.CreateInstance(Type.GetType("SVNMON.Panels." + pnlName));
                var newCtl = new BasePanel { Name = pnlName, ObjectPtr = control };
                myPanels.Add(newCtl);                
            }
            else
            {
                var Uc = myPanels.Find(x => x.Name == pnlName);
                if (Uc != null)
                {
                    control = (UserControl) Uc.ObjectPtr;
                }
                else
                {
                    control = (UserControl)Activator.CreateInstance(Type.GetType("SVNMON.Panels." + pnlName));
                    var newCtl = new BasePanel { Name = pnlName, ObjectPtr = control };
                    myPanels.Add(newCtl);
                }
            }

            mainPanel.Controls.Clear();

            if (control != null )
            {
                mainPanel.Controls.Add(control);
                control.Dock = DockStyle.Fill;
                control.Visible = true;
                lastNode = pnlName;
            }
            else
            {
                mainPanel.Refresh();
            }

        }

        private void tvwMenu_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            switch( e.Node.Name )
            {
                case "nDatabase":
                    LoadPanel("DatabaseCfg");
                    break;
                case "nDevice":
                    LoadPanel("DeviceCfg");
                    break;
                case "nUsers":
                    LoadPanel("UsersCfg");
                    break;
                case "nService":
                    LoadPanel("ServiceCfg");
                    break;
                default:
                    break;
            }
        }

        public void OpenDialog(Object sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            BringToFront();
        }

        public void HideDialog(Object sender, EventArgs e)
        {
            this.Visible = false;
            this.Hide();
        }

        public void ExitDialog(Object sender, EventArgs e)
        {
            IsMustExit = true;
            m_NotifyIcon.Dispose();
            m_ContextMenu.Dispose();
            m_Container.Dispose();
            Close();
            if (mNotifyTgr != null)
            {
                mNotifyTgr(0);
            }
        }

        protected override void OnClosed(EventArgs e)
        {           
            m_NotifyIcon.Dispose();
            m_ContextMenu.Dispose();
            m_Container.Dispose();
        }

        private void frmConfig_Load(object sender, EventArgs e)
        {
            Hide();
            m_ContextMenu = new ContextMenu();
            m_ContextMenu.MenuItems.Add(new MenuItem("Open", this.OpenDialog));
            m_ContextMenu.MenuItems.Add(new MenuItem("Hide", this.HideDialog));
            m_ContextMenu.MenuItems.Add(new MenuItem("-"));
            m_ContextMenu.MenuItems.Add(new MenuItem("Exit", this.ExitDialog));

            Icon icon = new Icon(SystemIcons.Application, 16, 16);
            m_Container = new Container();
            m_NotifyIcon = new NotifyIcon(m_Container);
            m_NotifyIcon.ContextMenu = m_ContextMenu;
            m_NotifyIcon.Icon = this.Icon;
            m_NotifyIcon.Visible = true;
            m_NotifyIcon.Tag = MyShared.ServiceDesc;

            // Handle the DoubleClick event to activate the form.
            m_NotifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon1_DoubleClick);


            m_NotifyIcon.ShowBalloonTip(200
                , "Schedule Monitoring Service"
                , MyShared.ServiceDesc
                , ToolTipIcon.Info
                );
        }

        private void notifyIcon1_DoubleClick(object Sender, EventArgs e)
        {
            this.OpenDialog(Sender,e);
        }

        private void frmConfig_FormClosing(object sender, FormClosingEventArgs e)
        {
          
            if (!IsMustExit)
            {
                WindowState = FormWindowState.Minimized;
                this.Hide(); 
                // Cancel the Closing event from closing the form. 
                e.Cancel = !IsMustExit;
            }
        }
    }
}
