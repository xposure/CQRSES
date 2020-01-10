namespace MindMatrix.EventSourcing
{
    public interface IAggregate<TAggregate>
    {
        string Id { get; }
        TAggregate Root { get; }
    }

    public class Aggregate<TAggregate> : IAggregate<TAggregate>
    {
        public string Id { get; }

        public TAggregate Root { get; }

        public Aggregate(string id, TAggregate root)
        {
            Id = id;
            Root = root;
        }
    }
}