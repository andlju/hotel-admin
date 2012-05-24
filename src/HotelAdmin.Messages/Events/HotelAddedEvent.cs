using System;

namespace HotelAdmin.Messages.Events
{
    public class HotelAddedEvent : IEvent
    {
        public Guid HotelAggregateId { get; set; }

        public Guid AggregateId
        {
            get { return HotelAggregateId; }
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public string ResortName { get; set; }
        public string ImageUrl { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}