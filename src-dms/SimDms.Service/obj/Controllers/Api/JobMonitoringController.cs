using SimDms.Service.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.Service.Controllers.Api
{
    public class JobMonitoringController : BaseController
    {
        public ActionResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                BranchCode = BranchCode,
                ProcessDate = DateTime.Now,
            });
        }

        public JsonResult ProcessInquiry(InqGetParams paras)
        {
            try
            {
                var data = GetInquiries(paras);
                return Json(new { message = "", data = data });
            }
            catch (Exception ex)
            {
                return Json(new { message = ex.Message });
            }
        }

        private IEnumerable<InqJobMonitoring> GetInquiries(InqGetParams paras)
        {
            var jobOrderDate = paras.jobOrderDate;
            var showOutstdSpk = paras.showOutstdSpk;
            var text = paras.text ?? "";
            var fieldName = paras.fieldName ?? "";
            var RTN = paras.RTN ?? "";
            var OTH = paras.OTH ?? "";
            var PDI = paras.PDI ?? "";
            var FSC = paras.FSC ?? "";
            var CLM = paras.CLM ?? "";
            var BDR = paras.BDR ?? "";

            #region -- Query --
            var sql = @"
select 
       row_number() over(order by JobOrderNo) AS [No]
    ,a.JobOrderNo
    ,substring(a.JobOrderNo, 5, len(a.JobOrderNo)) JobOrderNoCust
    ,a.PoliceRegNo
    --,c.Description BasicModel
    ,a.BasicModel
    ,a.JobType
    ,case convert(varchar,a.EstimateFinishDate,112) when '19000101' then null else a.EstimateFinishDate end EstimateFinishDate
    ,case convert(varchar,a.EstimateFinishDate,112) when '19000101' then null else a.EstimateFinishDate end EstimateFinishTime
    ,case when DATEDIFF(day, getDate(), a.EstimateFinishDate) < 0 then '-'
	    when DAY(a.EstimateFinishDate) = DAY(getDate()) then ('Hari ini, ' + substring(Convert(varchar,a.EstimateFinishDate, 24), 1, 5))
	    when DAY(a.EstimateFinishDate) = DAY(DATEADD(DAY, 1, getDate()  )) then ('Besok, '+ substring(Convert(varchar,a.EstimateFinishDate, 24), 1, 5))
	    when  DATEDIFF(day, getDate(), a.EstimateFinishDate) < DAY(DATEADD(DAY, 1, getDate())) then (CAST(DATEDIFF(DAY, getDate(), a.EstimateFinishDate) AS VARCHAR(5)) + ' Hari, ' + substring(Convert(varchar,a.EstimateFinishDate, 24), 1, 5))
	    end as EstimateFinish 
    ,d.EmployeeName ForemanID
    ,e.EmployeeName MechanicID
    ,b.Description 
    ,a.ServiceStatus
    from svTrnService a with(nolock, nowait)
    left join svMstRefferenceService b on b.CompanyCode = a.CompanyCode 
	    and	b.RefferenceCode = a.ServiceStatus and b.ProductType = a.ProductType
	    and	b.RefferenceType = 'SERVSTAS' 
    left join svMstRefferenceService c on c.CompanyCode = a.CompanyCode 
	    and	c.RefferenceCode = a.BasicModel and c.ProductType = a.ProductType
	    and	c.RefferenceType = 'BASMODEL' 
    left join GnMstEmployee d on a.CompanyCode = d.CompanyCode 
	    and d.BranchCode = a.BranchCode and d.EmployeeID = a.ForemanID 
    left join GnMstEmployee e on a.CompanyCode = e.CompanyCode 
	    and e.BranchCode = a.BranchCode and e.EmployeeID  = a.MechanicID 
    left join svMstJob f on f.CompanyCode = a.CompanyCode
		and f.ProductType = a.ProductType
		and f.BasicModel = a.BasicModel
		and f.JobType = a.JobType
   where a.CompanyCode= '{0}' and 
         a.BranchCode= '{1}' and
         a.ProductType = '{2}'";
            #endregion
            
            #region -- Query Extensions --
            if (RTN != string.Empty || OTH != string.Empty || PDI != string.Empty || FSC != string.Empty || CLM != string.Empty || BDR != string.Empty)
            {
                var queryRTN = "(case when '{5}' = '' then '' else f.GroupjobType end) = '{5}'";
                var queryOTH = "(case when '{6}' = '' then '' else f.GroupjobType end) = '{6}'";
                var queryPDI = "(case when '{7}' = '' then '' else f.GroupjobType end) = '{7}'";
                var queryFSC = "(case when '{8}' = '' then '' else f.GroupjobType end) = '{8}'";
                var queryCLM = "(case when '{9}' = '' then '' else f.GroupjobType end) = '{9}'";
                var queryBDR = "((case when '{10}' = '' then '' else f.GroupjobType end) = '{10}' or (case when '{10}' = '' then '' else f.GroupjobType end) like 'BR%')";

                sql += " and (";
                if (RTN != string.Empty)
                {
                    sql += queryRTN;
                }
                if (OTH != string.Empty)
                {
                    if (RTN != string.Empty)
                        sql += " or ";
                    sql += queryOTH;
                }
                if (PDI != string.Empty)
                {
                    if (RTN != string.Empty || OTH != string.Empty)
                        sql += " or ";
                    sql += queryPDI;
                }
                if (FSC != string.Empty)
                {
                    if (RTN != string.Empty || OTH != string.Empty || PDI != string.Empty)
                        sql += " or ";
                    sql += queryFSC;
                }
                if (CLM != string.Empty)
                {
                    if (RTN != string.Empty || OTH != string.Empty || PDI != string.Empty || FSC != string.Empty)
                        sql += " or ";
                    sql += queryCLM;
                }
                if (BDR != string.Empty)
                {
                    if (RTN != string.Empty || OTH != string.Empty || PDI != string.Empty || FSC != string.Empty || CLM != string.Empty)
                        sql += " or ";
                    sql += queryBDR;
                }

                sql += ")";
            }

            var p01 = " and a.ServiceType=2";
            if (showOutstdSpk)
            {
                p01 = string.Format(" {0} and a.ServiceStatus in (0,1,2,3,4,5)", p01);
            }
            else
            {
                p01 = " " + p01 + " and a.JobOrderDate >= '{3}' and a.JobOrderDate < '{4}'";
            }

            var p02 = string.Empty;
            if (text.Trim().Length > 0 && fieldName.Trim().Length > 0)
            {
                var value = text.Trim().Replace("*", "%").Replace(" ", "%").Replace("'", "");
                if (value.Length > 0 && !(value.StartsWith("%") || value.EndsWith("_")))
                {
                    value = "%" + value + "%";
                }

                if (fieldName == "ForemanID")
                {
                    p02 = string.Format(" and (a.ForemanID like '{0}' or d.EmployeeName like '{0}')", value);
                }
                else if (fieldName == "MechanicID")
                {
                    p02 = string.Format(" and (a.MechanicID like '{0}' or e.EmployeeName like '{0}')", value);
                }
                else
                {
                    p02 = string.Format(" and {0} like '{1}'", fieldName, value);
                }
            } 
            #endregion

            var query = string.Format(" {0}{1}{2}", sql, p01, p02);
            var finalQuery = string.Format(query, CompanyCode, BranchCode, ProductType, jobOrderDate, 
                jobOrderDate.AddDays(1), RTN, OTH, PDI, FSC, CLM, BDR);
            var data = ctx.Database.SqlQuery<InqJobMonitoring>(finalQuery);
            return data;
        }

        public class InqGetParams
        {
            public DateTime jobOrderDate { get; set; }
            public bool showOutstdSpk { get; set; }
            public string text { get; set; }
            public string fieldName { get; set; }
            public string RTN { get; set; }
            public string OTH { get; set; }
            public string PDI { get; set; }
            public string FSC { get; set; }
            public string CLM { get; set; }
            public string BDR { get; set; }
        }

        public class InqJobMonitoring
        {
            public long? No { get; set; }
            public string JobOrderNo { get; set; }
            public string JobOrderNoCust { get; set; }
            public string PoliceRegNo { get; set; }
            public string BasicModel { get; set; }
            public string JobType { get; set; }
            public DateTime? EstimateFinishDate { get; set; }
            public DateTime? EstimateFinishTime { get; set; }
            public string EstimateFinish { get; set; }
            public string ForemanID { get; set; }
            public string MechanicID { get; set; }
            public string Description { get; set; }
            public string ServiceStatus { get; set; }
        }
    }
}
