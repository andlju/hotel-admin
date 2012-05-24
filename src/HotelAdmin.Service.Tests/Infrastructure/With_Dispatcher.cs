using System.Collections.Generic;
using HotelAdmin.Messages;
using HotelAdmin.Service.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests.Infrastructure
{
    [TestClass]
    public abstract class With_Dispatcher
    {
        protected abstract void Given(MessageDispatcher dispatcher);
        protected abstract void When(IMessageDispatcher dispatcher);

        private MessageDispatcher _dispatcher;

        [TestInitialize]
        public void Run()
        {
            _dispatcher = new MessageDispatcher();

            Given(_dispatcher);

            When(_dispatcher);
        }
    }

    class SimpleTestMessage : IMessage
    {
        public string TestString;
    }

    class OtherTestMessage : IMessage
    {
        public int TestInt;
    }

    class GenericMessageHandler<T> : IMessageHandler<T> where T : IMessage
    {
        public List<T> RecievedMessages = new List<T>();

        public void Handle(T message, IDictionary<string, object> metaData = null)
        {
            RecievedMessages.Add(message);
        }
    }
}