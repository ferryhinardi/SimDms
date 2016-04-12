using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimDms.Sparepart.BLL
{
    public class SprProviderMgr
    {
        public enum SprProviderMgrBLL
        {
            GnMstFPJSignDateBLL,
            SalesBLL,
            CommonBLL
        }

        private static SprProviderMgr _ProviderMgr;
        private static object objIns = null;
        private static string username= "";

        public static SprProviderMgr Instance(string _username)
        {
            if (_ProviderMgr == null)
            {
                _ProviderMgr = new SprProviderMgr();
            }

            if(string.IsNullOrEmpty(username))
            {
                username = _username;
            }
            return _ProviderMgr;
        }

        public Object GetInstance(SprProviderMgrBLL manager)
        {
            switch (manager)
            {
                case SprProviderMgrBLL.GnMstFPJSignDateBLL:
                    objIns = (Object)GnMstFPJSignDateBLL.Instance(username);
                    break;
                case SprProviderMgrBLL.SalesBLL:
                    objIns = (Object)SalesBLL.Instance(username);
                    break;
                case SprProviderMgrBLL.CommonBLL:
                    objIns = (Object)CommonBLL.Instance(username);
                    break;
                default:
                    break;
            }
            return objIns;
        }
    }
}
