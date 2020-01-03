namespace AggCommon
{
    public interface IAggregrate
    {
        string AggregrateId { get; }

        long EventIndex { get; }
    }

    public class Aggregrate : IAggregrate
    {
        public string AggregrateId { get; }
        public long EventIndex { get; internal set; }

        protected Aggregrate(string aggregrateId)
        {
            AggregrateId = aggregrateId;
        }
    }
}