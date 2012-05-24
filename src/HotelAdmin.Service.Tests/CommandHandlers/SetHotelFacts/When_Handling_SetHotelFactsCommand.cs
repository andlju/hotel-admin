using System;
using System.Collections.ObjectModel;
using FakeItEasy;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Service.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests.CommandHandlers.SetHotelFacts
{
    [TestClass]
    public class When_Handling_SetHotelFactsCommand : With_CommandHandler<SetHotelFactsCommand, Hotel, IHotelRepository>
    {
        private readonly int _hotelId = 42;
        private readonly Guid _hotelAggregateId = Guid.NewGuid();

        private readonly int _fact1Id = 7;
        private readonly int _fact2Id = 17;
        private readonly int _fact3Id = 117;

        private readonly Guid _fact1AggregateId = Guid.NewGuid();
        private readonly Guid _fact2AggregateId = Guid.NewGuid();
        private readonly Guid _fact3AggregateId = Guid.NewGuid(); 

        private Hotel _hotel;

        protected override IMessageHandler<SetHotelFactsCommand> Given()
        {
            _hotel = new Hotel()
            {
                Id = _hotelId,
                Name = null,
                Description = null,
                Facts = new Collection<Fact>()
            };
            A.CallTo(() => IdentityMapperFake.GetModelId<Hotel>(_hotelAggregateId)).Returns(_hotelId);
            A.CallTo(() => IdentityMapperFake.GetModelId<Hotel>(_fact1AggregateId)).Returns(_fact1Id);
            A.CallTo(() => IdentityMapperFake.GetModelId<Hotel>(_fact2AggregateId)).Returns(_fact2Id);
            A.CallTo(() => IdentityMapperFake.GetModelId<Hotel>(_fact3AggregateId)).Returns(_fact3Id);

            A.CallTo(() => RepositoryFake.Get(null)).WithAnyArguments().Returns(_hotel);

            return new SetHotelFactsCommandHandler(ObjectContextFake, RepositoryFake, IdentityMapperFake);
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
        public void Then_Changes_Are_Saved()
        {
            A.CallTo(() => ObjectContextFake.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void Then_Correct_Number_Of_Facts_Are_Set()
        {
            Assert.AreEqual(3, _hotel.Facts.Count);
        }

    }
}