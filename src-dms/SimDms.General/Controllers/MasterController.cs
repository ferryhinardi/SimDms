using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SimDms.General.Controllers
{
    public class MasterController : BaseController
    {
        //master General
        public string Customer()
        {
            return HtmlRender("master/customer.js");
        }

        public string Customer2()
        {
            return HtmlRender("master/customer2.js");
        }

        public string Supplier()
        {
            return HtmlRender("master/supplier.js");
        }

        public string Organisasi()
        {
            return HtmlRender("master/organisasi.js");
        }

        public string CompanyProfile()
        {
            return HtmlRender("master/companyprofile.js");
        }

        public string CustomerClass()
        {
            return HtmlRender("master/customerclass.js");
        }

        public string SupplierClass()
        {
            return HtmlRender("master/supplierclass.js");
        }

        public string CustomerAdmin()
        {
            return HtmlRender("master/customeradmin.js");
        }

        public string ZipCode()
        {
            return HtmlRender("master/zipcode.js");
        }

        public string FPJSignDate()
        {
            return HtmlRender("master/fpjsigndate.js");
        }

        public string MasterMSGBoards()
        {
            return HtmlRender("master/mastermsgboards.js");
        }

        public string Karyawan()
        {
            return HtmlRender("master/karyawan.js");
        }

        public string Pajak() 
        {
            return HtmlRender("master/pajak.js");
        }

        public string Kolektor()
        {
            return HtmlRender("master/kolektor.js");
        }

        public string Dokumen()
        {
            return HtmlRender("master/dokumen.js");
        }

        public string Employee()
        {
            return HtmlRender("master/employee.js");
        }

        public string EmployeeAbsence()
        {
            return HtmlRender("master/employeeabsence.js");
        }

        public string DocumentSign()
        {
            return HtmlRender("master/documentsign.js");
        }

        public string FPJSignature()
        {
            return HtmlRender("master/fpjsignature.js");
        }

        public string MasterLookup()
        {
            return HtmlRender("master/masterlookup.js");
        }

        public string MasterParameter()
        {
            return HtmlRender("master/masterparameter.js");
        }

        public string MessageBoards()
        {
            return HtmlRender("master/messageboards.js");
        }

        public string MasterCalender()
        { 
            return HtmlRender("master/calender.js");
        }

        public string Faktur()
        {
            return HtmlRender("master/faktur.js");
        }

        public string employeemutation() 
        {
            return HtmlRender("master/employeemutation.js");
        }

        public string addholiday()
        {
            return HtmlRender("master/addholiday.js");
        }

        public string addholidays()
        {
            return HtmlRender("master/addholidays.js");
        }

        //master Finance
        public string SegmentChart()
        {
            return HtmlRender("master/segmentchart.js");
        }

        public string Chart()
        {
            return HtmlRender("master/chart.js");
        }

        public string Periode()
        {
            return HtmlRender("master/periode.js");
        }

        public string InqCreditLimit()
        {
            return HtmlRender("master/inqcreditlimit.js");
        }

        public string Reminder()
        {
            return HtmlRender("master/reminder.js"); 
        }

        public string Approval()
        {
            return HtmlRender("master/approval.js");
        }

        public string SetSecurityReport()
        {
            return HtmlRender("master/SetSecurityReport.js");
        }
    }
}
