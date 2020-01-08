namespace AggCommon
{
    using System;

    public class AggregateNotFound : Exception
    {
        public AggregateNotFound(string id) : base($"Aggregate [{id}] not found.") { }
    }
}