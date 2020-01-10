namespace MindMatrix.EventSourcing
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using EventStore.ClientAPI;
    using EventStore.ClientAPI.SystemData;

    public static class EventStoreExtensions
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
    }
}