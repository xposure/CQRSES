using System.Collections.Generic;

namespace MindMatrix.EventSourcing
{
    public interface ISerializer<T>
    {


    }

    public interface ISerializerFactory
    {
        ISerializer<T> Create<T>(string type);


    }

    public class SerializerFactory : ISerializerFactory
    {
        private Dictionary<Type, ISerializer> _serializers = new Dictionary<Type, EventSourcing.ISerializer>();

        public ISerializer<T> Create<T>(string type)
        {

        }
    }
}