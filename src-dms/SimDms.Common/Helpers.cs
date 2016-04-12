using SimDms.Common.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;

namespace SimDms.Common
{
    public static class Helpers
    {
        public static void ReplaceNullable(object obj)
        {
            foreach (var propInfo in obj.GetType().GetProperties())
            {
                if (propInfo.GetValue(obj, null) == null)
                {
                    var type = propInfo.PropertyType;
                    var nullableType = Nullable.GetUnderlyingType(propInfo.PropertyType);
                    switch (Type.GetTypeCode(nullableType != null ? nullableType : type))
                    {
                        case TypeCode.Decimal:
                        case TypeCode.Double:
                        case TypeCode.Int16:
                        case TypeCode.Int32:
                        case TypeCode.Int64:
                            propInfo.SetValue(obj, Convert.ChangeType(0, nullableType != null ? nullableType : type), null);
                            break;
                        case TypeCode.String:
                            propInfo.SetValue(obj, Convert.ChangeType(string.Empty, type), null);
                            break;
                        case TypeCode.Char:
                            propInfo.SetValue(obj, Convert.ChangeType(char.Parse(string.Empty), type), null);
                            break;
                        case TypeCode.DateTime:
                            propInfo.SetValue(obj, Convert.ChangeType("1900/01/01", nullableType != null ? nullableType : type), null);
                            break;
                        case TypeCode.Boolean:
                            propInfo.SetValue(obj, Convert.ChangeType(false, nullableType != null ? nullableType : type), null);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    var type = propInfo.PropertyType;
                    var nullableType = Nullable.GetUnderlyingType(propInfo.PropertyType);
                    switch (Type.GetTypeCode(nullableType != null ? nullableType : type))
                    {
                        case TypeCode.String:
                            propInfo.SetValue(obj, Convert.ChangeType(propInfo.GetValue(obj, null).ToString().ToUpper(), type), null);
                            //propInfo.GetValue(obj,null).ToString().ToUpper();
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        public static DateTime StartOfMonth()
        {
            DateTime today = DateTime.Today;
            return new DateTime(today.Year, today.Month, 1);
        }

        public static DateTime EndOfMonth()
        {
            DateTime today = DateTime.Today;
            int dayInMonth = DateTime.DaysInMonth(today.Year, today.Month);

            return new DateTime(today.Year, today.Month, dayInMonth);
        }

        /// <summary>
        /// To use with EPPlus (number starts from 1)
        /// </summary>
        /// <param name="columnNumber"></param>
        /// <returns></returns>
        public static string ExcelColumnNameFromNumber(int columnNumber)
        {
            int dividend = columnNumber;
            string columnName = String.Empty;
            int modulo;

            while (dividend > 0)
            {
                modulo = (dividend - 1) % 26;
                columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
                dividend = (int)((dividend - modulo) / 26);
            }

            return columnName;
        }

        /// <summary>
        /// To use with EPPlus (number starts from 1)
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int ExcelColumnNameToNumber(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");

            columnName = columnName.ToUpperInvariant();

            int sum = 0;

            for (int i = 0; i < columnName.Length; i++)
            {
                sum *= 26;
                sum += (columnName[i] - 'A' + 1);
            }

            return sum;
        }

        public static void ReportError(Exception ex)
        {
            Elmah.ErrorSignal.FromCurrentContext().Raise(ex);
        }

        public static DataTable ToDataTable<T>(this List<T> items)
        {

            DataTable dataTable = new DataTable(typeof(T).Name);

            //Get all the properties

            PropertyInfo[] Props = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo prop in Props)
            {

                //Setting column names as Property names

                dataTable.Columns.Add(prop.Name);

            }

            foreach (T item in items)
            {

                var values = new object[Props.Length];

                for (int i = 0; i < Props.Length; i++)
                {

                    //inserting property values to datatable rows

                    values[i] = Props[i].GetValue(item, null);

                }

                dataTable.Rows.Add(values);

            }

            //put a breakpoint here and check datatable

            return dataTable;

        }

        public static DateTime ChangeTime(this DateTime dateTime, int hours, int minutes, int seconds, int milliseconds, long tick)
        {
            //dateTime = dateTime.AddTicks(tick);

            return new DateTime(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                hours,
                minutes,
                seconds,
                milliseconds,
                dateTime.Kind);
        }

        public static Dictionary<int, string> GetOverLengthInputCollections(object model, string tableName){
            var dic = new Dictionary<int, string>();
            using(var ctx = new CommonContext()){
                var sql = string.Format(@"SELECT column_name as ColumnName, data_type as DataType, 
                    character_maximum_length as MaxLength FROM information_schema.columns WHERE table_name ='{0}'", tableName);

                var i = 1;
                var recs = ctx.Database.SqlQuery<TableProperties>(sql).ToList();
                foreach (var propInfo in model.GetType().GetProperties())
                {
                    var val = propInfo.GetValue(model, null);
                    if ( val != null)
                    {
                        var rec = recs.Where(x => x.ColumnName.Equals(propInfo.Name, StringComparison.InvariantCultureIgnoreCase)).FirstOrDefault();
                        if (rec != null)
                        {
                            var length = val.ToString().Length;
                            var maxLength = rec.MaxLength;
                            if (length > maxLength)
                            {
                                dic.Add(i, "ColumnName: '" + rec.ColumnName + "', MaxLength: '" 
                                    + rec.MaxLength + "', Input Value: '" + val + "', Input Length: '" + length + "'");
                                i++;
                            }
                        }
                        rec = null;
                    }
                }
                recs = null;
            }

            return dic;
        }

        public static string GetDFilter(string prmString)
        {
            var field = "";
            var value = "";

            string dynamicFilter = "";
            string filter = "";
            string[] str;

            string[] tokens = (prmString.Substring(prmString.IndexOf("["), prmString.Length - prmString.IndexOf("["))).Split(new[] { " and " }, StringSplitOptions.None);

            if (tokens.Length > 1)
            {
                for (int i = 0; i < tokens.Length; i++)
                {
                    int pFrom = (tokens[i].Substring(1, tokens[i].Length - 1)).IndexOf("[") + "[".Length;
                    int pTo = (tokens[i].Substring(1, tokens[i].Length - 9)).LastIndexOf("]");

                    str = ((tokens[i].Substring(1, tokens[i].Length - 1)).Substring(pFrom, pTo - pFrom)).Split(',');

                    field = str[1].ToString();
                    value = (str[0].ToString()).Substring(1, (str[0].ToString()).Length - 2).ToUpper();

                    dynamicFilter += value != "" ? " AND " + field + " LIKE ''%" + value + "%''" : "";
                }
            }
            else
            {
                int pFrom = prmString.IndexOf("[") + "[".Length;
                int pTo = prmString.LastIndexOf("]");

                str = (prmString.Substring(pFrom, pTo - pFrom)).Split(',');
                field = str[1].ToString();
                value = (str[0].ToString()).Substring(1, (str[0].ToString()).Length - 2).ToUpper();

                dynamicFilter += value != "" ? " AND " + field + " LIKE ''%" + value + "%''" : "";
            }
            return dynamicFilter;
        }

        public static string GetDynamicFilter(System.Web.HttpRequestBase Request){
            string dynamicFilter = "";
            var field = "";
            var value = "";

            string[] exclude = "take,skip,page,pageSize,cmbField,txtFilter,cmbTop,sort[".Split(',');
            var customFilters = Array.FindAll(Request.Form.AllKeys, x => !exclude.Contains(x) && x.IndexOf("[") == -1);
            var filterLength = Array.FindAll(Request.Form.AllKeys,
                x => x.StartsWith("filter[filters]", StringComparison.InvariantCultureIgnoreCase)).Length / 3;

            foreach (string key in customFilters)
            {
                value = Request[key] != null ? Request[key].ToString() : "";

                if (dynamicFilter == "")
                {
                    dynamicFilter += value != "" ? " AND " + key + " LIKE ''%" + value + "%'" : "";
                }
                else
                {
                    dynamicFilter += value != "" ? "' AND " + key + " LIKE ''%" + value + "%'" : "";
                }
            }

            for (int i = 0; i < filterLength; i++)
            {
                field = Request["filter[filters][" + i + "][field]"] ?? "";
                value = Request["filter[filters][" + i + "][value]"] ?? "";

                if (dynamicFilter == "")
                {
                    dynamicFilter += value != "" ? " AND " + field + " LIKE ''%" + value + "%'" : "";
                }
                else
                {
                    dynamicFilter += value != "" ? "' AND " + field + " LIKE ''%" + value + "%'" : "";
                }
            }

            dynamicFilter = dynamicFilter != "" ? dynamicFilter += "'" : "";

            return dynamicFilter;
        }
        public static string GetURL(string companyCode, string companyCodeTo)
        {
            using(var ctx = new CommonContext()){
                var sql = string.Format(@"SELECT UrlAddress Url FROM omMstCompanyAccount 
                    WHERE CompanyCode = '{0}' and companycodeto = '{1}'", companyCode, companyCodeTo);

                var rec = ctx.Database.SqlQuery<string>(sql).AsQueryable();

                return rec.FirstOrDefault().ToString();
            }
        }
    }

    public class TableProperties
    {
        public string ColumnName { get; set; }
        public string DataType { get; set; }
        public int? MaxLength { get; set; }
    }

    public enum ApplyFilterKendoGrid
    {
        True,
        False
    }
}