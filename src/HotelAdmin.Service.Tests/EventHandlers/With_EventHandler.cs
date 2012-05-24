using System;
using FakeItEasy;
using HotelAdmin.Messages.Events;
using HotelAdmin.Service.Tests.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petite;

namespace HotelAdmin.Service.Tests.EventHandlers
{
    public abstract class With_EventHandler<TEvent, TDomain, TRepository> 
        where TEvent : IEvent
        where TDomain : class
        where TRepository : IRepository<TDomain>
    {
        protected TRepository RepositoryFake { get; private set; }
        protected IIdentityMapper IdentityMapperFake { get; private set; }
        protected IObjectContext ObjectContextFake { get; private set; }

        protected Exception ThrownException { get; private set; }

        [TestInitialize]
        public void Initialize()
        {
            ObjectContextFake = A.Fake<IObjectContext>();
            RepositoryFake = A.Fake<TRepository>();
            IdentityMapperFake = A.Fake<IIdentityMapper>();

            try
            {
                IMessageHandler<TEvent> handler = Given();

                var evt = When();

                handler.Handle(evt);
            }
            catch (Exception ex)
            {
                ThrownException = ex;
            }
        }

        protected abstract TEvent When();
        protected abstract IMessageHandler<TEvent> Given();

    }
}