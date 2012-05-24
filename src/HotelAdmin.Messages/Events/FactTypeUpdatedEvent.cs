using System;

namespace HotelAdmin.Messages.Events
{
    public class FactTypeUpdatedEvent : IEvent
    {
        public Guid FactTypeAggregateId { get; set; }

        public Guid AggregateId
        {
            get { return FactTypeAggregateId; }
        }

        public string Code { get; set; }
        public string Name { get; set; }
    }
}