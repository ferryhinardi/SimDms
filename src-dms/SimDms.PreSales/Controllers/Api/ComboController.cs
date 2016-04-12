using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimDms.PreSales.Models;

namespace SimDms.PreSales.Controllers.Api
{
    public class ComboController : BaseController
    {
        public JsonResult Years()
        {
            List<object> listOfYears = new List<object>();
            for (int i = 1900; i <= 2100; i++)
            {
                listOfYears.Add(new { value = i, text = i });
            }
            return Json(listOfYears);
        }

        public JsonResult Lookups(string id = "")
        {
            var list = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == id).OrderBy(p => p.SeqNo).Select(p => new { value = p.LookUpValue.ToUpper(), text = p.LookUpValueName.ToUpper() }).ToList();
            return Json(list);
        }

        public JsonResult Grade(string NikSales)
        {
            var Grade = ctx.HrEmployees.Where(p=> p.CompanyCode == CompanyCode && p.EmployeeID == NikSales).FirstOrDefault().Grade;
            return Json(Grade);
        }

        public JsonResult ItsStatus(string last = "")
        {
            var qry = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "PSTS").OrderBy(p => p.SeqNo).Select(p => new { value = p.LookUpValue, text = p.LookUpValueName.ToUpper(), seqno = p.SeqNo });
            if (!string.IsNullOrWhiteSpace(last))
            {
                var oLast = ctx.LookUpDtls.Find(CompanyCode, "PSTS", last);
                if (oLast != null)
                {
                    qry = qry.Where(p => p.seqno >= oLast.SeqNo);
                }
            }
            return Json(qry.ToList());
        }

        public JsonResult ItsStatusWithoutDODelivery(string last = "")
        {
            var qry = ctx.LookUpDtls.Where(p => p.CompanyCode == CompanyCode && p.CodeID == "PSTS").OrderBy(p => p.SeqNo).Select(p => new { value = p.LookUpValue, text = p.LookUpValue.ToUpper(), seqno = p.SeqNo });
            if (!string.IsNullOrWhiteSpace(last))
            {
                var oLast = ctx.LookUpDtls.Find(CompanyCode, "PSTS", last);
                if (oLast != null)
                {
                    qry = qry.Where(p => p.seqno >= oLast.SeqNo && p.seqno != 4 && p.seqno != 5);
                }
            }
            return Json(qry.ToList());
        }

        public JsonResult CarTypes()
        {
            var qry = from p in ctx.GroupTypes
                      where p.CompanyCode == CompanyCode
                      orderby p.GroupCodeSeq, p.TypeCodeSeq
                      select new
                      {
                          value = p.GroupCode,
                          text = p.GroupCode,
                          GroupCodeSeq = p.GroupCodeSeq
                      };
            return Json(qry.Distinct().OrderBy(x => x.GroupCodeSeq).ToList());
        }

        public JsonResult CarVariants(string id = "")
        {
            var qry = from p in ctx.GroupTypes
                      where p.CompanyCode == CompanyCode
                      && p.GroupCode == id
                      orderby p.TypeCodeSeq
                      select new
                      {
                          value = p.TypeCode,
                          text = p.TypeCode,
                          TypeCodeSeq = p.TypeCodeSeq
                      };
            return Json(qry.Distinct().OrderByDescending(p => p.TypeCodeSeq).ToList());
        }

        public JsonResult Employee(string id = "")
        {
            var qry = from p in ctx.HrEmployees
                      join q in ctx.HrEmployeeMutations on
                          new { p.CompanyCode, p.EmployeeID } equals
                          new { q.CompanyCode, q.EmployeeID }
                      where p.CompanyCode == CompanyCode
                      && p.Department == "SALES"
                      && p.Position == "S" && p.PersonnelStatus == "1"
                      && q.BranchCode == BranchCode
                      && p.TeamLeader == id
                      select new
                      {
                          value = p.EmployeeID,
                          text = p.EmployeeName
                      };

            return Json(qry.ToList());
        }

        //Added by irfan
        public JsonResult SalesEmp(string id)
        {
            if (id != "" || id != null)
            {
                var qry = from p in ctx.HrEmployees
                          join q in ctx.HrEmployeeMutations on
                          new { p.CompanyCode, p.EmployeeID } equals
                          new { q.CompanyCode, q.EmployeeID }
                          where p.CompanyCode == CompanyCode
                          && p.EmployeeID == id && p.Department == "SALES"
                          && p.Position == "S" && p.PersonnelStatus == "1"
                          && q.BranchCode == BranchCode
                          select new
                          {
                              value = p.EmployeeID,
                              text = p.EmployeeName
                          };
                return Json( new {success = true, data = qry.ToList() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var qry = from p in ctx.HrEmployees
                          join q in ctx.HrEmployeeMutations on
                          new { p.CompanyCode, p.EmployeeID } equals
                          new { q.CompanyCode, q.EmployeeID }
                          where p.CompanyCode == CompanyCode
                          && p.Department == "SALES"
                          && p.Position == "S" && p.PersonnelStatus == "1"
                          && q.BranchCode == BranchCode
                          select new
                          {
                              value = p.EmployeeID,
                              text = p.EmployeeName
                          };
                return Json(new { success = true, data = qry.ToList() }, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult GetSales(string id)
        {    
            var qry = from p in ctx.HrEmployees
                      join q in ctx.HrEmployeeMutations on
                            new { p.CompanyCode, p.EmployeeID } equals
                            new { q.CompanyCode, q.EmployeeID }
                      where p.CompanyCode == CompanyCode
                      && p.EmployeeID == id && p.Department == "SALES"
                      && p.Position == "S" && p.PersonnelStatus == "1"
                      && q.BranchCode == BranchCode
                      && q.IsDeleted == false
                    select new
                    {
                        value = p.EmployeeID,
                        text = p.EmployeeName
                    };
            return Json(new { success = true, data = qry.ToList() }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getBranchList(string id)
        {
            if (id != "")
            {
               var qry = from p in ctx.OrganizationDtls
                      where p.CompanyCode == CompanyCode
                      && p.BranchCode == id
                      select new
                      {
                          value = p.BranchCode,
                          text = p.BranchName
                      };
               return Json(new { success = true, data = qry.ToList() }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var qry = from p in ctx.OrganizationDtls
                          where p.CompanyCode == CompanyCode
                          select new
                          {
                              value = p.BranchCode,
                              text = p.BranchName
                          };
                return Json(new { success = true, data = qry.ToList() }, JsonRequestBehavior.AllowGet);
            }
        }
        //End added by irfan

        public JsonResult Transmissions()
        {
            var cartype = Request["CarType"];
            var carvari = Request["CarVariant"];
            var qry = from p in ctx.MstModels
                      select new
                      {
                          value = p.TransmissionType,
                          text = p.TransmissionType
                      };
            return Json(qry.Distinct().ToList());
        }

        public JsonResult ModelColors()
        {
            var cartype = Request["CarType"];
            var carvari = Request["CarVariant"];
            var cartran = Request["CarTrans"];
            var qry = from p in ctx.MstModelColours
                      join q in ctx.MstModels on
                      new { p.CompanyCode, p.SalesModelCode } equals
                      new { q.CompanyCode, q.SalesModelCode }
                      join r in ctx.MstRefferences on
                      new { p.CompanyCode, ColourCode = p.ColourCode, GroupCode = "COLO" } equals
                      new { r.CompanyCode, ColourCode = r.RefferenceCode, GroupCode = r.RefferenceType }
                      where p.CompanyCode == CompanyCode
                      && q.GroupCode == cartype
                      && q.TypeCode == carvari
                      && q.TransmissionType == cartran
                      select new
                      {
                          value = p.ColourCode,
                          text = r.RefferenceDesc1
                      };
            return Json(qry.Distinct().ToList());
        }

        public JsonResult LostReasons()
        {
            var codeid = Request["CodeID"];
            var lostctg = Request["LostCtg"];
            var qry = from p in ctx.LookUpDtls
                      where p.CompanyCode == CompanyCode
                      && p.CodeID == codeid
                      select new
                      {
                          value = p.LookUpValue,
                          text = p.LookUpValueName.ToUpper()
                      };

            switch (lostctg)
            {
                case "A":
                    qry = qry.Where(p => (new[] { "10", "20", "30", "40", "50", "70" }).Contains(p.value));
                    break;
                case "B":
                    qry = qry.Where(p => (new[] { "40", "50", "60", "70" }).Contains(p.value));
                    break;
                case "C":
                    qry = qry.Where(p => (new[] { "10", "20", "30", "40", "50", "70" }).Contains(p.value));
                    break;
                case "D":
                    qry = qry.Where(p => (new[] { "70" }).Contains(p.value));
                    break;
                case "E":
                    qry = qry.Where(p => (new[] { "70" }).Contains(p.value));
                    break;
                case "F":
                    qry = qry.Where(p => (new[] { "70" }).Contains(p.value));
                    break;
                default:
                    qry = qry.Where(p => (new[] { "0" }).Contains(p.value));
                    break;
            }
            return Json(qry.Distinct().ToList());
        }

        public JsonResult StatusVehicles()
        {
            var list = new Dictionary<string, object>();
            list.Add("A", "Kendaraan Baru");
            list.Add("B", "Ganti Kendaraan - Dari Suzuki");
            list.Add("C", "Ganti Kendaraan - Dari Merek Lain");
            list.Add("D", "Tambah Kendaraan - Sebelumnya Merek Suzuki");
            list.Add("E", "Tambah Kendaraan - Sebelumnya Merek Lain");
            return Json(list.Select(p => new { value = p.Key, text = p.Value }));
        }

        public JsonResult GroupModels()
        {
            var qry = from p in ctx.GroupModels                      
                      orderby p.GroupModel
                      select new
                      {
                          value = p.GroupModel,
                          text = p.GroupModel
                      };
            return Json(qry.Distinct().ToList());
        }

        public JsonResult ModelTypes(string id = "")
        {
            var qry = from p in ctx.GroupModels
                      where p.GroupModel == id
                      orderby p.GroupModel
                      select new
                      {
                          value = p.ModelType,
                          text = p.ModelType
                      };
            return Json(qry.Distinct().ToList());
        }

        public JsonResult Dealers(string id)
        {
            var dealers = from d in ctx.DealerMappings
                          where d.Area == id
                          orderby d.DealerName
                          select new
                          {
                              value = d.DealerCode,
                              text = d.DealerName
                          };
            return Json(dealers.ToList());
        }

        public JsonResult Outlets(string id)
        {
            if (id == "ALL")
            {
                var dealers = from o in ctx.DealerOutletMappings
                              orderby o.OutletName
                              select new
                              {
                                  value = o.OutletCode,
                                  text = o.OutletName
                              };
                return Json(dealers.ToList());
            }
            var dealer = from o in ctx.DealerOutletMappings
                          where o.DealerCode == id
                          orderby o.OutletName
                          select new
                          {
                              value = o.OutletCode,
                              text = o.OutletName
                          };
            return Json(dealer.ToList());
        }

        public JsonResult SalesHead(string EmployeeID)
        {
            var Position = "";
            var me = ctx.HrEmployees.Where(p => p.RelatedUser == CurrentUser.UserId).FirstOrDefault();
            if (me == null && CurrentUser.UserId == "ga") { EmployeeID = ""; Position = "COO"; } else { EmployeeID = me.EmployeeID; Position = me.Position; }
            var addjob = ctx.HrEmployeeAdditionalJobs.Where(p => p.EmployeeID == EmployeeID).FirstOrDefault();

            if (EmployeeID != null || EmployeeID != "")
            {
                me = ctx.HrEmployees.Where(p => p.EmployeeID == EmployeeID).FirstOrDefault();
                addjob = ctx.HrEmployeeAdditionalJobs.Where(p => p.EmployeeID == EmployeeID).FirstOrDefault();
            }
          
            if (addjob != null)
            {
                if (me.Position != addjob.Position)
                {
                    Position = Position + addjob.Position;
                    //Position = me.Position + addjob.Position;
                }
                //else
                //{
                //    Position = me.Position;
                //}
            }
            //else
            //{
            //    Position = me.Position;
            //}

            //var Branch = ctx.HrEmployeeMutations.Where(a => a.CompanyCode == CompanyCode && a.EmployeeID == me.EmployeeID && a.IsDeleted == false).OrderByDescending(b => b.CreatedDate).FirstOrDefault();
            var Branch = ctx.HrEmployeeMutations.Where(a => a.CompanyCode == CompanyCode && a.EmployeeID == EmployeeID && a.IsDeleted == false).OrderByDescending(b => b.CreatedDate).FirstOrDefault();
            if (Position == "GM" || Position == "CEO" || Position == "COO")
            {
                var emplCoo = ctx.HrEmployees
                   .Where(x => (x.CompanyCode == CompanyCode && x.Department == "SALES" && x.Position == "SH" && x.PersonnelStatus == "1"))
                   .Select(x => new { value = x.EmployeeID, text = x.EmployeeName }).ToList();
                return Json(new { list = emplCoo });
            }
            else if (Position == "GMBM")
            {
                var emplCoo = ctx.HrEmployees
                   .Where(x => (x.CompanyCode == CompanyCode && x.Department == "SALES" && x.Position == "SH" && x.PersonnelStatus == "1" && x.TeamLeader == me.EmployeeID))
                   .Select(x => new { value = x.EmployeeID, text = x.EmployeeName }).ToList();
                return Json(new { list = emplCoo });
            } 
            else if (Position == "BM")
            {
                var emplCoo = ctx.HrEmployees
                   .Where(x => (x.CompanyCode == CompanyCode && x.Department == "SALES" && x.Position == "SH" && x.TeamLeader == me.EmployeeID && x.PersonnelStatus == "1"))
                   .Select(x => new { value = x.EmployeeID, text = x.EmployeeName }).ToList();

                var TeamLeader = ctx.HrEmployees
                    .Where(x => x.CompanyCode == CompanyCode && x.Department == "SALES" && x.Position == "SH" && x.TeamLeader == me.EmployeeID && x.PersonnelStatus == "1")
                    .Select(x => x.EmployeeID).ToList();

                var emplo = ctx.HrEmployees
                    .Where(x => (x.TeamLeader.Contains(x.TeamLeader) == true))
                   .Select(x => new { value = x.EmployeeID, text = x.EmployeeName }).ToList();

                return Json(new { list = emplCoo.Distinct(), listSM = emplo.Distinct(), });
            }
            else if (Position == "BMSH")
            {
                var emplCoo = (from x in ctx.HrEmployees
                           where x.CompanyCode == CompanyCode && x.Department == "SALES" && x.Position == "SH" && x.TeamLeader == me.EmployeeID && x.PersonnelStatus == "1"
                           select new { value = x.EmployeeID, text = x.EmployeeName })
                            .Union(from y in ctx.HrEmployeeAdditionalJobs
                                   join z in ctx.HrEmployees
                                   on new { y.CompanyCode, y.EmployeeID } equals new { z.CompanyCode, z.EmployeeID}
                                   where y.CompanyCode == CompanyCode && y.Department == "SALES" && y.Position == "SH" && y.EmployeeID == me.EmployeeID
                                   select new { value = y.EmployeeID, text = z.EmployeeName }).ToList();     
                     
                var TeamLeader = ctx.HrEmployees
                    .Where(x => x.CompanyCode == CompanyCode && x.Department == "SALES" && x.Position == "SH" && x.TeamLeader == me.EmployeeID && x.PersonnelStatus == "1")
                    .Select(x => x.EmployeeID).ToList();

                var emplo = (from x in ctx.HrEmployees
                             where TeamLeader.Contains(x.TeamLeader) == true
                             select new { value = x.EmployeeID, text = x.EmployeeName })
                            .Union(from x in ctx.HrEmployees
                                   where x.CompanyCode == CompanyCode && x.Department == "SALES" && x.Position == "S" && x.TeamLeader == me.EmployeeID && x.PersonnelStatus == "1"
                                   select new { value = x.EmployeeID, text = x.EmployeeName }).ToList();

                return Json(new { list = emplCoo.Distinct(), listSM = emplo.Distinct(), });
            }
            else if (Position == "SH" || me.Position == "SC" || Position == "SHSTD" || Position == "SHTS")
            {
                var emplCoos = ctx.HrEmployees
                   .Where(x => (x.CompanyCode == CompanyCode && x.Department == "SALES" && (x.Position.Contains("SH") || x.Position == "SC") && x.EmployeeID == me.EmployeeID && x.PersonnelStatus == "1"))
                   .Select(x => new { value = x.EmployeeID, text = x.EmployeeName }).ToList();
                return Json(new { list = emplCoos });
            }
            else 
            {
                var ListSH = ctx.HrEmployees
                   .Where(x => (x.CompanyCode == CompanyCode && x.Department == "SALES" && x.Position.Contains("SH") && x.EmployeeID == me.TeamLeader && x.PersonnelStatus == "1"))
                   .Select(x => new { value = x.EmployeeID, text = x.EmployeeName }).ToList();
                
                var emplSH = ctx.HrEmployees
                    .Where(x => (x.CompanyCode == CompanyCode && x.Department == "SALES" && x.Position.Contains("SH") && x.EmployeeID == me.TeamLeader && x.PersonnelStatus == "1"))
                    .FirstOrDefault().EmployeeID;

                var ListSM = ctx.HrEmployees
                   .Where(x => (x.CompanyCode == CompanyCode && x.Department == "SALES" && x.Position == "S" && x.EmployeeID == me.EmployeeID && x.PersonnelStatus == "1"))
                   .Select(x => new { value = x.EmployeeID, text = x.EmployeeName }).ToList();

                var emplSM = ctx.HrEmployees
                    .Where(x => (x.CompanyCode == CompanyCode && x.Department == "SALES" && x.Position == "S" && x.EmployeeID == me.EmployeeID && x.PersonnelStatus == "1"))
                    .FirstOrDefault().EmployeeID;

                return Json(new { ListSH = ListSH, emplSH = emplSH, ListSM = ListSM, emplSM = emplSM, });
            }
        }
        public JsonResult SalesCoordinator(string EmployeeID)
        {
            var empl = ctx.HrEmployees
                .Where(x => (x.CompanyCode == CompanyCode && x.Department == "SALES" && x.TeamLeader == EmployeeID))
                .Select(x => new { value = x.EmployeeID, text = x.EmployeeName }).ToList();
            return Json(empl);
        }
        public JsonResult Salesman(string EmployeeID)
        {
            var empl = ctx.HrEmployees
                .Where(x => (x.CompanyCode == CompanyCode && x.Department == "SALES" && x.TeamLeader == EmployeeID))
                .Select(x => new { value = x.EmployeeID, text = x.EmployeeName }).ToList();
            return Json(empl);
        }

        private class CboItem
        {
            public string value { get; set; }
            public string text { get; set; }
        }

        private class EmployeeBM
        {
            public string EmployeeID { get; set; }
            public DateTime MutationDate { get; set; }
        }

        private IEnumerable<CboItem> SelectSalesmanByPos(string EmployeeID, string pos)
        {
            var qry = String.Empty;
            qry = @"SELECT a.EmployeeID as value, a.EmployeeName as text FROM HrEmployee a
                        WHERE a.TeamLeader = @p0
                        and a.Department ='SALES'
		                and a.PersonnelStatus = '1'
                        and a.Position in (@p1)
                    GROUP BY a.EmployeeID, a.EmployeeName";

            return ctx.Database.SqlQuery<CboItem>(qry, EmployeeID, pos);
        }

        public JsonResult getSalesmanByPos(string emp, string pos)
        {
            var list = SelectSalesmanByPos(emp, pos);
            return Json(list);
        }

        public JsonResult getEmployee(string emp)
        {
            //var qry = string.Format(@"uspfn_SelectEmployeeOrganizationTree '{0}', '{1}', '{2}'", CompanyCode, BranchCode, emp);
            var qry = string.Format(@"select EmployeeID as value, EmployeeName as text
                            from HrEmployee
                            where CompanyCode = '{0}'
                            and Department = 'SALES'
                            and Position = 'S'
                            and PersonnelStatus = '1'
                            and TeamLeader = '{1}'", CompanyCode, emp);
            var list = ctx.Database.SqlQuery<CboItem>(qry).AsQueryable();
            return Json(list);
        }

        public JsonResult getTeamLeader(string emp)
        {
            var LeaderSM = ctx.HrEmployees.Where(a => a.EmployeeID == emp).FirstOrDefault();
            var LeaderSH = ctx.HrEmployees.Where(a => a.EmployeeID == LeaderSM.TeamLeader).FirstOrDefault();
            
            var qry = string.Format(@"select a.EmployeeID as value, a.EmployeeName as text
                            from HrEmployee a
                            where a.CompanyCode = '{0}'
                            and a.Department = 'SALES'
                            and a.TeamLeader = '{1}'
                            and a.PersonnelStatus = '1'
                            and a.Position in ('SH','SC')
                            group by a.EmployeeID, a.EmployeeName
                            UNION
							SELECT b.EmployeeID as value, c.EmployeeName as text
							FROM HrEmployeeAdditionalJob b
							INNER JOIN HrEmployee c
							on b.CompanyCode = c.CompanyCode
							AND b.EmployeeID = b.EmployeeID 
							where b.CompanyCode = '{0}'
							AND b.EmployeeID = '{1}'
							AND c.EmployeeID = '{1}'
							and b.Position IN ('SH','SC')'", CompanyCode, LeaderSH.TeamLeader);
            var listSH = ctx.Database.SqlQuery<CboItem>(qry).AsQueryable();

            return Json(new { listSH = listSH, empSH = LeaderSM.TeamLeader });
        }

        public JsonResult getEmployeeSH(string branch)
        {
            if (branch != "")
            {
                var qry = string.Format(@"select EmployeeID as value, EmployeeName as text
                                          from (
	                                             select a.EmployeeID , a.EmployeeName, 
		                                         (select top 1 BranchCode from HrEmployeemutation WHERE EmployeeID = a.EmployeeID and IsDeleted = 0 order by CreatedDate desc) as BranchCode
								                        from HrEmployee a
								                        where a.CompanyCode = '{0}'
                                                        and a.Department = 'SALES'
                                                        and a.Position in ('SH','SC')
                                                        and a.PersonnelStatus = '1'
                                                UNION
                                                select a.EmployeeID , b.EmployeeName,
                                                (select top 1 BranchCode from HrEmployeemutation WHERE EmployeeID = a.EmployeeID and IsDeleted = 0 order by CreatedDate desc) as BranchCode
	                                                    from HrEmployeeAdditionalJob a
	                                                    inner JOIN HrEmployee b
	                                                    ON a.CompanyCode = b.CompanyCode
	                                                    AND a.EmployeeID = b.EmployeeID
	                                                    where a.CompanyCode = '6641401'
	                                                    and a.Department = 'SALES'
	                                                    and a.Position in ('SH','SC')   
	                                                    AND a.Position <> b.Position
	                                        ) a
	                                        where BranchCode = '{1}'
                           ", CompanyCode, branch);
                var qry1 = string.Format(@"select a.EmployeeID , MAX(b.MutationDate) as MutationDate
                            from HrEmployee a
                            inner join HrEmployeeMutation b
								on a.CompanyCode = b.CompanyCode
								and a.EmployeeID = b.EmployeeID
                            where a.CompanyCode = '{0}'
                            and b.BranchCode = '{1}'
                            and b.IsDeleted = 0
                            and a.Department = 'SALES'
                            and a.Position = 'BM'
                            and a.PersonnelStatus = '1'
                            group by a.EmployeeID, a.EmployeeName", CompanyCode, branch);

                var qry2 = string.Format(@"select EmployeeID as value, EmployeeName as text
                                          from (
	                                             select a.EmployeeID , a.EmployeeName, 
		                                         (select top 1 BranchCode from HrEmployeemutation WHERE EmployeeID = a.EmployeeID and IsDeleted = 0 order by CreatedDate desc) as BranchCode
								                        from HrEmployee a
								                        where a.CompanyCode = '{0}'
                                                        and a.Department = 'SALES'
                                                        and a.Position = 'S'
                                                        and a.PersonnelStatus = '1'
	                                        ) a
	                                        where BranchCode = '{1}'
                           ", CompanyCode, branch);

                var list = ctx.Database.SqlQuery<CboItem>(qry).AsQueryable();
                var list1 = ctx.Database.SqlQuery<EmployeeBM>(qry1).AsQueryable();
                var list2 = ctx.Database.SqlQuery<CboItem>(qry2).AsQueryable();
                return Json(new { listSH = list, BM = list1.FirstOrDefault().EmployeeID, listSM = list2 });
            }
            else
            {
                var qry = string.Format(@"
                            select a.EmployeeID as value, a.EmployeeName as text, 
                            (select top 1 BranchCode from HrEmployeemutation WHERE EmployeeID = a.EmployeeID and IsDeleted = 0 order by CreatedDate desc) BranchCode)
                            from HrEmployee a
                            where a.CompanyCode = '{0}'
                            and a.Department = 'SALES'
                            and a.Position in ('SH','SC')
                            and a.PersonnelStatus = '1'", CompanyCode);

                var list = ctx.Database.SqlQuery<CboItem>(qry).AsQueryable();
                return Json(new { listSH = list, BM = "" });
            }
        }

        private IEnumerable<CboItem> selectInqItsCombo(string id, string jns)
        {
            var qry = String.Empty;
            if (jns == "A")
            {
                if (id != "" && id != null)
                {
                    qry = @"SELECT CAST(GroupNo As varchar) as value, Area as text FROM gnMstDealerMapping WHERE DealerCode = @p0";
                }
                else
                {
                    qry = @"SELECT Area as value, DealerName as text FROM gnMstDealerMapping";
                }
            }
            else if (jns == "D")
            {
                if (id != "" && id != null)
                {
                    qry = @"SELECT DealerCode as value, DealerName as text FROM gnMstDealerMapping WHERE DealerCode = @p0";
                }
                else
                {
                    qry = @"SELECT DealerCode as value, DealerName as text FROM gnMstDealerMapping";
                }
            }
            else if (jns == "O")
            {
                if (id != "" && id != null)
                {
                    qry = @"SELECT OutletCode as value, OutletName as text FROM gnMstDealerOutletMapping WHERE OutletCode = @p0";
                }
                else
                {
                    qry = @"SELECT OutletCode as value, OutletName as text FROM gnMstDealerOutletMapping";
                }
            }
            return ctx.Database.SqlQuery<CboItem>(qry, id);
        }

        private IEnumerable<CboItem> selectInqItsComboOutlet(string id, string p1)
        {
            var qry = String.Empty;
            if (id != "" && id != null)
            {
                qry = @"SELECT OutletCode as value, OutletName as text FROM gnMstDealerOutletMapping WHERE DealerCode=@p1 AND OutletCode = @p0";
            }
            else
            {
                qry = @"SELECT OutletCode as value, OutletName as text FROM gnMstDealerOutletMapping WHERE DealerCode=@p1";
            } 
            return ctx.Database.SqlQuery<CboItem>(qry, id, p1);
        }

        public JsonResult getInqItsCombo(string id, string jns)
        {
            var list = selectInqItsCombo(id, jns);
            return Json(list);
        }
        public JsonResult getInqItsComboOutlet(string id, string p1)
        {
            var list = selectInqItsComboOutlet(id, p1);
            return Json(list);
        }

        public JsonResult VariantsTransColors()
        {
            var cartype = Request["CarType"];
            var carvariant = Request["CarVariant"];
            var cartrans = Request["CarTrans"];

            var variants = from p in ctx.GroupTypes
                           where p.CompanyCode == CompanyCode
                           && p.GroupCode == cartype
                           orderby p.TypeCodeSeq descending
                           select new
                           {
                               value = p.TypeCode.ToUpper(),
                               text = p.TypeCode.ToUpper()
                           };
            var transmissions = from p in ctx.MstModels
                                select new
                                {
                                    value = p.TransmissionType.ToUpper(),
                                    text = p.TransmissionType.ToUpper()
                                };
            var colors = from p in ctx.MstModelColours
                         join q in ctx.MstModels on
                         new { p.CompanyCode, p.SalesModelCode } equals
                         new { q.CompanyCode, q.SalesModelCode }
                         join r in ctx.MstRefferences on
                         new { p.CompanyCode, ColourCode = p.ColourCode, GroupCode = "COLO" } equals
                         new { r.CompanyCode, ColourCode = r.RefferenceCode, GroupCode = r.RefferenceType }
                         where p.CompanyCode == CompanyCode
                         && q.GroupCode == cartype
                         && q.TypeCode == carvariant
                         && q.TransmissionType == cartrans
                         select new
                         {
                             value = p.ColourCode.ToUpper(),
                             text = r.RefferenceDesc1.ToUpper()
                         };
            return Json(new
            {
                variants = variants.Distinct().ToList(),
                transmissions = transmissions.Distinct().ToList(),
                colors = colors.Distinct().ToList()
            });
        }
        
        //public JsonResult getArea(string id)
        //{
        //    if (id != "" || id != null)
        //    {
        //        var dlr = ctx.DealerMappings
        //            .Where(x => (x.DealerCode == CompanyCode))
        //            .Select(x => new { value = x.Area, text = x.DealerName }).ToList();
        //        return Json(dlr);
        //    }
        //    else
        //    {
        //        var dlr = ctx.DealerMappings
        //            .Select(x => new { value = x.Area, text = x.DealerName }).ToList();
        //        return Json(dlr);
        //    }
        //}

        //public JsonResult getDealer(string id)
        //{
        //    if (id != "" || id != null)
        //    {
        //        var dlr = ctx.DealerMappings
        //            .Where(x => (x.DealerCode == id))
        //            .Select(x => new { value = x.DealerCode, text = x.DealerName }).ToList();
        //        return Json(dlr);
        //    }
        //    else
        //    {
        //        var dlr = ctx.DealerMappings
        //            .Select(x => new { value = x.DealerCode, text = x.DealerName }).ToList();
        //        return Json(dlr);
        //    }
        //}
        //public JsonResult getOutlet(string id)
        //{
        //    if (id != "" || id != null)
        //    {
        //        var dlr = ctx.DealerOutletMappings
        //            .Where(x => (x.DealerCode == id))
        //            .Select(x => new { value = x.OutletCode, text = x.OutletName }).ToList();
        //        return Json(dlr);
        //    }
        //    else
        //    {
        //        var dlr = ctx.DealerOutletMappings
        //            .Select(x => new { value = x.OutletCode, text = x.OutletName }).ToList();
        //        return Json(dlr);
        //    }
        //}

        public JsonResult Branch()
        { 
            var Branch = ctx.CoProfiles.Where(a=> a.CompanyCode == CompanyCode)
                            .Select(x => new { value = x.BranchCode, text = x.CompanyName }).ToList();
            return Json(Branch);
        }

        public JsonResult BranchManager()
        {
            var qry = string.Format(@"select a.EmployeeID as value, a.EmployeeName as text
                                        from HrEmployee a
                                        where a.CompanyCode = '{0}'
                                        and a.Department = 'SALES'
                                        and a.Position = 'BM'
                                        and a.PersonnelStatus = '1'
                                        group by a.EmployeeID, a.EmployeeName  
                                        union                          
                                        select a.EmployeeID as value, b.EmployeeName as text
                                        from HrEmployeeAdditionalJob a
                                        inner join HrEmployee b
                                        on a.CompanyCode = b.CompanyCode and a.EmployeeID = b.EmployeeID
                                        where a.Position <> b.Position
                                        and a.Position = 'BM'
                                        and a.CompanyCode = '{0}'
                                        and a.Department = 'SALES'
                                        and b.PersonnelStatus = '1'", CompanyCode);

            var Branch = ctx.Database.SqlQuery<CboItem>(qry).AsQueryable();
            return Json(Branch);
        }

        public JsonResult getSH(String branch)
        {
            var qry = string.Format(@"select a.EmployeeID as value, a.EmployeeName as text
                            from HrEmployee a
                            where a.CompanyCode = '{0}'
                            and a.Department = 'SALES'
                            and a.TeamLeader = '{1}'
                            and a.PersonnelStatus = '1'
                            and a.Position in ('SH','SC')
                            group by a.EmployeeID, a.EmployeeName
                            UNION
							SELECT b.EmployeeID as value, c.EmployeeName as text
							FROM HrEmployeeAdditionalJob b
							INNER JOIN HrEmployee c
							on b.CompanyCode = c.CompanyCode
							AND b.EmployeeID = b.EmployeeID 
							where b.CompanyCode = '{0}'
							AND b.EmployeeID = '{1}'
							AND c.EmployeeID = '{1}'
							and b.Position IN ('SH','SC')", CompanyCode, branch);

            var Branch = ctx.Database.SqlQuery<CboItem>(qry).AsQueryable();
            return Json(Branch);
        }

        public JsonResult SpkFilterCombo()
        {
            Dictionary<string, string> dicFilter = new Dictionary<string, string>();
            dicFilter.Add("1", "Inquiry Date");
            dicFilter.Add("2", "SPK Date");
            var qry = dicFilter.Select(p => new { value = p.Key, text = p.Value }).ToList();

            return Json(qry);
        }

        public JsonResult ListBranchCode()
        {
            var listBranch = ctx.OrganizationDtls.Select(m => new { value = m.BranchCode, text = m.BranchCode + " - " + m.BranchName }).OrderBy(x => x.value);
            return Json(listBranch);
        }

        public JsonResult CurrentBranchCode()
        {
            string branchcode = CurrentUser.BranchCode;
            var currBranch = ctx.OrganizationDtls.Where(p => p.BranchCode == branchcode).OrderBy(p => p.SeqNo).Select(m => new { value = m.BranchCode, text = m.BranchCode + " - " + m.BranchName }).OrderBy(x => x.value);
            return Json(currBranch);
        }

        public JsonResult IsHolding()
        {
            string branchcode = CurrentUser.BranchCode;
            var currBranch = ctx.OrganizationDtls.Where(p => p.BranchCode == branchcode).ToList().SingleOrDefault();
            if (currBranch.IsBranch == false)
                return Json(true);
            else
                return Json(false);
        }
    }
}
