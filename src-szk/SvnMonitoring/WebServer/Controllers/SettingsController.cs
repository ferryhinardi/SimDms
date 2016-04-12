using EventScheduler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;


namespace SCHEMON.Controllers
{
    public class SDMSController : ApiController
    {

        [HttpGet]
        public async Task<IHttpActionResult> Process()
        {

            MyShared.RunMergeProcess();

            return Ok(new
            {
                success = true, message = MyShared.State
            });
        }



    }
}
