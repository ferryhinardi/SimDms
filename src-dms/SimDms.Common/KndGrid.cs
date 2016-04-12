using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Web;
using GeLang;

namespace SimDms.Common
{  
    public static class KndGrid
    {
        private static IQueryable<T> ApplySort<T>(this IQueryable<T> source, List<string> fields, List<string> dirs)
        {
            var type = typeof(T);
            if (fields.Count == 0)
            {
                var prop = type.GetProperty(type.GetProperties()[0].Name);
                var para = Expression.Parameter(typeof(T), "p");
                var acce = Expression.MakeMemberAccess(para, prop);
                var expr = Expression.Lambda(acce, para);

                var arg0 = new[] { type, prop.PropertyType };
                var arg1 = source.AsQueryable().Expression;
                var arg2 = Expression.Quote(expr);                
                var result = Expression.Call(typeof(Queryable), "OrderByDescending", arg0, arg1, arg2);

                source = source.AsQueryable().Provider.CreateQuery<T>(result);
            }
            else
            {
                for (int i = 0; i < fields.Count(); i++)
                {
                    var prop = type.GetProperty(fields[i]);
                    var para = Expression.Parameter(typeof(T), "p");
                    var acce = Expression.MakeMemberAccess(para, prop);
                    var expr = Expression.Lambda(acce, para);

                    var dir = dirs[i];
                    var method = (i == 0) ? ((dir == "desc") ? "OrderByDescending" : "OrderBy") : ((dir == "desc") ? "ThenByDescending" : "ThenBy");
                    var arg0 = new[] { type, prop.PropertyType };
                    var arg1 = source.AsQueryable().Expression;
                    var arg2 = Expression.Quote(expr);
                    var result = Expression.Call(typeof(Queryable), method, arg0, arg1, arg2);

                    source = source.AsQueryable().Provider.CreateQuery<T>(result);
                }
            }

            return source;
        }

        private static IQueryable<T> ApplyFilter<T>(this IQueryable<T> source, List<string> fields, List<string> values)
        {
            var result = source;
            if (fields.Count() < 1) return result;

            var properties = typeof(T).GetProperties();
            var paraExpr = Expression.Parameter(typeof(T), "val");
            Expression queryExpr = Expression.Constant(true);

            for (int i = 0; i < fields.Count(); i++)
            {
                var value = values[i];
                if (!string.IsNullOrWhiteSpace(value))
                {
                    var name = fields[i];
                    var prop = properties.Where(p => p.Name == name).SingleOrDefault();
                    var toUpperCall = Expression.Call(Expression.Call(Expression.Property(paraExpr, prop), "ToString", new Type[0]),
                        typeof(string).GetMethod("ToUpper", new Type[0])
                        );

                    queryExpr = Expression.And(queryExpr, Expression.Call(toUpperCall, typeof(string).GetMethod("Contains"),
                        Expression.Constant(value.ToString().ToUpper())
                    ));
                }
            }
            // compile the expression into a lambda 
            var whereExpr = Expression.Lambda<Func<T, bool>>(queryExpr, paraExpr);

            return result.ToList().AsQueryable().Where(whereExpr);
        }

        public static KGridResult<T> toKG<T>(this IQueryable<T> query, ApplyFilterKendoGrid ApplyFilterKendoGrid = ApplyFilterKendoGrid.True )
        {
            var param = new KDataParams();
            query = query.ApplySort(param.SortFields, param.SortDirs);
            if (ApplyFilterKendoGrid == Common.ApplyFilterKendoGrid.True)
            {
                query = query.ApplyFilter(param.FilterFields, param.FilterValues);
            }

            var result = new KGridResult<T>
            {
                total = query.Count(),
                data = query.Skip(param.Skip).Take(param.Take)
            };

            return result;
        }

        public static KGridResult<T> toKG<T>(this IQueryable<T> query, HttpRequestBase request)
        {
            var param = new KDataParams(request);
            query = query.ApplySort(param.SortFields, param.SortDirs);
            query = query.ApplyFilter(param.FilterFields, param.FilterValues);

            var result = new KGridResult<T>
            {
                total = query.Count(),
                data = query.Skip(param.Skip).Take(param.Take)
            };

            return result;
        }
    }

    public class KGridResult<T>
    {
        public int total { get; set; }
        public IEnumerable<T> data { get; set; }
    }

    public class KDataParams
    {
        public KDataParams()
        {
            var request = HttpContext.Current.Request;
            Take = Convert.ToInt32(request["take"]);
            Skip = Convert.ToInt32(request["skip"]);

            var req = HttpContext.Current.Request;

            SortFields = new List<string> { };
            SortDirs = new List<string> { };
            SortCompares = new List<string> { };
            FilterFields = new List<string> { };
            FilterOperators = new List<string> { };
            FilterValues = new List<string> { };

            var allKeys = request.Params.AllKeys;
            var sorts = allKeys.Where(p => p.StartsWith("sort")).ToList();
            var filters = allKeys.Where(p => p.StartsWith("filter[filters]")).ToList();

            for (int i = 0; i < Math.Ceiling(sorts.Count() / 3.0); i++)
            {
                var field = request[string.Format("sort[{0}][field]", i)];
                var dir = request[string.Format("sort[{0}][dir]", i)];
                var compare = request[string.Format("sort[{0}][compare]", i)];

                SortFields.Add(field);
                SortDirs.Add(dir);
                SortCompares.Add(compare);
            }

            for (int i = 0; i < filters.Count() / 3; i++)
            {
                var field = request[string.Format("filter[filters][{0}][field]", i)];
                var opr = request[string.Format("filter[filters][{0}][operator]", i)];
                var value = request[string.Format("filter[filters][{0}][value]", i)];

                FilterFields.Add(field);
                FilterOperators.Add(opr);
                FilterValues.Add(value);
            }
        }

        public KDataParams(HttpRequestBase request)
        {
            Take = Convert.ToInt32(request["take"]);
            Skip = Convert.ToInt32(request["skip"]);

            var req = HttpContext.Current.Request;

            SortFields = new List<string> { };
            SortDirs = new List<string> { };
            SortCompares = new List<string> { };
            FilterFields = new List<string> { };
            FilterOperators = new List<string> { };
            FilterValues = new List<string> { };

            var allKeys = request.Params.AllKeys;
            var sorts = allKeys.Where(p => p.StartsWith("sort")).ToList();
            var filters = allKeys.Where(p => p.StartsWith("filter[filters]")).ToList();

            for (int i = 0; i < sorts.Count() / 3; i++)
            {
                var field = request[string.Format("sort[{0}][field]", i)];
                var dir = request[string.Format("sort[{0}][dir]", i)];
                var compare = request[string.Format("sort[{0}][compare]", i)];

                SortFields.Add(field);
                SortDirs.Add(dir);
                SortCompares.Add(compare);
            }

            for (int i = 0; i < filters.Count() / 3; i++)
            {
                var field = request[string.Format("filter[filters][{0}][field]", i)];
                var opr = request[string.Format("filter[filters][{0}][operator]", i)];
                var value = request[string.Format("filter[filters][{0}][value]", i)];

                FilterFields.Add(field);
                FilterOperators.Add(opr);
                FilterValues.Add(value);
            }
        }

        public int Take { get; set; }
        public int Skip { get; set; }
        public List<string> SortFields { get; set; }
        public List<string> SortDirs { get; set; }
        public List<string> SortCompares { get; set; }
        public List<string> FilterFields { get; set; }
        public List<string> FilterOperators { get; set; }
        public List<string> FilterValues { get; set; }
    }
}
