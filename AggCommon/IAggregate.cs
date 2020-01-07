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

        //void Apply(IAggregrateEvent<T> ev);
    }

}