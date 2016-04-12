using EventScheduler;
using Newtonsoft.Json;
using SVNMON.Models;
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
using System.Net;
using System.Net.Mail;

namespace SVNMON.Controllers
{
    [RoutePrefix("api/svnupdate")]
    public class SVNController : ApiController
    {        
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> Process()
        {
            MyShared.RunSvnUpdate();
            return Ok(new
            {
                success = true, message = MyShared.State
            });
        }        
    }

    [RoutePrefix("api/mail")]
    public class MailController : ApiController
    {
        [HttpGet]
        [Route("")]
        public async Task<IHttpActionResult> send()
        {

            MailMessage mail = new MailMessage();
            var strFrom = System.Configuration.ConfigurationManager.AppSettings["emailFrom"].ToString();
            var strUserId = System.Configuration.ConfigurationManager.AppSettings["emailuserid"].ToString();
            var strPassword = MyGlobalVar.Decrypt(System.Configuration.ConfigurationManager.AppSettings["emailpassword"].ToString(), "20151015-fvXt9FzImqs3p6zfxdzBoQ==", "fvXt9FzImqs3p6zfxdzBoQ==20151015");

            mail.From = new MailAddress(strFrom);
            mail.To.Add("osenxpsuite@gmail.com");
            mail.Subject = "Test Mail - 2";
            mail.Body = "mail with attachment";

            System.Net.Mail.Attachment attachment;
            attachment = new System.Net.Mail.Attachment("d:/CS SBT.xlsx");
            mail.Attachments.Add(attachment);

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(strUserId, strPassword),
                EnableSsl = true
            };

            client.Send(mail);

            return Ok(new
            {
                success = true,
                message = "OK"
            });
        }
    }    

}
