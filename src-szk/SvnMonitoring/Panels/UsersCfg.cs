using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using Microsoft.AspNet.SignalR;
using SCHEMON.WebServer.Configs;
using System.Security.Permissions;

using System.Threading;
using EventScheduler.Shared;
using EventScheduler;
using SCHEMON.Models;

namespace SCHEMON.Panels
{
    public partial class UsersCfg : UserControl
    {

        DataGridBinding binddata = null;

        public UsersCfg()
        {
            InitializeComponent();
        }

        private void grdDevice_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void UsersCfg_Load(object sender, EventArgs e)
        {
                binddata = new DataGridBinding("select * from IConfigUsers");
                bindingNavigator1.BindingSource = binddata.SourceDataBinding;
                gridUsers.DataSource = binddata.SourceDataBinding;
        }

        private void gridUsers_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            if (gridUsers.CurrentRow.Tag != null)
                e.Control.Text = gridUsers.CurrentRow.Tag.ToString();
        }

        private void gridUsers_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.ColumnIndex == 2)
            {
                if (e.Value != null)
                {
                    e.Value = new string('*', e.Value.ToString().Length);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            binddata.SaveData();
        }

    }

}
