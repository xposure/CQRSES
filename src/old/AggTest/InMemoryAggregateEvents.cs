namespace AggTest
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using AggCommon;
    using MediatR;

    public class InMemoryAggregrateEvents<T> : IAggregateStream<T>
            //where T : IAggregrate<T>
            where T : new()
    {
        public readonly Dictionary<string, List<IAggregateEvent<T>>> Events = new Dictionary<string, List<IAggregateEvent<T>>>();
        private IMediator _mediator;
        public InMemoryAggregrateEvents(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task Append<TEvent>(IAggregate<T> aggregate, TEvent eventData)
            where TEvent : IEventState<T>
        {
            if (!Events.TryGetValue(aggregate.AggregateId, out var eventList))
            {
                eventList = new List<IAggregateEvent<T>>();
                Events.Add(aggregate.AggregateId, eventList);
            }

            var ev = new AggregateEvent<T, TEvent>(Guid.NewGuid().ToString(), eventList.Count, eventData);
            eventList.Add(ev);
            eventData.Apply(aggregate.Root);
            return Task.CompletedTask;
        }

        public async IAsyncEnumerable<IAggregateEvent<T>> Read(string aggregateId)
        {
            if (!Events.TryGetValue(aggregateId, out var eventList))
                throw new AggregateNotFound(aggregateId);

            await Task.CompletedTask;

            foreach (var it in eventList)
                yield return it;

            //     var ev = new AggregateEvent<T, TEvent>(aggregrate, Guid.NewGuid().ToString(), eventList.Count, eventData);
            // eventList.Add(ev);
            // return Task.FromResult<IAggregateEvent<T, TEvent>>(ev);
            // throw new NotImplementedException();
        }
    }
}