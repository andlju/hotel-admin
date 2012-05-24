using System.Collections.ObjectModel;
using FakeItEasy;
using HotelAdmin.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests
{
    [TestClass]
    public class When_Getting_Hotel_By_Id : With_Repository<Hotel, IHotelRepository>
    {
        private HotelService _hotelService;

        private HotelModel _returnedHotel;

        protected override void Given()
        {
            _hotelService = new HotelService(RepositoryFake, null, null, null);

            A.CallTo(() => RepositoryFake.Get(null)).WithAnyArguments().Returns(
                new Hotel() { Id = 42, Name = "Test Beach Resort", Description = "Nice hotel on the beach", Image = "http://test.com/test.jpg", Facts = new Collection<Fact>()});
        }

        protected override void When()
        {
            _returnedHotel = _hotelService.GetHotel(42);
        }

        [TestMethod]
        public void Then_Hotel_Is_Returned()
        {
            Assert.IsNotNull(_returnedHotel);
        }

        [TestMethod]
        public void Then_Name_Of_Hotel_Is_Correct()
        {
            Assert.AreEqual("Test Beach Resort", _returnedHotel.Name);
        }

        [TestMethod]
        public void Then_Description_Of_Hotel_Is_Correct()
        {
            Assert.AreEqual("Nice hotel on the beach", _returnedHotel.Description);
        }

        [TestMethod]
        public void Then_Image_Of_Hotel_Is_Correct()
        {
            Assert.AreEqual("http://test.com/test.jpg", _returnedHotel.Image);
        }

    }
}