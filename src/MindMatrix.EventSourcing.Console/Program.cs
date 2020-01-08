using System;
using System.Linq;
using System.Reflection;
using StructureMap;
using StructureMap.Building;
using StructureMap.Pipeline;

namespace MindMatrix.EventSourcing
{
    public interface IAE<T> { void DoSomething(); }
    public interface IAE<T, D> : IAE<T> { }
    public class AE<T> : IAE<T> { public void DoSomething() => Console.WriteLine("Do SOmething"); }
    public class AE<T, D> : AE<T> { }

    public interface IAEF<T>
    {
        IAE<T> Create();
    }

    public abstract class AEF<T> : IAEF<T>
    {
        public abstract IAE<T> Create();
    }

    public class AEF<T, D> : AEF<T>
    {
        public override IAE<T> Create() => new AE<T, D>();
    }

    public class AggregateEventInstance : Instance
    {
        public override string Description => "Creates strongly typed AggregateEvents.";

        public override Type ReturnedType => typeof(IAE<>);

        public override IDependencySource ToDependencySource(Type pluginType) => throw new NotSupportedException();
        private Type _context;

        public override string Name => _context.FullName;
        public AggregateEventInstance(Type context)
        {
            _context = context;
        }

        public override Instance CloseType(Type[] types)
        {
            // StructureMap will cache the object built out of this,
            // so the expensive Reflection hit only happens
            // once

            Console.WriteLine(_context);
            Console.WriteLine(types[0]);
            var instanceType = typeof(AE<,>).MakeGenericType(_context, types[0]);
            return new ObjectInstance(Activator.CreateInstance(instanceType)).Named(_context.FullName);
        }
    }

    public class D
    {

    }

    public class DE : IEvent<D>
    {
        public void Apply(D aggregate)
        {
            Console.WriteLine("DDEEEEEEEEEEEEEEEEEEEE");
        }
    }

    class Program
    {

        static void Main(string[] args)
        {

            var container = new Container(cfg =>
            {
                cfg.Scan(scanner =>
                {
                    scanner.AssemblyContainingType<Program>();
                    //scanner.AddAllTypesOf(typeof(IEvent<>));
                    scanner.ConnectImplementationsToTypesClosing(typeof(IEvent<>));
                    //     .OnAddedPluginTypes(it => it.Add(new AggregateEventInstance()));

                });

                var FUCKOFF = from type in typeof(Program).Assembly.GetTypes()
                              from z in type.GetInterfaces()
                              where z.IsGenericType && z.GetGenericTypeDefinition() == typeof(IEvent<>)
                              select new { type, z };

                foreach (var it in FUCKOFF)
                {
                    var bt = it.z.GetGenericArguments()[0];
                    var ugh = typeof(IAE<>).MakeGenericType(bt);
                    cfg.For(ugh).Use(new AggregateEventInstance(it.type));

                    Console.WriteLine(it);
                }
                //cfg.For(typeof(IAE<>)).Use(typeof(AE<>));

                Json.DeserializeObject()
            });




            var ev = container.GetInstance<IAEF<D>>("MindMatrix.EventSourcing.DE");





            //Console.WriteLine(container.WhatDoIHave());
            //Console.WriteLine("Hello World!");
        }
    }
}
