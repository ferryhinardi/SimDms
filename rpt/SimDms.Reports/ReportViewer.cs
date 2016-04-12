using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.Reporting.WinForms;
using SimDms.Reports.Model;
using System.IO;
using System.Configuration;

namespace SimDms.Reports
{
    public partial class ReportViewer : Form
    {
        public ReportViewer()
        {
            InitializeComponent();
        }

        private DataSet renderRpt(string str, string path)
        {
            var urlDataCS = ConfigurationManager.ConnectionStrings["CSDataContext"].ConnectionString;
            SqlConnection cnn = new SqlConnection(urlDataCS);

            SqlDataAdapter da = new SqlDataAdapter(str, cnn);
            DataSet ds = new DataSet();
            da.Fill(ds);

            using (StreamReader rdlcSR = new StreamReader(path))
            {
                reportViewer1.LocalReport.ReportPath = path;
                reportViewer1.LocalReport.LoadReportDefinition(rdlcSR);
            }
            return ds;
        }

        private void ReportViewer_Load(object sender, EventArgs e)
        {
            this.reportViewer1.RefreshReport();
        }

        private void ribbonPanel2_Click(object sender, EventArgs e)
        {
            DataSet ds = renderRpt("exec uspfn_CsRptBirthday '%','%','%'", "CSBirthDayRpt.rdlc");
            var ent2 = ds.Tables[0].AsEnumerable().ToList().Select(o =>
                new CSBirthDayModel()
                {
                    companyname = o.ItemArray[0].ToString(),
                    branchname = o.ItemArray[1].ToString(),
                    customername = o.ItemArray[2].ToString(),
                    CustomerAddress = o.ItemArray[3].ToString(),
                    CustomerTelephone = o.ItemArray[4].ToString(),
                    cartype = o.ItemArray[5].ToString(),
                    color = o.ItemArray[6].ToString(),
                    policeregno = o.ItemArray[7].ToString(),
                    engine = o.ItemArray[8].ToString(),
                    chassis = o.ItemArray[9].ToString(),
                    CustomerBirthDate = o.ItemArray[10].ToString().Length > 0 ? o.ItemArray[10].ToString().Substring(0, o.ItemArray[10].ToString().IndexOf(' ')) : "",
                    salesmanname = o.ItemArray[11].ToString()
                });


            var ent4 = ds.Tables[1].AsEnumerable().ToList().Select(o =>
                 new CSSpouseModel()
                 {
                     SpouseName = o.ItemArray[0].ToString(),
                     SpouseTelephone = o.ItemArray[1].ToString(),
                     SpouseBirthDate = o.ItemArray[2].ToString().Length > 0 ? o.ItemArray[2].ToString().Substring(0, o.ItemArray[2].ToString().IndexOf(' ')) : ""
                 });

            var ent5 = ds.Tables[2].AsEnumerable().ToList().Select(o =>
                 new CSChildrenModel()
                 {
                     Children = o.ItemArray[0].ToString(),
                     BirthDate = o.ItemArray[1].ToString().Length > 0 ? o.ItemArray[1].ToString().Substring(0, o.ItemArray[1].ToString().IndexOf(' ')) : ""
                 });

            var reportDataSource2 = new ReportDataSource("DataSet2", ent2);
            var reportDataSource4 = new ReportDataSource("DataSet4", ent4);
            var reportDataSource5 = new ReportDataSource("DataSet5", ent5);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource2);
            reportViewer1.LocalReport.DataSources.Add(reportDataSource4);
            reportViewer1.LocalReport.DataSources.Add(reportDataSource5);

            this.reportViewer1.RefreshReport();
        }

        private void ribbonPanel3_Click(object sender, EventArgs e)
        {
            DataSet ds = renderRpt("exec uspfn_CsRptHoliday '%','%','%'", "CSHoliDayRpt.rdlc");
            var ent = ds.Tables[0].AsEnumerable().ToList().Select(o =>
                new CSHolidayModel()
                {
                    companyname = o.ItemArray[0].ToString(),
                    branchname = o.ItemArray[1].ToString(),
                    customername = o.ItemArray[2].ToString(),
                    address = o.ItemArray[3].ToString(),
                    phoneno = o.ItemArray[4].ToString(),
                    cartype = o.ItemArray[5].ToString(),
                    color = o.ItemArray[6].ToString(),
                    policeregno = o.ItemArray[7].ToString(),
                    engine = o.ItemArray[8].ToString(),
                    chassis = o.ItemArray[9].ToString(),
                    bpkdate = o.ItemArray[10].ToString().Substring(0, o.ItemArray[10].ToString().IndexOf(' ')),
                    religion = o.ItemArray[11].ToString(),
                    salesmanname = o.ItemArray[12].ToString(),
                    holiday_call = o.ItemArray[13].ToString()
                });


            var reportDataSource = new ReportDataSource("DataSet1", ent);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);

            this.reportViewer1.RefreshReport();
        }

        private void ribbonPanel4_Click(object sender, EventArgs e)
        {
            DataSet ds = renderRpt("exec uspfn_CsRptStnkExt '%','%','%'", "CS_STNK_EXTRpt.rdlc");
            var ent = ds.Tables[0].AsEnumerable().ToList().Select(o => new CS_STNK_EXTModel()
            {
                companyname = o.ItemArray[0].ToString(),
                branchname = o.ItemArray[1].ToString(),
                customername = o.ItemArray[2].ToString(),
                address = o.ItemArray[3].ToString(),
                phoneno = o.ItemArray[4].ToString(),
                cartype = o.ItemArray[5].ToString(),
                color = o.ItemArray[6].ToString(),
                policeregno = o.ItemArray[7].ToString(),
                engine = o.ItemArray[8].ToString(),
                chassis = o.ItemArray[9].ToString(),
                bpkdate = o.ItemArray[10].ToString().Substring(0, o.ItemArray[10].ToString().IndexOf(' ')),
                stnkdate = o.ItemArray[11].ToString().Substring(0, o.ItemArray[11].ToString().IndexOf(' ')),
                salesmanname = o.ItemArray[12].ToString(),
                STNK_CALL = o.ItemArray[13].ToString(),
                STNK_YN = o.ItemArray[14].ToString()
            });

            var reportDataSource = new ReportDataSource("DataSet1", ent);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);

            this.reportViewer1.RefreshReport();
        }

        private void ribbonPanel5_Click(object sender, EventArgs e)
        {
            DataSet ds = renderRpt("exec uspfn_CsRptBPKBNOF '%','%','%'", "CS_BPKB_NOFRpt.rdlc");
            var ent = ds.Tables[0].AsEnumerable().ToList().Select(o => new CS_BPKB_NOFModel()
                {
                   companyname   = o.ItemArray[0].ToString(),
                   branchname    = o.ItemArray[1].ToString(),
                   customername  = o.ItemArray[2].ToString(),
                   address       = o.ItemArray[3].ToString(),
                   phoneno       = o.ItemArray[4].ToString(),
                   cartype       = o.ItemArray[5].ToString(),
                   color         = o.ItemArray[6].ToString(),
                   policeregno   = o.ItemArray[7].ToString(),
                   engine        = o.ItemArray[8].ToString(),
                   chassis       = o.ItemArray[9].ToString(),
                   bpkdate = o.ItemArray[10].ToString().Substring(0, o.ItemArray[10].ToString().IndexOf(' ')),
                   stnkdate = o.ItemArray[11].ToString().Substring(0, o.ItemArray[11].ToString().IndexOf(' ')),
                   bpkbdate_ready= o.ItemArray[12].ToString(),
                   salesmanname  = o.ItemArray[13].ToString(),
                   bpkbdate_call = o.ItemArray[14].ToString()
                });

            var reportDataSource = new ReportDataSource("DataSet1", ent);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);

            this.reportViewer1.RefreshReport(); 
        }

        private void ribbonPanel6_Click(object sender, EventArgs e)
        {
            DataSet ds = renderRpt("exec uspfn_CsRptTDayCall '%','%','%'", "CSTDayCallRpt.rdlc");
            var ent = ds.Tables[0].AsEnumerable().ToList().Select(o => new CSTDayCallModel()
            {
                companyname = o.ItemArray[0].ToString(),
                branchname = o.ItemArray[1].ToString(),
                customername = o.ItemArray[2].ToString(),
                address = o.ItemArray[3].ToString(),
                phoneno = o.ItemArray[4].ToString(),
                cartype = o.ItemArray[5].ToString(),
                color = o.ItemArray[6].ToString(),
                policeregno = o.ItemArray[7].ToString(),
                engine = o.ItemArray[8].ToString(),
                chassis = o.ItemArray[9].ToString(),
                bpkdate = o.ItemArray[10].ToString().Substring(0, o.ItemArray[10].ToString().IndexOf(' ')),
                stnkdate = o.ItemArray[11].ToString().Substring(0, o.ItemArray[11].ToString().IndexOf(' ')),
                salesmanname = o.ItemArray[12].ToString(),
            });

            var reportDataSource = new ReportDataSource("DataSet1", ent);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(reportDataSource);

            this.reportViewer1.RefreshReport(); 
        }
    }
}
