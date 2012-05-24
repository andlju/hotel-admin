using System;

namespace HotelAdmin.Messages.Events
{
    public class HotelDeletedEvent : IEvent
    {
        public Guid HotelAggregateId { get; set; }

        public Guid AggregateId
        {
            get { return HotelAggregateId; }
        }
    }
}