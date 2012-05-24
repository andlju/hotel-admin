using System;
using System.Collections.Generic;
using System.Linq;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using Petite;

namespace HotelAdmin.Service.CommandHandlers
{
    public class SetHotelFactsCommandHandler : IMessageHandler<SetHotelFactsCommand>
    {
        private readonly IObjectContext _objectContext;
        private readonly IHotelRepository _hotelRepository;
        private readonly IIdentityMapper _identityMapper;

        public SetHotelFactsCommandHandler(IObjectContext objectContext, IHotelRepository hotelRepository, IIdentityMapper identityMapper)
        {
            _objectContext = objectContext;
            _hotelRepository = hotelRepository;
            _identityMapper = identityMapper;
        }

        public void Handle(SetHotelFactsCommand message, IDictionary<string, object> metaData)
        {
            var modelId = _identityMapper.GetModelId<Hotel>(message.HotelAggregateId);
            var hotel = _hotelRepository.Get(h => h.Id == modelId);
            if (hotel == null)
                throw new InvalidOperationException(string.Format("Unknown hotel with Id: {0}", message.HotelAggregateId));

            var facts = message.Facts.Select(f => new Fact()
                                                      {
                                                          FactTypeId = (int)_identityMapper.GetModelId<FactType>(f.FactTypeAggregateId),
                                                          Value = f.Value,
                                                          Details = f.Details
                                                      });
            hotel.Facts.Clear();
            foreach (var fact in facts)
            {
                hotel.Facts.Add(fact);
            }
            _objectContext.SaveChanges();
        }
    }
}