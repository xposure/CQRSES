namespace AggCommon
{
    using System.Threading.Tasks;

    //I want to move all the event append logic to the IAggragate instead

    public interface IAggregateStream
    {
        // Task<IAggregrateEvent> Append2<TAggregate, TEvent>(IAggregrate<TAggregate> aggregrate, TEvent eventData)
        //     where TEvent : IEventState;

    }
    public interface IAggregateStream<T> : IAggregateStream
        where T : new()
    {
        Task<IAggregateEvent<T, TEvent>> Append<TEvent>(IAggregate<T> aggregate, TEvent eventData)
            where TEvent : IEventState<T>;

        Task<IAggregate<T>> Read(string aggregateId);
    }

    // public interface IAggregrateEvents
    // //where T : IAggregrate<T>
    // {
    //     Task<IAggregrateEvent<TAggregate>> Append<TAggregate, TEvent>(IAggregrate aggregrate, TEvent eventData) 
    //         where TAggregate: IAggregrate<TAggregate>
    //         where TEvent : IEventState<TAggregate>;
    //     //Task<IAggregrateEvent<T>> Read(int start = 0, int length = -1);
    //     //Task Delete(IAggregrate<T> aggregrate);
    // }
}