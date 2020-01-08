namespace MindMatrix.EventSourcing
{
    public interface IEvent<T>
    {
        void Apply(T aggregate);
    }

    public interface IMetaData
    {

    }

    public interface IAggregateEvent2
    {
        string ID { get; }
        long Index { get; }
    }

    public interface IAggregateEvent2<T> : IAggregateEvent2, IEvent<T>
    {

    }


}