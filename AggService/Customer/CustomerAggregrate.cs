namespace AggService.Customer
{
    using AggCommon;

    public class CustomerAggregrate : Aggregrate
    {
        public string Name { get; internal set; }

        public CustomerAggregrate(string aggregrateId) : base(aggregrateId)
        {
        }
    }
}