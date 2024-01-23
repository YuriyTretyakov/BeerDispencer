using System;
using Confluent.Kafka;
using Newtonsoft.Json;

namespace BeerDispenser.Kafka.Core
{
    public class DefaultJsonDeserializer<T> : IDeserializer<T>
    {

        public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
        {
            var str = System.Text.Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}

