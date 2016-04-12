using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.BLL
{
    public class CommonBLL:BaseBLL
    {
        #region "Initiate"
        private static CommonBLL _CommonBLL;
        public static CommonBLL Instance(string _username)
        {
            if (_CommonBLL == null)
            {
                _CommonBLL = new CommonBLL();
            }
            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
            return _CommonBLL;
        }
        #endregion

        public string GetNoSeriFakturPajak(string FPJNo)
        {
            string NoSeriFPJ = "";
            var coProfile = ctx.CoProfiles.Find(CompanyCode, BranchCode);
            if (coProfile != null && FPJNo.Contains("FPJ") && FPJNo.Trim().Length == 13)
            {
                string codetrans = coProfile.TaxTransCode.Substring(0, 2).PadRight(3, '0');
                string codecabang = coProfile.TaxCabCode.Substring(0, 3);
                string tahun = FPJNo.Substring(7).PadLeft(8, '0');
                var seq = FPJNo.Substring(7).PadLeft(8, '0');
                NoSeriFPJ = string.Format("{0}.{1}-{2}.{3}", codetrans, codecabang, tahun, seq);
            }
            else if (coProfile != null && FPJNo.Contains("SDH") && FPJNo.Trim().Length == 13)
            {
                NoSeriFPJ = FPJNo;
            }
            return NoSeriFPJ;
        }

        protected string DateTransValidation(DateTime date)
        {
            var user = CurrentUser.UserId;
            var currDate = DateTime.Now.Date;
            var errMsg1 = string.Format(ctx.SysMsgs.Find("5006").MessageCaption, "Tanggal Transaksi", "Periode Transaksi");//string.Format("{0} tidak sesuai dengan {1}", "Tanggal transaksi", "periode transaksi");
            var errMsg2 = string.Format(ctx.SysMsgs.Find("5006").MessageCaption, "Tanggal Transaksi", "Tanggal Server");//string.Format("{0} tidak sesuai dengan {1}", "Tanggal Transaksi", "Tanggal Server");
            var errMsg3 = "Periode sedang di locked";//string.Format("Periode sedang di locked");
            var errMsg4 = "Tanggal Transaksi lebih kecil dari tanggal [TransDate]";//string.Format("Tanggal Transaksi lebih kecil dari tanggal [TransDate]");
            var msg = "";
            try
            {
                var oProfCenter = ctx.SysUserProfitCenters.Find(user);
                if (oProfCenter.ProfitCenter.Equals("300"))
                {
                    var oSpare = ctx.GnMstCoProfileSpares.Find(CurrentUser.CompanyCode, CurrentUser.BranchCode);
                    if (oSpare != null)
                    {
                        if (oSpare.TransDate.Equals(DBNull.Value) || oSpare.TransDate < new DateTime(1900, 1, 2)) oSpare.TransDate = DateTime.Now;
                        if (date >= oSpare.PeriodBeg.Date && date <= oSpare.PeriodEnd.Date)
                        {
                            if (date <= currDate)
                            {
                                if (date >= oSpare.TransDate.Date)
                                {
                                    if (oSpare.isLocked == true)
                                    {
                                        msg = errMsg3;
                                    }
                                }
                                {
                                    errMsg4 = errMsg4.Replace("[TransDate]", oSpare.TransDate.ToString("dd-MMM-yyyy"));
                                    msg = errMsg4;
                                }
                            }
                            else
                            {
                                msg = errMsg2;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return msg;
        }
    }

    public enum LoggerType
    {
        Fatal = 0,
        Error = 1,
        Warning = 2,
        Info = 3,
        Debug = 4,
    }
}
