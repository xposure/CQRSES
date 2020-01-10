using System;

namespace MindMatrix.EventSourcing
{
    public interface IJsonSerializer<T> : IJsonSerializer
    {
        byte[] Write(T data);
        T ReadT(byte[] json);
    }

    public interface IJsonSerializer
    {
        //byte[] Write(T data);
        object Read(byte[] json);
    }

    public class JsonSerializer<T> : IJsonSerializer<T>
    {
        public object Read(byte[] json) => ReadT(json);

        public T ReadT(byte[] json) => Json.ParseJson<T>(json);

        public byte[] Write(T data) => Json.ToJsonBytes(data);
    }

    public interface IJsonSerializerFactory
    {
        IJsonSerializer GetSerializer(Type type);
        IJsonSerializer<T> GetSerializer<T>();
    }


}