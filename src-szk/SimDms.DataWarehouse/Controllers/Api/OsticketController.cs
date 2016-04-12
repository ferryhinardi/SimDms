using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Data.Entity.Core.Objects;
using System.Data;
using System.Data.SqlClient;
using ClosedXML.Excel;
using System.Data.Odbc;
using System.IO;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class OsticketController : BaseController
    {


        string MyConString = "DRIVER={MySQL ODBC 5.3 ANSI Driver};" +
             "SERVER=tbsdmsdb01;" +
             "DATABASE=ssp;" +
             "UID=root;" +
             "PASSWORD=administrator;" +
             "OPTION=3";

        public JsonResult Default()
        {
            return Json(new {curdate=DateTime.Now.ToString("dd-MMM-yyyy")});
        }
        public void WRFReport()
        {
          
            DataSet ds = new DataSet();

            int[] cw = new int[] { 25, 17, 13, 6, 17, 17, 13, 6, 17,20,25 };
            using (var myconn = new OdbcConnection(MyConString))
            {
                myconn.Open();
                string sql = "";

                DateTime curdate = DateTime.Now;
                DateTime prevdate;
                DateTime aprdate;
                prevdate = curdate.AddMonths(-1);
                prevdate = new DateTime(prevdate.Year, prevdate.Month, DateTime.DaysInMonth(prevdate.Year, prevdate.Month));
                aprdate = new DateTime(curdate.Year - (curdate.Month < 4 ? 1 : 0), 4, 1);

                sql = string.Format(@"
                                    select mdl.*,
                                    opncurr.ttlopn,onpcurr.ttlonp,doncurr.ttldon,null ttlpend,
                                    opnapr.ttlopn apropn,onpapr.ttlonp apronp ,donapr.ttldon aprdon,null aprpend,
                                    opnall.ttlopn allopn
                                    from 
                                    (select id,value 'modul'  from osT_list_items where list_id=1 order by sort) mdl
                                    left join 
                                    (
	                                    select d.value_id 'mid',count(*) ttlopn from ost_ticket t
	                                    inner join ost_form_entry f on f.object_id=t.ticket_id
	                                    inner join ost_form_entry_values d on f.id=d.entry_id and field_id=17
	                                    WHERE t.status = 'open' AND t.isanswered =0  AND t.isoverdue =0
	                                    and date_format(t.created, '%Y%m%d') between '{0}' and '{1}'
	                                    group by d.value_id
                                    ) opncurr on opncurr.mid=mdl.id
                                    left join
                                    (
	                                    select d.value_id 'mid',count(*) ttlonp from ost_ticket t
	                                    inner join ost_form_entry f on f.object_id=t.ticket_id
	                                    inner join ost_form_entry_values d on f.id=d.entry_id and field_id=17
                                        WHERE t.status = 'open' AND (t.isanswered =1 or t.isoverdue=1)
	                                    and date_format(t.created, '%Y%m%d') between '{0}' and '{1}'
	                                    group by d.value_id
                                    ) onpcurr on onpcurr.mid=mdl.id
                                    left join
                                    (
	                                    select d.value_id 'mid',count(*) ttlpend from ost_ticket t
	                                    inner join ost_form_entry f on f.object_id=t.ticket_id
	                                    inner join ost_form_entry_values d on f.id=d.entry_id and field_id=17
	                                    WHERE t.status = 'open' AND t.isoverdue =1 
	                                    and date_format(t.created, '%Y%m%d') between  '{0}' and '{1}'
	                                    group by d.value_id
                                    ) pendcurr on pendcurr.mid=mdl.id
                                    left join
                                    (
	                                    select d.value_id 'mid',count(*) ttldon from ost_ticket t
	                                    inner join ost_form_entry f on f.object_id=t.ticket_id
	                                    inner join ost_form_entry_values d on f.id=d.entry_id and field_id=17
                                        WHERE t.status = 'closed'
	                                    and date_format(t.created, '%Y%m%d') between  '{0}' and '{1}'
	                                    group by d.value_id
                                    ) doncurr on doncurr.mid=mdl.id


                                    left join 
                                    (
	                                    select d.value_id 'mid',count(*) ttlopn from ost_ticket t
	                                    inner join ost_form_entry f on f.object_id=t.ticket_id
	                                    inner join ost_form_entry_values d on f.id=d.entry_id and field_id=17
	                                    WHERE t.status = 'open' AND t.isanswered =0  AND t.isoverdue =0
	                                    and date_format(t.created, '%Y%m%d') between  '{2}' and '{3}'
	                                    group by d.value_id
                                    ) opnapr on opnapr.mid=mdl.id
                                    left join
                                    (
	                                    select d.value_id 'mid',count(*) ttlonp from ost_ticket t
	                                    inner join ost_form_entry f on f.object_id=t.ticket_id
	                                    inner join ost_form_entry_values d on f.id=d.entry_id and field_id=17
                                        WHERE t.status = 'open' AND (t.isanswered =1 or t.isoverdue=1)
	                                    and date_format(t.created, '%Y%m%d') between '{2}' and '{3}'
	                                    group by d.value_id
                                    ) onpapr on onpapr.mid=mdl.id

                                    left join
                                    (
	                                    select d.value_id 'mid',count(*) ttlpend from ost_ticket t
	                                    inner join ost_form_entry f on f.object_id=t.ticket_id
	                                    inner join ost_form_entry_values d on f.id=d.entry_id and field_id=17
	                                    WHERE t.status = 'open' AND t.isoverdue =1 
	                                    and date_format(t.created, '%Y%m%d') between  '{2}' and '{3}'
	                                    group by d.value_id
                                    ) pendapr on pendapr.mid=mdl.id
                                    left join
                                    (
	                                    select d.value_id 'mid',count(*) ttldon from ost_ticket t
	                                    inner join ost_form_entry f on f.object_id=t.ticket_id
	                                    inner join ost_form_entry_values d on f.id=d.entry_id and field_id=17
                                        WHERE t.status = 'closed'
	                                    and date_format(t.created, '%Y%m%d') between  '{2}' and '{3}'
	                                    group by d.value_id
                                    ) donapr on donapr.mid=mdl.id


                                    left join 
                                    (
	                                    select d.value_id 'mid',count(*) ttlopn from ost_ticket t
	                                    inner join ost_form_entry f on f.object_id=t.ticket_id
	                                    inner join ost_form_entry_values d on f.id=d.entry_id and field_id=17
	                                    WHERE 1
	                                    and date_format(t.created, '%Y%m%d') between  '{4}' and '{5}'
	                                    group by d.value_id
                                    ) opnall on opnall.mid=mdl.id
                                    

                    ",
                    (curdate.ToString("yyyyMM") + "01"),
                    curdate.ToString("yyyyMMdd"),
                    aprdate.ToString("yyyyMMdd"),
                    prevdate.ToString("yyyyMMdd") ,
                    aprdate.ToString("yyyyMMdd"),
                    curdate.ToString("yyyyMMdd")
                    );

                var oda = new OdbcDataAdapter(sql, myconn);

                oda.Fill(ds);
                
                //genxls
                var dt = ds.Tables[0];
                int defln=1;
                int defcl=0;
                int ln = defln;
                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("TicketReport");


                int ct = defcl;
                foreach(int i in cw)
                {
                    ct++;
                    ws.Column(ct).Width = i;
                }
                ws.Cell(ln, defcl + 1).Value = "SDMS & SIMDMS MODULE";
                ws.Range(ln,defcl+1,ln+1,defcl+1).Merge();
                ws.Cell(ln, defcl + 1).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                ws.Cell(ln, defcl + 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(194, 214, 154));
                
                ws.Cell(ln, defcl+2).Value = "TICKET "+ curdate.ToString("MMM yyyy").ToUpper();
                ws.Range(ln, defcl+2, ln, defcl+5).Merge();
                ws.Cell(ln+1,defcl+2).Value="NEW / SCHEDULE";
                ws.Cell(ln+1,defcl+3).Value="ONPROGRESS";
                ws.Cell(ln+1,defcl+4).Value="DONE";
                ws.Cell(ln + 1, defcl+5).Value = "CANCEL / PENDING";
                ws.Range(ln, defcl + 2, ln + 1, defcl + 5).Style.Fill.SetBackgroundColor(XLColor.FromArgb(230, 185, 184));

                ws.Cell(ln, defcl + 6).Value = "TICKET 01 APR - " + prevdate.ToString("dd MMM yyyy").ToUpper();
                ws.Range(ln, defcl+6, ln,defcl+9).Merge();
                ws.Cell(ln+1,defcl+6).Value="NEW / SCHEDULE";
                ws.Cell(ln+1,defcl+7).Value="ONPROGRESS";
                ws.Cell(ln+1,defcl+8).Value="DONE";
                ws.Cell(ln+1,defcl+9).Value="CANCEL / PENDING";
                ws.Range(ln, defcl + 6, ln+1, defcl + 9).Style.Fill.SetBackgroundColor(XLColor.FromArgb(182, 221, 232));

                ws.Cell(ln, defcl + 10).Value = "TOTAL TICKET \n 01 APR - " + curdate.ToString("dd MMM yyyy").ToUpper();
                ws.Range(ln,defcl+10,ln+1,defcl+10).Merge();
                ws.Cell(ln, defcl + 10).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Top);
                ws.Cell(ln, defcl + 10).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Right);
                ws.Cell(ln, defcl + 10).Style.Alignment.WrapText = true;
                ws.Cell(ln, defcl + 10).Style.Fill.SetBackgroundColor(XLColor.FromArgb(191, 191, 191));

                ws.Cell(ln, defcl+11).Value = "REMARK";
                ws.Range(ln,defcl+11,ln+1,defcl+11).Merge();
                ws.Cell(ln, defcl + 11).Style.Alignment.SetVertical(XLAlignmentVerticalValues.Center);
                ws.Cell(ln, defcl + 11).Style.Fill.SetBackgroundColor(XLColor.FromArgb(178, 161, 199));

                ws.Cell(ln,defcl+2).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Cell(ln, defcl + 6).Style.Alignment.SetHorizontal(XLAlignmentHorizontalValues.Center);
                ws.Range(ln, defcl + 1, ln + 1, defcl + 11).Style.Font.SetBold();
                ln+=2;
                int c = defcl;
                foreach(DataRow dr in dt.Rows)
                {
                     c=defcl;
                    foreach(DataColumn dc in dt.Columns)
                    {
                        if (c != defcl)
                        { 
                            ws.Cell(ln, c).Value = dr[dc] ?? "";
                            if (c == defcl + 1)
                                ws.Cell(ln, c).Style.Alignment.Indent=2;
                        }
                        c++;
                    }
                    ln++;
                }

                ws.Range(defln + 2, defcl + 1, ln - 1, defcl + 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(215, 228, 188));
                ws.Range(defln + 2, defcl + 2, ln - 1, defcl + 5).Style.Fill.SetBackgroundColor(XLColor.FromArgb(242, 221, 220));
                ws.Range(defln + 2, defcl + 6, ln - 1, defcl + 9).Style.Fill.SetBackgroundColor(XLColor.FromArgb(219, 238, 243));
                ws.Range(defln + 2, defcl + 10, ln - 1, defcl + 10).Style.Fill.SetBackgroundColor(XLColor.FromArgb(216, 216, 216));
                ws.Range(defln + 2, defcl + 11, ln - 1, defcl + 11).Style.Fill.SetBackgroundColor(XLColor.FromArgb(204, 192, 218));




                ws.Cell(ln, defcl + 1).Value = "TOTAL";
                ws.Cell(ln, defcl + 1).Style.Fill.SetBackgroundColor(XLColor.FromArgb(194, 214, 154));
                ws.Range(ln, defcl + 2, ln , defcl + 5).Style.Fill.SetBackgroundColor(XLColor.FromArgb(230, 185, 184));
                ws.Range(ln, defcl + 6, ln, defcl + 9).Style.Fill.SetBackgroundColor(XLColor.FromArgb(182, 221, 232));
                ws.Cell(ln, defcl + 10).Style.Fill.SetBackgroundColor(XLColor.FromArgb(191, 191, 191));                
                ws.Cell(ln, defcl + 11).Style.Fill.SetBackgroundColor(XLColor.FromArgb(178, 161, 199));
                
                for (int i = 2; i < 11; i++)
                {
                    string cl = ws.Cell(ln, defcl + i).WorksheetColumn().ColumnLetter();
                    ws.Cell(ln, i).SetFormulaA1(string.Format("=sum({0}{1}:{0}{2})", cl, defln + 2, ln - 1));
                }

                ws.Range(defln, defcl + 1, ln, c).Style.Border.SetInsideBorder(XLBorderStyleValues.Thin);
                ws.Range(defln, defcl + 1, ln, c).Style.Border.SetOutsideBorder(XLBorderStyleValues.Thin);
                ws.Range(ln, defcl + 1, ln, defcl + 11).Style.Font.SetBold();

                ws.Range(defln+2,defcl+2,ln,defcl+10).Style.NumberFormat.NumberFormatId =3;
                MemoryStream ms = new MemoryStream();
                
                Response.Clear();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment;filename=" + "TICKET-" + curdate.ToString("yyyyMMdd") + ".xlsx");
                Response.ContentType = "application/vnd.ms-excel";

                
                wb.SaveAs(ms);
                Response.BinaryWrite(ms.ToArray());
                Response.End();
                
                

            }
               
            
           
        }



    }
}