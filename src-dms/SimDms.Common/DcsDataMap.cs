using SimDms.Common.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace SimDms.Common
{
    public class DcsDataMap
    {
        private CommonContext ctx = new CommonContext();

        public void MappingData(string code, string data, out IEnumerable<SysFlatFileHdr> recHdr, ref List<DataTable> listDT)
        {
            try
            {
                var aListData = new ArrayList();

                string[] lines = data.Trim().Split('\n'); string line = "";
                recHdr = ctx.SysFlatFileHdrs.Where(p => p.CodeID == code)
                    .OrderBy(p => p.SeqNo);
                
                if (recHdr.Count() > 0)
                {
                    int max_lenght = (int)ctx.SysFlatFileHdrs.Where(p => p.CodeID == code).Sum(p => p.FieldLength); 
                    line = lines[0].PadRight(max_lenght, ' ');
                    foreach (SysFlatFileHdr var in recHdr)
                    {
                        int pos = var.Position - 1;
                        int len = var.FieldLength;
                        string s = line.Substring(pos, len);
                        var.FieldValue = s;
                    }

                    var recDtl = ctx.SysFlatFileDtls.Where(p => p.CodeID == code && p.DetailID == 1).OrderBy(p => p.SeqNo);
                    DataTable dt_dtl_data = new DataTable();
                    //if(recDtl.Count() > 0){
                    //    foreach (SysFlatFileDtl var in recDtl)
                    //    {
                    //        //XDataGridView.AddColumn(grdDetail, var["FieldName.ToString(), var["FieldDesc.ToString(), 80);
                    //        dt_dtl_data.Columns.Add(var.FieldName, typeof(string));
                    //    }
                    //}
                    
                    if (lines.Length < 51)
                    {
                        //XDataGridView.Initialize(grdDetail);
                        //XDataGridView.AddColumn(grdDetail, "FieldDesc", "Field Name", 130);
                        dt_dtl_data.Columns.Add("FieldDesc", typeof(string));
                        for (int i = 1; i < lines.Length; i++)
                        {
                            //XDataGridView.AddColumn(grdDetail, "Value" + i.ToString(), "Value " + i.ToString(), 120);
                            dt_dtl_data.Columns.Add("Value" + i.ToString(), typeof(string));
                        }
                        //XDataGridView.SetFrozen(grdDetail, "FieldDesc");
                        //XDataGridView.SetDataSource(grdDetail, dt_dtl_data);

                        foreach (SysFlatFileDtl var in recDtl)
                        {
                            DataRow row = dt_dtl_data.NewRow();
                            row["FieldDesc"] = var.FieldDesc;
                            for (int i = 1; i < lines.Length; i++)
                            {
                                line = lines[i].PadRight(max_lenght, ' ');

                                int pos = var.Position - 1;
                                int len = var.FieldLength;
                                string s = line.Substring(pos, len);
                                row["Value" + i.ToString()] = s;
                            }

                            dt_dtl_data.Rows.Add(row);
                        }
                    }
                    else
                    {
                        if (recDtl.Count() > 0)
                        {
                            foreach (SysFlatFileDtl var in recDtl)
                            {
                                //XDataGridView.AddColumn(grdDetail, var["FieldName.ToString(), var["FieldDesc.ToString(), 80);
                                dt_dtl_data.Columns.Add(var.FieldName, typeof(string));
                            }
                        }

                        //XDataGridView.SetDataSource(grdDetail, dt_dtl_data);
                        for (int i = 1; i < lines.Length; i++)
                        {
                            line = lines[i].PadRight(max_lenght, ' ');
                            DataRow row = dt_dtl_data.NewRow();
                            foreach (SysFlatFileDtl var in recDtl)
                            {
                                int pos = var.Position - 1;
                                int len = var.FieldLength;
                                string s = line.Substring(pos, len);
                                row[var.FieldName] = s;
                            }
                            dt_dtl_data.Rows.Add(row);
                        }
                    }

                    listDT.Add(dt_dtl_data);
                }
            }
            catch (Exception ex)
            {
               throw new Exception(ex.Message);
            }
        }

        public void MappingData(string code, string data, int detailCount, out IEnumerable<SysFlatFileHdr> recHdr, ref List<DataTable> listDT)
        {
            try
            {
                string[] lines = data.Trim().Split('\n'); 
                string line = "";
                recHdr = ctx.SysFlatFileHdrs.Where(p => p.CodeID == code).OrderBy(p => p.SeqNo);
                //XDataGridView.Initialize(grdHeader);
                //XDataGridView.AddColumn(grdHeader, "FieldDesc", "Field Name", 130);
                //XDataGridView.AddColumn(grdHeader, "FieldValue", "Field Value");
                //XDataGridView.SetDataSource(grdHeader, recHdr);

                if (recHdr.Count() > 0)
                {
                    int max_lenght = (int)ctx.SysFlatFileHdrs.Where(p => p.CodeID == code).Sum(p => p.FieldLength);
                    line = lines[0].PadRight(max_lenght, ' ');
                    foreach (SysFlatFileHdr var in recHdr)
                    {
                        int pos = var.Position - 1;
                        int len = var.FieldLength;
                        string s = line.Substring(pos, len);
                        var.FieldValue = s;
                    }
                    

                    for (int i = 0; i < detailCount; i++)
                    {
                        if (i > 0)
                        {
                            int seq = i + 1;
                            
                            // ***Add dynamic tabpage
                            //TabPage tabpage = new TabPage("Detail - " + seq.ToString());
                            //DataGridView dgv = new DataGridView();
                            //tabpage.Controls.Add(dgv);
                            //tabControl1.Controls.Add(tabpage);

                            #region *** Properties Datagrid ***
                            //dgv.Width = 670; dgv.Height = 357; dgv.Location = new Point(3, 3); dgv.Dock = DockStyle.Fill;
                            #endregion

                            //XDataGridView.Initialize(dgv);
                            //dgv.ScrollBars = ScrollBars.Both;
                            var dtlID = i + 1;
                            var recDtl = ctx.SysFlatFileDtls.Where(p => p.CodeID == code && p.DetailID == dtlID).OrderBy(p => p.SeqNo);
                            DataTable dt_dtl_data = new DataTable();
                            
                            int counter = 1;
                            int countDtl = recDtl.Count();
                            if(countDtl > 0){
                                foreach (SysFlatFileDtl var in recDtl)
                                {
                                    //if (counter == countDtl)
                                    //    XDataGridView.AddColumn(dgv, var["FieldName.ToString(), var["FieldDesc.ToString(), 0);
                                    //else
                                    //    XDataGridView.AddColumn(dgv, var["FieldName.ToString(), var["FieldDesc.ToString(), 80);
                                        dt_dtl_data.Columns.Add(var.FieldName, typeof(string));
                                    counter++;
                                }
                                //XDataGridView.SetDataSource(dgv, dt_dtl_data);
                                for (int x = 1; x < lines.Length; x++)
                                {
                                    line = lines[x].PadRight(max_lenght, ' ');
                                    if (line.StartsWith(seq.ToString()))
                                    {
                                        DataRow row = dt_dtl_data.NewRow();
                                        foreach (SysFlatFileDtl var in recDtl)
                                        {
                                            int pos = var.Position - 1;
                                            int len = var.FieldLength;
                                            string s = line.Substring(pos, len);
                                            row[var.FieldName] = s;
                                        }
                                        dt_dtl_data.Rows.Add(row);
                                    }
                                }
                            }
                            listDT.Add(dt_dtl_data);
                        }
                        else
                        {
                            var recDtl = ctx.SysFlatFileDtls.Where(p => p.CodeID == code && p.DetailID == 1).OrderBy(p => p.SeqNo);
                            DataTable dt_dtl_data = new DataTable();

                            int counter = 1;
                            int countDtl = recDtl.Count();
                            if(countDtl > 0){
                                foreach (SysFlatFileDtl var in recDtl)
                                {
                                    //if (counter == countDtl)
                                    //    XDataGridView.AddColumn(grdDetail, var["FieldName.ToString(), var["FieldDesc.ToString(), 0);
                                    //else
                                    //    XDataGridView.AddColumn(grdDetail, var["FieldName.ToString(), var["FieldDesc.ToString(), 80);
                                    dt_dtl_data.Columns.Add(var.FieldName, typeof(string));
                                    counter++;
                                }
                                //XDataGridView.SetDataSource(grdDetail, dt_dtl_data);
                                for (int x = 1; x < lines.Length; x++)
                                {
                                    line = lines[x].PadRight(max_lenght, ' ');
                                    if (line.StartsWith("1"))
                                    {
                                        DataRow row = dt_dtl_data.NewRow();
                                        foreach (SysFlatFileDtl var in recDtl)
                                        {
                                            int pos = var.Position - 1;
                                            int len = var.FieldLength;
                                            string s = line.Substring(pos, len);
                                            row[var.FieldName] = s;
                                        }
                                        dt_dtl_data.Rows.Add(row);
                                    }
                                }
                            }
                            listDT.Add(dt_dtl_data);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);    
            }
        }

        public int GetCountDetail(string codeID)
        {
            var records = ctx.SysFlatFileDtls.Where(p => p.CodeID == codeID).GroupBy(p => p.DetailID);

            return records.Count();
        }

        public DataTable MappingDataHeader(string CodeId, string[] lines)
        {
            string line = "";
            int mxLenght = 0;
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;

            cmd.CommandText = string.Format("select *, convert(varchar(200), '') FieldValue from sysFlatFileHdr where CodeID = '{0}' order by SeqNo", CodeId);
            SqlDataAdapter sda2 = new SqlDataAdapter(cmd);
            DataTable dt2 = new DataTable();

            sda2.Fill(dt2);

            line = lines[0].PadRight(mxLenght, ' ');

            DataTable dt = new DataTable();
            dt.Columns.Add("FieldDesc", typeof(string));
            dt.Columns.Add("FieldValue", typeof(string));

            for (int i = 0; i < dt2.Rows.Count; i++)
            {
                int pos = Convert.ToInt32(dt2.Rows[i][2].ToString()) - 1;
                int len = Convert.ToInt32(dt2.Rows[i][3].ToString());
                string s = line.Substring(pos, len);
                string t = dt2.Rows[i][5].ToString();

                DataRow row = dt.NewRow();
                row["FieldDesc"] = t;
                row["FieldValue"] = s;
                dt.Rows.Add(row);
            }
            int test = dt.Rows.Count;
            return dt;
        }
    }
}
