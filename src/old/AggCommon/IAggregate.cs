using System.Threading.Tasks;

namespace AggCommon
{
    public interface IAggregate
    {
        string AggregateId { get; }

        long EventIndex { get; }

    }

    public interface IAggregate<T> : IAggregate
    {
        T Root { get; }

        Task Append(params IEventState<T>[] eventData);
        Task Snapshot();

        //void Apply(IAggregrateEvent<T> ev);
    }

}