namespace AggCommon
{
    using System.Threading.Tasks;

    public interface IAggregateRepository<T>
    {
        Task<IAggregate<T>> GetById(string aggregateId);
        Task<IAggregate<T>> Create();
        //Task Snapshot(IAggregate<T> aggregate);
    }
}