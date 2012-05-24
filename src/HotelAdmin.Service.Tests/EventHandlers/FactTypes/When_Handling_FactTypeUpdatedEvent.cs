using System;
using FakeItEasy;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Events;
using HotelAdmin.Service.EventHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests.EventHandlers.FactTypeAdded
{
    [TestClass]
    public class When_Handling_FactTypeUpdatedEvent : With_EventHandler<FactTypeUpdatedEvent, FactType, IFactTypeRepository>
    {
        private readonly int _factTypeId = 42;
        private readonly Guid _factTypeAggregatedId = Guid.NewGuid();

        private FactType _factType;

        protected override FactTypeUpdatedEvent When()
        {
            return new FactTypeUpdatedEvent()
                       {
                           FactTypeAggregateId = _factTypeAggregatedId,
                           Code = "AdultPool",
                           Name = "Adult pool",
                       };
        }

        protected override IMessageHandler<FactTypeUpdatedEvent> Given()
        {
            _factType = new FactType()
                            {
                                Code = "ChildrensPool",
                                Name = "Childrens pool",
                            };

            A.CallTo(() => IdentityMapperFake.GetModelId<FactType>(_factTypeAggregatedId)).Returns(_factTypeId);
            A.CallTo(() => RepositoryFake.Get(null)).WithAnyArguments().Returns(_factType);
            
            return new FactTypeEventHandlers(ObjectContextFake, RepositoryFake, IdentityMapperFake);
        }

        [TestMethod]
        public void Then_FactType_Name_Is_Updated()
        {
            Assert.AreEqual("Adult pool", _factType.Name);
        }

        [TestMethod]
        public void Then_FactType_Code_Is_Updated()
        {
            Assert.AreEqual("AdultPool", _factType.Code);
        }

        [TestMethod]
        public void Then_Changes_Are_Saved()
        {
            A.CallTo(() => ObjectContextFake.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

    }
}