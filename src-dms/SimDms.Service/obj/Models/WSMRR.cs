using SimDms.Service.BLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace SimDms.Service.Models
{
    public class WSMRR
    {
        private readonly WSMRRHeader header;
        private readonly List<WSMRRDetail> details;

        public WSMRR()
        {
            this.header = new WSMRRHeader();
            this.details = new List<WSMRRDetail>();
        }

        public WSMRR(string[] lines)
        {
            if (lines.Length == 0) return;

            this.header = new WSMRRHeader();
            this.details = new List<WSMRRDetail>();

            for (int i = 1; i < lines.Length; i++)
            {
                this.details.Add(new WSMRRDetail(lines[i]));
            }
        }

        public WSMRR(DataSet ds)
        {
            if (ds.Tables.Count > 1 && ds.Tables[0].Rows.Count > 0)
            {
                DataRow row = ds.Tables[0].Rows[0];
                header = new WSMRRHeader();
                header.DealerCode = Convert.ToString(row["DealerCode"]);
                header.ReceivingDealerCode = Convert.ToString(row["ReceivingDealerCode"]);
                header.DealerName = Convert.ToString(row["DealerName"]);
                header.TotalNumberOfItem = Convert.ToInt32(row["TotalNumberOfItem"]);
                header.SendDate = Convert.ToDateTime(row["SendDate"]);
                header.ProductType = Convert.ToString(row["ProductType"]);

                details = new List<WSMRRDetail>();
                for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
                {
                    row = ds.Tables[1].Rows[i];
                    WSMRRDetail detail = new WSMRRDetail();
                    detail.PoliceRegNo = Convert.ToString(row["PoliceRegNo"]);
                    detail.VinNo = Convert.ToString(row["VinNo"]);
                    detail.ServiceType = Convert.ToString(row["ServiceType"]);
                    detail.Initials = Convert.ToString(row["Initials"]);
                    detail.ServiceDate = Convert.ToDateTime(row["ServiceDate"]);
                    detail.Odometer = Convert.ToInt32(row["Odometer"]);
                    detail.Working = Convert.ToString(row["Working"]);
                    detail.CustomerSatisfy = Convert.ToString(row["CustomerSatisfy"]);
                    detail.ReasonNotSatisfy = Convert.ToString(row["ReasonNotSatisfy"]);
                    detail.RemarksNotSatisfy = Convert.ToString(row["RemarksNotSatisfy"]);
                    detail.ReasonDesc = Convert.ToString(row["ReasonDesc"]);
                    detail.Solution = Convert.ToString(row["Solution"]);
                    detail.ContactCustomer = Convert.ToString(row["ContactCustomer"]);
                    detail.ReasonNotContactCustomer = Convert.ToString(row["ReasonNotContactCustomer"]);
                    detail.IsCustomerBooking = Convert.ToString(row["IsCustomerBooking"]);
                    detail.BookingType = Convert.ToString(row["BookingType"]);
                    detail.BookingDate = Convert.ToDateTime(row["BookingDate"]);
                    detail.CustomerComing = Convert.ToString(row["CustomerComing"]);
                    details.Add(detail);
                }
            }
        }

        public WSMRRHeader Header
        {
            get { return header; }
        }

        public List<WSMRRDetail> Details
        {
            get { return details; }
        }

        public string Text
        {
            get
            {
                if (details.Count == 0) return "";
                StringBuilder sb = new StringBuilder();
                sb.AppendLine(header.Text);
                for (int i = 0; i < details.Count; i++)
                {
                    if (i + 1 < details.Count) sb.AppendLine(details[i].Text);
                    else sb.Append(details[i].Text);
                }

                return sb.ToString();
            }
        }

        #region  -- Flat File Class --

        public class WSMRRHeader : FlatFile
        {
            #region -- Initialize --

            public WSMRRHeader()
            {
                string text = "HWSMRR";
                line = "#" + text.PadRight(418, ' ');
            }

            public WSMRRHeader(string text)
            {
                line = "#" + text.PadRight(418, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DataID { get { return GetString(2, 5); } }

            public string DealerCode
            {
                get { return GetString(7, 10); }
                set { SetValue(7, 10, value); }
            }

            public string ReceivingDealerCode
            {
                get { return GetString(17, 10); }
                set { SetValue(17, 10, value); }
            }

            public string DealerName
            {
                get { return GetString(27, 50); }
                set { SetValue(27, 50, value); }
            }

            public int TotalNumberOfItem
            {
                get { return GetInt32(77, 6); }
                set { SetValue(77, 6, value); }
            }

            public DateTime SendDate
            {
                get { return GetDate(83, 8); }
                set { SetValue(83, 8, value); }
            }

            public string ProductType
            {
                get { return GetString(91, 1); }
                set { SetValue(91, 1, value); }
            }

            public string BlankFiller
            {
                get { return GetString(92, 326); }
                set { SetValue(92, 326, value); }
            }

            #endregion
        }

        public class WSMRRDetail : FlatFile
        {
            #region -- Initialize --

            public WSMRRDetail()
            {
                string text = "1";
                line = "#" + text.PadRight(418, ' ');
            }

            public WSMRRDetail(string text)
            {
                line = "#" + text.PadRight(418, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string PoliceRegNo
            {
                get { return GetString(2, 10); }
                set { SetValue(2, 10, value); }
            }

            public string VinNo
            {
                get { return GetString(12, 20); }
                set { SetValue(12, 20, value); }
            }

            public string ServiceType
            {
                get { return GetString(32, 1); }
                set { SetValue(32, 1, value); }
            }

            public string Initials
            {
                get { return GetString(33, 1); }
                set { SetValue(33, 1, value); }
            }

            public DateTime ServiceDate
            {
                get { return GetDate(34, 8); }
                set { SetValue(34, 8, value); }
            }

            public int Odometer
            {
                get { return GetInt32(42, 7); }
                set { SetValue(42, 7, value); }
            }

            public string Working
            {
                get { return GetString(49, 100); }
                set { SetValue(49, 100, value); }
            }

            public string CustomerSatisfy
            {
                get { return GetString(149, 1); }
                set { SetValue(149, 1, value); }
            }

            public string ReasonNotSatisfy
            {
                get { return GetString(150, 1); }
                set { SetValue(150, 1, value); }
            }

            public string RemarksNotSatisfy
            {
                get { return GetString(151, 3); }
                set { SetValue(151, 3, value); }
            }

            public string ReasonDesc
            {
                get { return GetString(154, 100); }
                set { SetValue(154, 100, value); }
            }

            public string Solution
            {
                get { return GetString(254, 150); }
                set { SetValue(254, 150, value); }
            }

            public string ContactCustomer
            {
                get { return GetString(404, 1); }
                set { SetValue(404, 1, value); }
            }

            public string ReasonNotContactCustomer
            {
                get { return GetString(405, 2); }
                set { SetValue(405, 2, value); }
            }

            public string IsCustomerBooking
            {
                get { return GetString(407, 1); }
                set { SetValue(407, 1, value); }
            }

            public string BookingType
            {
                get { return GetString(408, 1); }
                set { SetValue(408, 1, value); }
            }

            public DateTime BookingDate
            {
                get { return GetDate(409, 8); }
                set { SetValue(409, 8, value); }
            }

            public string CustomerComing
            {
                get { return GetString(417, 1); }
                set { SetValue(417, 1, value); }
            }

            #endregion
        }

        #endregion
    }
}