using SimDms.PreSales.BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.PreSales.Models
{
    public class HITSO
    {
        private readonly HITSOHeader header;
        private readonly List<HITSODetailPmKDP> detailPmKDP;
        private HITSODetailPmKDP dPmKDP;
        private HITSODetailStatusHistory dStatusHistory;
        private HITSODetailActivities dActivities;

        public HITSO()
        {
            this.header = new HITSOHeader();
            this.detailPmKDP = new List<HITSODetailPmKDP>();
        }

        public HITSO(string[] lines)
        {
            if (lines.Length == 0) return;

            this.header = new HITSOHeader(lines[0]);
            this.detailPmKDP = new List<HITSODetailPmKDP>();

            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].StartsWith("1"))
                {
                    if (dPmKDP != null) this.detailPmKDP.Add(dPmKDP);
                    dPmKDP = new HITSODetailPmKDP(lines[i]);
                }
                else if (lines[i].StartsWith("2"))
                {
                    dStatusHistory = new HITSODetailStatusHistory(lines[i]);
                    dPmKDP.DetailStatusHistories.Add(dStatusHistory);
                }
                else
                {
                    dActivities = new HITSODetailActivities(lines[i]);
                    dPmKDP.DetailActivities.Add(dActivities);
                }
            }

            if (dPmKDP != null) this.detailPmKDP.Add(dPmKDP);
        }

        public HITSOHeader Header
        {
            get { return header; }
        }

        public List<HITSODetailPmKDP> DetailPmKDP
        {
            get { return detailPmKDP; }
        }

        #region -- Flat File Class --

        public class HITSOHeader : FlatFile
        {
            #region -- Initialize --

            public HITSOHeader()
            {
                string text = "HHITSO";
                line = "#" + text.PadRight(1200, ' ');
            }

            public HITSOHeader(string text)
            {
                line = "#" + text.PadRight(1200, ' ');
            }


            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string DataID { get { return GetString(2, 5); } }

            public string CompanyCode { get { return GetString(7, 15); } }

            public string BranchCode { get { return GetString(22, 15); } }

            public string BlankFiller { get { return GetString(37, 1164); } }

            #endregion
        }

        public class HITSODetailPmKDP : FlatFile
        {
            #region -- Initialize --

            private List<HITSODetailActivities> detailActivities;
            private List<HITSODetailStatusHistory> detailStatusHistories;

            public HITSODetailPmKDP()
            {
                detailActivities = new List<HITSODetailActivities>();
                detailStatusHistories = new List<HITSODetailStatusHistory>();
                string text = "1";
                line = "#" + text.PadRight(1200, ' ');
            }

            public HITSODetailPmKDP(string text)
            {
                detailStatusHistories = new List<HITSODetailStatusHistory>();
                detailActivities = new List<HITSODetailActivities>();
                line = "#" + text.PadRight(1200, ' ');
            }

            public List<HITSODetailActivities> DetailActivities
            {
                get { return detailActivities; }
            }

            public List<HITSODetailStatusHistory> DetailStatusHistories
            {
                get { return detailStatusHistories; }
            }


            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public Int32 InquiryNumber { get { return GetInt32(2, 9); } }

            public string CompanyCode { get { return GetString(11, 15); } }

            public string BranchCode { get { return GetString(26, 15); } }

            public string EmployeeID { get { return GetString(41, 15); } }

            public string SpvemployeeID { get { return GetString(56, 15); } }

            public DateTime InquiryDate { get { return GetDate(71, 8); } }

            public string OutletID { get { return GetString(79, 15); } }

            public string StatusProspek { get { return GetString(94, 2); } }

            public string PerolehanData { get { return GetString(96, 15); } }

            public string NamaProspek { get { return GetString(111, 30); } }

            public string AlamatProspek { get { return GetString(141, 200); } }

            public string TelpRumah { get { return GetString(341, 15); } }

            public string CityID { get { return GetString(356, 15); } }

            public string NamaPerusahaan { get { return GetString(371, 50); } }

            public string AlamatPerusahaan { get { return GetString(421, 200); } }

            public string Jabatan { get { return GetString(621, 30); } }

            public string HandPhone { get { return GetString(651, 14); } }

            public string Faximile { get { return GetString(665, 15); } }

            public string Email { get { return GetString(680, 50); } }

            public string TipeKendaraan { get { return GetString(730, 20); } }

            public string Variant { get { return GetString(750, 50); } }

            public string Transmisi { get { return GetString(800, 2); } }

            public string ColourCode { get { return GetString(802, 3); } }

            public string CaraPembayaran { get { return GetString(805, 2); } }

            public string TestDrive { get { return GetString(807, 2); } }

            public Int32 QuantityInquiry { get { return GetInt32(809, 4); } }

            public string LastProgress { get { return GetString(813, 15); } }

            public DateTime LastUpdateStatus { get { return GetDate(828, 8); } }

            public DateTime SPKDate { get { return GetDate(836, 8); } }

            public DateTime LostCaseDate { get { return GetDate(844, 8); } }

            public string LostCaseCategory { get { return GetString(852, 1); } }

            public string LostCaseReasonID { get { return GetString(853, 2); } }

            public string LostCaseOtherReason { get { return GetString(855, 100); } }

            public string LostCaseVoiceOfCustomer { get { return GetString(955, 200); } }

            public DateTime CreationDate { get { return GetDate(1155, 8); } }

            public string CreatedBy { get { return GetString(1163, 15); } }

            public string LastUpdateBy { get { return GetString(1178, 15); } }

            public DateTime LastUpdateDate { get { return GetDate(1193, 8); } }

            #endregion
        }

        public class HITSODetailStatusHistory : FlatFile
        {
            #region -- Initialize --

            public HITSODetailStatusHistory()
            {
                string text = "2";
                line = "#" + text.PadRight(1200, ' ');
            }

            public HITSODetailStatusHistory(string text)
            {
                line = "#" + text.PadRight(1200, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public Int32 InquiryNumber { get { return GetInt32(2, 9); } }

            public string CompanyCode { get { return GetString(11, 15); } }

            public string BranchCode { get { return GetString(26, 15); } }

            public Int32 SequenceNo { get { return GetInt32(41, 9); } }

            public string LastProgress { get { return GetString(50, 15); } }

            public DateTime UpdateDate { get { return GetDate(65, 8); } }

            public string UpdateUser { get { return GetString(73, 15); } }

            public string BlankFiller { get { return GetString(88, 1113); } }

            #endregion
        }

        public class HITSODetailActivities : FlatFile
        {
            #region -- Initialize --

            public HITSODetailActivities()
            {
                string text = "3";
                line = "#" + text.PadRight(1200, ' ');
            }

            public HITSODetailActivities(string text)
            {
                line = "#" + text.PadRight(1200, ' ');
            }

            #endregion

            #region -- Public Properties --

            public string RecordID { get { return GetString(1, 1); } }

            public string CompanyCode { get { return GetString(2, 15); } }

            public string BranchCode { get { return GetString(17, 15); } }

            public Int32 InquiryNumber { get { return GetInt32(32, 9); } }

            public string ActivityID { get { return GetString(41, 9); } }

            public DateTime ActivityDate { get { return GetDate(50, 8); } }

            public string ActivityType { get { return GetString(58, 10); } }

            public string ActivityDetail { get { return GetString(68, 200); } }

            public DateTime NextFollowUpDate { get { return GetDate(268, 8); } }

            public DateTime CreationDate { get { return GetDate(276, 8); } }

            public string CreatedBy { get { return GetString(284, 15); } }

            public string LastUpdateBy { get { return GetString(299, 15); } }

            public DateTime LastUpdateDate { get { return GetDate(314, 8); } }

            public string BlankFiller { get { return GetString(322, 879); } }

            #endregion
        }

        #endregion
    }
}