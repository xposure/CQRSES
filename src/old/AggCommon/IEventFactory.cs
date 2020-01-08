namespace AggCommon
{
    public interface IEventFactory
    {
        IAggregateEvent<TAggregate, TEvent> Create<TAggregate, TEvent>(TAggregate aggregate)
            where TAggregate : IAggregate<TAggregate>
            where TEvent : IEventState<TAggregate>;
    }
}