namespace MindMatrix.EventSourcing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using EventStore.ClientAPI;
    using EventStore.ClientAPI.SystemData;
    using MindMatrix.EventSourcing;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Attributes;
    using MongoDB.Bson.Serialization.IdGenerators;
    using MongoDB.Driver;

    //we want to move events to eventsource
    //snapshots to mongodb

    public interface IEventFactory<TAggregate>
    {
        IAggregateEvent2<TAggregate> Deserialize(RecordedEvent ev);
        IAggregateEvent2<TAggregate> Deserialize(string type, BsonDocument document);

        EventData Serialize(IEvent<TAggregate> eventData);
        //BsonDocument Serialize<TEvent>(TEvent eventData);
    }

    public class MongoESRespository<TAggregate> : IEventSourceRepository<TAggregate>
        where TAggregate : new()
    {
        public class MongoAggregateEvent// : IAggregateEvent2<TAggregate>
        {
            public string EventType { get; set; }
            public long Version { get; set; }

            [BsonRepresentation(BsonType.ObjectId)]
            [BsonId]
            public string Id { get; set; }

            public long Index { get; set; }

            public BsonDocument EventData { get; set; }

            public void Apply(MongoAggregate aggregate, MongoAggregateEvent mongoEvent, IEvent<TAggregate> eventData)
            {
                eventData.Apply(aggregate.Root);
                aggregate.Version++;
            }

            public void Apply(IEventFactory<TAggregate> _eventFactory, MongoAggregate aggregate)
            {
                var data = _eventFactory.Deserialize(EventType, EventData);
                Apply(aggregate, this, data);
            }
        }

        public class MongoAggregate : IEventSource2<TAggregate>
        {
            internal IMongoCollection<MongoAggregate> _collection;

            [BsonRepresentation(BsonType.ObjectId)]
            [BsonId]
            [BsonElement("_id")]
            public string Id { get; set; }

            public TAggregate Root { get; set; }

            public long Version { get; set; }

            public List<MongoAggregateEvent> Events { get; set; }

            // public MongoAggregate(IMongoCollection<MongoAggregate> collection)
            // {
            //     _collection = collection;
            // }

            private IEventStoreConnection _eventStore;

            public MongoAggregate() { }
            public MongoAggregate(IEventStoreConnection eventStore)
            {
                _eventStore = eventStore;
            }

            // private byte[] Serialize(IEvent<TAggregate> ev)
            // {
            //     byte[] metaData = null;
            //     if (ev is IMetaData meta)
            //     {
            //         metaData = Json.ToJsonBytes(meta.Meta);

            //     }

            // }

            // public async Task Append(params IEvent<TAggregate>[] data)
            // {
            //     var events = data.Select(it =>
            //         new
            //         {
            //             Data = it,
            //             EventData = _eventFactory.Serialize(it)
            //         }
            //     ).ToArray();

            //     var result = await _eventStore.AppendToStreamAsync(Id, Version, events.Select(it => it.EventData));
            // }

            public async Task Append(params IEvent<TAggregate>[] data)
            {
                var mongoEvents = data.Select(it =>
                    new
                    {
                        Data = it,
                        MongoEvent = new MongoAggregateEvent()
                        {
                            EventType = it.GetType().FullName,
                            Id = ObjectId.GenerateNewId().ToString(),
                            Index = Version + Events.Count,
                            EventData = it.ToBsonDocument()
                        }
                    }
                ).ToArray();

                var updateEvents = new List<UpdateDefinition<MongoAggregate>>();
                updateEvents.Add(Builders<MongoAggregate>.Update.Inc(x => x.Version, data.Length));
                updateEvents.AddRange(mongoEvents.Select(it => Builders<MongoAggregate>.Update.Push(x => x.Events, it.MongoEvent)));


                var result = await _collection.UpdateOneAsync(
                    x => x.Id == this.Id && x.Version == Version,
                    Builders<MongoAggregate>.Update.Combine(updateEvents)
                );

                if (result.ModifiedCount != 1)
                    throw new OptimisticConcurrencyCheckException(Version);

                foreach (var it in mongoEvents)
                {
                    Events.Add(it.MongoEvent);
                    it.MongoEvent.Apply(this, it.MongoEvent, it.Data);
                }
            }

            public void Snapshot()
            {
                throw new NotImplementedException();
            }
        }

        private IMongoCollection<MongoAggregate> _collection;
        private IEventStoreConnection _eventStore;
        private IEventFactory<MongoAggregate> _eventFactory;
        public MongoESRespository(IMongoClient client, IEventStoreConnection eventStore)
        {
            _eventStore = eventStore;
            var database = client.GetDatabase("es");
            _collection = database.GetCollection<MongoAggregate>("aggregates");
        }

        public async Task<IEventSource2<TAggregate>> CreateAggregate()
        {
            var aggregate = new MongoAggregate();
            aggregate.Id = ObjectId.GenerateNewId().ToString();
            aggregate.Events = new List<MongoAggregateEvent>();
            aggregate.Root = new TAggregate();
            aggregate.Version = 0;

            aggregate._collection = _collection;

            await _collection.InsertOneAsync(aggregate);

            return aggregate;

        }

        public async Task<IEventSource2<TAggregate>> GetAggregate(string aggregateId)
        {
            var agg = new MongoAggregate();
            agg._collection = _collection;
            agg.Id = aggregateId;
            agg.Version = -1;
            agg.Events = null;


            var c = EventStoreConnection.Create(new Uri("tcp://agg:agg@localhost:1113"));
            await c.ConnectAsync();
            await foreach (var it in c.ReadEventsAsync(aggregateId, userCredentials: new UserCredentials("agg", "agg")))
            {
                var ev = _eventFactory.Deserialize(it.Event);
                ev.Apply(agg);
            }

            if (agg.Version == -1)
                throw new AggregateNotFoundException(aggregateId);

            return agg;
        }
    }


}