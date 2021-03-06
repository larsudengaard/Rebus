﻿using System.Text;
using Newtonsoft.Json;
using Rebus.Messages;
using Rebus.Persistence.InMemory;

namespace Rebus.Serialization.Json
{
    /// <summary>
    /// Implementation of <see cref="InMemorySubscriptionStorage"/> that uses
    /// the ubiquitous NewtonSoft JSON serializer to serialize and deserialize messages.
    /// </summary>
    public class JsonMessageSerializer : ISerializeMessages
    {
        static readonly JsonSerializerSettings Settings =
            new JsonSerializerSettings {TypeNameHandling = TypeNameHandling.All};

        static readonly Encoding Encoding = Encoding.UTF8;

        public TransportMessage Serialize(Message message)
        {
            var messageAsString = JsonConvert.SerializeObject(message, Formatting.Indented, Settings);
            
            return new TransportMessage {Data = Encoding.GetBytes(messageAsString)};
        }

        public Message Deserialize(TransportMessage transportMessage)
        {
            var messageAsString = Encoding.GetString(transportMessage.Data);

            return (Message) JsonConvert.DeserializeObject(messageAsString, Settings);
        }
    }
}