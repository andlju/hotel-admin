using System;
using FakeItEasy;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Messages.Events;
using HotelAdmin.Service.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests.CommandHandlers.UpdateHotel
{
    [TestClass]
    public class When_Handling_UpdateHotelCommand : With_CommandHandler<UpdateHotelCommand, Hotel, IHotelRepository>
    {
        private readonly int _hotelId = 42;
        private readonly Guid _hotelAggregatedId = Guid.NewGuid();

        private Hotel _hotel;

        protected override IMessageHandler<UpdateHotelCommand> Given()
        {
            _hotel = new Hotel()
                         {
                             Name = "Test Beach Hotel",
                             Description = "A nice hotel situated right at Test Beach",
                             ResortName = "Test Beach",
                             Image = "http://test.com/test.jpg",
                             Latitude = 12.0f,
                             Longitude = 32.4f
                         };

            A.CallTo(() => IdentityMapperFake.GetModelId<Hotel>(_hotelAggregatedId)).Returns(_hotelId);
            A.CallTo(() => RepositoryFake.Get(null)).WithAnyArguments().Returns(_hotel);

            return new UpdateHotelCommandHandler(RepositoryFake, IdentityMapperFake, EventStorage);
        }

        protected override UpdateHotelCommand When()
        {
            return new UpdateHotelCommand()
            {
                HotelAggregateId = _hotelAggregatedId,
                Name = "Testy Beach Resort",
                Description = "A nice resort situated right at Testy Beach",
                ImageUrl = "http://test.com/testar.jpg",
            };
        }

        [TestMethod]
        public void Then_Exactly_One_Event_Is_Stored()
        {
            AssertEvents.NumberOfEvents(1);
        }

        [TestMethod]
        public void Then_HotelUpdatedEvent_Is_Stored()
        {
            AssertEvents.IsType<HotelUpdatedEvent>(0);
        }

        [TestMethod]
        public void Then_Contents_Of_Event_Is_Correct()
        {
            AssertEvents.Contents<HotelUpdatedEvent>(0, e =>
            {
                Assert.AreEqual(_hotelAggregatedId, e.AggregateId);
                Assert.AreEqual("Testy Beach Resort", e.Name);
                Assert.AreEqual("A nice resort situated right at Testy Beach", e.Description);
                Assert.AreEqual("http://test.com/testar.jpg", e.ImageUrl);
            });
        }
    }
}