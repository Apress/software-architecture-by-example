using CashDesk.DataPersistence;
using CashDesk.Server.Entities;
using CashDesk.Server.EventTypes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CashDesk.Server.Persistence
{
    public class PersistData<TEventStreamBase> where TEventStreamBase : EventStreamBase, new()
    {
        private readonly IDataEventsPersistence _dataEvents;

        public PersistData(IDataEventsPersistence dataEvents)
        {
            _dataEvents = dataEvents;
        }
        
        public void Save<TDataEvent, TEvent>(TEventStreamBase eventStream) 
            where TDataEvent : IDataEvent, new()
            where TEvent : IEvent
        {            
            var changes = eventStream.Changes;
            if (changes == null || !changes.Any())
            {
                return;
            }
            
            var dataStream = new List<TDataEvent>();
            foreach (var change in changes)
            {
                var eventChange = (TEvent)change;
                if (!eventChange.IsNew) continue;

                var dataEvent = new TDataEvent()
                {
                    Transaction = eventChange.Transaction,
                    EventType = typeof(TDataEvent).AssemblyQualifiedName                    
                };
                dataStream.Add(dataEvent);
            }
            var saveChanges = new SaveChanges()
            {
                Changes = dataStream.Select(a => (object)a).ToList(),
                StreamName = eventStream.StreamName
            };

            var dataStreamSerialised = JsonConvert.SerializeObject(saveChanges);            

            _dataEvents.Append(eventStream.StreamName, dataStreamSerialised);            
        }

        public TEventStreamBase Load(string streamName)
        {
            var dataEventsSerialised = _dataEvents.Read(streamName);
            var dataEvents = new List<TEventStreamBase>();

            foreach (var dataEventSerialised in dataEventsSerialised)
            {
                var dataEvent = JsonConvert.DeserializeObject<SaveChanges>(dataEventSerialised);
                dataEvents.Add(new TEventStreamBase()
                {
                    Changes = dataEvent.Changes,
                    StreamName = dataEvent.StreamName
                });
            }

            if (dataEvents == null || !dataEvents.Any() 
                || (!dataEvents.Where(a => a?.Changes?.Any() ?? false)?.Any() ?? false))
            {
                var newStream = new TEventStreamBase()
                {
                    StreamName = streamName
                };

                return newStream;
            }

            var eventStream = new TEventStreamBase()
            {
                StreamName = dataEvents.First().StreamName
            };

            foreach (var dataEvent in dataEvents)
            {
                foreach (var eachEvent in dataEvent.Changes)
                {
                    var obj = JObject.Parse(eachEvent.ToString());
                    var eventType = obj["EventType"].Value<string>();
                    var type = Type.GetType(eventType);

                    if (type == typeof(DataCashDeskTransactionAddedEvent))
                    {
                        // https://www.pmichaels.net/tag/type-is-an-interface-or-abstract-class-and-cannot-be-instantiated/
                        var settings = new JsonSerializerSettings();
                        settings.Converters.Add(new CashDeskTransactionConverter());

                        var deserialisedObject = JsonConvert.DeserializeObject(eachEvent.ToString(), type, settings);

                        eventStream.Apply(deserialisedObject);                        
                    }
                }
            }
            return eventStream;
        }
    }
}
