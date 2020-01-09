// namespace MindMatrix.EventSourcing
// {
//     using Xunit;
//     using Shouldly;
//     using System;

//     public class Dummy
//     {
//         public string Name { get; internal set; }
//     }

//     public class DummyEvent : IEvent<Dummy>
//     {
//         public string Name { get; set; }
//         public void Apply(Dummy aggregate)
//         {
//             aggregate.Name = Name;
//         }
//     }

//     public class EventStreamTests
//     {
//         [Fact]
//         public async void CanAppendToStream()
//         {
//             using var di = DIFixture.Scope();
//             var factory = di.GetInstance<IEventStreamFactory<Dummy>>();
//             var stream = factory.Create(Guid.NewGuid().ToString());

//             await stream.Append(new DummyEvent() { Name = "Hello World" });

//             //stream.Read

//             stream.Version.ShouldBe(0);
//         }

//         [Fact]
//         public async void CanReadFromStream()
//         {
//             var aggregateId = Guid.NewGuid().ToString();
//             using var di = DIFixture.Scope();
//             var factory = di.GetInstance<IEventStreamFactory<Dummy>>();
//             var stream = factory.Create(aggregateId);

//             await stream.Append(new DummyEvent() { Name = "Hello World" });

//             var stream2 = factory.Create(aggregateId);
//             var result = await stream2.Read().ToListAsync();

//             result.Count.ShouldBe(1);
//             result[0].ShouldBeOfType<AggregateEvent<Dummy>>().Version.ShouldBe(0);
//             //result[0].Data.ShouldBeOfType<DummyEvent>().Name.ShouldBe("Hello World");


//             //stream.Read

//             stream.Version.ShouldBe(0);
//         }

//         [Fact]
//         public async void ShouldThrowOptimisticConcurrencyCheck()
//         {
//             var aggregateId = Guid.NewGuid().ToString();
//             using var di = DIFixture.Scope();
//             var factory = di.GetInstance<IEventStreamFactory<Dummy>>();

//             var stream = factory.Create(aggregateId);
//             var stream2 = factory.Create(aggregateId);

//             await stream.Append(new DummyEvent() { Name = "Hello World" });

//             Should.Throw<Exception>(async () => await stream2.Append(new DummyEvent() { Name = "Hello World" }));
//         }
//     }
// }