    using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Wordprocessing;
using SimDms.DataWarehouse.Models;
using System.Data.SqlClient;
using System.Data;
using System.Web.Script.Serialization;
using SimDms.DataWarehouse.Helpers;

namespace SimDms.DataWarehouse.Controllers.Api
{
    public class ComboController : BaseController
    {
        public JsonResult GroupAreas()
        {
            var data = ctx.GroupAreas.Select(x => new { text = x.AreaDealer, value = x.GroupNo }).OrderBy(x => x.value);

            return Json(data);
        }

        public JsonResult GroupAreas2W()
        {
            var data = ctx.GroupAreas2W.Select(x => new { text = x.AreaDealer, value = x.GroupNo }).OrderBy(x => x.value);

            return Json(data);
        }

        public JsonResult SrvGroupAreas()
        {
            var data = ctx.SrvGroupAreas.Select(x => new { text = x.AreaDealer, value = x.GroupNo }).OrderBy(x => x.value);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult LoadSrvGroupAreas()
        {
            var data = ctx.Database.SqlQuery<GroupAreaView>("exec SrvGroupAreaCustom").Select(x => new { text = x.area, value = x.groupNo }).ToList();
            data.Insert(0, new { text = "NASIONAL", value = 0 });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GroupAreas2()
        {
            var data = ctx.GroupAreas.Where(x => x.GroupNo == "100").Select(x => new { text = x.AreaDealer, value = x.GroupNo }).OrderBy(x => x.value);

            return Json(data);
        }

        public JsonResult GroupAreasCustom()
        {
            var data = ctx.GroupAreas.Select(x => new { text = x.AreaDealer, value = x.GroupNo }).ToList();
            data.Add(new { text = "NASIONAL", value = "0" });
            data.OrderBy(x => x.value).ToArray();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public JsonResult JobGroup()
        {
            var data = ctx.Database.SqlQuery<ParamValue>("exec [uspfn_svListJobGroup]").ToList();

            return Json(data);
        }


        public JsonResult Companies()
        {
            string groupNo = Request["id"] ?? "";

            var rawData = ctx.CompaniesGroupMappingViews.AsQueryable();

            if (!string.IsNullOrEmpty(groupNo))
            {
                rawData = ctx.CompaniesGroupMappingViews.Where(x => x.GroupNo == groupNo);
            }

            var data = ctx.CompaniesGroupMappingViews.Where(x => x.GroupNo == groupNo).Select(x => new { value = x.DealerCode, text = x.DealerCode + " - " + x.CompanyName, DealerName = x.CompanyName }).Distinct().OrderBy(x => x.DealerName);
            return Json(data);
        }


        public JsonResult Companies2()
        {
            string groupNo = "100";

            var rawData = ctx.CompaniesGroupMappingViews.AsQueryable();

            if (!string.IsNullOrEmpty(groupNo))
            {
                rawData = rawData.Where(x => x.GroupNo == groupNo);
            }

            rawData = rawData.Where(x => x.DealerCode == "6159401000");
            var data = rawData.Select(x => new { value = x.DealerCode, text = x.DealerCode + " - " + x.CompanyName, DealerName = x.CompanyName }).Distinct().OrderBy(x => x.DealerName);
            return Json(data);
        }

        public JsonResult ListDealers()
        {
            string groupNo = Request["id"] ?? "";

            var rawData = ctx.ActiveDealers.AsQueryable();

            if (!string.IsNullOrEmpty(groupNo))
            {
                rawData = rawData.Where(x => x.ProductType == groupNo);
            }

            rawData = rawData.OrderBy(x => x.ProductType);

            var data = rawData.Select(x => new { value = x.value, text = x.text });

            return Json(data);
        }

        public JsonResult ListDealersNew(string area = "")
        {
            var query = string.Format(@"declare @area varchar(100) 
                                        set @area='{0}'
                                        if (@area='CABANG')
                                        begin
                                            select a.GroupNo,a.DealerCode,a.DealerName from [dbo].[gnMstDealerMappingNew] a
                                            inner join DealerInfo b on b.DealerCode=a.DealerCode where isJV=1 and b.ProductType='4W'
                                        end
                                        else
                                        begin
                                            select a.GroupNo,a.DealerCode,a.DealerName from [dbo].[gnMstDealerMappingNew] a 
                                            inner join DealerInfo b on b.dealercode=a.DealerCode where Area =@area and b.ProductType='4W'
                                        end", area);
            var queryable = ctx.Database.SqlQuery<DealerV2View>(query).AsQueryable();
            return Json(queryable.Select(p => new { value = p.DealerName, text = p.DealerName.ToUpper(), p.DealerCode }).Distinct().OrderBy(p => p.DealerCode));

        }

        public JsonResult ListDealersNew2(string area = "")
        {
            var query = string.Format(@"declare @area varchar(100) 
                                        set @area='{0}'
                                        if (@area='100')
                                        begin
                                            select GroupNo,DealerCode,DealerName from [dbo].[gnMstDealerMappingNew] where isJV=1
                                        end
                                        else
                                        begin
                                            select GroupNo,DealerCode,DealerName from [dbo].[gnMstDealerMappingNew] 
                                            where GroupNo like case when @area='' then Area else @area end
                                        end", area);
            var queryable = ctx.Database.SqlQuery<DealerV2View>(query).AsQueryable();
            return Json(queryable.Select(p => new { value = p.DealerCode, text = p.DealerName.ToUpper(), p.DealerCode }).Distinct().OrderBy(p => p.DealerCode));
        }

        public JsonResult ListDealers2W(string area = "")
        {
            var query = string.Format(@"declare @area varchar(100) 
                                        set @area='{0}'
                                        if (@area='CABANG')
                                        begin
                                            select a.GroupNo,a.DealerCode,a.DealerName from [dbo].[gnMstDealerMappingNew] a
                                            inner join DealerInfo b on b.DealerCode=a.DealerCode where isJV=1 and b.ProductType='2W'
                                        end
                                        else
                                        begin
                                            select a.GroupNo,a.DealerCode,a.DealerName from [dbo].[gnMstDealerMappingNew] a 
                                            inner join DealerInfo b on b.dealercode=a.DealerCode where Area =@area and b.ProductType='2W'
                                        end", area);
            var queryable = ctx.Database.SqlQuery<DealerV2View>(query).AsQueryable();
            return Json(queryable.Select(p => new { value = p.DealerName, text = p.DealerName.ToUpper(), p.DealerCode }).Distinct().OrderBy(p => p.DealerCode));
        }

        public JsonResult ListDealersNew3()
        {
            string groupNo = Request["id"] ?? "";
            var query = string.Format(@"uspfn_DealerListNew '{0}'", groupNo);
            var queryable = ctx.Database.SqlQuery<DealerV3View>(query).AsQueryable();
            return Json(queryable.Select(p => new { value = p.groupNo + "|" + p.CompanyCode, text = p.CompanyCode + "-" + p.CompanyName.ToUpper(), p.CompanyCode }).Distinct().OrderBy(p => p.CompanyCode));
        }

        public JsonResult ListOutletsNew()
        {
            string gNo = Request["parm"] ?? "";
            var query = "";
            if (gNo.IndexOf('|') > 0) {
                string[] groupNo = gNo.Split('|');
                query = string.Format(@"uspfn_OutletListNew '{0}','{1}'", groupNo[0], groupNo[1]);
            }
            else
            {
                query = string.Format(@"uspfn_OutletListNew '',''");
            }
            var queryable = ctx.Database.SqlQuery<ListOutlet>(query).AsQueryable();
            return Json(queryable.Select(p => new { value = p.OutletCode, text = p.OutletCode + "-" + p.OutletName.ToUpper(), p.OutletCode }).Distinct().OrderBy(p => p.OutletCode));
        }

        

        public JsonResult CompaniesAll()
        {
            string groupNo = Request["id"] ?? "";
            string productType = Request["ProductType"] ?? "";

            var rawData = ctx.CompaniesGroupMapping.AsQueryable();

            if (!string.IsNullOrEmpty(groupNo))
            {
                rawData = rawData.Where(x => x.GroupNo == groupNo);
            }

            if (!string.IsNullOrEmpty(productType))
            {
                rawData = rawData.Where(x => x.ProductType == productType);
            }

            var data = rawData.Select(x => new { value = x.DealerCode, text = x.DealerCode + " - " + x.CompanyName, DealerName = x.CompanyName }).Distinct().OrderBy(x => x.DealerName);
            return Json(data);
        }

        public JsonResult CompaniesR2()
        {
            //string groupNo = Request["id"] ?? "";
            string groupNo = Request["id"] ?? "";
            var rawData = ctx.CompaniesGroupMappingViews.AsQueryable();

            if (!string.IsNullOrEmpty(groupNo))
            {
                rawData = rawData.Where(x => x.GroupNo == groupNo);
            }

            //var data = ctx.CompaniesGroupMappingViews.Where(x => x.GroupNo == groupNo).Select(x => new { value = x.DealerCode, text = x.CompanyName + " --------------- "+ x.DealerCode, DealerName = x.CompanyName }).Distinct().OrderBy(x => x.DealerName);
            var data = rawData.Select(x => new { value = x.DealerCode, text = x.DealerCode + " - " + x.CompanyName, DealerName = x.CompanyName }).Distinct().OrderBy(x => x.DealerName);

            return Json(data);
        }

        public JsonResult Dealers()
        {
            string comp = Request["id"] ?? "";

            var rawData = ctx.DealerGroupMappingViews.AsQueryable();

            if (!string.IsNullOrEmpty(comp))
            {
                rawData = ctx.DealerGroupMappingViews.Where(x => x.CompanyCode == comp);
            }

            var data = ctx.DealerGroupMappingViews.Where(x => x.CompanyCode == comp).Select(x => new { value = x.BranchCode, text = x.BranchCode + " - " + x.BranchName }).OrderBy(x => x.text).Distinct();
            return Json(data);
        }

        public JsonResult Organizations()
        {
            //var qry = ctx.Organizations.OrderBy(p => p.CompanyCode).AsQueryable();
            var qry = ctx.Organizations.OrderBy(p => p.CompanyName).AsQueryable();
            var dlr = DealerType;
            if (!string.IsNullOrWhiteSpace(dlr)) { qry = qry.Where(p => p.DealerType == dlr); }
            return Json(qry.Select(p => new { value = p.CompanyCode, text = p.CompanyCode + " - " + p.CompanyName.ToUpper(), p.CompanyName }).OrderBy(p => p.CompanyName));
        }

        //start fhi 27-03-2015 : add areas & OrganizationsV2
        public ActionResult Areas()
        {
            //var qry = ctx.GnMstDealerMappings.OrderBy(p => p.GroupNo).Distinct().AsQueryable();
            //return Json(qry.Select(p => new { value = p.Area, text = p.Area.ToUpper(), p.Area,p.GroupNo }).Distinct().OrderBy(p => p.GroupNo));
            var query = string.Format(@"select distinct groupNo, area,
	                                        case when area='SUMATERA' then (GroupNo+150) else groupno end orders
                                        from GnMstDealerMapping
                                        order by orders");
            var queryable = ctx.Database.SqlQuery<GroupAreaView>(query).AsQueryable();
            return Json(queryable.Select(p => new { value = p.area, text = p.area.ToUpper(), p.groupNo, p.orders }).Distinct().OrderBy(p => p.orders));
        }


        public ActionResult Areas2()
        {
            //var qry = ctx.GnMstDealerMappings.OrderBy(p => p.GroupNo).Distinct().AsQueryable();
            //return Json(qry.Select(p => new { value = p.Area, text = p.Area.ToUpper(), p.Area,p.GroupNo }).Distinct().OrderBy(p => p.GroupNo));
            var query = string.Format(@"select distinct groupNo, area,
	                                        case when area='SUMATERA' then (GroupNo+150) else groupno end orders
                                        from GnMstDealerMapping
                                        order by orders");
            var queryable = ctx.Database.SqlQuery<GroupAreaView>(query).AsQueryable();
            return Json(queryable.Select(p => new { value = p.groupNo, text = p.area.ToUpper(), p.groupNo, p.orders }).Distinct().OrderBy(p => p.orders));
        }

        public ActionResult OrganizationsV2(string area = "")
        {
            if (area == null || area == string.Empty)
            {
                var qry = ctx.Organizations.OrderBy(p => p.CompanyName).AsQueryable();
                var dlr = DealerType;
                if (!string.IsNullOrWhiteSpace(dlr)) { qry = qry.Where(p => p.DealerType == dlr); }
                return Json(qry.Select(p => new { value = p.CompanyCode, text = p.CompanyCode + " - " + p.CompanyName.ToUpper(), p.CompanyName }).OrderBy(p => p.CompanyName));
            }
            else
            {
                var qry1 = ctx.GnMstDealerMappings.Where(p => p.Area == area).AsQueryable();
                var companyCode = qry1.Select(q => q.DealerCode);
                var qry2 = ctx.Organizations.Where(y => companyCode.Contains(y.CompanyCode));
                var dlr = DealerType;
                if (!string.IsNullOrWhiteSpace(dlr))
                {
                    qry2 = qry2.Where(p => p.DealerType == dlr);
                }
                return Json(qry2.Select(p => new { value = p.CompanyCode, text = p.CompanyCode + " - " + p.CompanyName.ToUpper(), p.CompanyName }).OrderBy(p => p.CompanyName));
            }
        }

        public JsonResult BranchsV2(string area = "", string comp = "")
        {
            //var list = ctx.CoProfiles.Where(p => p.CompanyCode == comp).OrderBy(p => p.BranchCode).Select(p => new { value = p.BranchCode, text = p.BranchCode + " - " + p.CompanyName.ToUpper(), p.CompanyName }).OrderBy(p => p.CompanyName).ToList();
            //return Json(list);
            var query = string.Format(@"select gdom.DealerCode,gdom.OutletCode,gmcp.CompanyName 
                                        from [gnMstDealerOutletMapping] gdom 
                                        inner join (select * from GnMstDealerMapping where Area='{0}' ) gdm on gdm.DealerCode=gdom.DealerCode and gdm.GroupNo=gdom.GroupNo
                                        inner join gnMstCoProfile gmcp on gmcp.BranchCode=gdom.OutletCode
                                        where gdom.DealerCode='{1}'", area, comp);
            var queryable = ctx.Database.SqlQuery<BranchV2View>(query).AsQueryable();
            return Json(queryable.Select(p => new { value = p.OutletCode, text = p.CompanyName.ToUpper(), p.OutletCode }).Distinct().OrderBy(p => p.OutletCode));

        }

        //end

        public JsonResult Grades()
        {
            string position = Request["Position"] ?? "";

            if (position == "S")
            {
                var list = ctx.MstLookupDtls.Where(p => p.CodeID == "ITSG").OrderBy(p => p.LookUpValueName).Select(p => new { value = p.LookUpValue, text = p.LookUpValueName }).ToList();
                return Json(list);
            }
            else
            {
                var list = new List<object>();
                return Json(list);
            }
        }

        public JsonResult Branches()
        {
            string companyCode = Request["id"] ?? "";
            var list = ctx.CoProfiles.Where(p => p.CompanyCode == companyCode).OrderBy(p => p.BranchCode).Select(p => new { value = p.BranchCode, text = p.BranchCode + " - " + p.CompanyName.ToUpper() }).ToList();
            return Json(list);
        }

        public JsonResult ListBranchesNew(string area = "", string comp = "")
        {
            var query = string.Format(@"declare @area varchar(100),@dealername varchar(100),@groupno int
                                        set @area='{0}';set @dealername='{1}'
                                        if (@area='CABANG')
                                        begin
                                            set @groupno =(select groupno from gnMstDealerMappingNew where dealername='{1}' )
                                            select gdom.DealerCode,gdom.OutletCode,gmcp.CompanyName
                                            from [gnMstDealerOutletMappingNew] gdom
                                            inner join (select * from gnMstDealerMappingNew where isJV=1 ) gdm on gdm.DealerCode=gdom.DealerCode
	                                        inner join gnMstCoProfile gmcp on gmcp.BranchCode=gdom.OutletCode
	                                        where gdm.Dealername=@dealername and gdom.GroupNo=@groupno
                                        end
                                        else
                                        begin
                                            set @groupno =(select groupno from gnMstDealerMappingNew where dealername='{1}' )
                                            select gdom.DealerCode,gdom.OutletCode,gmcp.CompanyName 
	                                        from [gnMstDealerOutletMappingNew] gdom 
	                                        inner join (select * from gnMstDealerMappingNew where Area=@area ) gdm on gdm.DealerCode=gdom.DealerCode 
	                                        inner join gnMstCoProfile gmcp on gmcp.BranchCode=gdom.OutletCode
	                                        where gdm.Dealername=@dealername and gdom.GroupNo=@groupno
                                        end", area, comp);
            var queryable = ctx.Database.SqlQuery<BranchV2View>(query).AsQueryable();
            return Json(queryable.Select(p => new { value = p.OutletCode, text = p.CompanyName.ToUpper(), p.OutletCode }).Distinct().OrderBy(p => p.OutletCode));

        }

        public JsonResult ListBranchesNew2(string area = "", string comp = "")
        {

            var query = string.Format(@"declare @area varchar(100),@dealername varchar(100),@groupno int
                                        set @area='{0}';set @dealername='{1}'
                                        if (@area='100')
                                        begin
                                            set @groupno =(select groupno from gnMstDealerMappingNew where dealercode='{0}' )
                                            select gdom.DealerCode,gdom.OutletCode,gmcp.CompanyName
                                            from [gnMstDealerOutletMappingNew] gdom
                                            inner join (select * from gnMstDealerMappingNew where isJV=1 ) gdm on gdm.DealerCode=gdom.DealerCode
	                                        inner join gnMstCoProfile gmcp on gmcp.BranchCode=gdom.OutletCode
	                                        where gdm.dealercode=@dealername and gdom.GroupNo=@groupno
                                        end
                                        else
                                        begin
                                            set @groupno =(select groupno from gnMstDealerMappingNew where dealercode='{1}' )
                                            select gdom.DealerCode,gdom.OutletCode,gmcp.CompanyName 
	                                        from [gnMstDealerOutletMappingNew] gdom 
	                                        inner join (select * from gnMstDealerMappingNew where Area=@area ) gdm on gdm.DealerCode=gdom.DealerCode 
	                                        inner join gnMstCoProfile gmcp on gmcp.BranchCode=gdom.OutletCode
	                                        where gdm.dealercode=@dealername and gdom.GroupNo=@groupno
                                        end", area, comp);
            var queryable = ctx.Database.SqlQuery<BranchV2View>(query).AsQueryable();
            return Json(queryable.Select(p => new { value = p.OutletCode, text = p.CompanyName.ToUpper(), p.OutletCode }).Distinct().OrderBy(p => p.OutletCode));

        }

//        public JsonResult ListBranchesNew3()
//        {
//            var query = string.Format(@"declare @area varchar(100),@dealername varchar(100),@groupno int
//                                        set @area='{0}';set @dealername='{1}'
//                                        if (@area='100')
//                                        begin
//                                            set @groupno =(select groupno from gnMstDealerMappingNew where dealercode='{1}' )
//                                            select gdom.DealerCode,gdom.OutletCode,gmcp.CompanyName
//                                            from [gnMstDealerOutletMappingNew] gdom
//                                            inner join (select * from gnMstDealerMappingNew where isJV=1 ) gdm on gdm.DealerCode=gdom.DealerCode
//	                                        inner join gnMstCoProfile gmcp on gmcp.BranchCode=gdom.OutletCode
//	                                        where gdm.dealercode=@dealername and gdom.GroupNo=@groupno
//                                        end
//                                        else
//                                        begin
//                                            set @groupno =(select groupno from gnMstDealerMappingNew where dealercode='{1}' )
//                                            select gdom.DealerCode,gdom.OutletCode,gmcp.CompanyName 
//	                                        from [gnMstDealerOutletMappingNew] gdom 
//	                                        inner join (select * from gnMstDealerMappingNew where Area=@area ) gdm on gdm.DealerCode=gdom.DealerCode 
//	                                        inner join gnMstCoProfile gmcp on gmcp.BranchCode=gdom.OutletCode
//	                                        where gdm.dealercode=@dealername and gdom.GroupNo=@groupno
//                                        end", area, comp);
//            var queryable = ctx.Database.SqlQuery<BranchV2View>(query).AsQueryable();
//            return Json(queryable.Select(p => new { value = p.OutletCode, text = p.CompanyName.ToUpper(), p.OutletCode }).Distinct().OrderBy(p => p.OutletCode));

//        }

        public JsonResult CouponNo()
        {
            string DateFrom = Request["DateFrom"] ?? "";
            string DateTo = Request["DateTo"] ?? "";
            string CompanyCode = Request["Dealer"] ?? "";
            string BranchCode = Request["Outlet"] ?? "";

            var data = ctx.Database.SqlQuery<pmKDPCoupon>(string.Format(@"SELECT DISTINCT a.* FROM pmKDPCoupon a 
                INNER JOIN pmKDP b 
	                ON a.CompanyCode = b.CompanyCode 
	                AND a.InquiryNumber = b.InquiryNumber 
                WHERE a.CompanyCode = '{0}' AND b.BranchCode = '{1}'
                AND a.TestDriveDate BETWEEN '{2}' AND '{3}'", CompanyCode, BranchCode, DateFrom, DateTo));

            if (data != null && data.Count() > 0)
            {
                return Json(data.Select(x => new
                {
                    text = x.CoupunNumber,
                    value = x.CoupunNumber
                }));
            }

            return Json(new
            {
                text = "-- SELECT ALL --",
                value = ""
            });
        }

        public JsonResult ProblemService()
        {
            var data = ctx.Database.SqlQuery<RefferenceService>(string.Format(@"SELECT RefferenceCode, Description FROM svMstRefferenceService WHERE RefferenceType = 'DELAY_CODE'"));
            if (data != null && data.Count() > 0)
            {
                return Json(data.Select(x => new
                {
                    text = x.Description,
                    value = x.RefferenceCode
                }));
            }
            else
            {
                return Json(new
                {
                    text = "-- SELECT ALL --",
                    value = ""
                });
            }
        }

        public JsonResult ModelService()
        {
            var data = ctx.Database.SqlQuery<RefferenceService>(string.Format(@"SELECT RefferenceCode, Description FROM svMstRefferenceService WHERE RefferenceType = 'BASMODEL'"));
            if (data != null && data.Count() > 0)
            {
                return Json(data.Select(x => new
                {
                    text = x.Description,
                    value = x.RefferenceCode
                }));
            }
            else
            {
                return Json(new
                {
                    text = "-- SELECT ALL --",
                    value = ""
                });
            }
        }

        public JsonResult BranchesAll()
        {
            string companyCode = Request["id"] ?? "";
            string productType = Request["ProductType"] ?? "";

            var rawData = ctx.BranchList.AsQueryable();

            if (!string.IsNullOrEmpty(companyCode))
            {
                rawData = rawData.Where(x => x.CompanyCode == companyCode);
            }

            if (!string.IsNullOrEmpty(productType))
            {
                rawData = rawData.Where(x => x.ProductType == productType);
            }

            var list = rawData.Select(p => new { value = p.BranchCode, text = p.BranchCode + " - " + p.CompanyName.ToUpper() }).ToList();

            return Json(list);
        }

        public JsonResult BranchesR2()
        {
            string companyCode = Request["id"] ?? "";
            string group = Request["Area"] ?? "";
            int? area;
            if (group == "") { area = 0; } else { area = Convert.ToInt32(Request["Area"] ?? ""); }

            //var list = ctx.CoProfiles.Where(p => p.CompanyCode == companyCode).OrderBy(p => p.CompanyName).Select(p => new { value = p.BranchCode, text = p.CompanyName.ToUpper() + " ===============> " + p.BranchCode }).ToList();
            var list = ctx.GnMstDealerOutletMappings.Where(p => p.DealerCode == companyCode && p.GroupNo == area).OrderBy(p => p.OutletName).Select(p => new { value = p.OutletCode, text = p.OutletCode + " - " + p.OutletName.ToUpper() }).ToList();
            return Json(list);
            //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            //cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_Branch_list";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            //da.Fill(dt);
            //return Json(GetJson(dt));
        }

        public JsonResult BranchList()
        {
            string companyCode = Request["CompanyCode"] ?? "";
            string group = Request["Group"] ?? "";
            int? area;
            if (group == "") { area = 0; } else { area = Convert.ToInt32(Request["Group"] ?? ""); }

            //var list = ctx.CoProfiles.Where(p => p.CompanyCode == companyCode).OrderBy(p => p.CompanyName).Select(p => new { value = p.BranchCode, text = p.CompanyName.ToUpper() + " ===============> " + p.BranchCode }).ToList();
            var list = ctx.GnMstDealerOutletMappings.Where(p => p.DealerCode == companyCode && p.GroupNo == area).OrderBy(p => p.OutletName).Select(p => new { value = p.OutletCode, text = p.OutletCode + " - " + p.OutletName.ToUpper() }).ToList();
            return Json(list);
            //SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            //cmd.CommandTimeout = 3600;  cmd.CommandText = "uspfn_Branch_list";
            //cmd.CommandType = CommandType.StoredProcedure;
            //cmd.Parameters.Clear();
            //cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
            //SqlDataAdapter da = new SqlDataAdapter(cmd);
            //DataTable dt = new DataTable();
            //da.Fill(dt);
            //return Json(GetJson(dt));
        }

        public JsonResult Branchs(string comp = "")
        {
            var list = (dynamic)null;
            if (comp == "")
            {
                list = ctx.CoProfiles.OrderBy(p => p.BranchCode).Select(p => new { value = p.BranchCode, text = p.BranchCode + " - " + p.CompanyName.ToUpper(), p.CompanyName }).OrderBy(p => p.CompanyName).ToList();
            }
            else
            {
                list = ctx.CoProfiles.Where(p => p.CompanyCode == comp).OrderBy(p => p.BranchCode).Select(p => new { value = p.BranchCode, text = p.BranchCode + " - " + p.CompanyName.ToUpper(), p.CompanyName }).OrderBy(p => p.CompanyName).ToList();
            }
            return Json(list);
        }

        public JsonResult Departments(string comp = "")
        {
            var list = ctx.HrOrgGroups.Where(p => p.OrgGroupCode == "DEPT").OrderBy(p => p.OrgSeq).Select(p => new { value = p.OrgCode, text = p.OrgName.ToUpper() }).OrderBy(x => x.text).ToList();
            return Json(list);
        }

        public JsonResult Positions(string comp = "", string dept = "")
        {
            //var qry = ctx.Positions.Where(p => p.CompanyCode == comp && (p.DeptCode.Equals(dept) || p.DeptCode.Equals("COM"))).ToList();
            var qry = ctx.HrPositions.Where(p => (p.DeptCode.Equals(dept) || p.DeptCode.Equals("COM"))).ToList();
            var list = qry.OrderBy(p => p.PosLevel).Select(p => new { value = p.PosCode, text = (p.PosName.ToUpper() + string.Format("  ({0} - {1})", p.PosCode, p.PosLevel)) }).ToList();
            return Json(list);
        }

        public JsonResult Positionsv2(string dept = "")
        {
            //var qry = ctx.Positions.Where(p => p.CompanyCode == comp && (p.DeptCode.Equals(dept) || p.DeptCode.Equals("COM"))).ToList();
            //var qry = ctx.HrPositions.Where(p => (p.DeptCode.Equals(dept) || p.DeptCode.Equals("COM"))).ToList();
            //var list = qry.OrderBy(p => p.PosLevel).Select(p => new { value = p.PosCode, text = (p.PosName.ToUpper() + string.Format("  ({0} - {1})", p.PosCode, p.PosLevel)) }).ToList();
            //return Json(list);

            var query = string.Format(@"select 
	                                        DeptCode,case when PosCode='S' then 'S0' else PosCode end as posCode ,
	                                        case when posname='WIRANIAGA' then 'Sales Person' else posname end as PosName,
	                                        case when PosLevel=10 then PosLevel+ (select count(LookUpValue)+1 from [dbo].[MstLookUpDtl] where CodeID='ITSG') else  PosLevel end as PosLevel
                                        from HrMstPosition 
                                        where DeptCode='{0}' and PosLevel not in (20,50) 
                                        union
                                        select 
	                                        distinct Department,position+hdt.grade as PosCode,'Sales '+mstDtl.LookUpValueName as PosName,10+mstDtl.ParaValue as poslevel
                                        from [HrDepartmentTraining] hdt 
                                        inner join (select * from [dbo].[MstLookUpDtl] where CodeID='ITSG') mstDtl on mstDtl.ParaValue=hdt.grade
                                        where department='{0}' and Position='s'
                                        order by PosLevel desc", dept);
            var queryable = ctx.Database.SqlQuery<CbPosition>(query).AsQueryable();
            return Json(queryable.Select(p => new { value = p.PosCode, text = p.PosName.ToUpper(), p.PosCode }).Distinct());

        }

        public ActionResult TrainingPrograms(string dept = "")
        {
            //var qry = ctx.Positions.Where(p => p.CompanyCode == comp && (p.DeptCode.Equals(dept) || p.DeptCode.Equals("COM"))).ToList();
            var qry = ctx.HrMstTrainings.ToList();
            var list = qry.OrderBy(p => p.TrainingCode).Select(p => new { value = p.TrainingCode, text = (p.TrainingDescription.ToUpper() + string.Format("  ({0})", p.TrainingCode)) }).ToList();
            return Json(list);
        }

        public JsonResult Years()
        {
            var list = new List<int>();
            for (int i = DateTime.Now.Year - 3; i <= DateTime.Now.Year; i++)
            {
                list.Add(i);
            }
            return Json(list.OrderByDescending(p => p).Select(p => new { value = p, text = p }));
        }

        public JsonResult ListYears()
        {
            var list = new List<int>();
            for (int i = DateTime.Now.Year; i <= 2099; i++)
            {
                list.Add(i);
            }
            return Json(list.Select(p => new { value = p, text = p }));
        }

        public JsonResult YearsNoDefaultText()
        {
            var list = new List<int>();
            for (int i = DateTime.Now.Year - 3; i <= DateTime.Now.Year - 1; i++)
            {
                list.Add(i);
            }
            return Json(list.OrderByDescending(p => p).Select(p => new { value = p, text = p }));
        }
        public JsonResult YearsMSI()
        {
            var list = new List<int>();
            for (int i = DateTime.Now.Year - 3; i <= DateTime.Now.Year; i++)
            {
                list.Add(i);
            }
            return Json(list.OrderByDescending(p => p).Select(p => new { value = p, text = p }));
        }

        public JsonResult GetDealerType()
        {
            return Json(DealerType);
        }

        public JsonResult ListOfMonth()
        {
            List<Object> listObj = new List<Object>();
            string[] listMonth = new string[] { "Januari", "Februari", "Maret", "April", "Mei", "Juni", "Juli", "Agustus", "September", "Oktober", "November", "Desember" };
            int idx = 1;
            foreach (var month in listMonth)
            {
                listObj.Add(new { value = idx, text = month });
                idx++;
            }
            return Json(listObj);
        }

        public JsonResult ListOfYear()
        {
            var year = DateTime.Now.Year;
            List<Object> listObj = new List<Object>();
            for (int i = year - 5; i <= year; i++)
            {
                listObj.Add(new { value = i.ToString(), text = i.ToString() });
            }
            return Json(listObj);
        }

        public JsonResult ListOfYear10() 
        {
            var year = DateTime.Now.Year;
            var year10 = DateTime.Now.Year + 10;
            List<Object> listObj = new List<Object>();
            for (int i = year - 10; i <= year10; i++)
            {
                listObj.Add(new { value = i.ToString(), text = i.ToString() });
            }
            return Json(listObj);
        }

        public JsonResult ListOfYearMin10() 
        {
            var year = DateTime.Now.Year;
            var year10 = DateTime.Now.Year - 10;
            List<Object> listObj = new List<Object>();
            for (int i = 1980; i <= year; i++)
            {
                listObj.Add(new { value = i.ToString(), text = i.ToString() });
            }
            return Json(listObj);
        }

        public JsonResult MpListOfYear()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_MpListOfYear";
            cmd.CommandTimeout = 3600;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult ListBranchCode()
        {
            var listBranch = ctx.Organizations.Select(m => new { value = m.CompanyCode, text = m.CompanyName });
            return Json(listBranch);
        }

        public JsonResult ListDealerCS()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_CsListDealer";
            cmd.CommandTimeout = 3600;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult ListDealer()
        {
            string module = Request["Module"] ?? "";
            string roleModule = ctx.SysUserViews.Where(x => x.Username == User.Identity.Name).Select(x => x.RoleName).FirstOrDefault();

            var rawData = (from x in ctx.Organizations
                           from y in ctx.DealerModuleMappings
                           where
                           x.CompanyCode == y.DealerCode
                           &&
                           y.IsActive == true
                           select new
                           {
                               text = x.CompanyCode + " - " + x.CompanyName,
                               value = x.CompanyCode,
                               module = y.Module,
                               dealerName = x.CompanyName,
                               productType = y.ProductType
                           }).AsQueryable();

            if (!string.IsNullOrWhiteSpace(module))
            {
                rawData = rawData.Where(x => x.module == module);
            }

            if (roleModule != null && roleModule.ToLower() == "sfm2w")
            {
                rawData = rawData.Where(x => x.productType != null && x.productType.ToLower() == "2w");
            }
            else if (roleModule != null && roleModule.ToLower() == "sfm")
            {
                rawData = rawData.Where(x => x.productType != null && x.productType.ToLower() == "4w");
            }

            rawData = rawData.OrderBy(x => x.dealerName);

            return Json(rawData);
        }

        public JsonResult DealerList()
        {
            string groupArea = Request["GroupArea"] ?? "";
            string dealerType = Request["DealerType"] ?? "";
            string linkedModule = Request["LinkedModule"] ?? "";

            if (string.IsNullOrWhiteSpace(dealerType))
            {
                dealerType = DealerType;
            }

            var data = ctx.Database.SqlQuery<DealerList>("exec uspfn_DealerList @DealerType=@p0, @LinkedModule=@p1, @GroupArea=@p2", dealerType, linkedModule, groupArea);

            if (data != null && data.Count() > 0)
            {
                return Json(data.Select(x => new
                {
                    text = x.CompanyName,
                    value = x.CompanyCode,
                    //GroupNo = x.GroupNo
                }).Distinct());
            }

            return Json(new
            {
                text = "-- SELECT ALL --",
                value = ""
            });
        }

        public JsonResult DealerListNew(string GroupArea)
        {
            var data = ctx.Database.SqlQuery<DealerList>("exec uspfn_DealerListNew @GroupArea=@p0", GroupArea).ToList();

            if (data.Count() > 0)
            {
                return Json(data.Select(x => new
                {
                    text = x.CompanyName,
                    value = x.GroupNo + "|" + x.CompanyCode,
                    //GroupNo = x.GroupNo
                }).Distinct());
            }

            return Json(new
            {
                text = "-- SELECT ALL --",
                value = ""
            });
        }

        public JsonResult ComboDealerList(string GroupArea)
        {
            var data = ctx.Database.SqlQuery<CboItem>("exec uspfn_ComboDealerList @GroupArea=@p0", GroupArea).ToList();

            return Json(data);
        }

        public JsonResult ComboOutletList(string companyCode)
        {
            string query = string.Format(@"uspfn_ComboOutletList '{0}'", companyCode);

            var queryable = ctx.Database.SqlQuery<CboItem>(query).AsQueryable();
            return Json(queryable);
        }

        public JsonResult SvDealerList()
        {
            string groupArea = Request["GroupArea"] ?? "";
            string dealerType = Request["DealerType"] ?? "";
            string linkedModule = Request["LinkedModule"] ?? "";

            if (string.IsNullOrWhiteSpace(dealerType))
            {
                dealerType = DealerType;
            }

            var data = ctx.Database.SqlQuery<DealerList>("exec uspfn_SvDealerList @DealerType=@p0, @LinkedModule=@p1, @GroupArea=@p2", dealerType, linkedModule, groupArea);

            if (data != null && data.Count() > 0)
            {
                return Json(data.Select(x => new
                {
                    text = x.CompanyCode + " - " + x.CompanyName,
                    value = x.CompanyCode
                }));
            }

            return Json(new
            {
                text = "-- SELECT ALL --",
                value = ""
            });
        }

        public JsonResult SrvDealerList()
        {
            string groupArea = Request["GroupArea"] ?? "";
            string dealerType = Request["DealerType"] ?? "";
            string linkedModule = Request["LinkedModule"] ?? "";

            if (string.IsNullOrWhiteSpace(dealerType))
            {
                dealerType = DealerType;
            }

            var data = ctx.Database.SqlQuery<DealerList>("exec uspfn_SrvDealerList @DealerType=@p0, @LinkedModule=@p1, @GroupArea=@p2", dealerType, linkedModule, groupArea);

            if (data != null && data.Count() > 0)
            {
                return Json(data.Select(x => new
                {
                    text = x.CompanyName,
                    value = x.CompanyCode
                }));
            }

            return Json(new
            {
                text = "-- SELECT ALL --",
                value = ""
            });
        }

        public JsonResult SrvDealerListVin()
        {
            string groupArea = Request["GroupArea"] ?? "";
            string dealerType = Request["DealerType"] ?? "";
            string linkedModule = Request["LinkedModule"] ?? "";

            if (string.IsNullOrWhiteSpace(dealerType))
            {
                dealerType = DealerType;
            }

            var data = ctx.Database.SqlQuery<DealerList>("exec uspfn_SrvDealerListVin @DealerType=@p0, @LinkedModule=@p1, @GroupArea=@p2", dealerType, linkedModule, groupArea);

            if (data != null && data.Count() > 0)
            {
                return Json(data.Select(x => new
                {
                    text = x.CompanyName,
                    value = x.CompanyCode
                }));
            }

            return Json(new
            {
                text = "-- SELECT ALL --",
                value = ""
            });
        }

        public JsonResult SvBranchList()
        {
            string area = Request["area"] ?? "";
            string comp = Request["comp"] ?? "";

            var data = ctx.Database.SqlQuery<DealerList>("exec uspfn_SvBranchList @CompanyCode=@p0, @GroupArea=@p1", comp, area);

            if (data != null && data.Count() > 0)
            {
                return Json(data.Select(x => new
                {
                    text = x.CompanyCode + " - " + x.CompanyName,
                    value = x.CompanyCode
                }));
            }

            return Json(new
            {
                text = "-- SELECT ALL --",
                value = ""
            });
        }
        public JsonResult SrvAreas() 
        {
            var data = ctx.Database.SqlQuery<SrvArea>("exec uspfn_SrvArea");

            if (data != null && data.Count() > 0)
            {
                return Json(data.Select(x => new
                {
                    text = x.Area,
                    value = x.GroupNo
                }));
            }

            return Json(new
            {
                text = "-- SELECT ALL --",
                value = ""
            });
        }

        public JsonResult SrvBranchList()
        {
            string area = Request["area"] ?? "";
            string comp = Request["comp"] ?? "";

            var data = ctx.Database.SqlQuery<DealerList>("exec uspfn_SrvBranchList @CompanyCode=@p0, @GroupArea=@p1", comp, area);

            if (data != null && data.Count() > 0)
            {
                return Json(data.Select(x => new
                {
                    text = x.CompanyName,
                    value = x.CompanyCode
                }));
            }

            return Json(new
            {
                text = "-- SELECT ALL --",
                value = ""
            });
        }

        public JsonResult SrvBranchListVin()
        {
            string area = Request["area"] ?? "";
            string comp = Request["comp"] ?? "";

            var data = ctx.Database.SqlQuery<DealerList>("exec uspfn_SrvBranchListVin @CompanyCode=@p0, @GroupArea=@p1", comp, area);

            if (data != null && data.Count() > 0)
            {
                return Json(data.Select(x => new
                {
                    text = x.CompanyName,
                    value = x.CompanyCode
                }));
            }

            return Json(new
            {
                text = "-- SELECT ALL --",
                value = ""
            });
        }

        public JsonResult TableList()
        {
            //var pcenter = Request["pcenter"] ?? "";
            //var data = ctx.Database.SqlQuery<DealerList>("exec uspfn_InqSdmsDataList @DealerType=@p0, @LinkedModule=@p1, @GroupArea=@p2", dealerType, linkedModule, groupArea);

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_InqSdmsDataList";
            cmd.CommandTimeout = 3600;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("type", Request["pcenter"] ?? "");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));

            //var list = new List<dynamic>();
            //list.Add(new { text = "GnMstCustomer", value = "GnMstCustomer", type = "gn", });
            //list.Add(new { text = "CsTDayCall", value = "CsTDayCall", type = "gn", });
            //list.Add(new { text = "CsCustBirthDay", value = "CsCustBirthDay", type = "gn", });
            //list.Add(new { text = "CsCustBpkb", value = "CsCustBpkb", type = "gn", });
            //list.Add(new { text = "CsStnkExt", value = "CsStnkExt", type = "gn", });
            //list.Add(new { text = "HrEmployee", value = "HrEmployee", type = "gn", });
            //list.Add(new { text = "HrEmployeeAchievement", value = "HrEmployeeAchievement", type = "gn", });
            //list.Add(new { text = "HrEmployeeMutation", value = "HrEmployeeMutation", type = "gn", });
            //list.Add(new { text = "HrEmployeeTraining", value = "HrEmployeeTraining", type = "gn", });

            //list.Add(new { text = "SvMstCustomerVehicle", value = "SvMstCustomerVehicle", type = "sv", });

            //list.Add(new { text = "PmHstITS", value = "PmHstITS", type = "sl", });

            //if (pcenter == "")
            //{
            //    return Json(list);
            //}
            //else
            //{
            //    return Json(list.Where(p => p.type == pcenter));
            //}
        }

        public JsonResult MpTableList()
        {
            //var pcenter = Request["pcenter"] ?? "";
            //var data = ctx.Database.SqlQuery<DealerList>("exec uspfn_InqSdmsDataList @DealerType=@p0, @LinkedModule=@p1, @GroupArea=@p2", dealerType, linkedModule, groupArea);

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_InqMpDataList";
            cmd.CommandTimeout = 3600;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("type", Request["pcenter"] ?? "");
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));


            //var list = new List<dynamic>();
            //list.Add(new { text = "GnMstCustomer", value = "GnMstCustomer", type = "gn", });
            //list.Add(new { text = "CsTDayCall", value = "CsTDayCall", type = "gn", });
            //list.Add(new { text = "CsCustBirthDay", value = "CsCustBirthDay", type = "gn", });
            //list.Add(new { text = "CsCustBpkb", value = "CsCustBpkb", type = "gn", });
            //list.Add(new { text = "CsStnkExt", value = "CsStnkExt", type = "gn", });
            //list.Add(new { text = "HrEmployee", value = "HrEmployee", type = "gn", });
            //list.Add(new { text = "HrEmployeeAchievement", value = "HrEmployeeAchievement", type = "gn", });
            //list.Add(new { text = "HrEmployeeMutation", value = "HrEmployeeMutation", type = "gn", });
            //list.Add(new { text = "HrEmployeeTraining", value = "HrEmployeeTraining", type = "gn", });

            //list.Add(new { text = "SvMstCustomerVehicle", value = "SvMstCustomerVehicle", type = "sv", });

            //list.Add(new { text = "PmHstITS", value = "PmHstITS", type = "sl", });

            //if (pcenter == "")
            //{
            //    return Json(list);
            //}
            //else
            //{
            //    return Json(list.Where(p => p.type == pcenter));
            //}
        }

        public JsonResult GroupModelList()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_InqGroupModelList";
            cmd.CommandTimeout = 3600;
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataTable dt = new DataTable();
            da.Fill(dt);
            return Json(GetJson(dt));
        }

        public JsonResult PartSalesFilter()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_SpPartSalesFilter";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            if (!string.IsNullOrWhiteSpace(Request["ProductType"]))
            {
                cmd.Parameters.AddWithValue("ProductType", Request["ProductType"]);
            }
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            return Json(GetJson(ds));
        }

        public JsonResult RegisterSpkFilter()
        {
            var json = Exec(new { query = "uspfn_SvRegisterSpkFilter", result = "dataset" });
            return Json(json.Data);
        }

        public JsonResult UnitIntakeFilter()
        {
            var json = Exec(new { query = "uspfn_vwUnitIntakeFilter", result = "dataset" });
            return Json(json.Data);
        }

        public JsonResult PmItsByTestDriveFilter()
        {
            var json = Exec(new { query = "uspfn_PmItsByTestDriveFilter", result = "dataset" });
            return Json(json.Data);
        }

        public JsonResult PmItsByLeadTimeFilter()
        {
            var json = Exec(new { query = "uspfn_PmItsByLeadTimeFilter", result = "dataset" });
            return Json(json.Data);
        }

        public JsonResult InqITSByLostCaseFilter()
        {
            List<object> data = new List<object>();
            data.Add(new
            {
                text = "Inquiry Date",
                value = "0"
            });
            data.Add(new
            {
                text = "Lost Case Date",
                value = "1"
            });

            return Json(data);
        }

        public JsonResult InqITSByPerolehanDataFilter()
        {
            List<object> data = new List<object>();
            data.Add(new
            {
                text = "Inquiry Date",
                value = "0"
            });
            data.Add(new
            {
                text = "SPK Date",
                value = "1"
            });

            return Json(data);
        }

        public JsonResult GnSchedulerLogFilter()
        {
            var json = Exec(new { query = "uspfn_GnSchdulerLogFilter", result = "dataset" });
            return Json(json.Data);
        }

        public JsonResult GnCollDataLogFilter()
        {
            var json = Exec(new { query = "uspfn_GnCollDataLogFilter", result = "dataset" });
            return Json(json.Data);
        }

        public JsonResult Lookups(string id = "")
        {
            //var companyCode = string.IsNullOrEmpty(Request["CompanyCode"]) || Request["CompanyCode"] == "undefined" ? CompanyCode : Request["CompanyCode"];

            var list = ctx.GnMstLookUpDtls.Where(
                p => //p.CompanyCode == companyCode &&
                p.CodeID == id).OrderBy(p => p.SeqNo).
                Select(p => new
                {
                    value = p.LookUpValue,
                    text = p.LookUpValueName.ToUpper()
                }).Distinct().ToList();

            return Json(list);
        }

        public JsonResult CarTypes()
        {
            var qry = ctx.PmGroupTypeSeqs.OrderBy(p => p.GroupCodeSeq).Select(
                p => new
                    {
                        value = p.GroupCode,
                        text = p.GroupCode,
                    }
                );

            return Json(qry.Distinct().ToList());
        }

        public JsonResult CarVariants(string id = "")
        {
            var qry = ctx.PmGroupTypeSeqs.OrderBy(p => p.TypeCodeSeq).Where(p => p.GroupCode == id).Select(
                    p => new
                    {
                        value = p.TypeCode,
                        text = p.TypeCode
                    }
                );

            return Json(qry.Distinct().ToList());
        }

        public JsonResult MasterModel()
        {
            var rslt = ctx.OmMstModels
                .Where(x=>x.Status=="1")
                .ToList()                
                .Select(x => new { value = x.GroupCode == null ? "OTHERS" : x.GroupCode == "" ? "OTHERS" : x.GroupCode, text = x.GroupCode == null ? "OTHERS" : x.GroupCode == "" ? "OTHERS" : x.GroupCode })
                .Distinct();
            return Json(rslt);
        }

        public JsonResult MasterModelVariant()
        {
            var rslt = ctx.MsMstGroupModels
                .Where(x => x.Status == true)
                .ToList()       
                .OrderBy(x=>x.ModelType)
                .Select(x => new { value = x.ModelType == null ? "OTHERS" : x.ModelType == "" ? "OTHERS" : x.ModelType, 
                                    text = x.ModelType == null ? "OTHERS" : x.ModelType == "" ? "OTHERS" : x.ModelType,
                                    GroupModel=x.GroupModel
                                })
                .Distinct();
            return Json(rslt);
        }


        public JsonResult GetVariantFromModel( string[] lstModel)
        {

            var rslt = ctx.OmMstModels.AsQueryable();
            if(lstModel!=null)
            {
                rslt = rslt.Where(x => lstModel.Contains(x.GroupCode));
            }
            var rs= rslt.Select(x => x.TypeCode)
                    .Distinct();

            return Json(new { variant = rs });
        }
        public JsonResult GroupModel()
        {           
                var lst = ctx.MsMstGroupModels
                        .Where(x=>x.Status==true)
                        .OrderBy(x => x.GroupModel)
                        .Select(y => new { value = y.GroupModel, text = y.GroupModel })
                        .Distinct()
                        .ToList();
                return Json(lst);            
        }
        public JsonResult Transmissions()
        {
            var cartype = Request["CarType"];
            var carvari = Request["CarVariant"];
            var qry = from p in ctx.OmMstModels
                      join q in ctx.GnMstLookUpDtls on
                          new { TransmissionType = p.TransmissionType }
                          equals new { TransmissionType = q.LookUpValue }
                      where q.CodeID == "TRTY"
                      && p.GroupCode == cartype
                      && p.TypeCode == carvari
                      select new
                      {
                          value = p.TransmissionType,
                          text = p.TransmissionType + " - " + q.LookUpValueName
                      };

            return Json(qry.Distinct().ToList());
        }

        public JsonResult ModelColors()
        {
            var cartype = Request["CarType"];
            var carvari = Request["CarVariant"];
            var cartran = Request["CarTrans"];

            var qry = from p in ctx.OmMstModelColours
                      join q in ctx.OmMstModels on
                      new { p.SalesModelCode } equals
                      new { q.SalesModelCode }
                      join r in ctx.OmMstRefferences on
                      new { p.CompanyCode, ColourCode = p.ColourCode, GroupCode = "COLO" } equals
                      new { r.CompanyCode, ColourCode = r.RefferenceCode, GroupCode = r.RefferenceType }
                      where
                      q.GroupCode == cartype
                      && q.TypeCode == carvari
                      && q.TransmissionType == cartran
                      select new
                      {
                          value = p.ColourCode,
                          text = p.ColourCode + " - " + r.RefferenceDesc1
                      };

            return Json(qry.Distinct().ToList());
        }

        public JsonResult ItsStatus(string last = "")
        {
            var companyCode = string.IsNullOrEmpty(Request["CompanyCode"]) ? CompanyCode : Request["CompanyCode"];
            var qry = ctx.GnMstLookUpDtls.Where(p => p.CompanyCode == companyCode && p.CodeID == "PSTS").OrderBy(p => p.SeqNo).Select(p => new { value = p.LookUpValue, text = p.LookUpValueName.ToUpper(), seqno = p.SeqNo });
            if (!string.IsNullOrWhiteSpace(last))
            {
                var oLast = ctx.GnMstLookUpDtls.Find(companyCode, "PSTS", last);
                if (oLast != null)
                {
                    qry = qry.Where(p => p.seqno >= oLast.SeqNo);
                }
            }

            return Json(qry.ToList());
        }

        public JsonResult Employee(string id = "")
        {
            var companyCode = string.IsNullOrEmpty(Request["CompanyCode"]) ? CompanyCode : Request["CompanyCode"];
            var qry = ctx.HrEmployees.Where(p => p.CompanyCode == companyCode && p.TeamLeader == id).Select(
                    p => new
                    {
                        value = p.EmployeeID,
                        text = p.EmployeeName
                    }
                );

            return Json(qry.ToList());
        }

        public JsonResult CityName(string id = "")
        {
            var companyCode = string.IsNullOrEmpty(Request["CompanyCode"]) ? CompanyCode : Request["CompanyCode"];

            var city = ctx.GnMstLookUpDtls.Find(companyCode, "CITY", id);
            if (city != null)
            {
                return Json(new { success = true, data = city.LookUpValueName ?? "" });
            }
            else
            {
                return Json(new { success = false, data = "" });
            }
        }

        public JsonResult ExhibitionValue()
        {
            var companyCode = string.IsNullOrEmpty(Request["CompanyCode"]) ? CompanyCode : Request["CompanyCode"];
            var list = ctx.GnMstLookUpDtls.Where(p => p.CompanyCode == companyCode && p.CodeID == "PSRC" && p.LookUpValue == "Exhibition").OrderBy(p => p.SeqNo).Select(p => new { value = p.LookUpValue, text = p.LookUpValueName.ToUpper() }).ToList();
            return Json(list);
        }

        public JsonResult ItsStatusOneValue(string value = "")
        {
            var companyCode = string.IsNullOrEmpty(Request["CompanyCode"]) ? CompanyCode : Request["CompanyCode"];
            var qry = ctx.GnMstLookUpDtls.Where(p => p.CompanyCode == companyCode && p.CodeID == "PSTS" && p.LookUpValue == value).OrderBy(p => p.SeqNo).Select(p => new { value = p.LookUpValue, text = p.LookUpValueName.ToUpper(), seqno = p.SeqNo });

            return Json(qry.ToList());
        }

        public JsonResult SpkFilterCombo()
        {
            Dictionary<string, string> dicFilter = new Dictionary<string, string>();
            dicFilter.Add("1", "Inquiry Date");
            dicFilter.Add("2", "SPK Date");
            var qry = dicFilter.Select(p => new { value = p.Key, text = p.Value }).ToList();

            return Json(qry);
        }

        #region Indent

        private class CboItem
        {
            public string value { get; set; }
            public string text { get; set; }
        }

        public JsonResult Lookupdtls(string id = "")
        {
            var list = ctx.GnMstLookUpDtls.Where(p => p.CompanyCode == "0000000" && p.CodeID == id).OrderBy(p => p.SeqNo)
                .Select(p => new { value = p.LookUpValue.ToUpper(), text = p.LookUpValueName.ToUpper() })
                .ToList();
            return Json(list);
        }

        public JsonResult SM()
        {
            var qry = string.Format(@"select EmployeeID as value, EmployeeName as text
                                          from HrEmployee a
								                        where a.CompanyCode = '{0}'
                                                        and a.Department = 'SALES'
                                                        and a.Position = 'S'
                                                        and a.PersonnelStatus = '1'
                           ", CompanyCode);
            var empl = ctx.Database.SqlQuery<CboItem>(qry).AsQueryable();
            return Json(empl);
        }

        public JsonResult SH()
        {
            var qry = string.Format(@"select EmployeeID as value, EmployeeName as text
                                          from HrEmployee a
								                        where a.CompanyCode = '{0}'
                                                        and a.Department = 'SALES'
                                                        and a.Position = 'SH'
                                                        and a.PersonnelStatus = '1'
                           ", CompanyCode);
            var empl = ctx.Database.SqlQuery<CboItem>(qry).AsQueryable();
            return Json(empl);
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

        public JsonResult LostReasons()
        {
            var codeid = Request["CodeID"];
            var lostctg = Request["LostCtg"];
            var qry = from p in ctx.GnMstLookUpDtls
                      where p.CompanyCode == "0000000"
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

        #endregion

        //public JsonResult Sales(string Company)
        //{
        //    string dept = "SALES";
        //    string position = "S";
        //    string personStat = "1";

        //    var empl = (from p in ctx.HrEmployees
        //                where p.CompanyCode == Company && p.Department == dept &&
        //                p.Position == position && p.PersonnelStatus == personStat
        //                select new
        //                {
        //                    value = p.EmployeeID,
        //                    text = p.EmployeeName
        //                }).OrderBy(p => p.text).ToList();

        //    return Json(empl);
        //}

        private class HrMutation
        {
            public string BranchCode { get; set; }
            public string EmployeeID { get; set; }
            public DateTime MutationDate { get; set; }
        }

        public JsonResult Sales(string Company)
        {
            string dept = "SALES";
            string position = "S";
            string personStat = "1";

            var qry = String.Format(@"SELECT BranchCode, EmployeeID, MAX(MutationDate) as MutationDate
                            FROM HrEmployeeMutation a
                            where CompanyCode = '{0}' and BranchCode IN (SELECT OutletCode
                            FROM gnMstDealerOutletMapping
                            where DealerCode = '{0}' AND isActive = '1') 
                            GROUP by BranchCode, EmployeeID", Company);
            var EmployeeID = ctx.Database.SqlQuery<HrMutation>(qry).Select(a => a.EmployeeID).ToList();

            var empl = (from p in ctx.HrEmployees
                        where p.CompanyCode == Company && p.Department == dept &&
                        p.Position == position && p.PersonnelStatus == personStat &&
                        EmployeeID.Contains(p.EmployeeID) == true
                        select new
                        {
                            value = p.EmployeeID,
                            text = p.EmployeeName
                        }).OrderBy(p => p.text).ToList();

            return Json(empl);
        }

        public JsonResult SalesCoordinator(string Company)
        {
            string dept = "SALES";
            string position = "SC";
            string personStat = "1";

            var empl = (from p in ctx.HrEmployees
                        where p.CompanyCode == Company && p.Department == dept &&
                        p.Position == position && p.PersonnelStatus == personStat
                        select new
                        {
                            value = p.EmployeeID,
                            text = p.EmployeeName
                        }).OrderBy(p => p.text).ToList();

            return Json(empl);
        }

        public JsonResult SalesHead(string Company)
        {
            string dept = "SALES";
            string position = "SH";
            string personStat = "1";

            var empl = (from p in ctx.HrEmployees
                        where p.CompanyCode == Company && p.Department == dept &&
                        p.Position == position && p.PersonnelStatus == personStat
                        select new
                        {
                            value = p.EmployeeID,
                            text = p.EmployeeName
                        }).OrderBy(p => p.text).ToList();

            return Json(empl);
        }

        public JsonResult DealerAbbre()
        {
            var list = ctx.GnMstDealerMappings.OrderBy(p => p.DealerAbbreviation).Where(p => (p.GroupNo == 100 || p.GroupNo == 105)
                && p.DealerCode.Substring(4, 1) == "4" && p.isActive == true)
                .Select(p => new
                {
                    value = p.DealerCode,
                    text = p.DealerCode + " - " + p.DealerAbbreviation
                }).ToList();

            return Json(list);
        }

        public JsonResult DealerCode()
        {
            var list = ctx.GnMstDealerMappings.OrderBy(p => p.DealerAbbreviation).Where(p => p.isActive == true)
                .Select(p => new
                {
                    value = p.DealerCode,
                    text = p.DealerCode + " - " + p.DealerAbbreviation
                }).ToList();

            return Json(list);
        }

        public JsonResult OutletAbbre()
        {
            string companyCode = Request["id"] ?? "";
            var list = ctx.GnMstDealerOutletMappings.OrderBy(p => p.OutletAbbreviation).Where(p => p.DealerCode == companyCode && p.isActive == true)
                .Select(p => new
                {
                    value = p.OutletCode,
                    text = p.OutletCode + " - " + p.OutletAbbreviation
                }).ToList();

            return Json(list);
        }

        public JsonResult Gift()
        {
            var companyCode = string.IsNullOrEmpty(Request["CompanyCode"]) || Request["CompanyCode"] == "undefined" ? CompanyCode : Request["CompanyCode"];
            var list = ctx.GnMstLookUpDtls.Where(p => p.CompanyCode == companyCode &&
                p.CodeID == "GIFT").OrderBy(p => p.LookUpValueName).Select(p => new { value = p.LookUpValue, text = p.LookUpValueName.ToUpper() }).ToList();

            return Json(list);
        }

        public JsonResult LiveStockArea()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spLiveStockArea";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
        }

        public JsonResult LiveStockParts()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spLiveStockParts";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
        }

        public JsonResult LiveStockSubs()
        {
            var partno = Request.Params["PartNo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spGetSubstitusi";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("partno", partno ?? "");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
        }


        public JsonResult LiveStockArea2()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spLiveStockArea2";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
        }

        public JsonResult LiveStockParts2()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spLiveStockParts2";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
        }

        public JsonResult LiveStockSubs2()
        {
            var partno = Request.Params["PartNo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spGetSubstitusi2";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("partno", partno ?? "");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
        }

        public JsonResult QaFilter()
        {
            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_qaRefFilter";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
        }

        public JsonResult QaTypeByMerk()
        {
            var Merk = Request.Params["MerkCode"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spQaTypeByMerk";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("merkcode", Merk ?? "");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
        }

        public JsonResult QaOccupationDetail()
        {
            var Occ = Request.Params["OccupationCode"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600; cmd.CommandText = "uspfn_spQaOccupationDetail";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("OccupationPart", Occ ?? "");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
        }

        public JsonResult autoFillByChassis()
        {
            var cc = Request.Params["ChassisCode"];
            var cn = Request.Params["ChassisNo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_spGetCustomerDetailsByChassis";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("chassisCode", cc ?? "");
            cmd.Parameters.AddWithValue("chassisNo", cn ?? "");
            cmd.Parameters.AddWithValue("companyCode", CompanyCode ?? "");
            cmd.Parameters.AddWithValue("branchCode", BranchCode ?? "");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
        }

        public JsonResult QaMerkByAsCode()
        {
            var asCode = Request.Params["AsCode"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_getMerkByAsCode";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("ascode", asCode ?? "");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
        }

        public JsonResult QaProdBranchs(string comp = "")
        {
            comp = CompanyCode;
            var list = ctx.Database.SqlQuery<qaBranchModel>("exec uspfn_SpGetBranchesByCompanyCode @companyCode=@p0", comp).AsQueryable().OrderBy(p => p.value);
            return Json(list);
        }

        public JsonResult QaCompanyBranch()
        {
            DataSet com = new DataSet();
            DataTable data = new DataTable();

            data.Columns.Add("CompanyCode", typeof(string));
            data.Columns.Add("BranchCode", typeof(string));
            data.Columns.Add("CompanyName", typeof(string));
            data.Columns.Add("BranchName", typeof(string));
            data.Columns.Add("Area", typeof(string));
            data.Columns.Add("GroupNo", typeof(string));

            var rawData = ctx.DealerGroupMappingViews.AsQueryable();

            string area = "";
            string groupno = "";

            if (!string.IsNullOrEmpty(CompanyCode))
            {
                area = ctx.DealerGroupMappingViews.Where(x => x.CompanyCode == CompanyCode).Select(p => p.Area).FirstOrDefault();
                groupno = ctx.DealerGroupMappingViews.Where(x => x.CompanyCode == CompanyCode).Select(p => p.GroupNo).FirstOrDefault();
            }

            data.Rows.Add(CompanyCode, BranchCode, CompanyName, BranchName, area, groupno);

            com.Tables.Add(data);

            return new LargeJsonResult() { Data = GetJson(com), MaxJsonLength = int.MaxValue };
        }

        public JsonResult QaCompanyBranchV2()
        {
            DataSet com = new DataSet();
            DataTable data = new DataTable();

            data.Columns.Add("CompanyCode", typeof(string));
            data.Columns.Add("Company", typeof(string));
            data.Columns.Add("BranchCode", typeof(string));
            data.Columns.Add("CompanyName", typeof(string));
            data.Columns.Add("BranchName", typeof(string));
            data.Columns.Add("Area", typeof(string));
            data.Columns.Add("GroupNo", typeof(string));

            var rawData = ctx.DealerGroupMappingViews.AsQueryable();

            string area = "";
            string groupno = "";
            string company = "";

            if (!string.IsNullOrEmpty(CompanyCode))
            {
                area = ctx.DealerGroupMappingViews.Where(x => x.CompanyCode == CompanyCode).Select(p => p.Area).FirstOrDefault();
                groupno = ctx.DealerGroupMappingViews.Where(x => x.CompanyCode == CompanyCode).Select(p => p.GroupNo).FirstOrDefault();
                company = ctx.GnMstDealerOutletMappingNews.Where(x => x.DealerCode == CompanyCode && x.OutletCode == BranchCode).Select(p => p.GroupNo + "|" + p.DealerCode).FirstOrDefault();
            }

            data.Rows.Add(CompanyCode,company, BranchCode, CompanyName, BranchName, area, groupno);

            com.Tables.Add(data);

            return new LargeJsonResult() { Data = GetJson(com), MaxJsonLength = int.MaxValue };
        }

        public JsonResult QaMstException()
        {
            var list = ctx.QaMstExceptions.OrderBy(p => p.Event)
                .Select(p => new
                {
                    value = p.Event,
                    text = p.Event
                }).ToList().Distinct();

            return Json(list);
        }

        public JsonResult Qa2Filter()
        {
            var ev = Request.Params["ErtigaOrWagonR"] ?? "";
            var a = Request.Params["StatusKonsumen"];
            var ps = (Request.Params["StatusKonsumen"] ?? "") == "A" ? "Individu" : "Fleet/Perusahaan";

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_qa2RefFilter";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("event", ev ?? "");
            cmd.Parameters.AddWithValue("productsource", ps ?? "");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
        }

        public JsonResult autoFillByChassis2()
        {
            var cc = Request.Params["ChassisCode"];
            var cn = Request.Params["ChassisNo"];

            SqlCommand cmd = ctx.Database.Connection.CreateCommand() as SqlCommand;
            cmd.CommandTimeout = 3600;
            cmd.CommandText = "uspfn_spGetCustomerDetailsByChassis2";
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("chassisCode", cc ?? "");
            cmd.Parameters.AddWithValue("chassisNo", cn ?? "");
            cmd.Parameters.AddWithValue("companyCode", CompanyCode ?? "");
            cmd.Parameters.AddWithValue("branchCode", BranchCode ?? "");

            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            return new LargeJsonResult() { Data = GetJson(ds), MaxJsonLength = int.MaxValue };
        }

        public JsonResult Company()
        {
            var list = ctx.GnMstDealerMappings.OrderBy(p => p.DealerAbbreviation).Where(p => p.isActive == true)
                .Select(p => new
                {
                    value = p.DealerCode,
                    text = p.DealerCode + " - " + p.DealerAbbreviation
                }).ToList();

            return Json(list);
        }

        public JsonResult FilterRevenue()
        {
            List<object> data = new List<object>();
            data.Add(new
            {
                text = "Total Workshop Parts Sales Revenue",
                value = "TotalWorkshopPartRevenue"
            });
            data.Add(new
            {
                text = "Total Counter Part Sales Revenue",
                value = "TotalCounterPartSlsRevenue"
            });
            data.Add(new
            {
                text = "Total Lubricant Sales Revenue",
                value = "TotalRubricantSlsRevenue"
            });
            data.Add(new
            {
                text = "Total Suzuki Genuine Accessories Sales Revenue - Chargeable to Customer CPUS (External)",
                value = "TotalSGASalesRevenue"
            });
            data.Add(new
            {
                text = "SGA Accessories Parts Sales",
                value = "SGAAccessoriesPartSales"
            });
            data.Add(new
            {
                text = "SGA Materials Sales",
                value = "SGAMeterialSales"
            });
            data.Add(new
            {
                text = "Total Sublet Sales Revenue - Chargebale to Customer CPUS (External)",
                value = "TotalSubletSlsRevenue"
            });
            data.Add(new
            {
                text = "Total Service Sales Revenue",
                value = "TotalServiceSlsRevenue"
            });


            return Json(data);
        }

        public JsonResult TipeKendaraan()
        {
            var query = string.Format(@"SELECT GroupModel
                                        FROM msMstGroupModel
                                        GROUP BY GroupModel
                                        union
                                        select 'TTL ERTIGA' GroupModel");
            var queryable = ctx.Database.SqlQuery<ParamValueTipeKendaraan>(query).AsQueryable();
            return Json(queryable.Select(p => new { value = p.GroupModel, text = p.GroupModel.ToUpper() }));

        }
		public JsonResult LoadComboData(string CodeID)
        {
            var trans = ctx.GnMstLookUpDtls
                .Where(x => (x.CompanyCode == CompanyCode && x.CodeID == CodeID))
                .OrderBy(x => x.SeqNo)
                .Select(x => new { value = x.LookUpValue, text = x.LookUpValueName }).ToList();
            return Json(trans, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ListTypeOfGoods()
        {
            var ltog = ctx.GnMstLookUpDtls.Where(p => p.CodeID == "TPGO" && p.CompanyCode == "0000000")
                .Select(x => new { value = x.ParaValue, text = x.LookUpValueName }).ToList();
            return Json(ltog, JsonRequestBehavior.AllowGet);
        }
        public JsonResult ReviewPlans()
        {
            var data = (from x in ctx.CsSettings
                        where
                        x.SettingLink2 != null && x.SettingLink3 != null
                        orderby x.SettingLink2 ascending
                        select new
                        {
                            text = x.SettingLink2.ToUpper(),
                            value = x.SettingLink2.ToUpper()
                        }).Distinct().ToList();

            return Json(data);
        }

        public JsonResult InqSalesModel()
        {

            return Json(ctx.OmMstModels
                .Where(x=>x.GroupCode!=null && x.GroupCode!="")
                .Select(x => new { value = x.GroupCode, text = x.GroupCode }).Distinct()
                );
        }
    }
}
