using System;
using FakeItEasy;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Service.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests.CommandHandlers.SetHotelFacts
{
    [TestClass]
    public class When_Handling_SetHotelFactsCommand_On_Nonexisting_Hotel :
        With_CommandHandler<SetHotelFactsCommand, Hotel, IHotelRepository>
    {
        private readonly Guid _fact1AggregateId = Guid.NewGuid();
        private readonly Guid _fact2AggregateId = Guid.NewGuid();
        private readonly Guid _fact3AggregateId = Guid.NewGuid();

        private readonly Guid _hotelAggregateId = Guid.NewGuid();

        protected override IMessageHandler<SetHotelFactsCommand> Given()
        {
            A.CallTo(() => IdentityMapperFake.GetModelId<Hotel>(_hotelAggregateId)).Returns(0);

            A.CallTo(() => RepositoryFake.Get(null)).WithAnyArguments().Returns(null);

            return new SetHotelFactsCommandHandler(RepositoryFake, IdentityMapperFake, EventStorage);
        }

        protected override SetHotelFactsCommand When()
        {
            return new SetHotelFactsCommand()
                       {
                           HotelAggregateId = _hotelAggregateId,
                           Facts = new[]
                                       {
                                           new SetHotelFactsCommand.Fact { FactTypeAggregateId = _fact1AggregateId, Value = true, Details = "" },
                                           new SetHotelFactsCommand.Fact { FactTypeAggregateId = _fact2AggregateId, Value = true, Details = "1200m"},
                                           new SetHotelFactsCommand.Fact { FactTypeAggregateId = _fact3AggregateId, Value = true, Details = "€2 per day/€10 per week" }, 
                                       }
                       };
        }

        [TestMethod]
        public void Then_InvalidOperationException_Is_Thrown()
        {
            Assert.IsInstanceOfType(ThrownException, typeof(InvalidOperationException));
        }
    }
}