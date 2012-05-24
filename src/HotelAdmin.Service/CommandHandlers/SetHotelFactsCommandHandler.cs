using System;
using System.Collections.Generic;
using System.Linq;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Messages.Events;
using HotelAdmin.Service.Infrastructure;
using Petite;

namespace HotelAdmin.Service.CommandHandlers
{
    public class SetHotelFactsCommandHandler : IMessageHandler<SetHotelFactsCommand>
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IIdentityMapper _identityMapper;
        private readonly IEventStorage _eventStorage;

        public SetHotelFactsCommandHandler(IHotelRepository hotelRepository, IIdentityMapper identityMapper, IEventStorage eventStorage)
        {
            _hotelRepository = hotelRepository;
            _identityMapper = identityMapper;
            _eventStorage = eventStorage;
        }

        public void Handle(SetHotelFactsCommand message, IDictionary<string, object> metaData)
        {
            var modelId = _identityMapper.GetModelId<Hotel>(message.HotelAggregateId);
            var hotel = _hotelRepository.Get(h => h.Id == modelId);
            if (hotel == null)
                throw new InvalidOperationException(string.Format("Unknown hotel with Id: {0}", message.HotelAggregateId));

            _eventStorage.Store(new HotelFactsSetEvent()
                                    {
                                        HotelAggregateId = message.HotelAggregateId,
                                        Facts = message.Facts.Select(f => new HotelFactsSetEvent.Fact()
                                                                              {
                                                                                  FactTypeAggregateId = f.FactTypeAggregateId,
                                                                                  Value = f.Value,
                                                                                  Details = f.Details
                                                                              }).ToArray()
                                    });
        }
    }
}