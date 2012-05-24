using System.Collections.ObjectModel;
using FakeItEasy;
using HotelAdmin.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests
{
    [TestClass]
    public class When_Setting_Hotel_Facts_On_Existing_Hotel : With_Repository<Hotel, IHotelRepository>
    {
        private const int HotelId = 42;
        private HotelService _hotelService;
        private Hotel _hotel;

        protected override void Given()
        {
            _hotelService = new HotelService(ObjectContextFake, RepositoryFake, null);
            _hotel = new Hotel()
                         {
                             Id = HotelId,
                             Name = null,
                             Description = null,
                             Facts = new Collection<Fact>()
                         };

            A.CallTo(() => RepositoryFake.Get(null)).WithAnyArguments().Returns(_hotel);
        }

        protected override void When()
        {
            _hotelService.SetHotelFacts(HotelId, new []
                                                     {
                                                         new FactModel(1, "OnBeach", "", true, ""),
                                                         new FactModel(2, "DistanceToTownCenter", "", true, "1200m"),
                                                         new FactModel(10, "InternetInRooms", "", true, "€2 per day/€10 per week"), 
                                                     } );
        }

        [TestMethod]
        public void Then_Correct_Number_Of_Facts_Are_Set()
        {
            Assert.AreEqual(3, _hotel.Facts.Count);
        }

        [TestMethod]
        public void Then_Changes_Are_Saved()
        {
            A.CallTo(() => ObjectContextFake.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}