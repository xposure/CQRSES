// namespace AggTest
// {
//     using System;
//     using System.Collections.Generic;
//     using MindMatrix.EventSourcing;
//     using Utf8Json;

//     public class InMemoryEventSourceRepository<TAggregate> : IEventSourceRepository<TAggregate>
//         where TAggregate : new()
//     {

//         public class InMemoryAggregate : IEventSource<TAggregate>
//         {
//             public TAggregate Root { get; set; } = new TAggregate();
//             public List<AggregateEvent2<TAggregate>> Events { get; set; } = new List<AggregateEvent2<TAggregate>>();
//             public long Version { get; set; } = 0;

//             public string AggregateId { get; set; }

//             public InMemoryAggregate()
//             {
//             }

//             public InMemoryAggregate(string aggregateId)
//             {
//                 AggregateId = aggregateId;
//             }

//             public void Append<TEvent>(TEvent eventData)
//                 where TEvent : IEvent<TAggregate>
//             {
//                 var aggregateEvent = new AggregateEvent2<TAggregate, TEvent>();
//                 aggregateEvent.ID = Guid.NewGuid().ToString();
//                 aggregateEvent.Index = Version + Events.Count;
//                 aggregateEvent.Data = eventData;
//                 Events.Add(aggregateEvent);
//                 aggregateEvent.Apply(this);
//             }

//             public void Snapshot()
//             {
//                 throw new System.NotImplementedException();
//             }
//         }

//         private Dictionary<string, string> _aggregates = new Dictionary<string, string>();

//         public IEventSource<TAggregate> GetAggregate(string aggregateId)
//         {
//             if (!_aggregates.TryGetValue(aggregateId, out var aggData))
//                 return null;

//             var aggregate = Utf8Json.JsonSerializer.Deserialize<InMemoryAggregate>(aggData);
//             foreach (var it in aggregate.Events)
//                 it.Apply(aggregate);

//             return aggregate;
//         }

//         public IEventSource<TAggregate> CreateAggregate()
//         {
//             var aggregateId = Guid.NewGuid().ToString();
//             var aggregate = new InMemoryAggregate(aggregateId);

//             _aggregates.Add(aggregateId, Utf8Json.JsonSerializer.ToJsonString(aggregate));
//             return aggregate;
//         }
//     }
// }