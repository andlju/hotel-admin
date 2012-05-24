using System;
using FakeItEasy;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Service.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests.CommandHandlers.AddFactType
{
    [TestClass]
    public class When_Handling_AddFactTypeCommand : With_CommandHandler<AddFactTypeCommand, FactType, IFactTypeRepository>
    {
        private readonly int _factTypeId = 42;
        private readonly Guid _factTypeAggregatedId = Guid.NewGuid();

        protected override IMessageHandler<AddFactTypeCommand> Given()
        {
            A.CallTo(() => RepositoryFake.Add(null)).WithAnyArguments().
                Invokes(call => ((FactType)call.Arguments[0]).Id = _factTypeId);

            return new AddFactTypeCommandHandler(ObjectContextFake, RepositoryFake, IdentityMapperFake);
        }

        protected override AddFactTypeCommand When()
        {
            return new AddFactTypeCommand()
                       {
                           FactTypeAggregateId = _factTypeAggregatedId,
                           Code = "ChildrensPool",
                           Name = "Childrens pool",
                       };
        }

        [TestMethod]
        public void Then_Aggregate_Id_Is_Mapped()
        {
            A.CallTo(() => IdentityMapperFake.Map<FactType>(_factTypeId, _factTypeAggregatedId)).MustHaveHappened();
        }

        [TestMethod]
        public void Then_Changes_Are_Saved()
        {
            A.CallTo(() => ObjectContextFake.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void Then_Name_Of_Added_FactType_Is_Correct()
        {
            A.CallTo(() => RepositoryFake.Add(null)).WhenArgumentsMatch(a => a.Get<FactType>(0).Name == "Childrens pool").MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void Then_Code_Of_Added_Hotel_Is_Correct()
        {
            A.CallTo(() => RepositoryFake.Add(null)).WhenArgumentsMatch(a => a.Get<FactType>(0).Code == "ChildrensPool").MustHaveHappened(Repeated.Exactly.Once);
        }

    }
}