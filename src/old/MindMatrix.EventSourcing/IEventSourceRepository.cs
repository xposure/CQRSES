namespace MindMatrix.EventSourcing
{
    using System.Threading.Tasks;

    public interface IEventSourceRepository<T>
    {
        Task<IEventSource2<T>> GetAggregate(string aggregateId);
        Task<IEventSource2<T>> CreateAggregate();
    }
}