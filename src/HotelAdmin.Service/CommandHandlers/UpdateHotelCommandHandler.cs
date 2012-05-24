using System;
using System.Collections.Generic;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Messages.Events;
using HotelAdmin.Service.Infrastructure;
using Petite;

namespace HotelAdmin.Service.CommandHandlers
{
    public class UpdateHotelCommandHandler : IMessageHandler<UpdateHotelCommand>
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IIdentityMapper _identityMapper;
        private readonly IEventStorage _eventStorage;

        public UpdateHotelCommandHandler(IHotelRepository hotelRepository, IIdentityMapper identityMapper, IEventStorage eventStorage)
        {
            _hotelRepository = hotelRepository;
            _identityMapper = identityMapper;
            _eventStorage = eventStorage;
        }

        public void Handle(UpdateHotelCommand message, IDictionary<string, object> metaData)
        {
            var modelId = _identityMapper.GetModelId<Hotel>(message.HotelAggregateId);

            var hotel = _hotelRepository.Get(h => h.Id == modelId);
            if (hotel == null)
                throw new InvalidOperationException(string.Format("No hotel found with id {0}", message.HotelAggregateId));

            _eventStorage.Store(new HotelUpdatedEvent()
                                    {
                                        HotelAggregateId = message.HotelAggregateId,
                                        Name = message.Name,
                                        Description = message.Description,
                                        ImageUrl = message.ImageUrl
                                    });
        }
    }
}