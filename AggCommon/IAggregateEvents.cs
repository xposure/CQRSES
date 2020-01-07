namespace AggCommon
{
    using System.Threading.Tasks;

    public interface IAggregrateEvents
    {
        // Task<IAggregrateEvent> Append2<TAggregate, TEvent>(IAggregrate<TAggregate> aggregrate, TEvent eventData)
        //     where TEvent : IEventState;

    }
    public interface IAggregrateEvents<T> : IAggregrateEvents
    {
        Task<IAggregrateEvent<T, TEvent>> Append<TEvent>(IAggregrate<T> aggregrate, TEvent eventData)
            where TEvent : IEventState<T>;
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