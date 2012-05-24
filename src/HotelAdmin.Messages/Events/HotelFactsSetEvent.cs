using System;

namespace HotelAdmin.Messages.Events
{
    public class HotelFactsSetEvent : IEvent
    {
        public class Fact
        {
            public Guid FactTypeAggregateId { get; set; }
            public bool Value { get; set; }
            public string Details { get; set; }
        }

        public Guid HotelAggregateId { get; set; }

        public Guid AggregateId
        {
            get { return HotelAggregateId; }
        }

        public Fact[] Facts { get; set; }
    }
}