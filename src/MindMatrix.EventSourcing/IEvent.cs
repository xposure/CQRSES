namespace MindMatrix.EventSourcing
{
    public interface IEvent<TAggregate>
    {

        void Apply(TAggregate aggregate);

    }
}