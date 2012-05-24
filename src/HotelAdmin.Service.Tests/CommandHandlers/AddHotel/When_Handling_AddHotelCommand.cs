using System;
using FakeItEasy;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Service.CommandHandlers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace HotelAdmin.Service.Tests.CommandHandlers.AddHotel
{
    [TestClass]
    public class When_Handling_AddHotelCommand : With_CommandHandler<AddHotelCommand, Hotel, IHotelRepository>
    {
        private readonly int _hotelId = 42;
        private readonly Guid _hotelAggregatedId = Guid.NewGuid();

        protected override IMessageHandler<AddHotelCommand> Given()
        {
            A.CallTo(() => RepositoryFake.Add(null)).WithAnyArguments().
                Invokes(call => ((Hotel)call.Arguments[0]).Id = _hotelId);

            return new AddHotelCommandHandler(ObjectContextFake, RepositoryFake, IdentityMapperFake);
        }

        protected override AddHotelCommand When()
        {
            return new AddHotelCommand()
                       {
                           HotelAggregateId = _hotelAggregatedId,
                           Name = "Test Beach Hotel",
                           Description =  "A nice hotel situated right at Test Beach",
                           ResortName = "Test Beach",
                           ImageUrl = "http://test.com/test.jpg",
                           Latitude = 12.0f,
                           Longitude = 32.4f
                       };
        }

        [TestMethod]
        public void Then_Hotel_Is_Added()
        {
            A.CallTo(() => RepositoryFake.Add(null)).WithAnyArguments().MustHaveHappened(Repeated.Exactly.Once);
        }

        [TestMethod]
        public void Then_Aggregate_Id_Is_Mapped()
        {
            A.CallTo(() => IdentityMapperFake.Map<Hotel>(_hotelId, _hotelAggregatedId)).MustHaveHappened();
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