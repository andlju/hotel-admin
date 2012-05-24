using System;
using FakeItEasy;
using HotelAdmin.Messages.Commands;
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

        protected Exception ThrownException { get; private set; }

        [TestInitialize]
        public void Initialize()
        {
            ObjectContextFake = A.Fake<IObjectContext>();
            RepositoryFake = A.Fake<TRepository>();
            IdentityMapperFake = A.Fake<IIdentityMapper>();

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
    }
}