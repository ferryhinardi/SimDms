using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SCHEMON.Models;

namespace SCHEMON.Panels
{
    public partial class DeviceCfg : UserControl
    {

        DataGridBinding binddata = null;
        Device ActiveDevice = null;

        public DeviceCfg()
        {
            InitializeComponent();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

 
        private void DeviceCfg_Load(object sender, EventArgs e)
        {
            binddata = new DataGridBinding("select * from IConfigDevices");
            bindingNavigator1.BindingSource = binddata.SourceDataBinding;
            grdDevice.DataSource = binddata.SourceDataBinding;

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            binddata.SaveData();
        }

        private void grdDevice_SelectionChanged(object sender, EventArgs e)
        {
            try
            {
                txtID.Text = grdDevice.CurrentRow.Cells[0].Value.ToString();
                txtName.Text = grdDevice.CurrentRow.Cells[1].Value.ToString();
                txtPort.Text = grdDevice.CurrentRow.Cells[2].Value.ToString();
                txtBD.Text = grdDevice.CurrentRow.Cells[3].Value.ToString();
                txtPhone.Text = grdDevice.CurrentRow.Cells[4].Value.ToString();

            }
            catch (System.Exception ex)
            {

            }
        }

        private async void btnConnect_Click(object sender, EventArgs e)
        {
            SMSDB db = new SMSDB();
            ActiveDevice = await db.Devices.FindAsync(Convert.ToInt32(txtID.Text));

            if (ActiveDevice != null)
            {
                await ActiveDevice.Connect();
                await ActiveDevice.GetDeviceInfo();
                txtOperator.Text = ActiveDevice.Network;
                txtIMEI.Text = ActiveDevice.IMEI;
                txtSignal.Text = ActiveDevice.Signal;
                txtManufacture.Text = ActiveDevice.Manufacture;
                txtModel.Text = ActiveDevice.Model;

                grpTest.Enabled = true;
                ActiveDevice.Subscribe();

                db.Entry(ActiveDevice).State = System.Data.Entity.EntityState.Modified;
                await db.SaveChangesAsync();
            }
        }

        private void grdDevice_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private async void btnDisconnect_Click(object sender, EventArgs e)
        {
            if ( ActiveDevice != null )
            {
                await ActiveDevice.Disconnect();
            }
        }

        private async void btnSend_Click(object sender, EventArgs e)
        {
            if (ActiveDevice != null)
            {
                string Phone = txtDst.Text;
                if (Phone.Substring(0,1) == "*")
                {
                    string Pulsa = await ActiveDevice.ActiveDevice.CheckPulsaAsync(Phone);
                    MessageBox.Show(Pulsa, "Check Pulsa");
                } else 
                    await ActiveDevice.SendSMSAsync(txtDst.Text, txtMsg.Text);
            }
        }
    }
}
