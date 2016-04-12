using SimDms.CStatisfication.Models;
using SimDms.CStatisfication.Models.General;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.CStatisfication.Controllers.Api
{
    public class CustBirthdayController : BaseController
    {
        [HttpPost]
        public JsonResult Default()
        {
            return Json(new
            {
                CompanyCode = CompanyCode,
                CompanyName = CompanyName,
                BranchCode = BranchCode,
                BranchName = BranchName,
            });
        }

        [HttpPost]
        public JsonResult Get(string CustomerCode)
        {
            var data = (from x in ctx.CsLkuBirthdayViews
                        where
                        x.CustomerCode.Equals(CustomerCode) == true
                        select x).FirstOrDefault();

            ResultModel result = new ResultModel();
            if (data != null)
            {
                result.success = true;
                result.data = data;
            }
            else
            {
                result.success = false;
                result.data = null;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult Save(CustomerBirthday model)
        //{
        //    bool isHasSpouse = Convert.ToBoolean(Request["CustomerIsMarried"] ?? "false");
        //    int numberOfChildren = Convert.ToInt32(Request["NumberOfChildren"] ?? "0");
        //    string userID = CurrentUser.UserId;
        //    CsCustRelation[] childs = new CsCustRelation[3];
        //    ResultModel result = InitializeResultModel();
        //    DateTime currentDateTime = DateTime.Now;

        //    var customerData = ctx.Customers.Where(x => x.CompanyCode==model.CompanyCode && x.CustomerCode==model.CustomerCode).FirstOrDefault();
        //    var customerBirthday = ctx.CsCustBirthDays.Where(x => x.CompanyCode == model.CompanyCode && x.CustomerCode == model.CustomerCode && x.PeriodYear==currentDateTime.Year).FirstOrDefault();
        //    var parent =  ctx.CsCustRelations.Where(x => x.CustomerCode.Equals(model.CustomerCode) == true && x.RelationType.Equals("PARENT") == true).FirstOrDefault();
        //    var spouse = ctx.CsCustRelations.Where(x => x.CustomerCode.Equals(model.CustomerCode) == true && x.RelationType.Equals("SPOUSE") == true).FirstOrDefault();
        //    var child1 = ctx.CsCustRelations.Where(x => x.CustomerCode.Equals(model.CustomerCode) == true && x.RelationType.Equals("CHILD_1") == true).FirstOrDefault();
        //    var child2 = ctx.CsCustRelations.Where(x => x.CustomerCode.Equals(model.CustomerCode) == true && x.RelationType.Equals("CHILD_2") == true).FirstOrDefault();
        //    var child3 = ctx.CsCustRelations.Where(x => x.CustomerCode.Equals(model.CustomerCode) == true && x.RelationType.Equals("CHILD_3") == true).FirstOrDefault();

        //    childs[0] = child1;
        //    childs[1] = child2;
        //    childs[2] = child3;

        //    if (customerBirthday == null)
        //    {
        //        customerBirthday = new CsCustBirthDay();
        //        customerBirthday.CompanyCode = model.CompanyCode;
        //        customerBirthday.CustomerCode = model.CustomerCode;
        //        customerBirthday.CreatedBy = userID;
        //        customerBirthday.CreatedDate = currentDateTime;
        //        customerBirthday.PeriodYear = currentDateTime.Year;
        //        ctx.CsCustBirthDays.Add(customerBirthday);
        //    }

        //    customerBirthday.Comment = model.CustomerComment;
        //    customerBirthday.TypeOfGift = model.CustomerTypeOfGift;
        //    customerBirthday.SentGiftDate = model.CustomerGiftSentDate;
        //    customerBirthday.ReceivedGiftDate = model.CustomerGiftReceivedDate;
        //    customerBirthday.Comment = model.CustomerComment;
        //    customerBirthday.AdditionalInquiries = model.AdditionalInquiries;
        //    customerBirthday.UpdatedBy = userID;
        //    customerBirthday.UpdatedDate = currentDateTime;
        //    if (isHasSpouse)
        //    {
        //        customerBirthday.Status = true;
        //    }
        //    else
        //    {
        //        customerBirthday.Status = false;
        //    }

        //    if (isHasSpouse)
        //    {
        //        //Spouse Data
        //        if (spouse == null)
        //        {
        //            spouse = new CsCustRelation();
        //            spouse.CompanyCode = model.CompanyCode;
        //            spouse.CustomerCode = model.CustomerCode;
        //            spouse.RelationType = "SPOUSE";
        //            ctx.CsCustRelations.Add(spouse);
        //        }
        //        spouse.FullName = model.SpouseName;
        //        spouse.PhoneNo = model.SpouseTelephone;
        //        spouse.BirthDate = model.SpouseBirthDate;
        //        spouse.TypeOfGift = model.SpouseTypeOfGift;
        //        spouse.SentGiftDate = model.SpouseGiftSentDate;
        //        spouse.ReceivedGiftDate = model.SpouseGiftReceivedDate;
        //        spouse.RelationInfo = model.SpouseRelation;
        //        spouse.Comment = model.SpouseComment;
        //        spouse.CreatedBy = userID;
        //        spouse.CreatedDate = currentDateTime;
        //        spouse.UpdatedBy = userID;
        //        spouse.UpdatedDate = currentDateTime;  


        //        //Child data
        //        for (int i = 0; i < childs.Length; i++)
        //        {
        //            if ((i + 1) > numberOfChildren)
        //            {
        //                if (childs[i] != null)
        //                {
        //                    ctx.CsCustRelations.Remove(childs[i]);
        //                }
        //            }
        //        }

        //        if (numberOfChildren >= 1)
        //        {
        //            //Child 1
        //            if (child1 == null)
        //            {
        //                child1 = new CsCustRelation();
        //                child1.CompanyCode = DealerInfo().CompanyCode;
        //                child1.CustomerCode = model.CustomerCode;
        //                child1.RelationType = "CHILD_1";
        //                ctx.CsCustRelations.Add(child1);
        //            }
        //            child1.FullName = model.ChildName1;
        //            child1.PhoneNo = model.ChildTelephone1;
        //            child1.BirthDate = model.ChildBirthDate1;
        //            child1.TypeOfGift = model.ChildTypeOfGift1;
        //            child1.SentGiftDate = model.ChildGiftSentDate1;
        //            child1.ReceivedGiftDate = model.ChildGiftReceivedDate1;
        //            child1.RelationInfo = "First Child";
        //            child1.Comment = model.ChildComment1;
        //            child1.CreatedBy = userID;
        //            child1.CreatedDate = currentDateTime;
        //            child1.UpdatedBy = userID;
        //            child1.UpdatedDate = currentDateTime;
        //        }

        //        if (numberOfChildren >= 2)
        //        {
        //            //Child 2
        //            if (child2 == null)
        //            {
        //                child2 = new CsCustRelation();
        //                child2.CompanyCode = model.CompanyCode;
        //                child2.CustomerCode = model.CustomerCode;
        //                child2.RelationType = "CHILD_2";
        //                ctx.CsCustRelations.Add(child2);
        //            }

        //            child2.FullName = model.ChildName2;
        //            child2.PhoneNo = model.ChildTelephone2;
        //            child2.TypeOfGift = model.ChildTypeOfGift2;
        //            child2.SentGiftDate = model.ChildGiftSentDate2;
        //            child2.ReceivedGiftDate = model.ChildGiftReceivedDate2;
        //            child2.BirthDate = model.ChildBirthDate2;
        //            child2.RelationInfo = "Second Child";
        //            child2.Comment = model.ChildComment2;
        //            child2.CreatedBy = userID;
        //            child2.CreatedDate = currentDateTime;
        //            child2.UpdatedBy = userID;
        //            child2.UpdatedDate = currentDateTime; 
        //        }


        //        if (numberOfChildren >= 3)
        //        {
        //            //Child 3
        //            if (child3 == null)
        //            {
        //                child3 = new CsCustRelation();
        //                child3.CompanyCode = model.CompanyCode;
        //                child3.CustomerCode = model.CustomerCode;
        //                child3.RelationType = "CHILD_3";
        //                ctx.CsCustRelations.Add(child3);
        //            }

        //            child3.FullName = model.ChildName3;
        //            child3.PhoneNo = model.ChildTelephone3;
        //            child3.TypeOfGift = model.ChildTypeOfGift3;
        //            child3.SentGiftDate = model.ChildGiftSentDate3;
        //            child3.ReceivedGiftDate = model.ChildGiftReceivedDate3;
        //            child3.BirthDate = model.ChildBirthDate3;
        //            child3.Comment = model.ChildComment3;
        //            child3.CreatedBy = userID;
        //            child3.RelationInfo = "Third Child";
        //            child3.CreatedDate = currentDateTime;
        //            child3.UpdatedBy = userID;
        //            child3.UpdatedDate = currentDateTime; 
        //        }
        //    }
        //    else
        //    {
        //        if (spouse != null)
        //        {
        //            ctx.CsCustRelations.Remove(spouse);
        //        }
        //        for (int i = 0; i < 3; i++)
        //        {
        //            if (childs[i] != null)
        //            {
        //                if (childs[i] != null)
        //                {
        //                    ctx.CsCustRelations.Remove(childs[i]);
        //                }
        //            }
        //        }
        //    }

        //    try
        //    {
        //        ctx.SaveChanges();
        //        result.success = true;
        //        result.message = "Customer Birthday's data saved to database.";

        //        ctx.Database.ExecuteSqlCommand("exec uspfn_CRUDCustBdayResource @CompanyCode=@p0, @CustomerCode=@p1", model.CompanyCode, model.CustomerCode);

        //        if (IsHasRelation(model.CustomerCode) == true)
        //        {
        //            result.data = new
        //            {
        //                hasRelation = true
        //            };
        //        }
        //        result.data = new { 
        //            hasRelation = customerBirthday.Status
        //        };
        //    }
        //    catch (Exception)
        //    {
        //        result.success = false;
        //        result.message = "Customer Birthday's data cannot be saved.";
        //    }

        //    return Json(result, JsonRequestBehavior.AllowGet);
        //}

        private bool IsHasRelation(string customerCode)
        {
            int numberOfRelation = (from x in ctx.CsCustRelations
                                    where
                                    x.CustomerCode.Equals(customerCode) == true
                                    select x).Count();

            if (numberOfRelation > 0)
            {
                return true;
            }

            return false;
        }

        public JsonResult Delete()
        {
            ResultModel result = InitializeResultModel();
            string customerCode = Request["CustomerCode"] ?? "";
            string companyCode = Request["CompanyCode"] ?? "";

            var data = (from x in ctx.CsCustBirthDays
                        where
                        x.CompanyCode == companyCode
                        &&
                        x.CustomerCode == customerCode
                        select
                        x).FirstOrDefault();

            if (data != null)
            {
                ctx.CsCustBirthDays.Remove(data);
            }
            
            try {
                ctx.SaveChanges();

                result.success = true;
                result.message = "Customer Birthday's record has been deleted.";

                //ctx.Database.ExecuteSqlCommand("exec uspfn_CRUDCustBdayResource @CompanyCode=@p0, @CustomerCode=@p1", companyCode, customerCode);
            }
            catch(Exception) {
                result.message = "Cannot delete Customer BirthDay's record from database.";
            }

            return Json(result);
        }

        public JsonResult Customers(int paramException = 0)
        {
            IQueryable<CustomerView> queryable;

            switch (paramException)
            {
                case 0: //All customer data showed
                    queryable = ctx.CustomerViews.Where(p => p.CustomerName != "");
                    return Json(GeLang.DataTables<CustomerView>.Parse(queryable, Request));
                case 1: //Showing customer data that not exist in CsCustBirthDay
                    IQueryable<string> customerCode = ctx.CsCustBirthDays.Select(x => x.CustomerCode);
                    queryable = ctx.CustomerViews.Where(p =>
                        p.CustomerName != "" &&
                        p.CustomerName != null &&
                        !customerCode.Contains(p.CustomerCode)
                    );
                    return Json(GeLang.DataTables<CustomerView>.Parse(queryable, Request));
            }

            return null;
        }

        //public JsonResult CustomerBirthDays(string CustomerCode)
        //{
        //    var queryable = ctx.CustomerBirthdays.Where(x => x.CompanyCode == CompanyCode);
        //    return Json(GeLang.DataTables<CustomerBirthday>.Parse(queryable, Request));
        //}

        //private void AddCustomerRelation(CustomerBirthday model)
        //{
               
        //}

        [HttpPost]
        public JsonResult Save(CustomerBirthday model)
        {
            bool isHasSpouse = Convert.ToBoolean(Request["CustomerIsMarried"] ?? "false");
            int numberOfChildren = 0;

            try
            {
                numberOfChildren = Convert.ToInt32(Request["NumberOfChildren"] ?? "0");
            }
            catch (Exception) { }

            string userID = CurrentUser.UserId;
            CsCustRelation[] childs = new CsCustRelation[3];
            ResultModel result = InitializeResultModel();
            DateTime currentDateTime = DateTime.Now;

            var customerData = ctx.Customers.Where(x => x.CompanyCode == model.CompanyCode && x.CustomerCode == model.CustomerCode).FirstOrDefault();
            var customerBirthday = ctx.CsCustBirthDays.Where(x => x.CompanyCode == model.CompanyCode && x.CustomerCode == model.CustomerCode && x.PeriodYear == currentDateTime.Year).FirstOrDefault();

            if (customerBirthday == null)
            {
                customerBirthday = new CsCustBirthDay();
                customerBirthday.CompanyCode = model.CompanyCode;
                customerBirthday.CustomerCode = model.CustomerCode;
                customerBirthday.CreatedBy = userID;
                customerBirthday.CreatedDate = currentDateTime;
                customerBirthday.PeriodYear = currentDateTime.Year;
                ctx.CsCustBirthDays.Add(customerBirthday);
            }

            customerBirthday.Comment = model.CustomerComment;
            customerBirthday.TypeOfGift = model.CustomerTypeOfGift;
            customerBirthday.SentGiftDate = model.CustomerGiftSentDate;
            customerBirthday.ReceivedGiftDate = model.CustomerGiftReceivedDate;
            customerBirthday.Comment = model.CustomerComment;
            customerBirthday.AdditionalInquiries = model.AdditionalInquiries;
            customerBirthday.UpdatedBy = userID;
            customerBirthday.UpdatedDate = currentDateTime;
            customerBirthday.Status = model.Status;
            customerBirthday.Reason = model.Reason;

            if (customerBirthday.Status == true)
            {
                customerBirthday.Reason = null;
            }
            else
            {
                customerBirthday.Reason = model.Reason;
            }
            //if (isHasSpouse)
            //{
            //    customerBirthday.Status = true;
            //}
            //else
            //{
            //    customerBirthday.Status = false;
            //}


            customerBirthday.SpouseName = model.SpouseName;
            customerBirthday.SpouseTelephone = model.SpouseTelephone;
            customerBirthday.SpouseBirthday = model.SpouseBirthDate;
            customerBirthday.SpouseTypeOfGift = model.SpouseTypeOfGift;
            customerBirthday.SpouseGiftSentDate = model.SpouseGiftSentDate;
            customerBirthday.SpouseComment = model.SpouseComment;
            customerBirthday.SpouseGiftReceivedDate = model.SpouseGiftReceivedDate;
            customerBirthday.SpouseRelation = model.SpouseRelation;

            customerBirthday.ChildName1 = model.ChildName1;
            customerBirthday.ChildBirthday1 = model.ChildBirthDate1;
            customerBirthday.ChildTypeOfGift1 = model.ChildTypeOfGift1;
            customerBirthday.ChildGiftSentDate1 = model.ChildGiftSentDate1;
            customerBirthday.ChildComment1 = model.ChildComment1;
            customerBirthday.ChildGiftReceivedDate1 = model.ChildGiftReceivedDate1;
            customerBirthday.ChildRelation1 = model.ChildRelation1;
            customerBirthday.ChildTelephone1 = model.ChildTelephone1;

            customerBirthday.ChildName2 = model.ChildName2;
            customerBirthday.ChildBirthday2 = model.ChildBirthDate2;
            customerBirthday.ChildTypeOfGift2 = model.ChildTypeOfGift2;
            customerBirthday.ChildGiftSentDate2 = model.ChildGiftSentDate2;
            customerBirthday.ChildComment2 = model.ChildComment2;
            customerBirthday.ChildGiftReceivedDate2 = model.ChildGiftReceivedDate2;
            customerBirthday.ChildRelation2 = model.ChildRelation2;
            customerBirthday.ChildTelephone2 = model.ChildTelephone2;

            customerBirthday.ChildName3 = model.ChildName3;
            customerBirthday.ChildBirthday3 = model.ChildBirthDate3;
            customerBirthday.ChildTypeOfGift3 = model.ChildTypeOfGift3;
            customerBirthday.ChildGiftSentDate3 = model.ChildGiftSentDate3;
            customerBirthday.ChildComment3 = model.ChildComment3;
            customerBirthday.ChildGiftReceivedDate3 = model.ChildGiftReceivedDate3;
            customerBirthday.ChildRelation3 = model.ChildRelation3;
            customerBirthday.ChildTelephone3 = model.ChildTelephone3;
                
            try
            {
                ctx.SaveChanges();
                result.success = true;
                result.message = "Customer Birthday's data saved to database.";

                //if (IsHasRelation(model.CustomerCode) == true)
                //{
                //    result.data = new
                //    {
                //        hasRelation = true
                //    };
                //}
                result.data = new
                {
                    Reason = model.Reason,
                    Finish = model.Status,
                    hasRelation = customerBirthday.Status
                };
            }
            catch (Exception)
            {
                result.success = false;
                result.message = "Customer Birthday's data cannot be saved.";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}
