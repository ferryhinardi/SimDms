using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.BLL
{
    public class GnMstFPJSignDateBLL:BaseBLL
    {
        #region "Initiate"
        private static GnMstFPJSignDateBLL _GnMstFPJSignDateBLL;

        public static GnMstFPJSignDateBLL Instance(string _username)
        {
            if (_GnMstFPJSignDateBLL == null)
            {
                _GnMstFPJSignDateBLL = new GnMstFPJSignDateBLL();
            }
            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
            return _GnMstFPJSignDateBLL;
        }
        #endregion

        public DateTime GetSignDate(DateTime transDate, DateTime DueDate)
        {
            DateTime signDate = transDate.Date;
            DateTime returnDate = new DateTime();
            try
            {
                var FPJOption = ctx.GnMstFPJSignDates.Find(CompanyCode, BranchCode, ProfitCenter).FPJOption;
                if (FPJOption == "1")
                {
                    if (DueDate.Month == transDate.Month)
                    {
                        signDate = transDate.Date;
                    }
                    else
                    {
                        signDate = new DateTime(transDate.Date.Year, transDate.Date.Month + 1, 1, 0, 0, 0);
                    }
                }
                else
                {
                    signDate = transDate.Date;
                }
            }
            catch (Exception ex)
            {
                signDate = DateTime.Parse("1900/01/01");
            }
            return signDate;
        }

        public void Testasd()
        {
            string a = "sad";
            a = "sad";
            a = "sad";
            a = "sad";
            a = "sad";
            a = "sad";
            a = "sad";
            a = "sad";
            a = "sad";
            a = "sad";
        }

    }
}
