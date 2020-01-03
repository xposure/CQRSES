namespace AggCommon
{
    using System.Threading.Tasks;

    public interface IAggregrateEvents<T>
    {
        Task Append<TEvent>(string aggregrateId, TEvent eventData) where TEvent : IAggregrateEvent<T>;
    }
}