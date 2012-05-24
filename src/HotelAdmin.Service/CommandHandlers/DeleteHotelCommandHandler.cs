using System;
using System.Collections.Generic;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Messages.Events;
using HotelAdmin.Service.Infrastructure;
using Petite;

namespace HotelAdmin.Service.CommandHandlers
{
    public class DeleteHotelCommandHandler : IMessageHandler<DeleteHotelCommand>
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IIdentityMapper _identityMapper;
        private readonly IEventStorage _eventStorage;

        public DeleteHotelCommandHandler(IHotelRepository hotelRepository, IIdentityMapper identityMapper, IEventStorage eventStorage)
        {
            _hotelRepository = hotelRepository;
            _identityMapper = identityMapper;
            _eventStorage = eventStorage;
        }

        public void Handle(DeleteHotelCommand message, IDictionary<string, object> metaData)
        {
            var modelId = _identityMapper.GetModelId<Hotel>(message.HotelAggregateId);
            var hotel = _hotelRepository.Get(h => h.Id == modelId);
            if (hotel == null)
                throw new InvalidOperationException(string.Format("No hotel found with id {0}", modelId));

            _eventStorage.Store(new HotelDeletedEvent()
                                    {
                                        HotelAggregateId = message.HotelAggregateId
                                    });
        }
    }
}