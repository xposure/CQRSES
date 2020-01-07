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
        public readonly Dictionary<string, List<IAggregateEvent>> Events = new Dictionary<string, List<IAggregateEvent>>();
        private IMediator _mediator;
        public InMemoryAggregrateEvents(IMediator mediator)
        {
            _mediator = mediator;
        }

        public Task<IAggregateEvent<T, TEvent>> Append<TEvent>(IAggregate<T> aggregate, TEvent eventData)
            where TEvent : IEventState<T>
        {
            if (!Events.TryGetValue(aggregate.AggregateId, out var eventList))
            {
                eventList = new List<IAggregateEvent>();
                Events.Add(aggregate.AggregateId, eventList);
            }

            var ev = new AggregateEvent<T, TEvent>(aggregate, Guid.NewGuid().ToString(), eventList.Count, eventData);
            eventList.Add(ev);
            return Task.FromResult<IAggregateEvent<T, TEvent>>(ev);
        }

        public Task<IAggregate<T>> Read(string aggregateId)
        {
            if (!Events.TryGetValue(aggregateId, out var eventList))
                throw new AggregateNotFound(aggregateId);

            var agg = new Aggregrate<T>(aggregateId, new T());



            //     var ev = new AggregateEvent<T, TEvent>(aggregrate, Guid.NewGuid().ToString(), eventList.Count, eventData);
            // eventList.Add(ev);
            // return Task.FromResult<IAggregateEvent<T, TEvent>>(ev);
            // throw new NotImplementedException();
        }
    }
}