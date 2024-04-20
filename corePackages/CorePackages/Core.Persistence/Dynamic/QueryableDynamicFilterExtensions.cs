using System;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;

namespace Core.Persistence.Dynamic;

public static class QueryableDynamicFilterExtensions
{
    public static readonly string[] _orders = { "asc", "desc" };
    public static readonly string[] _logics = { "and", "or" };

    public static readonly IDictionary<string, string> _operators = new Dictionary<string, string>
    {
        {"eq","=" },
        {"neq" , "!=" },
        {"lt" , "<" },
        {"lte" , "<=" },
        {"gt" , ">" },
        {"gte" , ">=" },
        {"isnull" , "==null" },
        {"isnotnull" , "!=null" },
        {"startswith" , "StartsWith" },
        {"endswith" , "EndsWith" },
        {"contains" , "Contains" },
        {"doesnotcontain" , "Contains" }
    };

    public static IQueryable<T> ToDynamic<T>(this IQueryable<T> query, DynamicQuery dynamicQuery)
    {
        if (dynamicQuery.Filter is not null) query = Filter(query, dynamicQuery.Filter);

        if (dynamicQuery.Sort is not null && dynamicQuery.Sort.Any()) query = Sort(query, dynamicQuery.Sort);

        return query;
    }

    private static IQueryable<T> Filter<T>(IQueryable<T> queryable, Filter filter)
    {
        IList<Filter> filters = GetAllFilters(filter);
        string?[] values = filters.Select(f => f.Value).ToArray();

        string where = Transform(filter, filters);
        if (!string.IsNullOrEmpty(where) && values != null) queryable = queryable.Where(where, values);

        return queryable;
    }

    public static IQueryable<T> Sort<T>(IQueryable<T> queryable, IEnumerable<Sort> sort)
    {
        foreach (Sort item in sort)
        {
            if (string.IsNullOrEmpty(item.Field))
                throw new ArgumentException("Invalid Filed");
            if (string.IsNullOrEmpty(item.Dir) && !_orders.Contains(item.Dir))
                throw new ArgumentException("Invalid Order Type");
        }

        if (sort.Any())
        {
            string ordering = string.Join(separator: ",", values: sort.Select(s => $"{s.Field} {s.Dir}"));

            return queryable.OrderBy(ordering);
        }
        return queryable;
    }

    public static IList<Filter> GetAllFilters(Filter filter)
    {
        List<Filter> filters = new();
        GetFilters(filter, filters);

        return filters;
    }

    private static void GetFilters(Filter filter, List<Filter> filters)
    {
        filters.Add(filter);

        if (filter.Filters is not null && filter.Filters.Any())
            foreach (Filter item in filter.Filters)
                GetFilters(item, filters);
    }

    private static string Transform(Filter filter, IList<Filter> filters)
    {
        if (string.IsNullOrEmpty(filter.Field)) throw new ArgumentException("Invalid Field");
        if (string.IsNullOrEmpty(filter.Operation) || !_operators.ContainsKey(filter.Operation)) throw new ArgumentException("Invalid Operator");

        int index = filters.IndexOf(filter);
        string comparison = _operators[filter.Operation];
        StringBuilder where = new();

        if (!string.IsNullOrEmpty(filter.Value))
        {
            if (filter.Operation == "doesnotconstain")
                where.Append($"!np({filter.Field}).{comparison}(@{index.ToString()}))");
            else if (comparison is "StartsWith" or "EndsWith" or "Contains")
                where.Append($"(np({filter.Field}).{comparison}(@{index.ToString()}))");
            else
                where.Append($"np({filter.Field}) {comparison} @{index.ToString()}");
        }
        else if (filter.Operation is "isnull" or "isnotnull")
        {
            where.Append($"np({filter.Field}) {comparison}");
        }

        if (filter.Logic is not null && filter.Filters is not null && filter.Filters.Any())
        {
            if (!_logics.Contains(filter.Logic))
                throw new ArgumentException("Invalid Logic");
            return $"{where} {filter.Logic} ({string.Join(separator: $" {filter.Logic} ", value: filter.Filters.Select(f => Transform(f, filters)).ToArray())})";
        }

        return where.ToString();

    }
}
