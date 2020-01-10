namespace MindMatrix.EventSourcing
{
    using System;

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




}