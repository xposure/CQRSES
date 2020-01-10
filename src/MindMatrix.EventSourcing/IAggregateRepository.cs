namespace MindMatrix.EventSourcing
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using MediatR;


    public class AggreventEvent2
    {
        public string Id { get; }
        public long Version { get; }
        public string Type { get; }
        public byte[] Data { get; }
        public byte[] Metadata { get; }
    }

    public interface IAggregateRepository<TAggregate>
        where TAggregate : new()
    {
        Task<IAggregate<TAggregate>> Get(string aggregateId);
        Task Append(IAggregate<TAggregate> aggregate, params INotification[] events);

        //Task Delete(TAggregate aggregate);
    }

    public interface IAggregateRepository2<TAggregate>
        where TAggregate : new()
    {
        Task<IAggregate<TAggregate>> Get(string aggregateId);
        Task Append(IAggregate<TAggregate> aggregate, params AggreventEvent2[] events);

        //Task Delete(TAggregate aggregate);
    }


    public class MemoryAggregateRepository<TAggregate> : IAggregateRepository<TAggregate>
        where TAggregate : new()
    {
        private readonly IMediator _mediator;
        private readonly IEventStreamFactory<TAggregate> _eventStream;
        private readonly IAggregateEventFactoryGenerator<TAggregate> _eventFactoryGenerator;
        private Dictionary<string, IEventStream> _aggregates = new Dictionary<string, IEventStream>();

        public MemoryAggregateRepository(IMediator mediator, IEventStreamFactory<TAggregate> eventStream, IAggregateEventFactoryGenerator<TAggregate> eventFactoryGenerator)
        {
            _mediator = mediator;
            _eventStream = eventStream;
            _eventFactoryGenerator = eventFactoryGenerator;
        }

        public async Task Append(IAggregate<TAggregate> aggregate, params INotification[] events)
        {
            if (!_aggregates.TryGetValue(aggregate.Id, out var aggregateStream))
                throw new InvalidOperationException($"The repository did not have a reference to the aggregate [{aggregate.Id}].");

            foreach (var it in events)
            {
                //TODO: push all at once
                var ev = await aggregateStream.Append(it);
                var x = _eventFactoryGenerator.GetFactory(ev.Type);
                var y = x.Create(aggregate, ev);

                await _mediator.Publish(y);
            }
        }

        public async Task<IAggregate<TAggregate>> Get(string aggregateId)
        {
            //TODO: read from snapshot
            if (!_aggregates.TryGetValue(aggregateId, out var aggregateStream))
            {
                aggregateStream = _eventStream.Create(aggregateId);
                _aggregates.Add(aggregateId, aggregateStream);
            }

            var aggregate = new Aggregate<TAggregate>(aggregateId, new TAggregate());
            await foreach (var ev in aggregateStream.Read())
            {
                var x = _eventFactoryGenerator.GetFactory(ev.Type);
                var y = x.Create(aggregate, ev);
                await _mediator.Publish(y);
            }

            return aggregate;
        }
    }
}