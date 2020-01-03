namespace AggCommon
{
    using System;

    public class AggregrateNotFound : Exception
    {
        public AggregrateNotFound(string id) : base($"Aggregrate [{id}] not found.") { }
    }
}