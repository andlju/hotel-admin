using FakeItEasy;
using HotelAdmin.Domain;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests
{
    [TestClass]
    public class When_Adding_A_Hotel : With_Repository<Hotel, IHotelRepository>
    {
        private const int HotelId = 42;

        private HotelService _hotelService;
        private int _returnValue;
        
        protected override void Given()
        {
            _hotelService = new HotelService(ObjectContextFake, RepositoryFake, null);

            A.CallTo(() => RepositoryFake.Add(null)).WithAnyArguments().
                Invokes(call => ((Hotel)call.Arguments[0]).Id = HotelId);
        }

        protected override void When()
        {
            _returnValue = _hotelService.AddHotel("Test Beach Hotel", "A nice hotel situated right at Test Beach", "Test Beach", "http://test.com/test.jpg", 0.0f, 0.0f);
        }

        [TestMethod]
        public void Then_Hotel_Is_Added()
        {
            A.CallTo(() => RepositoryFake.Add(null)).WithAnyArguments().MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void Then_Returned_Id_Is_Correct()
        {
            Assert.AreEqual(HotelId, _returnValue);
        }

        [TestMethod]
        public void Then_Changes_Are_Saved()
        {
            A.CallTo(() => ObjectContextFake.SaveChanges()).MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void Then_Name_Of_Added_Hotel_Is_Correct()
        {
            A.CallTo(() => RepositoryFake.Add(null)).WhenArgumentsMatch(a => a.Get<Hotel>(0).Name == "Test Beach Hotel").MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void Then_Description_Of_Added_Hotel_Is_Correct()
        {
            A.CallTo(() => RepositoryFake.Add(null)).WhenArgumentsMatch(a => a.Get<Hotel>(0).Description == "A nice hotel situated right at Test Beach").MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void Then_Image_Of_Added_Hotel_Is_Correct()
        {
            A.CallTo(() => RepositoryFake.Add(null)).WhenArgumentsMatch(a => a.Get<Hotel>(0).Image == "http://test.com/test.jpg").MustHaveHappened(Repeated.Exactly.Once);
        }

    }
}