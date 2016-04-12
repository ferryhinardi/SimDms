using SimDms.Service.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Service.Models
{
    public class WSMRDHeader
    {
        public String RecordID { get; set; }
        public String DataID { get; set; }
        public String DealerCode { get; set; }
        public String ReceivingDealerCode { get; set; }
        public String DealerName { get; set; }
        public Int32? TotalItem { get; set; }
        public String SendDate { get; set; }
        public String ProductType { get; set; }
    }

    public class WSMRDDetail
    {
        public String PoliceRegNo { get; set; }
        public String VIN { get; set; }
        public String CustomerName { get; set; }
        public String Gender { get; set; }
        public DateTime? BirthDate { get; set; }
        public String Address { get; set; }
        public String PhoneNo { get; set; }
        public String HPNo { get; set; }
        public String Email { get; set; }
        public String Remarks { get; set; }
    }
}
    public class WSMRDHeaderFlat : FlatFile
    {
        #region -- Initialize --
        public WSMRDHeaderFlat()
        {
            string text = "HWSMRD";
            line = "#" + text.PadRight(300, ' ');
        }
        public WSMRDHeaderFlat(string text)
        {
            line = "#" + text.PadRight(300, ' ');
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

        public string TotalNumberOfItem
        {
            get { return GetString(77, 6); }
            set { SetValue(77, 6, value); }
        }

        public string SendDate
        {
            get { return GetString(83, 8); }
            set { SetValue(83, 8, value); }
        }

        public string ProductType
        {
            get { return GetString(91, 1); }
            set { SetValue(91, 1, value); }
        }

        public string BlankFiller
        {
            get { return GetString(92, 158); }
        }

        public string Text { get { return line.Substring(1); } }

        #endregion

    }

    public class WSMRDDetailFlat : FlatFile
    {
        
        #region -- Initialize --
        public WSMRDDetailFlat()
        {
            string text = "1";
            line = "#" + text.PadRight(300, ' ');
        }

        public WSMRDDetailFlat(string text)
        {
            line = "#" + text.PadRight(300, ' ');
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
        public string CustomerName
        {
            get { return GetString(32, 50); }
            set { SetValue(32, 50, value); }
        }
        public string Gender
        {
            get { return GetString(82, 1); }
            set { SetValue(82, 1, value); }
        }
        public DateTime BirthDate
        {
            get { return GetDate(83, 8); }
            set { SetValue(83, 8, value); }
        }
        public string Address
        {
            get { return GetString(91, 100); }
            set { SetValue(91, 100, value); }
        }
        public string PhoneNo
        {
            get { return GetString(191, 15); }
            set { SetValue(191, 15, value); }
        }
        public string MobileNo
        {
            get { return GetString(206, 20); }
            set { SetValue(206, 20, value); }
        }
        public string EmailAddress
        {
            get { return GetString(226, 50); }
            set { SetValue(226, 50, value); }
        }
        public string Remarks
        {
            get { return GetString(276, 24); }
            set { SetValue(276, 24, value); }
        }
        public string Text { get { return line.Substring(1); } }

        #endregion
    }

    
    