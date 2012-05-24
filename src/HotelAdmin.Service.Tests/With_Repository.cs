using System;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Petite;

namespace HotelAdmin.Service.Tests
{
    [TestClass]
    public abstract class With_Repository<TDomain, TRepository>
        where TDomain : class
        where TRepository : IRepository<TDomain>
    {

        protected TRepository RepositoryFake { get; private set; }
        protected IObjectContext ObjectContextFake { get; private set; }

        protected Exception ThrownException { get; private set; }

        [TestInitialize]
        public void Initialize()
        {
            ObjectContextFake = A.Fake<IObjectContext>();
            RepositoryFake = A.Fake<TRepository>();

            Given();
            try
            {
                When();
            }
            catch(Exception ex)
            {
                ThrownException = ex;
            }
        }

        protected abstract void Given();
        protected abstract void When();

    }
}