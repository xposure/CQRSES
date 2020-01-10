namespace MindMatrix.EventSourcing
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public static class Extensions
    {
        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> data)
        {
            var list = new List<T>();
            await foreach (var it in data)
                list.Add(it);

            return list;
        }
    }
}