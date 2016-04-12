using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using eXpressAPI.Models;
using System.Globalization;
using Common;
using Microsoft.AspNet.Identity;

namespace eXpressAPI.Controllers
{
    [RoutePrefix("appconfig/system")]
    [Route("{action=index}")]
    public class SystemController : DefaultController
    {
        CodeEditorRepository _codeRepo = new CodeEditorRepository();

        public string Index()
        {
            return "Welcome";
        }

        public JsonResult SaveConfiguration(KeyValueParam[] id)
        {
            if (!User.IsInRole("Administrator") || !User.IsInRole("Superuser") || !User.IsInRole("Developer"))
                return Json(new {success = false, message = "Access denied"}, JsonRequestBehavior.AllowGet);

            foreach (var item in id)
            {
                var itemx = new KeyValues();
                itemx.key = item.key;
                itemx.value = item.value;
                if (item.remove == "True")
                {
                    _codeRepo.RemoveConfig(item.key);
                }
                else
                {
                    _codeRepo.AddOrUpdateConfig(itemx, item._id);
                }
            }
            return Json(new { success = true, data = _codeRepo.GetAllConfig() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ListConfiguration()
        {
            if (!User.IsInRole("Administrator") || !User.IsInRole("Superuser") || !User.IsInRole("Developer"))
                return Json(new { success = false, message = "Access denied" }, JsonRequestBehavior.AllowGet);

            return Json(new { success = true, data = _codeRepo.GetAllConfig() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadAccess(string id)
        {
            var ret = SqlQuery("exec [sp_user_checkpermision_detail] '" + UserId + "','" + id + "',''");
            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public string IsSales()
        {
            return LoginAsSales ? "true" : "false";
        }

        private string GenerateType(string type, string isNull)
        {
            string str = (type == "uniqueidentifier") ? "string" : ((type == "varchar") ? "string" : ((type == "nvarchar") ? "string" : ((type == "numeric") ? "number" : ((type == "bit") ? "boolean" : ((type == "datetime") ? "DateTime" : ((type == "char") ? "string" : ((type == "text") ? "string" : ((type == "int") ? "int" : ((type == "bigint") ? "long" : ((type == "smallint") ? "short" : string.Empty))))))))));
            if (!new string[] { "varchar", "nvarchar", "char", "text" }.Contains<string>(type))
            {
                str = str + ((isNull == "YES") ? "?" : "");
            }
            return str;
        }

        private string GetTypeColumn(string type)
        {
            return (("uniqueidentifier,varchar,nvarchar,text").Contains(type) ? "text" : (("numeric,float,int,bigint,long,short,decimal,smallint,double,real").Contains(type) ? "number" : ((type == "bit") ? "boolean" : ((type == "datetime") ? "datetime" : "text"))));
        }

        public JsonResult CreateReportSession(ReportSession data)
        {
            data.SessionId = MyGlobalVar.GetMD5(UserId + DateTime.Now.ToString("G"));
            data.CreatedBy = UserId;
            data.CreatedDate = DateTime.Now;
            
            SaveResult ret = EntryAdd(data, "");
            ret.data = data.SessionId;

            // clean up
            var sql = "delete from reportsession where createdby='" + UserId + "' and sessionid !='" + data.SessionId + "'";
            SqlQuery(sql);

            return Json(ret, JsonRequestBehavior.AllowGet);
        }

        public string CheckAccess(string id)
        {
            return IsEditable(id) ? "true" : "false";
        }

        public string CheckPermission(string id)
        {
            return IsPermission(id);
        }

        private DataTable GetTableInfo(string id)
        {
            using (var connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand();

                if (id.StartsWith("#"))
                {
                    command = new SqlCommand(string.Format("SELECT COLUMN_NAME, IS_NULLABLE, DATA_TYPE, ORDINAL_POSITION\r\n\t                    FROM tempdb.INFORMATION_SCHEMA.COLUMNS\r\n                        WHERE TABLE_NAME LIKE '{0}%'", id), connection);
                    //command2 = new SqlCommand(string.Format("SELECT b.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS a\r\n\t                    INNER JOIN tempdb.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE b\r\n\t                    ON a.CONSTRAINT_NAME = b.CONSTRAINT_NAME\r\n\t                    WHERE a.CONSTRAINT_TYPE = 'PRIMARY KEY'\r\n                        AND a.TABLE_NAME LIKE '{0}%'", id), connection);
                }
                else
                {
                    command = new SqlCommand(string.Format("SELECT COLUMN_NAME, IS_NULLABLE, DATA_TYPE, ORDINAL_POSITION\r\n\t                    FROM INFORMATION_SCHEMA.COLUMNS\r\n                        WHERE TABLE_NAME = '{0}'", id), connection);
                    //command2 = new SqlCommand(string.Format("SELECT b.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS a\r\n\t                    INNER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE b\r\n\t                    ON a.CONSTRAINT_NAME = b.CONSTRAINT_NAME\r\n\t                    WHERE a.CONSTRAINT_TYPE = 'PRIMARY KEY'\r\n                        AND a.TABLE_NAME = '{0}'", id), connection);
                }

                command.Connection.Open();
                DataTable table = new DataTable();

                table.Load(command.ExecuteReader());

                command.Connection.Close();

                return table;
            }

            return null;
        }
        
        public JsonResult info(string id)
        {
            string strMessage = "";
            string strForm = "";
            string strFields = "";
            //string strButton = "";

            using (var connection = new SqlConnection(db.Database.Connection.ConnectionString))
            {
                SqlCommand command = new SqlCommand();
                SqlCommand command2 = new SqlCommand();

                if (id.StartsWith("#"))
                {
                    command = new SqlCommand(string.Format("SELECT COLUMN_NAME, IS_NULLABLE, DATA_TYPE, ORDINAL_POSITION\r\n\t                    FROM tempdb.INFORMATION_SCHEMA.COLUMNS\r\n                        WHERE TABLE_NAME LIKE '{0}%'", id), connection);
                    command2 = new SqlCommand(string.Format("SELECT b.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS a\r\n\t                    INNER JOIN tempdb.INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE b\r\n\t                    ON a.CONSTRAINT_NAME = b.CONSTRAINT_NAME\r\n\t                    WHERE a.CONSTRAINT_TYPE = 'PRIMARY KEY'\r\n                        AND a.TABLE_NAME LIKE '{0}%'", id), connection);
                }
                else
                {
                    command = new SqlCommand(string.Format("SELECT COLUMN_NAME, IS_NULLABLE, DATA_TYPE, ORDINAL_POSITION\r\n\t                    FROM INFORMATION_SCHEMA.COLUMNS\r\n                        WHERE TABLE_NAME = '{0}'", id), connection);
                    command2 = new SqlCommand(string.Format("SELECT b.COLUMN_NAME FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS a\r\n\t                    INNER JOIN INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE b\r\n\t                    ON a.CONSTRAINT_NAME = b.CONSTRAINT_NAME\r\n\t                    WHERE a.CONSTRAINT_TYPE = 'PRIMARY KEY'\r\n                        AND a.TABLE_NAME = '{0}'", id), connection);
                }

                command.Connection.Open();
                DataTable table = new DataTable();
                DataSet dsModel = new DataSet();

                table.Load(command.ExecuteReader());

                if (table.Rows.Count == 0)
                {
                    return Json(new { column = strMessage, form = strForm, field = strFields }, JsonRequestBehavior.AllowGet); 
                }

                //dsModel.Tables.Clear();
                //dsModel.Tables.Add(table);

                DataTable table2 = new DataTable();
                table2.Load(command2.ExecuteReader());
                //this.dsModel.Tables.Add(table2);
                command.Connection.Close();

                DataRowCollection rows = table.Rows;
                DataRowCollection rows2 = table2.Rows;
                List<string> list = new List<string>();
                TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
                for (int i = 0; i < rows2.Count; i++)
                {
                    list.Add(rows2[i][0].ToString());
                }

                strForm = string.Format("<form id=\"form.{0}\" xt-form name=\"form.{0}\" class=\"smart-form form-horizontal\" role=\"form\" novalidate> " + Environment.NewLine +
                    "      <fieldset> " + Environment.NewLine +
                    "        <legend><b>General Info</b></legend> ", id) + Environment.NewLine +
                    "   <xt-validation-summary></xt-validation-summary>" + Environment.NewLine;

                strFields = "$scope.formLabel = { " + Environment.NewLine;
                for (int j = 0; j < rows.Count; j++)
                {
                    string item = rows[j][0].ToString();
                    string isNull = rows[j][1].ToString();
                    string type = GetTypeColumn(rows[j][2].ToString());
                    string str5 = rows[j][3].ToString();
                    bool flag = list.Contains(item);
                     
                    strFields += "                  " + item + " : '" + myTI.ToTitleCase(item) + "', " + Environment.NewLine;

                    strMessage += "{" + string.Format(" title: \"{0}\", field: \"{0}\", width: 100, ", rows[j][0].ToString());

                    if (type=="number"){
                        strMessage += " format: \"{0:#,0}\" , attributes: { style: \"text-align: right\"}, ";
                    } else if (type=="datetime"){
                        strMessage += "  attributes: { style: \"text-align: center\"}, template: \"#= kendo.toString(kendo.parseDate(" + item + ", 'yyyy-MM-dd'), 'dd MMM yyyy') #\" ";
                    } else if (type=="boolean"){ 
                        strMessage += " template: \"<input class='" + item + "' type='checkbox' #= " + item + " ? 'checked=checked' : '' # disabled='disabled'/>\",  attributes: { style: \"text-align: center\"}, ";
                    }

                    strMessage +=  " }," + Environment.NewLine;

                    if (type == "number")
                    {
                        str5 = " step = \"any\" ";
                    }
                    else
                    {
                        str5 = "";
                    }

                    string field = string.Format(" <div class=\"form-group\"> " + Environment.NewLine +
                                    "     <label for=\"{0}\" class=\"col-xs-12 col-sm-4 col-lg-4 control-label\">{1}</label> " + Environment.NewLine +
                                    "     <div class=\"col-xs-12 col-sm-4 col-lg-4\"> " + Environment.NewLine +
                                    "         <input name=\"{0}\"  ng-model=\"data.{0}\" type=\"" + type + "\" xt-validate class=\"form-control\" id=\"{0}\" " + str5 + "> " + Environment.NewLine +
                                    "     </div> " + Environment.NewLine +
                                    " </div> ", item, "{{formLabel." + item + "}}");

                    //string field = string.Format(" <div class=\"form-group\"> " + Environment.NewLine +
                    //                "     <label for=\"{0}\" class=\"col-xs-3 control-label\">{1}</label> " + Environment.NewLine +
                    //                "     <div class=\"col-xs-3\"> " + Environment.NewLine +
                    //                "         <input name=\"{0}\"  ng-model=\"data.{0}\" type=\"" + type + "\" xt-validate class=\"form-control\" id=\"{0}\" " + str5 + "> " + Environment.NewLine +
                    //                "     </div> " + Environment.NewLine +
                    //                " </div> ", item, "{{captionLabel." + item + "}}");

                    strForm += field + Environment.NewLine;
                }
                
                strForm += "</fieldset></form>";
            }

            var ce = _codeRepo.GetByFileName("MASTERFORM");
            if (ce != null)
            {
                strMessage = ce.js.Replace("{{COLUMNDEF}}", strMessage).Replace("{{TABLENAME}}", id).Replace("{{LABELDEF}}", strFields + Environment.NewLine + "          } ");
                strForm = ce.html.Replace("{{TABLENAME}}", id).Replace("{{FORM}}", strForm);
            }
            return Json(new { column= strMessage, form= strForm, field=""}, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult Setup()
        {
            var db1 = new DataAccessContext();

            var ce = _codeRepo.GetByFileName("SETUPDB");
            int n = 0;

            if (!db1.Menus.Any())
            {
                string sql = ce.js;

                foreach (var sqlBatch in sql.Split(new[] { "GO" }, StringSplitOptions.RemoveEmptyEntries))
                {
                    if (sqlBatch != null)
                    {
                        try
                        {
                            n++;
                            db1.Database.ExecuteSqlCommand(sqlBatch);
                        } catch(Exception ex)
                        {
                            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                        }                       
                    }                    
                }

                string cnStr = System.Configuration.ConfigurationManager.ConnectionStrings["IdentityDB"].ToString();
                UserManager uman = new UserManager(new UserStore(new Context(cnStr)));
                string defaultUserId = System.Configuration.ConfigurationManager.AppSettings["config:DefaultUser"];
                string defaultPwd = System.Configuration.ConfigurationManager.AppSettings["config:DefaultPwd"];

                User uid = new User();
                uid.UserName = defaultUserId;
                uid.FirstName = "Osen";
                uid.LastName = "Kusnadi";
                uid.Email = "support@osenxpsuite.net";

                try
                {
                    var ret = uman.Create(uid, defaultPwd);
                    if (ret.Succeeded)
                    {
                        uid = uman.FindByName(uid.UserName);
                        if (uid != null)
                        {
                            var mi = new Users();
                            mi.Pass = uid.Id;
                            mi.Name = "Osen Kusnadi";
                            mi.UserId = defaultUserId;
                            mi.RoleId = "ADMIN";
                            mi.StatusUsers = 1;
                            mi.Code = "01"; 
                            mi.CreatedDate = DateTime.Now;
                            mi.CreatedBy = defaultUserId;
                            db1.Users.Add(mi);
                            db1.SaveChanges();
                        }
                    }
                    else
                    {
                        var message = (ret.Errors.First());
                        Elmah.ErrorSignal.FromCurrentContext().Raise(new Exception(message));
                        uid = uman.FindByName(uid.UserName);
                        if (uid != null)
                        {
                            var mi = new Users();
                            mi.Pass = uid.Id;
                            mi.Name = "Osen Kusnadi";
                            mi.UserId = defaultUserId;
                            mi.RoleId = "ADMIN";
                            mi.StatusUsers = 1;
                            mi.Code = "01";
                            mi.CreatedDate = DateTime.Now;
                            mi.CreatedBy = defaultUserId;
                            db1.Users.Add(mi);
                            db1.SaveChanges(); 
                        }
                     
                    }
                }
                catch (Exception ex)
                {
                    Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
                    //throw new Exception(ex.Message);
                }
            }

            //string UrlHome = System.Configuration.ConfigurationManager.AppSettings["config:Home"].ToString();
            //HttpResponseMessage oResponse = new HttpResponseMessage();
            //oResponse.StatusCode = System.Net.HttpStatusCode.Found;
            //oResponse.Headers.Location = new Uri(UrlHome);
            //return oResponse;

            return Json(new { success = true, changes = n }, JsonRequestBehavior.AllowGet);
        }
        
        public JsonResult info1(string id)
        {
            string strMessage = "";
            string strForm = "";
            string strFields = "";

            string masterId = "";

            string strMasterName = id, strDetailName = "", strPopup = "";

            DataTable table = GetTableInfo(strMasterName);

            if (table.Rows.Count == 0)
            {
                return Json(new { column = strMessage, form = strForm, field = strFields }, JsonRequestBehavior.AllowGet);
            }
            
            DataRowCollection rows = table.Rows;

            strForm = ("<form id=\"form.{0}\" xt-form name=\"form.{0}\" class=\"smart-form form-horizontal\" role=\"form\" novalidate> " + Environment.NewLine +
                "      <fieldset> " + Environment.NewLine +
                "        <legend><b>General Info</b></legend> ") + Environment.NewLine +
                "   <xt-validation-summary></xt-validation-summary>" + Environment.NewLine;

            strForm = string.Format(strForm, strMasterName);


            strFields = "$scope.formLabel = { " + Environment.NewLine;

            for (int j = 0; j < rows.Count; j++)
            {
                string item = rows[j][0].ToString();
                string type = GetTypeColumn(rows[j][2].ToString());

                string str5 = "";

                if ( j == 0 )
                {
                    str5 = " required ng-disabled='!isNew' ";
                }

                strFields += "                  " + item + " : '" + item + "', " + Environment.NewLine;

                strMessage += "{" + string.Format(" title: \"{0}\", field: \"{0}\", width: 100, ", item);

                if (type == "number")
                {
                    strMessage += " format: \"{0:#,0}\" , attributes: { style: \"text-align: right\"}, ";
                }
                else if (type == "datetime")
                {
                    strMessage += "  attributes: { style: \"text-align: center\"}, template: \"#= kendo.toString(kendo.parseDate(" + item + ", 'yyyy-MM-dd'), 'dd MMM yyyy') #\" ";
                }
                else if (type == "boolean")
                {
                    strMessage += " template: \"<input class='" + item + "' type='checkbox' #= " + item + " ? 'checked=checked' : '' # disabled='disabled'/>\",  attributes: { style: \"text-align: center\"}, ";
                }

                strMessage += " }," + Environment.NewLine;

                if (type == "number")
                {
                    str5 = str5 +  " step = \"any\" ";
                }

                string field = string.Format(" <div class=\"form-group\"> " + Environment.NewLine +
                                "     <label for=\"{0}\" class=\"col-sm-3 control-label\">{1}</label> " + Environment.NewLine +
                                "     <div class=\"col-sm-3\"> " + Environment.NewLine +
                                "         <input name=\"{0}\"  ng-model=\"data.{0}\" type=\"" + type + "\" xt-validate class=\"form-control\" id=\"{0}\" " + str5 + "> " + Environment.NewLine +
                                "     </div> " + Environment.NewLine +
                                " </div> ", item, "{{formLabel." + item + "}}");

                strForm += field + Environment.NewLine;
            }


            strForm += "</fieldset><div ng-include=\"'simple_footer_btn'\" ng-show=\"mainActive\"></form>";

            //strPopup = strForm.Replace(@"</form>", "<footer> " +
            //    "<button type='button' ng-click='cancel()' class='btn btn-danger'> Cancel </button> " +
            //    "<button type='button' ng-click='save()' class='btn btn-success' ng-disabled='form." + strMasterName + ".$invalid'> {{SaveCaption}} </button> " +
            //"</footer></form>");

            var ce = _codeRepo.GetByFileName("MASTERFORM2");
            if (ce != null)
            {
                strMessage = ce.js.Replace("{{COLUMNDEF}}", strMessage).Replace("{{MASTERNAME}}", strMasterName)
                    .Replace("{{LABELDEF}}", strFields + Environment.NewLine + "          } ");
                strForm = ce.html.Replace("{{MASTERNAME}}", strMasterName).Replace("{{MASTERFORM}}", strForm);
            }
            return Json(new { column = strMessage, form = strForm, field = "" }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult infodetail(string id)
        {
            string strMessage = "";
            string strForm = "";
            string strFields = "";

            string strMessage2 = "";
            string strForm2 = "";
            string strFields2 = "";
            string masterId = "";

            string strMasterName = "", strDetailName = "", strPopup = "";

            //string strButton = "";

            if (!id.Contains("@"))
            {
                return  info1(id);
            }
            else
            {
                var myTables = id.Split('@');
                strMasterName = myTables[1];
                strDetailName = myTables[0];
            }

                DataTable table = GetTableInfo(strMasterName);
                DataTable table2 = GetTableInfo(strDetailName);

                if (table.Rows.Count == 0)
                {
                    return Json(new { column = strMessage, form = strForm, field = strFields }, JsonRequestBehavior.AllowGet);
                }
                        
                if (table2.Rows.Count == 0)
                {
                    return Json(new { column = strMessage, form = strForm, field = strFields }, JsonRequestBehavior.AllowGet);
                }

                DataRowCollection rows = table.Rows;
                DataRowCollection rows2 = table2.Rows;

                strForm = ("<form id=\"form.{0}\" xt-form name=\"form.{0}\" class=\"smart-form form-horizontal\" role=\"form\" novalidate> " + Environment.NewLine +
                    "      <fieldset> " + Environment.NewLine +
                    "        <legend><b>General Info</b></legend> ") + Environment.NewLine +
                    "   <xt-validation-summary></xt-validation-summary>" + Environment.NewLine;
                
                strForm2 = string.Format(strForm, strDetailName);

                strForm = string.Format(strForm, strMasterName);


                strFields = "$scope.formLabel = { " + Environment.NewLine;
                strFields2 = "$scope.form2Label = { " + Environment.NewLine;
                masterId = rows[0][0].ToString();

                for (int j = 0; j < rows.Count; j++)
                {
                    string item = rows[j][0].ToString();
                    string type = GetTypeColumn(rows[j][2].ToString());
                    string str5 = "";

                    strFields += "                  " + item + " : '" + item + "', " + Environment.NewLine;

                    strMessage += "{" + string.Format(" title: \"{0}\", field: \"{0}\", width: 100, ", item);

                    if (type == "number")
                    {
                        strMessage += " format: \"{0:#,0}\" , attributes: { style: \"text-align: right\"}, ";
                    }
                    else if (type == "datetime")
                    {
                        strMessage += "  attributes: { style: \"text-align: center\"}, template: \"#= kendo.toString(kendo.parseDate(" + item + ", 'yyyy-MM-dd'), 'dd MMM yyyy') #\" ";
                    }
                    else if (type == "boolean")
                    {
                        strMessage += " template: \"<input class='" + item + "' type='checkbox' #= " + item + " ? 'checked=checked' : '' # disabled='disabled'/>\",  attributes: { style: \"text-align: center\"}, ";
                    }

                    strMessage += " }," + Environment.NewLine;

                    if (type == "number")
                    {
                        str5 = " step = \"any\" ";
                    }
                    
                    string field = string.Format(" <div class=\"form-group\"> " + Environment.NewLine +
                                    "     <label for=\"{0}\" class=\"col-xs-12 col-sm-4 col-lg-4 control-label\">{1}</label> " + Environment.NewLine +
                                    "     <div class=\"col-xs-12 col-sm-4 col-lg-4\"> " + Environment.NewLine +
                                    "         <input name=\"{0}\"  ng-model=\"data.{0}\" type=\"" + type + "\" xt-validate class=\"form-control\" id=\"{0}\" " + str5 + "> " + Environment.NewLine +
                                    "     </div> " + Environment.NewLine +
                                    " </div> ", item, "{{formLabel." + item + "}}");

                    strForm += field + Environment.NewLine;
                }


                strForm += "</fieldset></form>";

            strPopup = strForm.Replace(@"</form>","<footer> " +
                "<button type='button' ng-click='cancel()' class='btn btn-danger'> Cancel </button> " + 
                "<button type='button' ng-click='save()' class='btn btn-success' ng-disabled='form." + strMasterName + ".$invalid'> {{SaveCaption}} </button> " + 
            "</footer></form>");

             for (int j = 0; j < rows2.Count; j++)
                {
                    string item = rows2[j][0].ToString();
                    string type = GetTypeColumn(rows2[j][2].ToString());
                    string str5 = "";

                    strFields2 += "                  " + item + " : '" + item + "', " + Environment.NewLine;

                    strMessage2 += "{" + string.Format(" title: \"{0}\", field: \"{0}\", width: 100, ", item);

                    if (type == "number")
                    {
                        strMessage2 += " format: \"{0:#,0}\" , attributes: { style: \"text-align: right\"}, ";
                    }
                    else if (type == "datetime")
                    {
                        strMessage2 += "  attributes: { style: \"text-align: center\"}, template: \"#= kendo.toString(kendo.parseDate(" + item + ", 'yyyy-MM-dd'), 'dd MMM yyyy') #\" ";
                    }
                    else if (type == "boolean")
                    {
                        strMessage2 += " template: \"<input class='" + item + "' type='checkbox' #= " + item + " ? 'checked=checked' : '' # disabled='disabled'/>\",  attributes: { style: \"text-align: center\"}, ";
                    }

                    strMessage2 += " }," + Environment.NewLine;

                    if (type == "number")
                    {
                        str5 = " step = \"any\" ";
                    }
                    
                    string field = string.Format(" <div class=\"form-group\"> " + Environment.NewLine +
                                    "     <label for=\"{0}\" class=\"col-xs-12 col-sm-4 col-lg-4 control-label\">{1}</label> " + Environment.NewLine +
                                    "     <div class=\"col-xs-12 col-sm-4 col-lg-4\"> " + Environment.NewLine +
                                    "         <input name=\"{0}\"  ng-model=\"data2.{0}\" type=\"" + type + "\" xt-validate class=\"form-control\" id=\"{0}\" " + str5 + "> " + Environment.NewLine +
                                    "     </div> " + Environment.NewLine +
                                    " </div> ", item, "{{form2Label." + item + "}}");

                    strForm2 += field + Environment.NewLine;
                }
            
                strForm2 += "</fieldset></form>";

                strForm2 = strForm2.Replace(@"</form>", "<footer> " +
                    "<button type='button' ng-click='cancel2()' class='btn btn-danger'> Cancel </button> " +
                    "<button type='button' ng-click='save2()' class='btn btn-success' ng-disabled='form." + strDetailName + ".$invalid'> {{SaveCaption}} </button> " +
                "</footer></form>");


            var ce = _codeRepo.GetByFileName( "MASTERDETAILFORM");
            if (ce != null)
            {
                strMessage = ce.js.Replace("{{COLUMNDEF}}", strMessage).Replace("{{MASTERNAME}}", strMasterName)
                    .Replace("{{LABELDEF}}", strFields + Environment.NewLine + "          } ").Replace("{{MASTERID}}", masterId);
                strForm = ce.html.Replace("{{MASTERNAME}}", strMasterName).Replace("{{MASTERFORM}}", strForm).Replace("{{MASTERFORMPOPUP}}", strPopup);                
                
                strMessage = strMessage.Replace("{{COLUMNDEF2}}", strMessage2).Replace("{{DETAILNAME}}", strDetailName).Replace("{{LABELDEF2}}", strFields2 + Environment.NewLine + "          } ");
                strForm = strForm.Replace("{{DETAILNAME}}", strDetailName).Replace("{{DETAILFORM}}", strForm2);      
            }
            return Json(new { column = strMessage, form = strForm, field = ""}, JsonRequestBehavior.AllowGet);
        }
    }
}