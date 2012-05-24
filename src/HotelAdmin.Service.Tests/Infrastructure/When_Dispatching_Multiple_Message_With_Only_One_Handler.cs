using HotelAdmin.Service.Infrastructure;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests.Infrastructure
{
    [TestClass]
    public class When_Dispatching_Multiple_Message_With_Only_One_Handler : With_Dispatcher
    {
        private GenericMessageHandler<SimpleTestMessage> _handler = new GenericMessageHandler<SimpleTestMessage>();

        protected override void Given(MessageDispatcher dispatcher)
        {
            dispatcher.RegisterHandler(_handler);
        }

        protected override void When(IMessageDispatcher dispatcher)
        {
            dispatcher.Dispatch(new SimpleTestMessage() { TestString = "Test0" });
            dispatcher.Dispatch(new SimpleTestMessage() { TestString = "Test1" });
            dispatcher.Dispatch(new SimpleTestMessage() { TestString = "Test2" });
        }

        [TestMethod]
        public void Then_Three_Messages_Are_Dispatched()
        {
            Assert.AreEqual(3, _handler.RecievedMessages.Count);
        }

        [TestMethod]
        public void Then_Message0_Is_Correct()
        {
            Assert.AreEqual("Test0", _handler.RecievedMessages[0].TestString);
        }

        [TestMethod]
        public void Then_Message1_Is_Correct()
        {
            Assert.AreEqual("Test1", _handler.RecievedMessages[1].TestString);
        }

        [TestMethod]
        public void Then_Message2_Is_Correct()
        {
            Assert.AreEqual("Test2", _handler.RecievedMessages[2].TestString);
        }
    }
}