namespace SimDms.Common
{
    using GeLang;
    using System;
    using System.Collections.Generic;
    //using System.Data.Objects.SqlClient;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Web;

    public class DataTableSp<T> where T : class
    {
        private DataParams _dataParams;
        private HttpRequestBase _httpRequest;
        private PropertyInfo[] _properties;
        private IQueryable<T> _queryable;
        private readonly Type _type;

        public DataTableSp(IQueryable<T> queryable, HttpRequestBase httpRequest)
        {
            this._queryable = queryable;
            this._type = typeof(T);
            this._properties = this._type.GetProperties();
            this._httpRequest = httpRequest;
            this.ParsingRequest();
        }

         private DataResult<T> ApplyParse()
        {
            DataResult<T> result = new DataResult<T>();
            IQueryable<T> qry = this._queryable;
            try
            {
                qry = this.ApplySort(qry);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString());
            }
            result.sEcho = this._dataParams.sEcho;
            result.iTotalRecords = qry.Count<T>();
            result.iTotalDisplayRecords = result.iTotalRecords;
            result.aaData = ((result.iTotalRecords < this._dataParams.take) ? qry : qry.Skip<T>(this._dataParams.skip).Take<T>(this._dataParams.take)).ToList<T>();
            return result;
        }

        private IQueryable<T> ApplySort(IQueryable<T> qry)
        {
            IQueryable<T> queryable = qry;
            for (int i = 0; i < this._dataParams.iSortCols.Count<int>(); i++)
            {
                ParameterExpression expression;
                int iSortCol = this._dataParams.iSortCols[i];
                string str = this._dataParams.sSortDirs[i];
                Type type = (from p in this._properties
                             where p.Name == ((DataTableSp<T>)this)._dataParams.mDataProps[iSortCol]
                             select p.PropertyType).SingleOrDefault<Type>();
                Type funcType = Expression.GetFuncType(new Type[] { typeof(T), type });
                MemberExpression body = Expression.Property(expression = Expression.Parameter(typeof(T), "val"), this._dataParams.mDataProps[iSortCol]);
                LambdaExpression expression3 = Expression.Lambda(funcType, body, new ParameterExpression[] { expression });
                string sSort = (i == 0) ? ((str == "asc") ? "OrderBy" : "OrderByDescending") : ((str == "asc") ? "ThenBy" : "ThenByDescending");
                queryable = typeof(Queryable).GetMethods().Single<MethodInfo>(method => ((((method.Name == sSort) && method.IsGenericMethodDefinition) && (method.GetGenericArguments().Length == 2)) && (method.GetParameters().Length == 2))).MakeGenericMethod(new Type[] { typeof(T), type }).Invoke(null, new object[] { queryable, expression3 }) as IOrderedQueryable<T>;
            }
            return queryable;
        }

        public static DataResult<T> Parse(IQueryable<T> queryable, HttpRequestBase httpRequest)
        {
            return new DataTableSp<T>(queryable, httpRequest).ApplyParse();
        }

        private void ParsingRequest()
        {
            this._dataParams = new DataParams();
            this._dataParams.take = Convert.ToInt32(this._httpRequest["iDisplayLength"]);
            this._dataParams.skip = Convert.ToInt32(this._httpRequest["iDisplayStart"]);
            this._dataParams.sSearch = this._httpRequest["sSearch"];
            this._dataParams.sEcho = this._httpRequest["sEcho"];
            this._dataParams.mDataProps = new List<string>();
            foreach (string str in (from x in this._httpRequest.Params.AllKeys
                                    where x.StartsWith("mDataProp_")
                                    select x).ToList<string>())
            {
                this._dataParams.mDataProps.Add(this._httpRequest[str]);
            }
            this._dataParams.sSearchs = new List<string>();
            foreach (string str2 in (from x in this._httpRequest.Params.AllKeys
                                     where x.StartsWith("sSearch_")
                                     select x).ToList<string>())
            {
                this._dataParams.sSearchs.Add(this._httpRequest[str2]);
            }
            this._dataParams.iSortCols = new List<int>();
            this._dataParams.sSortDirs = new List<string>();
            List<string> source = (from x in this._httpRequest.Params.AllKeys
                                   where x.StartsWith("iSortCol_")
                                   select x).ToList<string>();
            if (source.Count<string>() > 0)
            {
                foreach (string str3 in source)
                {
                    this._dataParams.iSortCols.Add(Convert.ToInt32(this._httpRequest[str3]));
                    this._dataParams.sSortDirs.Add((this._httpRequest["sSortDir_" + str3.Substring(9)] ?? "asc").ToLower());
                }
            }
            else
            {
                this._dataParams.iSortCols.Add(0);
                this._dataParams.sSortDirs.Add("asc");
            }
        }
    }
}

