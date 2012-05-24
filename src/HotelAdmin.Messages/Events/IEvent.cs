using System;

namespace HotelAdmin.Messages.Events
{
    public interface IEvent : IMessage
    {
        Guid AggregateId { get; }
    }
}