using System;
using System.Collections.Generic;
using Common.Logging;
using HotelAdmin.Messages;

namespace HotelAdmin.Service.Infrastructure
{
    public class MessageDispatcher : IMessageDispatcher
    {
        private readonly ILog _log;
        private readonly IDictionary<Type, List<object>> _handlers = new Dictionary<Type, List<object>>();

        public MessageDispatcher()
        {
            _log = LogManager.GetCurrentClassLogger();
        }

        public void RegisterHandler<T>(IMessageHandler<T> handler) where T : IMessage
        {
            List<object> handlers;
            if (!_handlers.TryGetValue(typeof(T), out handlers))
            {
                handlers = new List<object>();
                _handlers.Add(typeof(T), handlers);
            }
            handlers.Add(handler);
        }

        public void Dispatch<T>(T message, IDictionary<string, object> metaData) where T : IMessage
        {
            List<object> handlers;
            if (!_handlers.TryGetValue(typeof(T), out handlers))
            {
                _log.Warn(m => m("Dispatched a message with no handler. Message type {0}", typeof(T)));
                return; 
            }
            foreach(object handlerObject in handlers)
            {
                var handler = (IMessageHandler<T>)handlerObject;
                handler.Handle(message, metaData);
            }
        }

        public void DispatchNonGeneric(IMessage message, IDictionary<string, object> metaData = null)
        {
            var dispatchMethod = GetType().GetMethod("Dispatch").MakeGenericMethod(message.GetType());
            dispatchMethod.Invoke(this, new object[]{ message, metaData });
        }
    }
}