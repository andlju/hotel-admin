
using System.Collections.Generic;
using HotelAdmin.Messages;

namespace HotelAdmin.Service
{
    public interface IMessageHandler<TMessage> where TMessage : IMessage
    {
        void Handle(TMessage message, IDictionary<string, object> metaData = null);
    }
}