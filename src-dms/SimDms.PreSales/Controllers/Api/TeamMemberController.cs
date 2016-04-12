using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Transactions;
using SimDms.PreSales.Models;

namespace SimDms.PreSales.Controllers.Api
{
    public class TeamMemberController : BaseController 
    {
        private string msg = "";
        private bool isSupervisor = false;
        private bool result = false;
        private int position = 0;

        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                Address = CurrentUser.CoProfile.Address1,
                UserID = CurrentUser.UserId
            });
        }

        #region ~Deprecated~
        public JsonResult GetGrid(string teamId)
        {
            var data = from a in ctx.Employees
                       join b in ctx.TeamMembers
                       on new { a.CompanyCode, a.BranchCode, a.EmployeeID, TeamID = teamId }
                       equals new { b.CompanyCode, b.BranchCode, b.EmployeeID, b.TeamID }
                       where a.CompanyCode == CompanyCode && a.BranchCode == BranchCode
                       orderby b.IsSupervisor descending
                       select new
                       {
                           a.EmployeeID,
                           a.EmployeeName,
                           b.IsSupervisor
                       };

            return Json(data);
        }

        public JsonResult Save(TeamSave model)
        {
            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var record = ctx.Teams.Find(CompanyCode, model.BranchCode, model.TeamID);
                    if (record == null)
                    {
                        var teamId = Convert.ToInt32(ctx.Teams.Max(a => a.TeamID)) + 1;

                        record = new Team()
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = model.BranchCode,
                            TeamID = teamId.ToString(),
                            StartDateActive = DateTime.Now,
                            EndDateActive = DateTime.Now,
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now,
                        };
                        ctx.Teams.Add(record);
                    }
                    record.TeamName = model.TeamName.ToUpper();
                    record.IsLock = false;
                    record.LockedBy = null;
                    record.LockedDate = DateTime.Now;
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;
                    ctx.SaveChanges();
                    trans.Complete();
                    return Json(new { success = true, data = record, message = "Proses Save berhasil !" });

                }
                catch
                {
                    trans.Dispose();
                    return Json(new { success = false, message = "Proses Save gagal !" });

                }
            };
        }

        public JsonResult Delete(TeamSave model)
        {
            var result = false;
            var members = ctx.TeamMembers.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == model.BranchCode && a.TeamID == model.TeamID);
            if (members.Count() > 0)
            {
                var msg = "Tidak Bisa Delete Team.\nTeam '" + model.TeamName + " (" + model.TeamID + ")' masih memiliki anggota.";
                return Json(new { success = false, message = msg });
            }

            var team = ctx.Teams.Find(CompanyCode, model.BranchCode, model.TeamID);
            if (team != null)
            {
                ctx.Teams.Remove(team);
                result = ctx.SaveChanges() > 0;
            }
            if (result) return Json(new { success = result, message = "Proses Delete berhasil !" });
            return Json(new { success = result, message = "Proses Delete gagal" });
        }

        public JsonResult SaveDetail(TeamSave model, TeamMemberSave detail)
        {
            if (!isPositionValid(model, detail)) return Json(new { success = result, message = msg });

            using (TransactionScope trans = new TransactionScope())
            {
                try
                {
                    var record = ctx.TeamMembers.Find(CompanyCode, model.BranchCode, detail.EmployeeID, model.TeamID);
                    if (record == null)
                    {
                        var member = ctx.TeamMembers.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.BranchCode == model.BranchCode && a.EmployeeID == detail.EmployeeID && a.IsSupervisor == detail.IsSupervisor);

                        if (member != null)
                        {
                            msg = "Employee '" + detail.EmployeeName + " (" + detail.EmployeeID + ")' telah menjadi " + (isSupervisor ? "supervisor" : "anggota") + " dari team lain";
                            return Json(new { success = false, message = msg });
                        }
                        record = new TeamMember()
                        {
                            CompanyCode = CompanyCode,
                            BranchCode = model.BranchCode,
                            EmployeeID = detail.EmployeeID,
                            MemberID = (CheckDataExist(CompanyCode, model.BranchCode)) ? 1 :
                                            GetMemberID(CompanyCode, model.BranchCode),
                            CreatedBy = CurrentUser.UserId,
                            CreatedDate = DateTime.Now
                        };
                        ctx.TeamMembers.Add(record);
                    }

                    record.TeamID = model.TeamID;
                    record.IsSupervisor = isSupervisor;
                    record.IsSalesMan = (position.ToString() == SALESMAN) ? true : false;
                    record.IsLock = false;
                    record.LockedBy = CurrentUser.UserId;
                    record.LockedDate = DateTime.Now;
                    record.LastUpdateBy = CurrentUser.UserId;
                    record.LastUpdateDate = DateTime.Now;

                    ctx.SaveChanges();
                    trans.Complete();
                    return Json(new { success = true, message = "Save Anggota berhasil !" });
                }
                catch
                {
                    trans.Dispose();
                    return Json(new { success = false, message = msg });
                }
            };
        }

        public JsonResult DeleteDetail(TeamSave model, TeamMemberSave detail)
        {
            var record = ctx.TeamMembers.Find(CompanyCode, model.BranchCode, detail.EmployeeID, model.TeamID);
            if (record != null)
            {
                // for salesman only
                var dtKDP = ctx.PmKdps.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.BranchCode == model.BranchCode && a.EmployeeID == detail.EmployeeID);
                if (dtKDP != null)
                {
                    msg = "Employee '" + detail.EmployeeName + " (" + detail.EmployeeID + ")' tidak bisa dihapus,\n karena masih memiliki Inquiry KDP.";
                    return Json(new { success = false, message = msg });
                }

                ctx.TeamMembers.Remove(record);
                ctx.SaveChanges();
                return Json(new { success = true, message = "Proses Delete Detail berhasil !" });
            }
            return Json(new { success = false, message = "Proses Delete Detail gagal !" });
        }

        private bool CheckDataExist(string companyCode, string branchCode)
        {
            var exist = ctx.TeamMembers.Where(a => a.CompanyCode == companyCode && a.BranchCode == branchCode).Count();
            return (exist == 0) ? true : false;
        }

        private int GetMemberID(string companyCode, string branchCode)
        {
            var memberID = ctx.TeamMembers.Where(a => a.CompanyCode == companyCode && a.BranchCode == branchCode).Max(a => a.MemberID) + 1;
            return memberID;
        }

        private bool isPositionValid(TeamSave model, TeamMemberSave detail)
        {
            result = true;
            var memberPosition = 0;
            int maxPosition = 0;
            Position record = ctx.Positions.Find(CompanyCode, model.BranchCode, detail.EmployeeID);
            if (record != null)
            {
                position = Convert.ToInt32(record.PositionId);
            }
            else
            {
                msg = "Karyawan bersangkutan belum di-set di master position";
                return false;
            }

            // cek Data Team Members Based On TeamID
            var dataTeamMembers = ctx.TeamMembers.Where(a => a.CompanyCode == CompanyCode && a.BranchCode == model.BranchCode && a.TeamID == model.TeamID);
            if (dataTeamMembers.Count() == 0)
                return true; //Karena data masih kosong


            // Cek Jenis Position            
            var Check = (from a in ctx.TeamMembers
                         join b in ctx.Positions
                         on new { a.CompanyCode, a.BranchCode, a.EmployeeID }
                         equals new { b.CompanyCode, b.BranchCode, b.EmployeeID } into _b
                         from b in _b.DefaultIfEmpty()
                         where a.CompanyCode == CompanyCode && a.BranchCode == model.BranchCode && a.TeamID == model.TeamID
                         select new MstTeamMember
                         {
                             PositionId = b.PositionId
                         }).Distinct();

            memberPosition = Convert.ToInt32(Check.First().PositionId);

            if (Check.Count() == 1)
            {
                // Cek posisi COO (hanya boleh sekali)
                if (Check.First().PositionId == COO.ToString())
                {
                    msg = "Posisi " + detail.EmployeeName + " tidak memungkinkan masuk kedalam Team ini !";
                    return false;
                }
                if (position == memberPosition || position == (memberPosition + 10) || position == (memberPosition - 10))
                {
                    if (position == memberPosition + 10)
                    {
                        isSupervisor = true;
                    }
                    if (position == memberPosition - 10)
                    {

                        //var upTeamMember = ctx.TeamMembers.FirstOrDefault(a => a.CompanyCode == CompanyCode && a.BranchCode == model.BranchCode && a.TeamID == model.TeamID && a.EmployeeID != detail.EmployeeID);

                        //upTeamMember.IsSupervisor = true;
                        //result = ctx.SaveChanges() > 0;

                        result = UpdateSupervisor(CompanyCode, model.BranchCode,
                                                           model.TeamID, detail.EmployeeID);

                    }
                }
                else
                {
                    msg = "Posisi " + detail.EmployeeName + " tidak memungkinkan masuk kedalam Team ini !";
                    result = false;
                }
            }
            else
            {
                // Get Greater Position 
                maxPosition = (from a in ctx.TeamMembers
                               join b in ctx.Positions
                               on new { a.CompanyCode, a.BranchCode, a.EmployeeID }
                               equals new { b.CompanyCode, b.BranchCode, b.EmployeeID } into _b
                               from b in _b.DefaultIfEmpty()
                               where a.CompanyCode == CompanyCode && a.BranchCode == model.BranchCode && a.TeamID == model.TeamID
                               select new MstTeamMember
                               {
                                   PositionId = b.PositionId
                               }).Max(a => Convert.ToInt32(a.PositionId));

                isSupervisor = false;
                if (position >= maxPosition || position < (maxPosition - 10))
                {
                    msg = "Posisi " + detail.EmployeeName + " tidak memungkinkan masuk kedalam Team ini !";
                    result = false;
                }
            }

            return result;
        }

        private bool UpdateSupervisor(string companyCode, string branchCode, string teamID, string empID)
        {
            var query = string.Format(@"UPDATE PmMstTeamMembers SET IsSupervisor = 1 WHERE 
                                    CompanyCode={0} AND BranchCode={1} AND TeamID='{2}' 
                                    AND EmployeeID <> '{3}' ", companyCode, branchCode, teamID, empID);


            return (ctx.Database.ExecuteSqlCommand(query) > 0) ? true : false;
        }
        #endregion

        public JsonResult GetProductType()
        {
            return Json(CurrentUser.CoProfile.ProductType);
        }

        public JsonResult GetMembers(string branchCode, string employeeID)
        {
            var query = @"
                SELECT a.BranchCode, d.BranchName, a.EmployeeID, b.EmployeeName, b.Position
                FROM hrEmployeeMutation a
                JOIN (
	                SELECT c.EmployeeId, c.EmployeeName, c.Position, MAX(d.MutationDate) AS MutationDate
	                FROM hrEmployee c
	                JOIN hrEmployeeMutation d
	                ON c.EmployeeId = d.EmployeeId
	                WHERE c.Department = 'SALES' AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
	                AND c.TeamLeader = @p2
	                GROUP BY c.EmployeeId, c.EmployeeName, c.Position
                    UNION
	                SELECT c.EmployeeId, c.EmployeeName, c.Position, MAX(d.MutationDate) AS MutationDate
	                FROM hrEmployee c
	                JOIN hrEmployeeMutation d
	                ON c.EmployeeId = d.EmployeeId
	                WHERE c.Department = 'SALES' AND c.PersonnelStatus = 1 AND c.IsDeleted = 0 AND d.IsDeleted = 0
	                AND c.TeamLeader in (SELECT EmployeeID FROM HrEmployee WHERE TeamLeader = @p2)
	                GROUP BY c.EmployeeId, c.EmployeeName, c.Position
                ) b
                ON a.EmployeeId = b.EmployeeId AND a.MutationDate = b.MutationDate
                JOIN gnMstOrganizationDtl d
                ON a.BranchCode = d.BranchCode
                WHERE a.CompanyCode = @p0 AND a.BranchCode = @p1
                ";
            var result = ctx.Database.SqlQuery<PositionItem>(query, CompanyCode, branchCode, employeeID);
            return Json(result);
        }

        public JsonResult Add(string leaderID, string employeeID)
        {
            var success = true;
            try
            {
                var employee = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.EmployeeID == employeeID);
                if (employee.TeamLeader != "" && employee.TeamLeader != null) throw new Exception();
                employee.TeamLeader = leaderID;
                ctx.SaveChanges();
            }
            catch
            {
                success = false;
            }
            return Json(new { success = success });
        }

        public JsonResult Remove(string leaderID, string employeeID)
        {
            var success = true;
            try
            {
                var employee = ctx.HrEmployees.FirstOrDefault(x => x.CompanyCode == CompanyCode && x.EmployeeID == employeeID);
                if (employee.TeamLeader == "" || employee.TeamLeader == null || employee.TeamLeader != leaderID) throw new Exception();
                employee.TeamLeader = "";
                ctx.SaveChanges();
            }
            catch
            {
                success = false;
            }
            return Json(new { success = success });
        }

    }
}