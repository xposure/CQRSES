namespace AggCommon
{
    using System.Threading.Tasks;

    public interface IAggregrateEvents<T>
    //where T : IAggregrate<T>
    {
        Task<IAggregrateEvent<T>> Append<TEvent>(IAggregrate<T> aggregrate, TEvent eventData) where TEvent : IEventState<T>;
    }
}