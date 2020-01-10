namespace MindMatrix.EventSourcing
{
    using System;
    using System.Collections.Generic;
    using MediatR;
    using StructureMap;

    public interface IAggregateEventFactory<TAggregate>
    {
        IAggregateEvent Create(IAggregate<TAggregate> aggregate, IAggregateStreamEvent ev);
    }

    public interface IAggregateEventFactory<TAggregate, TEvent> : IAggregateEventFactory<TAggregate>
    {
    }


    public interface IAggregateEventFactoryGenerator<TAggregate>
    {
        IAggregateEventFactory<TAggregate> GetFactory(Type type);
    }


    public class AggregateEventFactory<TAggregate, TEvent> : IAggregateEventFactory<TAggregate, TEvent>
    {
        private class AggregateEvent : IAggregateEvent<TAggregate, TEvent>
        {
            public string Id { get; set; }

            public long Version { get; set; }

            public IAggregate<TAggregate> Aggregate { get; set; }

            public TEvent Data { get; set; }
        }

        private readonly IJsonSerializer<TEvent> _eventSerializer;
        public AggregateEventFactory(IJsonSerializerFactory _serializerFactory)
        {
            _eventSerializer = _serializerFactory.GetSerializer<TEvent>();
        }

        public IAggregateEvent Create(IAggregate<TAggregate> aggregate, IAggregateStreamEvent ev)
        {
            var e = _eventSerializer.ReadT(ev.Bytes);
            return new AggregateEvent() { Aggregate = aggregate, Id = ev.Id, Version = ev.Version, Data = e };
        }
    }

    public class AggregateEventFactoryGenerator<TAggregate> : IAggregateEventFactoryGenerator<TAggregate>
    {
        private readonly IContainer _container;
        private Dictionary<Type, IAggregateEventFactory<TAggregate>> _eventFactories = new Dictionary<Type, IAggregateEventFactory<TAggregate>>();

        public AggregateEventFactoryGenerator(IContainer container)
        {
            _container = container;
        }

        public IAggregateEventFactory<TAggregate> GetFactory(Type type)
        {
            if (!_eventFactories.TryGetValue(type, out var factory))
            {
                var serializerType = typeof(IAggregateEventFactory<,>).MakeGenericType(new[] { typeof(TAggregate), type });
                factory = (IAggregateEventFactory<TAggregate>)_container.GetInstance(serializerType);
                _eventFactories.Add(type, factory);
            }

            return factory;
        }
    }

}