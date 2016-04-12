using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Config;
using System.Threading;
using System.Data;

namespace Sim.Dms.SendData
{
    class Program
    {
        private static readonly ILog logger = LogManager.GetLogger(typeof(Program));

        static Program()
        {
            DOMConfigurator.Configure();
        }

        static void Main(string[] args)
        {
            Console.WriteLine("Running SDMS Send Data.");
            RunApps();
        }

        private static void RunApps()
        {
            try
            {
                DataMgr dataMgr = new DataMgr();
                //get data sdmsupdate 
                var listData = dataMgr.GetSdmsUpdateData();
                foreach (var itemData in listData)
                {
                    logger.Info("Process DMS Data, DataType="+ itemData.DataType);
                    dataMgr.ProcessUpdateData(itemData);
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex.ToString());
            }
        }
    }
}
