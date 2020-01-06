namespace AggCommon
{
    using System.Threading.Tasks;

    public interface IAggregrateEvents<T>
    //where T : IAggregrate<T>
    {
        Task<IAggregrateEvent<T>> Append<TEvent>(IAggregrate<T> aggregrate, TEvent eventData) where TEvent : IEventState<T>;
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