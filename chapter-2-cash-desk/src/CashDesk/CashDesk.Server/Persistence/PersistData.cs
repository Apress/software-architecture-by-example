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
    public class PersistData<EventStreamBaseT> where EventStreamBaseT : EventStreamBase, new()
    {
        private readonly IDataEventsPersistence _dataEvents;

        public PersistData(IDataEventsPersistence dataEvents)
        {
            _dataEvents = dataEvents;
        }
        
        public void Save(EventStreamBaseT eventStream)
        {            
            var changes = eventStream.Changes;
            if (changes == null || !changes.Any())
            {
                return;
            }
            
            var dataStream = new List<DataCashDeskTransactionAddedEvent>();
            foreach (var change in changes)
            {
                if (!((CashDeskTransactionAddedEvent)change).IsNew) continue;

                var dataEvent = new DataCashDeskTransactionAddedEvent()
                {
                    Transaction = ((CashDeskTransactionAddedEvent)change).Transaction,
                    EventType = typeof(DataCashDeskTransactionAddedEvent).AssemblyQualifiedName                    
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

        public EventStreamBaseT Load(string streamName)
        {
            var dataEventsSerialised = _dataEvents.Read(streamName);
            var dataEvents = new List<EventStreamBaseT>();

            foreach (var dataEventSerialised in dataEventsSerialised)
            {
                var dataEvent = JsonConvert.DeserializeObject<SaveChanges>(dataEventSerialised);
                dataEvents.Add(new EventStreamBaseT()
                {
                    Changes = dataEvent.Changes,
                    StreamName = dataEvent.StreamName
                });
            }

            if (dataEvents == null || !dataEvents.Any() 
                || (!dataEvents.Where(a => a?.Changes?.Any() ?? false)?.Any() ?? false))
            {
                var newStream = new EventStreamBaseT()
                {
                    StreamName = streamName
                };

                return newStream;
            }

            var eventStream = new EventStreamBaseT()
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

                    // https://www.pmichaels.net/tag/type-is-an-interface-or-abstract-class-and-cannot-be-instantiated/
                    var settings = new JsonSerializerSettings();
                    settings.Converters.Add(new CashDeskTransactionConverter());

                    var deserialisedObject = JsonConvert.DeserializeObject(eachEvent.ToString(), type, settings);

                    eventStream.Apply(deserialisedObject);
                }
            }
            return eventStream;
        }
    }
}
