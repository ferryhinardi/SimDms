using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using SimDms.SUtility.Models;

namespace SimDms.SUtility.Controllers.Api
{
    public class TokenAccessController : BaseController
    {
        //
        // GET: /TokenAccess/

        public ActionResult Index()
        {
            return View();
        }

        public JsonResult GetTokenAccess(string CompanyCode, string ComputerName,string DataType, string callback="")
        {
            var tokenEntity = ctxDealer.TokenAccesses.Find(CompanyCode);
            if (tokenEntity != null)
            {
                //check token timeout
                if (DateTime.Now > tokenEntity.ExpiredDate)
                {
                    //Token expired
                    //generate new token
                    int expiredTime = int.Parse(ConfigurationManager.AppSettings["ExpiredTimeToken"].ToString());
                    tokenEntity.TokenID = Guid.NewGuid().ToString();
                    tokenEntity.ExpiredDate = DateTime.Now.AddMinutes(expiredTime);
                    tokenEntity.ComputerName = ComputerName;
                    try
                    {
                        ctxDealer.SaveChanges();
                        return Jsonp(new
                        {
                            Success = true,
                            CompanyCode = CompanyCode,
                            ResponseData = GetLastUpdateDateDataByCompanyNameDataType(CompanyCode,DataType),
                            Message = tokenEntity.TokenID
                        }, callback, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {

                        return Jsonp(new
                        {
                            Success = false,
                            CompanyCode = CompanyCode,
                            Message = "Error pada saat generate TokenID, message=" + ex.ToString()
                        }, callback, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    //token still valid
                    if (tokenEntity.ComputerName == ComputerName)
                    {
                        return Jsonp(new
                        {
                            Success = true,
                            CompanyCode = CompanyCode,
                            ResponseData = GetLastUpdateDateDataByCompanyNameDataType(CompanyCode, DataType),
                            Message = tokenEntity.TokenID
                        }, callback, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Jsonp(new
                        {
                            Success = false,
                            CompanyCode = CompanyCode,
                            Message = "Maaf Anda tidak dapat connect ke server karena komputer lain sedang melakukan proses"
                        }, callback, JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else
            {
                return Jsonp(new
                            {
                                Success = false,
                                CompanyCode = CompanyCode,
                                Message = "Maaf komputer anda tidak memiliki akses ke server"
                            }, callback, JsonRequestBehavior.AllowGet);
            }
            //return Json("");
        }

        public string GetLastUpdateDateDataByCompanyNameDataType(string companyCode, string dataType)
        {
            string timeToString = "";
            var entity = ctxDealer.GnMstScheduleDatas.Where(m => m.CompanyCode == companyCode && m.DataType == dataType).OrderByDescending(m=>m.LastSendDate).FirstOrDefault();
            if (entity != null)
            {
                timeToString = entity.LastSendDate.ToString("yyyy-MM-dd HH:mm:ss.fff");
            }
            return timeToString;
        }

    }
}
