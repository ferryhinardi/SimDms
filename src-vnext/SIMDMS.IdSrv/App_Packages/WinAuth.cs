using System.IO;
using System.Security.Cryptography;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Web.Mvc;
using System.DirectoryServices;

using System.Data.Common;
using System.Data;

namespace eXpressAPI
{


    public class SaveResult
    {
        public SaveResult(int a, bool b)
        {
            Count = a;
            success = b;
        }

        public int Count { get; set; }
        public bool success { get; set; }
        public string message { get; set; }
        public object data { get; set; }

        public DataTable Table()
        {
            if (data != null)
            {
                return ((DataSet)data).Tables[0];
            }
            return null;
        }


        public DataSet Tables()
        {
            if (data != null)
            {
                return ((DataSet)data);
            }
            return null;
        }

    }


}