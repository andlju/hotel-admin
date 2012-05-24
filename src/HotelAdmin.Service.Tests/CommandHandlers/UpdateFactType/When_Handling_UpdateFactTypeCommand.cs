using System;
using FakeItEasy;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
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

            return new UpdateFactTypeCommandHandler(ObjectContextFake, RepositoryFake, IdentityMapperFake);
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