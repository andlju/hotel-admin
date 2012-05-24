using System;
using System.Collections.ObjectModel;
using FakeItEasy;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Messages.Events;
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
        public void Then_Exactly_One_Event_Is_Stored()
        {
            AssertEvents.NumberOfEvents(1);
        }

        [TestMethod]
        public void Then_HotelFactsSetEvent_Is_Stored()
        {
            AssertEvents.IsType<HotelFactsSetEvent>(0);
        }

        [TestMethod]
        public void Then_Contents_Of_Event_Is_Correct()
        {
            AssertEvents.Contents<HotelFactsSetEvent>(0, e =>
            {
                Assert.AreEqual(_hotelAggregateId, e.AggregateId);
                
                Assert.AreEqual(_fact1AggregateId, e.Facts[0].FactTypeAggregateId);
                Assert.AreEqual("", e.Facts[0].Details);
                Assert.AreEqual(true, e.Facts[0].Value);

                Assert.AreEqual(_fact2AggregateId, e.Facts[1].FactTypeAggregateId);
                Assert.AreEqual("1200m", e.Facts[1].Details);
                Assert.AreEqual(true, e.Facts[1].Value);

                Assert.AreEqual(_fact3AggregateId, e.Facts[2].FactTypeAggregateId);
                Assert.AreEqual("€2 per day/€10 per week", e.Facts[2].Details);
                Assert.AreEqual(true, e.Facts[2].Value);

            });
        }

    }
}