using System.Linq;
using HotelAdmin.Service.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests.Infrastructure
{
    [TestClass]
    public class When_Dispatching_A_Single_Message_With_Only_One_Handler : With_Dispatcher
    {
        private GenericMessageHandler<SimpleTestMessage> _handler = new GenericMessageHandler<SimpleTestMessage>();
        
        protected override void Given(MessageDispatcher dispatcher)
        {
            dispatcher.RegisterHandler(_handler);
        }

        protected override void When(IMessageDispatcher dispatcher)
        {
            dispatcher.Dispatch(new SimpleTestMessage() { TestString = "Test" });
        }

        [TestMethod]
        public void Then_A_Message_Is_Dispatched()
        {
            Assert.IsTrue(_handler.RecievedMessages.Any());
        }

        [TestMethod]
        public void Then_Only_One_Message_Is_Dispatched()
        {
            Assert.AreEqual(1, _handler.RecievedMessages.Count);
        }

        [TestMethod]
        public void Then_Dispatched_Message_Is_Correct()
        {
            Assert.AreEqual("Test", _handler.RecievedMessages[0].TestString);
        }
    }
}