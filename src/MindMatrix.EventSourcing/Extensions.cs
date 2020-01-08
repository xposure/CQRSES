namespace MindMatrix.EventSourcing
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EventStore.ClientAPI;
    using EventStore.ClientAPI.SystemData;

    public static class Extensions
    {
        public static async IAsyncEnumerable<ResolvedEvent> ReadEventsAsync(this IEventStoreConnection connection, string aggregateId, long start = 0, int count = 1024, bool resolve = false, UserCredentials userCredentials = null)
        {
            while (start >= 0)
            {
                var stream = await connection.ReadStreamEventsForwardAsync(aggregateId, start, count, resolve, userCredentials);
                for (var i = 0; i < stream.Events.Length; i++)
                    yield return stream.Events[i];

                if (stream.IsEndOfStream)
                    start = -1;
                else
                    start = stream.NextEventNumber;
            }
        }

        public static async Task<List<T>> ToListAsync<T>(this IAsyncEnumerable<T> data)
        {
            var list = new List<T>();
            await foreach (var it in data)
                list.Add(it);

            return list;
        }
    }
}