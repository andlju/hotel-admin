using System;
using FakeItEasy;
using HotelAdmin.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests.Infrastructure
{
    [TestClass]
    public class When_Mapping_An_Identity : With_Repository<IdentityMap, IIdentityMapRepository>
    {
        public class TestDomain
        {
            
        }

        private readonly long _modelId = 42;
        private readonly Guid _aggregateId = Guid.NewGuid();

        private IdentityMapper _identityMapper;

        protected override void Given()
        {
            A.CallTo(() => RepositoryFake.Get(null)).WithAnyArguments().Returns(null);

            _identityMapper = new IdentityMapper(ObjectContextFake, RepositoryFake);
        }

        protected override void When()
        {
            _identityMapper.Map<TestDomain>(_modelId, _aggregateId);
        }

        [TestMethod]
        public void Then_New_Identity_Is_Added()
        {
            A.CallTo(() => RepositoryFake.Add(null)).WhenArgumentsMatch(a => a.Get<IdentityMap>(0) != null).MustHaveHappened();
        }

        [TestMethod]
        public void Then_ModelId_Added_Is_Correct()
        {
            A.CallTo(() => RepositoryFake.Add(null)).WhenArgumentsMatch(a => a.Get<IdentityMap>(0).ModelId == _modelId).MustHaveHappened();
        }

        [TestMethod]
        public void Then_AggregateId_Added_Is_Correct()
        {
            A.CallTo(() => RepositoryFake.Add(null)).WhenArgumentsMatch(a => a.Get<IdentityMap>(0).AggregateId == _aggregateId).MustHaveHappened();
        }

        [TestMethod]
        public void Then_TypeName_Added_Is_Correct()
        {
            A.CallTo(() => RepositoryFake.Add(null)).WhenArgumentsMatch(a => a.Get<IdentityMap>(0).TypeName == "TestDomain").MustHaveHappened();
        }

        [TestMethod]
        public void Then_Changes_Are_Saved()
        {
            A.CallTo(() => ObjectContextFake.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

    }
}