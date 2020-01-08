namespace MindMatrix.EventSourcing
{
    using System;

    public class OptimisticConcurrencyCheckException : Exception
    {
        public OptimisticConcurrencyCheckException(long version) : base($"Optimistic concurrency check failed using version {version}.") { }

    }

    public class AggregateNotFoundException : Exception
    {
        public AggregateNotFoundException(string aggregateId) : base($"Could not find aggregate stream [{aggregateId}].") { }
    }
}