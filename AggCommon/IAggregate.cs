namespace AggCommon
{
    public interface IAggregrate
    {
        string AggregrateId { get; }

        long EventIndex { get; }

    }

    public interface IAggregrate<T> : IAggregrate
    {
        T Root { get; }

        void Apply(IAggregrateEvent<T> ev);
    }

}