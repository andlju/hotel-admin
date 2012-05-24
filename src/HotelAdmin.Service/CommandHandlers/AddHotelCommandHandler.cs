using System.Collections.Generic;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using Petite;

namespace HotelAdmin.Service.CommandHandlers
{
    public class AddHotelCommandHandler : IMessageHandler<AddHotelCommand>
    {
        private readonly IObjectContext _objectContext;
        private readonly IHotelRepository _hotelRepository;
        private readonly IIdentityMapper _identityMapper;

        public AddHotelCommandHandler(IObjectContext objectContext, IHotelRepository hotelRepository, IIdentityMapper identityMapper)
        {
            _objectContext = objectContext;
            _hotelRepository = hotelRepository;
            _identityMapper = identityMapper;
        }

        public void Handle(AddHotelCommand message, IDictionary<string, object> metaData)
        {
            var hotel = new Hotel()
                            {
                                Name = message.Name, 
                                ResortName = message.ResortName, 
                                Description = message.Description, 
                                Image = message.ImageUrl, 
                                Latitude = message.Latitude, 
                                Longitude = message.Latitude
                            };
            _hotelRepository.Add(hotel);
            _objectContext.SaveChanges();
            
            var id = hotel.Id;
            _identityMapper.Map<Hotel>(id, message.HotelAggregateId);
        }
    }
}