using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.General.Models;
using SimDms.General.Models.Others;
using SimDms.Common;

namespace SimDms.General.Controllers.Api
{
    public class CustomerBankController : BaseController
    {
        public JsonResult Save(CustomerBankModel model)
        {
            ResultModel result = InitializeResultModel();
            string userId = CurrentUser.UserId;
            DateTime currentTime = DateTime.Now;

            if (string.IsNullOrEmpty(model.CustomerCode))
            {
                result.message = "Anda belum memilih customer untuk ditambahkan data Bank.";
                return Json(result);
            }
            if (string.IsNullOrEmpty(model.BankCode))
            {
                result.message = "Anda belum mengisi Kode Bank untuk ditambahkan data Bank.";
                return Json(result);
            }
            //if (string.IsNullOrEmpty(model.AccountName))
            //{
            //    result.message = "Anda belum mengisi Nama Akun untuk ditambahkan data Bank.";
            //    return Json(result);
            //}
            //if (string.IsNullOrEmpty(model.AccountBank))
            //{
            //    result.message = "Anda belum mengisi Akun bank untuk ditambahkan data Bank.";
            //    return Json(result);
            //}

            var data = ctx.GnMstCustomerBanks.Find(CompanyCode, model.CustomerCode, model.BankCode);
            if (data == null)
            {
                data = new GnMstCustomerBank();
                data.CompanyCode = CompanyCode;
                data.CustomerCode = model.CustomerCode;
                data.BankCode = model.BankCode;
                data.BankName = model.BankName;

                data.CreatedBy = userId;
                data.CreatedDate = currentTime;

                ctx.GnMstCustomerBanks.Add(data);
            }

            data.LastUpdateBy = userId;
            data.LastUpdateDate = currentTime;
            data.AccountBank = model.AccountBank;
            data.AccountName = model.AccountName;

            try
            {
                Helpers.ReplaceNullable(data);
                ctx.SaveChanges();
                result.data = ctx.GnMstCustomerBanks.Where(a => a.CompanyCode == CompanyCode && a.CustomerCode == model.CustomerCode);
                result.status = true;
                result.message = "Data bank berhasil disimpan.";
            }
            catch (Exception ex)
            {
                result.message = "Data bank gagal disimpan.";
            }

            return Json(result);
        }

        public JsonResult Delete(CustomerBankModel model)
        {
            ResultModel result = InitializeResultModel();

            var data = ctx.GnMstCustomerBanks.Find(CompanyCode, model.CustomerCode, model.BankCode);
            if (data == null)
            {
                result.message = "Tidak bisa menemukan data bank yang akan dihapus.";
                return Json(result);
            }
            else
            {
                ctx.GnMstCustomerBanks.Remove(data);
            }

            try
            {
                ctx.SaveChanges();
                result.data = ctx.GnMstCustomerBanks.Where(a => a.CompanyCode == CompanyCode && a.CustomerCode == model.CustomerCode);
                result.status = true;
                result.message = "Data bank berhasil dihapus.";
            }
            catch (Exception)
            {
                result.message = "Data bank gagal dihapus.";
            }

            return Json(result);
        }
       
    }
}
