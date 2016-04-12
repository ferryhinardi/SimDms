using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.CStatisfication.Models;
using SimDms.CStatisfication.Models.General;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class SettingsController : BaseController
    {
        public JsonResult Save(CsSetting model)
        {
            ResultModel result = InitializeResultModel();
            string companyCode = CompanyCode;
            string userID = CurrentUser.UserId;
            DateTime time = DateTime.Now;

            var data = ctx.CsSettings.Find(companyCode, model.SettingCode);
            if (data == null)
            {
                data = new CsSetting();
                data.CompanyCode = companyCode;
                data.SettingCode = model.SettingCode;
                data.CreatedBy = userID;
                data.CreatedDate = time;

                ctx.CsSettings.Add(data);
            }

            data.SettingDesc = model.SettingDesc;
            data.SettingParam1 = model.SettingParam1;
            data.SettingParam2 = model.SettingParam2;
            data.SettingParam3 = model.SettingParam3;
            data.SettingParam4 = model.SettingParam4;
            data.SettingParam5 = model.SettingParam5;
            data.SettingLink1 = model.SettingLink1;
            data.SettingLink2 = model.SettingLink2;
            data.SettingLink3 = model.SettingLink3;
            data.UpdatedBy = userID;
            data.UpdatedDate = time;

            try
            {
                ctx.SaveChanges();

                result.success = true;
                result.message = "Data has been saved into database.";
            }
            catch (Exception)
            {
                result.message = "Sorry, data cannot be saved into database.";
            }

            return Json(result);
        }

        public JsonResult Delete(CsSetting model)
        {
            ResultModel result = InitializeResultModel();
            var data = ctx.CsSettings.Find(CompanyCode, model.SettingCode);

            if (data != null)
            {
                try
                {
                    ctx.CsSettings.Remove(data);
                    ctx.SaveChanges();

                    result.success = true;
                    result.message = "Data has been removesd from database.";
                }
                catch (Exception)
                {
                    result.message = "Sorry, data cannot be removed into database.";
                }
            }
            else
            {
                result.message = "Sorry, your request cannot be processed.";
            }



            return Json(result);
        }
    }
}
