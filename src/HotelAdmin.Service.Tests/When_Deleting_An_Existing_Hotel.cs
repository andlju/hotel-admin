using FakeItEasy;
using HotelAdmin.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests
{
    [TestClass]
    public class When_Deleting_An_Existing_Hotel : With_Repository<Hotel, IHotelRepository>
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
            _hotelService.DeleteHotel(HotelId);
        }

        [TestMethod]
        public void Then_Hotel_Is_Deleted()
        {
            A.CallTo(() => RepositoryFake.Delete(null)).WhenArgumentsMatch(ac => ac.Get<Hotel>(0).Id == HotelId).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void Then_Changes_Are_Saved()
        {
            A.CallTo(() => ObjectContextFake.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }
    }
}