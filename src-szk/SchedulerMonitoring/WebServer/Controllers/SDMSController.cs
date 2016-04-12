using EventScheduler;
using Newtonsoft.Json;
using SCHEMON.Models;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Data.SQLite;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;


namespace SCHEMON.Controllers
{
    public class SDMSController : ApiController
    {

        SimDMSDB db = new SimDMSDB();

        [HttpGet]
        public async Task<IHttpActionResult> OnlineDealers()
        {
            IQueryable<OnlineDealer> data = db.OnlineDealers.OrderByDescending(x => x.LastUpdate);
            //var grid = MyShared.OrderingHelper<OnlineDealer>(data, param);

            return Ok(new
            {
                success = true,
                Results = data
            });
        }

        [HttpGet]
        public async Task<IHttpActionResult> BackupDB()
        {

            MyShared.BackupDaily();

            return Ok(new
            {
                success = true,
                message = "Backup Daily"
            });
        }


        [HttpGet]
        public async Task<IHttpActionResult> Process()
        {

            MyShared.RunMergeProcess();

            MyShared.RunMergeProcess2();

            return Ok(new
            {
                success = true, message = MyShared.State
            });
        }

        [HttpGet]
        public async Task<IHttpActionResult> MergeManual(string id)
        {

            MyShared.RunMergeProcess3(id);

            return Ok(new
            {
                success = true,
                message = id
            });
        }


        [HttpGet]
        public IHttpActionResult ManualProcess([FromUri] string filedb)
        {

            string fileName = filedb;
            int rowChanges = 0;
            string optDB = fileName.Split('_')[1].ToString();

            using (var CN = MyShared.Conn)
            {
                CN.Open();
                rowChanges = MyShared.MergeDatabase(MyShared.extractDir + filedb, optDB, CN, "0");
            }

            return Ok(new
            {
                success = true,
                message = fileName,
                changes = rowChanges
            });
        }

        [HttpPost]
        public async Task<IHttpActionResult> SendError(mailParam param)
        {

            MyShared.SendErrorLog(param.subject, param.message, "", "", param.filename);

            return Ok(new
            {
                success = true,
                subject = param.subject,
                message = param.message
            });
        }

        [HttpGet]
        public IHttpActionResult ManualProcess2([FromUri] string filedb)
        {

            string fileName = filedb;
            int rowChanges = 0;
            string optDB = fileName.Split('_')[1].ToString();
            string SQLMerge;

            using (var CN = MyShared.Conn)
            {
                CN.Open();
                rowChanges = MyShared.MergeDatabaseDummy(MyShared.extractDir + filedb, optDB, CN, "0", out SQLMerge);
            }

            return Ok(new
            {
                success = true,
                message = fileName,
                changes = rowChanges,
                SQL = SQLMerge
            });
        }

    }

    public class mailParam
    {
        public string subject { get; set; }
        public string message { get; set; }
        public string filename { get; set; }
    }



}
