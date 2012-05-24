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
        private readonly IHotelRepository _hotelRepository;
        private readonly IIdentityMapper _identityMapper;
        private readonly IEventStorage _eventStorage;

        public AddHotelCommandHandler(IHotelRepository hotelRepository, IIdentityMapper identityMapper, IEventStorage eventStorage)
        {
            _hotelRepository = hotelRepository;
            _identityMapper = identityMapper;
            _eventStorage = eventStorage;
        }

        public void Handle(AddHotelCommand message, IDictionary<string, object> metaData)
        {
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