using System;
using FakeItEasy;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Messages.Events;
using HotelAdmin.Service.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests.CommandHandlers.UpdateFactType
{
    [TestClass]
    public class When_Handling_UpdateFactTypeCommand : With_CommandHandler<UpdateFactTypeCommand, FactType, IFactTypeRepository>
    {
        private readonly int _factTypeId = 42;
        private readonly Guid _factTypeAggregatedId = Guid.NewGuid();

        private FactType _factType;

        protected override IMessageHandler<UpdateFactTypeCommand> Given()
        {
            _factType = new FactType()
                         {
                             Code = "ChildrensPool",
                             Name = "Childrens pool",
                         };

            A.CallTo(() => IdentityMapperFake.GetModelId<FactType>(_factTypeAggregatedId)).Returns(_factTypeId);
            A.CallTo(() => RepositoryFake.Get(null)).WithAnyArguments().Returns(_factType);

            return new UpdateFactTypeCommandHandler(RepositoryFake, IdentityMapperFake, EventStorage);
        }

        protected override UpdateFactTypeCommand When()
        {
            return new UpdateFactTypeCommand()
            {
                FactTypeAggregateId = _factTypeAggregatedId,
                Code = "AdultPool",
                Name = "Adult pool",
            };
        }

        [TestMethod]
        public void Then_Exactly_One_Event_Is_Stored()
        {
            AssertEvents.NumberOfEvents(1);
        }

        [TestMethod]
        public void Then_FactTypeUpdatedEvent_Is_Stored()
        {
            AssertEvents.IsType<FactTypeUpdatedEvent>(0);
        }

        [TestMethod]
        public void Then_Contents_Of_Event_Is_Correct()
        {
            AssertEvents.Contents<FactTypeUpdatedEvent>(0, e =>
            {
                Assert.AreEqual(_factTypeAggregatedId, e.AggregateId);
                Assert.AreEqual("Adult pool", e.Name);
                Assert.AreEqual("AdultPool", e.Code);
            });
        }
    }
}