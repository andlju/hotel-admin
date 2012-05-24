using FakeItEasy;
using HotelAdmin.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests
{
    [TestClass]
    public class When_Updating_An_Existing_Hotel : With_Repository<Hotel, IHotelRepository>
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
                             Name = "Test Beach Hotel",
                             Description = "A nice hotel situated right at Test Beach",
                             Image = "http://test.com/test.jpg"
                         };

            A.CallTo(() => RepositoryFake.Get(null)).WithAnyArguments().Returns(_hotel);
        }

        protected override void When()
        {
            _hotelService.UpdateHotel(HotelId, "Test Beach Resort", "A nice resort situated right at Test Beach", "http://test.com/testar.jpg");
        }

        [TestMethod]
        public void Then_Hotel_Name_Is_Updated()
        {
            Assert.AreEqual("Test Beach Resort", _hotel.Name);
        }

        [TestMethod]
        public void Then_Hotel_Description_Is_Updated()
        {
            Assert.AreEqual("A nice resort situated right at Test Beach", _hotel.Description);
        }

        [TestMethod]
        public void Then_Hotel_Image_Is_Updated()
        {
            Assert.AreEqual("http://test.com/testar.jpg", _hotel.Image);
        }

        [TestMethod]
        public void Then_Changes_Are_Saved()
        {
            A.CallTo(() => ObjectContextFake.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}