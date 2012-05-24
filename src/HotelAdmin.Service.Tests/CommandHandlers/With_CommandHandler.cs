using System;
using FakeItEasy;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Messages.Events;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petite;

namespace HotelAdmin.Service.Tests.CommandHandlers
{
    [TestClass]
    public abstract class With_CommandHandler<TCommand, TDomain, TRepository> 
        where TCommand : ICommand
        where TDomain : class
        where TRepository : IRepository<TDomain>
    {
        protected TRepository RepositoryFake { get; private set; }
        protected IIdentityMapper IdentityMapperFake { get; private set; }
        protected IObjectContext ObjectContextFake { get; private set; }
        protected MockEventStorage EventStorage { get; private set; }

        protected Exception ThrownException { get; private set; }

        [TestInitialize]
        public void Initialize()
        {
            ObjectContextFake = A.Fake<IObjectContext>();
            RepositoryFake = A.Fake<TRepository>();
            IdentityMapperFake = A.Fake<IIdentityMapper>();
            EventStorage = new MockEventStorage();

            try
            {
                IMessageHandler<TCommand> handler = Given();

                var command = When();
                
                handler.Handle(command);
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
        }

        protected abstract TCommand When();
        protected abstract IMessageHandler<TCommand> Given();

        protected EventAsserts AssertEvents
        {
            get { return new EventAsserts(EventStorage.Events); }
        }

        public class EventAsserts
        {
            private readonly IEvent[] _events;

            public EventAsserts(IEvent[] events)
            {
                _events = events;
            }

            public void NumberOfEvents(int expected)
            {
                Assert.AreEqual(expected, _events.Length, string.Format("Wrong number of events. Expected {0}, got {1}", expected, _events.Length));
            }

            public void IsType<T>(int index)
                where T : class, IEvent
            {
                Assert.IsTrue(_events.Length > index, string.Format("No event with index {0}", index));
                Assert.IsInstanceOfType(_events[index], typeof (T));
            }

            public void Contents<T>(int index, Action<T> assertions)
                where T : class, IEvent
            {
                Assert.IsTrue(_events.Length > index, string.Format("No event with index {0}", index));
                var evt = _events[index] as T;
                Assert.IsNotNull(evt, string.Format("Expected event with index {0} to be of type {1}, got {2}", index, typeof(T), _events[index].GetType()));

                assertions(evt);
            }
        }
    }
}