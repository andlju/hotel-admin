using System;
using FakeItEasy;
using HotelAdmin.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests.Infrastructure
{
    [TestClass]
    public class When_Getting_The_AggregateId_Of_A_Mapped_Identity : With_Repository<IdentityMap, IIdentityMapRepository>
    {
        public class TestDomain
        {

        }

        private readonly long _modelId = 42;
        private readonly Guid _aggregateId = Guid.NewGuid();
        private Guid _returnedId;

        private IdentityMapper _identityMapper;

        protected override void Given()
        {
            A.CallTo(() => RepositoryFake.Get(null)).WithAnyArguments().Returns(new IdentityMap() { AggregateId = _aggregateId, ModelId = _modelId, TypeName = "TestDomain" });

            _identityMapper = new IdentityMapper(ObjectContextFake, RepositoryFake);
        }

        protected override void When()
        {
            _returnedId = _identityMapper.GetAggregateId<TestDomain>(_modelId);
        }

        [TestMethod]
        public void Then_Returned_Id_Is_Correct()
        {
            Assert.AreEqual(_aggregateId, _returnedId);
        }
    }
}