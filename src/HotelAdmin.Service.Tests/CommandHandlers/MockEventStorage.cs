using System.Collections.Generic;
using HotelAdmin.Messages.Events;
using HotelAdmin.Service.Infrastructure;

namespace HotelAdmin.Service.Tests.CommandHandlers
{
    public class MockEventStorage : IEventStorage
    {
        private readonly List<IEvent> _storedEvents = new List<IEvent>();

        public IEvent[] Events
        {
            get { return _storedEvents.ToArray(); }    
        }

        public void Store(IEvent evt, IDictionary<string, object> metaData)
        {
            _storedEvents.Add(evt);
        }

        public IEnumerable<IEvent> GetAllEvents()
        {
            return _storedEvents;
        }
    }
}