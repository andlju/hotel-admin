using System;
using System.Collections.Generic;
using System.Linq;
using EventStore;
using EventStore.Dispatcher;
using HotelAdmin.Messages;
using HotelAdmin.Messages.Events;

namespace HotelAdmin.Service.Infrastructure
{
    public interface IEventStorage
    {
        void Store(IEvent evt, IDictionary<string, object> metaData = null);
        IEnumerable<IEvent> GetAllEvents();
    }

    public class EventStoreEventStorage : IEventStorage
    {
        private readonly IStoreEvents _store;
        private readonly IMessageDispatcher _eventDispatcher;

        public EventStoreEventStorage(IMessageDispatcher eventDispatcher)
        {
            _eventDispatcher = eventDispatcher;
            _store = Wireup.Init().
                UsingSqlPersistence("HotelAdminEventStore").
                InitializeStorageEngine().
                UsingServiceStackJsonSerialization().
                UsingSynchronousDispatchScheduler(
                    new DelegateMessageDispatcher(c =>
                                                      {
                                                          foreach(var e in c.Events)
                                                          {
                                                              _eventDispatcher.DispatchNonGeneric((IMessage)e.Body);
                                                          }
                                                      })).

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

        public IEnumerable<IEvent> GetAllEvents()
        {
            return _store.Advanced.GetFrom(DateTime.MinValue).SelectMany(c => c.Events).Select(e => (IEvent) e.Body);
        }
    }
    
}