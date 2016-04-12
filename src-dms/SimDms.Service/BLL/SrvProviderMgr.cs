using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SimDms.Service.BLL
{
    public class SrvProviderMgr
    {
        public enum SprProviderMgrBLL
        {
            GnMstFPJSignDateBLL,
            SalesBLL,
            CommonBLL
        }

        private static SrvProviderMgr _ProviderMgr;
        private static object objIns = null;
        private static string username = "";

        public static SrvProviderMgr Instance(string _username)
        {
            if (_ProviderMgr == null)
            {
                _ProviderMgr = new SrvProviderMgr();
            }

            //if (string.IsNullOrEmpty(username))
            //{
                username = _username;
            //}
            return _ProviderMgr;
        }

        public Object GetInstance(SprProviderMgrBLL manager)
        {
            switch (manager)
            {
                case SprProviderMgrBLL.SalesBLL:
                    //objIns = (Object)SalesBLL.Instance(username);
                    break;
                case SprProviderMgrBLL.CommonBLL:
                    //objIns = (Object)CommonBLL.Instance(username);
                    break;
                default:
                    break;
            }
            return objIns;
        }
    }
}