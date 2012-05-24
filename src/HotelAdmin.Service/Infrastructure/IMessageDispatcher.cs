using System.Collections.Generic;
using HotelAdmin.Messages;

namespace HotelAdmin.Service.Infrastructure
{
    public interface IMessageDispatcher
    {
        void Dispatch<T>(T message, IDictionary<string, object> metaData = null) where T : IMessage;
    }
}