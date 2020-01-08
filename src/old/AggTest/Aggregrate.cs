namespace AggTest
{
    using System;
    using System.Threading.Tasks;
    using AggCommon;

    public class Aggregrate<T> : IAggregate<T>
        where T : new()
        //where T : IAggregrateEvent<T>
    {
        public string AggregateId { get; internal set; }
        public long EventIndex { get; internal set; }

        public T Root { get; }

        private readonly IAggregateStream<T> _stream;

        public Aggregrate(string aggregrateId, T root, IAggregateStream<T> stream)
        {
            AggregateId = aggregrateId;
            Root = root;
            _stream = stream;
        }

        public async Task Append(params IEventState<T>[] eventData)
        {
            //transaction?
            foreach (var it in eventData)
            {
                await _stream.Append(this, it);
                //it.Apply(Root);
            }
        }

        public Task Snapshot()
        {
            throw new NotImplementedException();
        }
    }
}