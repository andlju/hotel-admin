using System.Linq;
using HotelAdmin.Service.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests.Infrastructure
{
    [TestClass]
    public class When_Dispatching_A_Single_Message_With_Multiple_Handlers : With_Dispatcher
    {
        private GenericMessageHandler<SimpleTestMessage> _simpleHandler = new GenericMessageHandler<SimpleTestMessage>();
        private GenericMessageHandler<OtherTestMessage> _otherHandler = new GenericMessageHandler<OtherTestMessage>();

        protected override void Given(MessageDispatcher dispatcher)
        {
            dispatcher.RegisterHandler(_otherHandler);
            dispatcher.RegisterHandler(_simpleHandler);
        }

        protected override void When(IMessageDispatcher dispatcher)
        {
            dispatcher.Dispatch(new SimpleTestMessage() { TestString = "Test" });
        }

        [TestMethod]
        public void Then_A_Message_Is_Dispatched()
        {
            Assert.IsTrue(_simpleHandler.RecievedMessages.Any());
        }

        [TestMethod]
        public void Then_No_Message_Is_Dispatched_To_Other_Handler()
        {
            Assert.IsFalse(_otherHandler.RecievedMessages.Any());
        }

        [TestMethod]
        public void Then_Only_One_Message_Is_Dispatched()
        {
            Assert.AreEqual(1, _simpleHandler.RecievedMessages.Count);
        }

        [TestMethod]
        public void Then_Dispatched_Message_Is_Correct()
        {
            Assert.AreEqual("Test", _simpleHandler.RecievedMessages[0].TestString);
        }
    }
}