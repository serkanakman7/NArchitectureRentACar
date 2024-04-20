using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.Persistence.Paging
{
    public static class QueryablePaginateExtensions
    {
        public static async Task<Paginate<T>> ToPaginateAsync<T>(this IQueryable<T> source,int index, int size,CancellationToken cancellationToken=default)
        {
            int count = await source.CountAsync(cancellationToken).ConfigureAwait(false);
            List<T> items = await source.Skip(index*size).Take(size).ToListAsync(cancellationToken).ConfigureAwait(false);

            Paginate<T> list = new()
            {
                Size = size,
                Index = index,
                Count = count,
                Items = items,
                Pages = (int)Math.Ceiling(count /(double)size)
            };
            return list;
        }

        public static Paginate<T> ToPaginate<T>(this IQueryable<T> source,int index,int size)
        {
            var count = source.Count();
            var items = source.Skip(index*size).Take(count).ToList();

            Paginate<T> list = new()
            {
                Size = size,
                Index = index,
                Count = count,
                Items = items,
                Pages = (int)Math.Ceiling(count / (double)index)
            };
            return list;
        }
    }
}
