using System.Collections.Generic;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Messages.Events;
using HotelAdmin.Service.Infrastructure;
using Petite;

namespace HotelAdmin.Service.CommandHandlers
{
    public class AddHotelCommandHandler : IMessageHandler<AddHotelCommand>
    {
        private readonly IObjectContext _objectContext;
        private readonly IHotelRepository _hotelRepository;
        private readonly IIdentityMapper _identityMapper;
        private readonly IEventStorage _eventStorage;

        public AddHotelCommandHandler(IObjectContext objectContext, IHotelRepository hotelRepository, IIdentityMapper identityMapper, IEventStorage eventStorage)
        {
            _objectContext = objectContext;
            _hotelRepository = hotelRepository;
            _identityMapper = identityMapper;
            _eventStorage = eventStorage;
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
            
            _eventStorage.Store(new HotelAddedEvent()
                                    {
                                        HotelAggregateId = message.HotelAggregateId,
                                        Name = message.Name,
                                        Description = message.Description,
                                        ResortName = message.ResortName,
                                        ImageUrl = message.ImageUrl,
                                        Latitude = message.Latitude,
                                        Longitude = message.Longitude
                                    });
        }
    }
}