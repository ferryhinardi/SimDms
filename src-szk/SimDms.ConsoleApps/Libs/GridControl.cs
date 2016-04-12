using System;
using System.Collections.Generic;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;

namespace SimDms.ConsoleApps.Libs
{
    public class GridControl<T> where T : class
    {
        private IQueryable<T> _queryable;
        private readonly Type _type;
        private PropertyInfo[] _properties;
        private DataParams _dataParams;
        private HttpRequestBase _httpRequest;

        public GridControl(IQueryable<T> queryable, HttpRequestBase httpRequest)
        {
            _queryable = queryable;
            _type = typeof(T);
            _properties = _type.GetProperties();
            _httpRequest = httpRequest;
            ParsingRequest();
        }

        private void ParsingRequest()
        {
            _dataParams = new DataParams();
            _dataParams.take = Convert.ToInt32(_httpRequest["iDisplayLength"]);
            _dataParams.skip = Convert.ToInt32(_httpRequest["iDisplayStart"]);
            _dataParams.sSearch = _httpRequest["sSearch"];
            _dataParams.sEcho = _httpRequest["sEcho"];

            _dataParams.mDataProps = new List<string>();
            var props = _httpRequest.Params.AllKeys.Where(x => x.StartsWith("mDataProp_")).ToList();
            foreach (var prop in props) { _dataParams.mDataProps.Add(_httpRequest[prop]); }

            _dataParams.sSearchs = new List<string>();
            var keys = _httpRequest.Params.AllKeys.Where(x => x.StartsWith("sSearch_")).ToList();
            foreach (var key in keys) { _dataParams.sSearchs.Add(_httpRequest[key]); }

            _dataParams.iSortCols = new List<int>();
            _dataParams.sSortDirs = new List<string>();
            var sortcols = _httpRequest.Params.AllKeys.Where(x => x.StartsWith("iSortCol_")).ToList();
            if (sortcols.Count() > 0)
            {
                foreach (var sortcol in sortcols)
                {
                    _dataParams.iSortCols.Add(Convert.ToInt32(_httpRequest[sortcol]));
                    _dataParams.sSortDirs.Add((_httpRequest["sSortDir_" + sortcol.Substring(9)] ?? "asc").ToLower());
                }
            }
            else
            {
                _dataParams.iSortCols.Add(0);
                _dataParams.sSortDirs.Add("asc");
            }
        }

        private IQueryable<T> ApplySort(IQueryable<T> qry)
        {
            var result = qry;
            for (int i = 0; i < _dataParams.iSortCols.Count(); i++)
            {
                var iSortCol = _dataParams.iSortCols[i];
                var sSortDir = _dataParams.sSortDirs[i];
                var paraExpr = Expression.Parameter(typeof(T), "val");

                var propType = _properties.Where(p => p.Name == _dataParams.mDataProps[iSortCol]).Select(p => p.PropertyType).SingleOrDefault();
                var deleType = Expression.GetFuncType(typeof(T), propType);
                var exprBody = Expression.Property(paraExpr, _dataParams.mDataProps[iSortCol]);
                var propExpr = Expression.Lambda(deleType, exprBody, paraExpr);

                var sSort = (i == 0) ?
                    ((sSortDir == "asc") ? "OrderBy" : "OrderByDescending") :
                    ((sSortDir == "asc") ? "ThenBy" : "ThenByDescending");

                result = typeof(Queryable).GetMethods().Single(
                              method => method.Name == sSort
                                          && method.IsGenericMethodDefinition
                                          && method.GetGenericArguments().Length == 2
                                          && method.GetParameters().Length == 2)
                                  .MakeGenericMethod(typeof(T), propType)
                                  .Invoke(null, new object[] { result, propExpr }) as IOrderedQueryable<T>;
            }
            return result;
        }

        private IQueryable<T> ApplyGenericSearch(IQueryable<T> qry)
        {
            if (string.IsNullOrWhiteSpace(_dataParams.sSearch)) return qry;

            var searExpr = Expression.Constant(_dataParams.sSearch.ToLower());
            var paraExpr = Expression.Parameter(typeof(T), "val");
            List<MethodCallExpression> searProps = new List<MethodCallExpression>();

            foreach (string mDataProp in _dataParams.mDataProps)
            {
                var prop = _properties.Where(p => p.Name == mDataProp).SingleOrDefault();
                var propExpr = Expression.Property(paraExpr, prop);

                if (prop.PropertyType == typeof(string))
                {
                    searProps.Add(Expression.Call(propExpr, typeof(string).GetMethod("Contains"), searExpr));
                }
            }

            var propQuery = searProps.ToArray();
            Expression queryExpr = propQuery[0];

            // add the other expressions
            for (int i = 1; i < propQuery.Length; i++)
            {
                queryExpr = Expression.Or(queryExpr, propQuery[i]);
            }

            // compile the expression into a lambda 
            var whereExpr = Expression.Lambda<Func<T, bool>>(queryExpr, paraExpr);

            return qry.Where(whereExpr);
        }

        private IQueryable<T> ApplyIndividualSearch(IQueryable<T> qry)
        {
            var paraExpr = Expression.Parameter(typeof(T), "val");
            List<MethodCallExpression> searProps = new List<MethodCallExpression>();

            for (int idx = 0; idx < _dataParams.sSearchs.Count(); idx++)
            {
                var search = _dataParams.sSearchs[idx];
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var name = _dataParams.mDataProps[idx];
                    var prop = _properties.Where(p => p.Name == name).SingleOrDefault();
                    var propExpr = Expression.Property(paraExpr, prop);
                    var searExpr = Expression.Constant(search);

                    if (prop.PropertyType == typeof(string))
                    {
                        searProps.Add(Expression.Call(propExpr, typeof(string).GetMethod("Contains"), searExpr));
                    }
                    else if (prop.PropertyType == typeof(int) || prop.PropertyType == typeof(double) || prop.PropertyType == typeof(decimal))
                    {
                        var toDecimalCall = Expression.Convert(propExpr, typeof(Nullable<decimal>));
                        var decimalConvert = typeof(SqlFunctions).GetMethod("StringConvert", new Type[] { typeof(Nullable<decimal>) });
                        var strExpr = Expression.Call(decimalConvert, toDecimalCall);

                        searProps.Add(Expression.Call(strExpr, typeof(string).GetMethod("Contains"), searExpr));
                    }
                }
            }

            if (searProps.Count() < 1) return qry;

            var propQuery = searProps.ToArray();
            Expression queryExpr = propQuery[0];

            // add the other expressions
            for (int i = 1; i < propQuery.Length; i++)
            {
                queryExpr = Expression.And(queryExpr, propQuery[i]);
            }

            // compile the expression into a lambda 
            var whereExpr = Expression.Lambda<Func<T, bool>>(queryExpr, paraExpr);

            return qry.Where(whereExpr);
        }

        private DataResult<T> ApplyParse()
        {
            var result = new DataResult<T>();
            var qry = _queryable;

            try
            {
                qry = ApplySort(qry);
                qry = ApplyGenericSearch(qry);
                qry = ApplyIndividualSearch(qry);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            // display the result
            result.sEcho = _dataParams.sEcho;
            result.iTotalRecords = qry.Count();
            result.iTotalDisplayRecords = result.iTotalRecords;
            result.aaData = ((result.iTotalRecords < _dataParams.take) ?
                                qry : qry.Skip(_dataParams.skip).Take(_dataParams.take)).ToList();
            return result;
        }

        public static DataResult<T> Parse(IQueryable<T> queryable, HttpRequestBase httpRequest)
        {
            return new GridControl<T>(queryable, httpRequest).ApplyParse();
        }
    }

    public class DataParams
    {
        public int take { get; set; }
        public int skip { get; set; }
        public List<int> iSortCols { get; set; }
        public List<string> sSortDirs { get; set; }
        public List<string> mDataProps { get; set; }
        public List<string> sSearchs { get; set; }
        public string sSearch { get; set; }
        public string sEcho { get; set; }
    }

    public class DataResult<T>
    {
        public DataResult()
        {
            sEcho = "1";
        }
        public string sEcho { get; set; }
        public int iTotalRecords { get; set; }
        public int iTotalDisplayRecords { get; set; }
        public List<T> aaData { get; set; }
        public string sColumns { get; set; }
    }
}
