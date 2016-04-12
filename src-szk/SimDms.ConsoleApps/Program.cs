using Newtonsoft.Json;
using SimDms.ConsoleApps.Models;
using SimDms.SUtility.Controllers;
using SimDms.SUtility.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SimDms.ConsoleApps
{
    static class Program
    {
        static void Main(string[] args)
        {
            var ctx = new DataContext();
            var qry = from p in ctx.SvTrnInvoices
                      join q in ctx.SvTrnServices
                      on new { p.CompanyCode, p.BranchCode, p.JobOrderNo }
                      equals new { q.CompanyCode, q.BranchCode, q.JobOrderNo }
                      where p.BranchCode == "6006406"
                      select new
                      {
                          p.CompanyCode,
                          p.BranchCode,
                          p.InvoiceNo,
                          p.InvoiceDate,
                          p.JobOrderNo,
                          p.JobOrderDate
                      };

            var options = new DataParams
            {
                Take = 10,
                Skip = 250,
                SoftFields = new List<string> { "InvoiceNo", "InvoiceDate" },
                SortDirs = new List<string> { "asc", "desc" },
                FilterFields = new List<string> { "InvoiceNo" },
                FilterValues = new List<string> { "INF/13" }
            };
            qry.GenericTest();

            //Console.WriteLine(kgrid.total);
            //foreach (var item in kgrid.data)
            //{
            //    Console.WriteLine(item.InvoiceNo);
            //}
        }

        public static void GenericTest<T>(this IQueryable<T> query)
        {
            //Console.WriteLine(typeof(T));

            var props = typeof(T).GetProperties();
            Console.WriteLine(props[0].Name);
        }
    }
}
