using System;
using System.Collections.Generic;
using System.Linq;
using EventStore;
using HotelAdmin.Messages.Events;

namespace HotelAdmin.Service.Infrastructure
{
    public interface IEventStorage
    {
        void Store(IEvent evt, IDictionary<string, object> metaData = null);
    }

    public class EventStoreEventStorage : IEventStorage
    {
        private readonly IStoreEvents _store;

        public EventStoreEventStorage()
        {
            _store = Wireup.Init().
                UsingSqlPersistence("HotelAdminEventStore").
                InitializeStorageEngine().
                UsingServiceStackJsonSerialization().
                
                Build();
        }

        public void Store(IEvent evt, IDictionary<string, object> metaData)
        {
            using (var stream = _store.OpenStream(evt.AggregateId, 0, int.MaxValue))
            {
                var evtMessage = new EventMessage() {Body = evt};
                if (metaData != null)
                {
                    foreach (var item in metaData)
                    {
                        evtMessage.Headers[item.Key] = item.Value;
                    }
                }
                // HACK This should probably be earlier in the chain
                evtMessage.Headers["Timestamp"] = DateTime.Now;
                stream.Add(evtMessage);
                
                stream.CommitChanges(Guid.NewGuid());
            }

        }
    }
    
}