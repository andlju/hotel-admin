using System;
using System.Collections.ObjectModel;
using FakeItEasy;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Events;
using HotelAdmin.Service.EventHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests.EventHandlers.Hotels
{
    [TestClass]
    public class When_Handling_HotelDeletedEvent : With_EventHandler<HotelDeletedEvent, Hotel, IHotelRepository>
    {
        private readonly int _hotelId = 42;
        private readonly Guid _hotelAggregatedId = Guid.NewGuid();
        private Hotel _hotel;

        protected override IMessageHandler<HotelDeletedEvent> Given()
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

            return new HotelEventHandlers(ObjectContextFake, RepositoryFake, IdentityMapperFake);
        }

        protected override HotelDeletedEvent When()
        {
            return new HotelDeletedEvent()
            {
                HotelAggregateId = _hotelAggregatedId
            };
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

    [TestClass]
    public class When_Handling_HotelFactsSetEvent : With_EventHandler<HotelFactsSetEvent, Hotel, IHotelRepository>
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

        protected override IMessageHandler<HotelFactsSetEvent> Given()
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

            return new HotelEventHandlers(ObjectContextFake, RepositoryFake, IdentityMapperFake);
        }

        protected override HotelFactsSetEvent When()
        {
            return new HotelFactsSetEvent()
                       {
                           HotelAggregateId = _hotelAggregateId,
                           Facts = new[]
                                       {
                                           new HotelFactsSetEvent.Fact { FactTypeAggregateId = _fact1AggregateId, Value = true, Details = "" },
                                           new HotelFactsSetEvent.Fact { FactTypeAggregateId = _fact2AggregateId, Value = true, Details = "1200m"},
                                           new HotelFactsSetEvent.Fact { FactTypeAggregateId = _fact3AggregateId, Value = true, Details = "€2 per day/€10 per week" }, 
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