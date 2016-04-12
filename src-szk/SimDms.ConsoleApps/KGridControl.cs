using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace SimDms.ConsoleApps
{
    public static class KGridControl
    {
        public static IQueryable<T> ApplySort<T>(this IQueryable<T> source, List<string> mDataProps, List<string> sSortDirs)
        {
            var type = typeof(T);
            for (int i = 0; i < mDataProps.Count(); i++)
            {
                var prop = type.GetProperty(mDataProps[i]);
                var para = Expression.Parameter(typeof(T), "p");
                var acce = Expression.MakeMemberAccess(para, prop);
                var expr = Expression.Lambda(acce, para);

                var dir = sSortDirs[i];
                var method = (i == 0) ? ((dir == "desc") ? "OrderByDescending" : "OrderBy") : ((dir == "desc") ? "ThenByDescending" : "ThenBy");
                var arg0 = new[] { type, prop.PropertyType };
                var arg1 = source.AsQueryable().Expression;
                var arg2 = Expression.Quote(expr);
                var result = Expression.Call(typeof(Queryable), method, arg0, arg1, arg2);

                source = source.AsQueryable().Provider.CreateQuery<T>(result);
            }

            return source;
        }

        public static IQueryable<T> ApplyFilter<T>(this IQueryable<T> source, List<string> filterFields, List<string> filterValues)
        {
            var result = source;
            var paraExpr = Expression.Parameter(typeof(T), "val");
            var properties = typeof(T).GetProperties();
            List<MethodCallExpression> searProps = new List<MethodCallExpression>();

            for (int i = 0; i < filterFields.Count(); i++)
            {
                var search = filterValues[i];
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var name = filterFields[i];
                    var prop = properties.Where(p => p.Name == name).SingleOrDefault();
                    var propExpr = Expression.Property(paraExpr, prop);
                    var searExpr = Expression.Constant(search);

                    if (prop.PropertyType == typeof(string))
                    {
                        searProps.Add(Expression.Call(propExpr, typeof(string).GetMethod("Contains"), searExpr));
                    }
                }
            }

            if (searProps.Count() < 1) return result;

            var propQuery = searProps.ToArray();
            Expression queryExpr = propQuery[0];

            // add the other expressions
            for (int i = 1; i < propQuery.Length; i++)
            {
                queryExpr = Expression.And(queryExpr, propQuery[i]);
            }

            // compile the expression into a lambda 
            var whereExpr = Expression.Lambda<Func<T, bool>>(queryExpr, paraExpr);

            return result.Where(whereExpr);
        }

        public static KGridResult<T> KGrid<T>(this IQueryable<T> query, DataParams param)
        {
            query = query.ApplySort(param.SoftFields, param.SortDirs);
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

    public class DataParams
    {
        public int Take { get; set; }
        public int Skip { get; set; }
        public List<string> SoftFields { get; set; }
        public List<string> SortDirs { get; set; }
        public List<string> FilterFields { get; set; }
        public List<string> FilterValues { get; set; }
    }
}
