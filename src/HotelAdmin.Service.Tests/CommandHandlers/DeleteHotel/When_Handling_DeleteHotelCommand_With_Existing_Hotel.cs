using System;
using FakeItEasy;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Messages.Events;
using HotelAdmin.Service.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests.CommandHandlers.DeleteHotel
{
    [TestClass]
    public class When_Handling_DeleteHotelCommand_With_Existing_Hotel : With_CommandHandler<DeleteHotelCommand, Hotel, IHotelRepository>
    {
        private readonly int _hotelId = 42;
        private readonly Guid _hotelAggregatedId = Guid.NewGuid();
        private Hotel _hotel;

        protected override IMessageHandler<DeleteHotelCommand> Given()
        {
            _hotel = new Hotel()
            {
                Id = _hotelId,
                Name = "Test Beach Hotel",
                Description = "A nice hotel situated right at Test Beach",
                Image = "http://test.com/test.jpg"
            };

            A.CallTo(() => IdentityMapperFake.GetModelId<Hotel>(_hotelAggregatedId)).Returns(_hotelId);
            A.CallTo(() => RepositoryFake.Get(null)).WithAnyArguments().Returns(_hotel);

            return new DeleteHotelCommandHandler(ObjectContextFake, RepositoryFake, IdentityMapperFake, EventStorage);
        }

        protected override DeleteHotelCommand When()
        {
            return new DeleteHotelCommand()
                       {
                           HotelAggregateId = _hotelAggregatedId
                       };
        }

        [TestMethod]
        public void Then_Exactly_One_Event_Is_Stored()
        {
            AssertEvents.NumberOfEvents(1);
        }

        [TestMethod]
        public void Then_HotelDeletedEvent_Is_Stored()
        {
            AssertEvents.IsType<HotelDeletedEvent>(0);
        }

        [TestMethod]
        public void Then_Contents_Of_Event_Is_Correct()
        {
            AssertEvents.Contents<HotelDeletedEvent>(0, e =>
            {
                Assert.AreEqual(_hotelAggregatedId, e.AggregateId);
            });
        }

        [TestMethod]
        public void Then_Changes_Are_Saved()
        {
            A.CallTo(() => ObjectContextFake.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void Then_Hotel_Is_Deleted()
        {
            A.CallTo(() => RepositoryFake.Delete(null)).WhenArgumentsMatch(ac => ac.Get<Hotel>(0).Id == _hotelId).MustHaveHappened(Repeated.Exactly.Once);

        }


    }
}