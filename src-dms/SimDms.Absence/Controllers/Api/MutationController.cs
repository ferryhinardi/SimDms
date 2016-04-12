using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using SimDms.Absence.Models;

namespace SimDms.Absence.Controllers.Api
{
    public class MutationController : BaseController
    {
        public ActionResult Get(HrEmployee model)
        {
            var entity = ctx.HrEmployees.Find(model.CompanyCode, model.EmployeeID);
            if (entity != null)
            {
                return Json(new { success = true, data = entity, isNew = false });
            }
            else
            {
                var data = ctx.HrEmployees.Find(model.CompanyCode, model.EmployeeID);
                return Json(new { success = true, data = data, isNew = true });
            }
        }

        public JsonResult GetDataMutation(HrEmployee model)
        {
            DataTable dt = new DataTable();
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandText = "uspfn_HrGetMutationByEmployeeId";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Add(new SqlParameter("@EmployeeID", (string.IsNullOrWhiteSpace(model.EmployeeID) ? "" : model.EmployeeID)));
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            var list = GetJson(dt);


            if (list != null)
            {
                return Json(new { success = true, list = list });
            }
            else
            {
                return Json(new { success = false, message = "data not found" }); ;
            }
        }

        public JsonResult List(HrEmployee model)
        {
            string companyCode = CompanyCode;
            IQueryable<MutationModel> data = ctx.Database.SqlQuery<MutationModel>("exec uspfn_HrGetMutationByEmployeeId @CompanyCode=@p0, @EmployeeID=@p1", companyCode, model.EmployeeID).AsQueryable();

            return Json(GeLang.DataTables<MutationModel>.Parse(data, Request));
        }

        public JsonResult CheckMutationDate(HrEmployeeMutation model)
        {
            object returnMsg = null;
            var mutationList = ctx.HrEmployeeMutations.Where(m => m.CompanyCode == (model.CompanyCode ?? CompanyCode) && m.EmployeeID == model.EmployeeID && m.MutationDate <= model.MutationDate && (m.IsDeleted == null || m.IsDeleted == false)).ToList();
            if (mutationList.Count != 0)
            {
                //check join date 
                var mutation = mutationList.Where(m => m.MutationDate == model.MutationDate && m.IsDeleted != true).FirstOrDefault();
                if (mutation != null)
                {
                    if (mutation.IsJoinDate.Value)
                    {
                        returnMsg = new { success = false, message = "Tanggal Mutasi harus lebih besar dari tanggal join, Tanggal Join=" + model.MutationDate.ToString("dd-MMM-yyyy") };
                    }
                    else if (mutation.MutationDate == model.MutationDate)
                    {
                        returnMsg = new { success = false, message = "Sudah ada mutasi pada tanggal ini " + model.MutationDate.ToString("dd-MMM-yyyy") };
                    }
                    else
                    {
                        returnMsg = new { success = true };
                    }
                }
                else
                {
                    returnMsg = new { success = true };
                }
            }
            else
            {
                var employeeEntity = ctx.HrEmployees.Where(m => m.CompanyCode == (model.CompanyCode ?? CompanyCode) && m.EmployeeID == model.EmployeeID).FirstOrDefault();
                if (employeeEntity != null)
                {
                    if (model.MutationDate < employeeEntity.JoinDate.Value)
                    {
                        returnMsg = new { success = false, message = "Tanggal Mutasi harus lebih besar dari tanggal join, Tanggal Join=" + employeeEntity.JoinDate.Value.ToString("dd-MMM-yyyy") };
                    }
                    else
                    {
                        returnMsg = new { success = true };
                    }
                }
            }
            return Json(returnMsg);
        }

        public JsonResult CheckMutationBranch(HrEmployeeMutation model)
        {
            string mutationBranch = Request["MutationBranch"] ?? "";
            var Prev2Mutation = ctx.HrEmployeeMutations.Where(m => m.MutationDate <= model.MutationDate && m.IsDeleted != true && m.EmployeeID == model.EmployeeID).OrderByDescending(m => m.MutationDate).Take(2).ToList();
            var Next2Mutations = ctx.HrEmployeeMutations.Where(m => m.MutationDate >= model.MutationDate && m.IsDeleted != true && m.EmployeeID == model.EmployeeID).OrderByDescending(m => m.MutationDate).Take(2).ToList();
            var mutationPrev = Prev2Mutation.Where(m => m.BranchCode == (model.BranchCode ?? mutationBranch) && m.IsDeleted != true && m.EmployeeID == model.EmployeeID).FirstOrDefault();
            var mutationNext = Next2Mutations.Where(m => m.BranchCode == (model.BranchCode ?? mutationBranch) && m.IsDeleted != true && m.EmployeeID == model.EmployeeID).FirstOrDefault();
            if (mutationPrev != null)
            {
                if (mutationPrev.BranchCode == model.BranchCode)
                {
                    return Json(new { success = false, message = "Silahkan Pilih Branch lain karena branch ini adalah branch pada mutasi sebelumnya, pada tanggal mutasi = " + mutationPrev.MutationDate.ToString("dd-MMM-yyyy") });
                }
                else
                {
                    return Json(new { success = true });
                }
            }
            else if (mutationNext != null)
            {
                if (mutationNext.BranchCode == model.BranchCode)
                {
                    return Json(new { success = false, message = "Silahkan Pilih Branch lain karena branch ini adalah branch pada mutasi sesudah, pada tanggal mutasi = " + mutationNext.MutationDate.ToString("dd-MMM-yyyy") });
                }
                else
                {
                    return Json(new { success = true });
                }
            }
            else
            {
                return Json(new { success = true });
            }
        }

        private ValidationMessage ValidateMutationDate(HrEmployeeMutation model)
        {
            ValidationMessage result = new ValidationMessage();
            result.status = false;
            result.message = "";

            object returnMsg = null;
            var mutationList = ctx.HrEmployeeMutations.Where(m => m.CompanyCode == (model.CompanyCode ?? CompanyCode) && m.EmployeeID == model.EmployeeID && m.MutationDate <= model.MutationDate && (m.IsDeleted == false || m.IsDeleted == null)).ToList();
            if (mutationList.Count != 0)
            {
                //check join date 
                var mutation = mutationList.Where(m => m.MutationDate == model.MutationDate && m.IsDeleted != true).FirstOrDefault();
                if (mutation != null)
                {
                    if (mutation.IsJoinDate.Value)
                    {
                        //returnMsg = new { success = false, message = "Tanggal Mutasi harus lebih besar dari tanggal join, Tanggal Join=" + model.MutationDate.ToString("dd-MMM-yyyy") };
                        result.status = false;
                        result.message = "Tanggal Mutasi harus lebih besar dari tanggal join, Tanggal Join=" + model.MutationDate.ToString("dd-MMM-yyyy");
                    }
                    else if (mutation.MutationDate == model.MutationDate)
                    {
                        //returnMsg = new { success = false, message = "Sudah ada mutasi pada tanggal ini " + model.MutationDate.ToString("dd-MMM-yyyy") };
                        result.status = false;
                        result.message = "Sudah ada mutasi pada tanggal ini " + model.MutationDate.ToString("dd-MMM-yyyy");
                    }
                    else
                    {
                        //returnMsg = new { success = true };
                        result.status = true;
                    }
                }
                else
                {
                    //returnMsg = new { success = true };
                    result.status = true;
                }
            }
            else
            {
                var employeeEntity = ctx.HrEmployees.Where(m => m.CompanyCode == (model.CompanyCode ?? CompanyCode) && m.EmployeeID == model.EmployeeID).FirstOrDefault();
                if (employeeEntity != null)
                {
                    if (model.MutationDate < employeeEntity.JoinDate.Value)
                    {
                        //returnMsg = new { success = false, message = "Tanggal Mutasi harus lebih besar dari tanggal join, Tanggal Join=" + employeeEntity.JoinDate.Value.ToString("dd-MMM-yyyy") };
                        result.status = false;
                        result.message = "Tanggal Mutasi harus lebih besar dari tanggal join, Tanggal Join=" + employeeEntity.JoinDate.Value.ToString("dd-MMM-yyyy");
                    }
                    else
                    {
                        //returnMsg = new { success = true };
                        result.status = true;
                    }
                }
            }

            return result;
        }

        private ValidationMessage ValidateMutationBranch(HrEmployeeMutation model)
        {
            ValidationMessage result = new ValidationMessage();
            result.status = false;
            result.message = "";

            string mutationBranch = Request["MutationBranch"] ?? "";
            var Prev2Mutation = ctx.HrEmployeeMutations.Where(m => m.MutationDate <= model.MutationDate && m.IsDeleted != true && m.EmployeeID == model.EmployeeID).OrderByDescending(m => m.MutationDate).Take(2).ToList();
            var Next2Mutations = ctx.HrEmployeeMutations.Where(m => m.MutationDate >= model.MutationDate && m.IsDeleted != true && m.EmployeeID == model.EmployeeID).OrderByDescending(m => m.MutationDate).Take(2).ToList();
            var mutationPrev = Prev2Mutation.Where(m => m.BranchCode == (model.BranchCode ?? mutationBranch) && m.IsDeleted != true && m.EmployeeID == model.EmployeeID).FirstOrDefault();
            var mutationNext = Next2Mutations.Where(m => m.BranchCode == (model.BranchCode ?? mutationBranch) && m.IsDeleted != true && m.EmployeeID == model.EmployeeID).FirstOrDefault();
            if (mutationPrev != null)
            {
                if (mutationPrev.BranchCode == model.BranchCode)
                {
                    //return Json(new { success = false, message = "Silahkan Pilih Branch lain karena branch ini adalah branch pada mutasi sebelumnya, pada tanggal mutasi = " + mutationPrev.MutationDate.ToString("dd-MMM-yyyy") });
                    result.status = false;
                    result.message = "Silahkan Pilih Branch lain karena branch ini adalah branch pada mutasi sebelumnya, pada tanggal mutasi = " + mutationPrev.MutationDate.ToString("dd-MMM-yyyy");
                }
                else
                {
                    //return Json(new { success = true });
                    //return true;
                    result.status = true;
                }
            }
            else if (mutationNext != null)
            {
                if (mutationNext.BranchCode == model.BranchCode)
                {
                    //return Json(new { success = false, message = "Silahkan Pilih Branch lain karena branch ini adalah branch pada mutasi sesudah, pada tanggal mutasi = " + mutationNext.MutationDate.ToString("dd-MMM-yyyy") });
                    //return false;
                    result.status = false;
                    result.message = "Silahkan Pilih Branch lain karena branch ini adalah branch pada mutasi sesudah, pada tanggal mutasi = " + mutationNext.MutationDate.ToString("dd-MMM-yyyy");
                }
                else
                {
                    //return Json(new { success = true });
                    //return true;
                    result.status = true;
                }
            }
            else
            {
                //return Json(new { success = true });
                //return true;
                result.status = true;
            }

            return result;
        }

        public JsonResult Save(HrEmployeeMutation model)
        {
            bool isJoinDateMutation = Convert.ToBoolean(Request["IsJoinDateMutation"] ?? "false");
            //var userID = CurrentUser.UserId;
            //var currentDate = DateTime.Now;

            //ValidationMessage validationState = this.ValidateMutationDate(model);
            //if (validationState.status == false)
            //{
            //    return Json(new
            //    {
            //        success = false,
            //        message = validationState.message
            //    });
            //}

            //validationState = this.ValidateMutationBranch(model);
            //if (validationState.status == false)
            //{
            //    return Json(new
            //    {
            //        success = false,
            //        message = validationState.message
            //    });
            //}

            //object returnMsg = null;
            //var entity = ctx.HrEmployeeMutations.Where(m =>
            //        m.CompanyCode == (model.CompanyCode ?? CompanyCode)
            //        &&
            //        m.EmployeeID == model.EmployeeID
            //        &&
            //        m.MutationDate.Year == model.MutationDate.Year
            //        &&
            //        m.MutationDate.Month == model.MutationDate.Month
            //        &&
            //        m.MutationDate.Day == model.MutationDate.Day
            //        ).FirstOrDefault();

            //if (entity == null)
            //{
            //    entity = model;
            //    ctx.HrEmployeeMutations.Add(entity);
            //    entity.CompanyCode = model.CompanyCode ?? CompanyCode;
            //    entity.CreatedBy = userID;
            //    entity.CreatedDate = currentDate;
            //}
            //entity.IsDeleted = false;
            //entity.MutationDate = entity.MutationDate;
            //entity.IsJoinDate = model.IsJoinDate ?? isJoinDateMutation;
            //entity.BranchCode = model.BranchCode;
            //entity.UpdatedBy = userID;
            //entity.UpdatedDate = currentDate;
            //try
            //{
            //    ctx.SaveChanges();
            //    returnMsg = new { success = true };
            //}
            //catch (Exception)
            //{
            //    returnMsg = new { success = false, message = "Data mutasi gagal disimpan" };
            //    throw;
            //}
            //return Json(returnMsg);

            string companyCode = CompanyCode;
            string userID = CurrentUser.UserId;

            ValidationMessage result = ctx.Database.SqlQuery<ValidationMessage>("exec uspfn_SaveEmployeeMutation @CompanyCode=@p0, @EmployeeID=@p1, @MutationDate=@p2, @IsJoinDate=@p3, @BranchCode=@p4, @UserID=@p5", companyCode, model.EmployeeID, model.MutationDate, isJoinDateMutation, model.BranchCode, userID).FirstOrDefault();

            return Json(result);
        }

        public JsonResult Delete(HrEmployeeMutation model)
        {
            object returnMsg = null;
            var entity = ctx.HrEmployeeMutations.Where(m => m.CompanyCode == (model.CompanyCode ?? CompanyCode) && m.MutationDate == model.MutationDate && m.EmployeeID == model.EmployeeID).FirstOrDefault();
            if (entity != null)
            {
                //ctx.HrEmployeeMutations.Remove(entity);
                entity.IsDeleted = true;

                try
                {
                    ctx.SaveChanges();
                    returnMsg = new { success = true };
                }
                catch
                {
                    returnMsg = new { success = false, message = "Data mutasi gagal dihapus" };
                }
            }
            return Json(returnMsg);
        }

        public JsonResult IsJoinDateExist(HrEmployeeMutation model)
        {
            object returnMsg = null;
            int JoinDateExist = ctx.HrEmployeeMutations.Where(m => m.IsJoinDate == true && m.CompanyCode == (model.CompanyCode ?? CompanyCode) && m.EmployeeID == model.EmployeeID && m.IsDeleted != true).Count(m => m.IsJoinDate.Value);
            if (JoinDateExist > 0)
            {
                returnMsg = new { status = true };
            }
            else
            {
                returnMsg = new { status = false };
            }
            return Json(returnMsg);
        }

        public JsonResult GetJoinDetailsByEmployeeID(HrEmployeeMutation model)
        {
            object returnMsg = null;
            var employeeEntity = ctx.HrEmployees.Where(m => m.CompanyCode == (model.CompanyCode ?? CompanyCode) && m.EmployeeID == model.EmployeeID).FirstOrDefault();
            if (employeeEntity != null)
            {
                returnMsg = new { status = true, data = employeeEntity.JoinDate.Value.ToString("dd-MMM-yyyy") };
            }
            else
            {
                returnMsg = new { status = false, data = "" };
            }
            return Json(returnMsg);
        }

        public JsonResult GetEmployeeDetails(string EmployeeID)
        {
            object returnObj = null;
            var employeeEntity = ctx.HrEmployees.Where(m => m.EmployeeID == EmployeeID).FirstOrDefault();
            if (employeeEntity != null)
            {
                returnObj = employeeEntity;
            }
            else
            {
                returnObj = new HrEmployee();
            }
            return Json(returnObj);
        }

        public JsonResult CheckKDPCount(HrEmployeeMutation model)
        {
            try
            {
                var emp = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode 
                    && x.EmployeeID == model.EmployeeID && x.Department == "SALES");

                var exists = ctx.HrEmployeeMutations.Any(x => x.CompanyCode == CompanyCode
                    && x.EmployeeID == model.EmployeeID);
                
                if (!exists) return Json(new { message = "" });
                
                var mut1 = ctx.HrEmployeeMutations
                        .Where(x => x.CompanyCode == CompanyCode && x.EmployeeID == model.EmployeeID)
                        .GroupBy(x => new { x.CompanyCode, x.EmployeeID }, y => y.MutationDate)
                        .Select(x => new
                        {
                            x.Key.CompanyCode,
                            x.Key.EmployeeID,
                            MutationDate = x.Max()
                        });

                var maxDate = mut1.FirstOrDefault().MutationDate;

                if (emp.Position == "S")
                {
                    var mut2 = (from a in mut1
                                join b in ctx.HrEmployeeMutations
                                on new { a.CompanyCode, a.EmployeeID, a.MutationDate }
                                equals new { b.CompanyCode, b.EmployeeID, b.MutationDate }
                                select new
                                {
                                    a.CompanyCode,
                                    b.BranchCode,
                                    a.EmployeeID,
                                    a.MutationDate
                                }).FirstOrDefault();

                    var kdps = ctx.PmKdps
                        .Count(x => x.CompanyCode == CompanyCode && x.EmployeeID == model.EmployeeID
                            && x.EmployeeID == model.EmployeeID
                            && x.BranchCode == mut2.BranchCode);
                    if (kdps > 0 && model.MutationDate >= maxDate) throw new Exception("Tidak bisa Add/Edit/Delete Mutasi karena Employee ini masih memiliki KDP. Harap lakukan mutasi melalui ITS > Master > Organization untuk Transfer KDP");
                    
                }
                else
                {
                    var members = ctx.HrEmployees
                        .Count(x => x.CompanyCode == CompanyCode 
                            && x.Department == "SALES"
                            && x.PersonnelStatus == "1"
                            && x.TeamLeader == model.EmployeeID);
                    if (members > 0 && model.MutationDate >= maxDate) throw new Exception("Tidak bisa Add/Edit/Delete Mutasi karena Employee ini masih memiliki Team Member. Harap kosongkan team dahulu.");
                }

                return Json(new { message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        public JsonResult CheckMutationDateBeforeSave(HrEmployeeMutation model)
        {
            try
            {
                var emp = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode
                    && x.EmployeeID == model.EmployeeID && x.Department == "SALES");

                var maxMutationDate = ctx.HrEmployeeMutations.Where(x => x.CompanyCode == CompanyCode
                    && x.EmployeeID == model.EmployeeID).Max(x => x.MutationDate);

                if (model.MutationDate >= maxMutationDate)
                    throw new Exception("Add/Edit Mutasi hanya diperbolehkan untuk tanggal sebelum Mutasi Terkini. Silakan menggunakan fitur ITS > Master Organization.");

                return Json(new { message = "" });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }
    }
}
