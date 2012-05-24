using System;
using FakeItEasy;
using HotelAdmin.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests
{
    [TestClass]
    public class When_Setting_Hotel_Facts_On_Nonexisting_Hotel : With_Repository<Hotel, IHotelRepository>
    {
        private const int HotelId = 42;
        private HotelService _hotelService;

        protected override void Given()
        {
            _hotelService = new HotelService(ObjectContextFake, RepositoryFake, null);

            A.CallTo(() => RepositoryFake.Get(null)).WithAnyArguments().Returns(null);
        }

        protected override void When()
        {
            _hotelService.SetHotelFacts(HotelId, new[]
                                                     {
                                                         new FactModel(1, "OnBeach", "", true, ""),
                                                         new FactModel(2, "DistanceToTownCenter", "", true, "1200m"),
                                                         new FactModel(10, "InternetInRooms", "", true, "€2 per day/€10 per week"), 
                                                     });
        }

        [TestMethod]
        public void Then_InvalidOperationException_Is_Thrown()
        {
            Assert.IsInstanceOfType(ThrownException, typeof(InvalidOperationException));
        }

        [TestMethod]
        public void Then_Changes_Are_Not_Saved()
        {
            A.CallTo(() => ObjectContextFake.SaveChanges()).MustNotHaveHappened();
        }
    }
}