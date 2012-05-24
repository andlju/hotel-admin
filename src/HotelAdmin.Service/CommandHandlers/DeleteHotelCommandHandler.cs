using System;
using System.Collections.Generic;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using Petite;

namespace HotelAdmin.Service.CommandHandlers
{
    public class DeleteHotelCommandHandler : IMessageHandler<DeleteHotelCommand>
    {
        private readonly IObjectContext _objectContext;
        private readonly IHotelRepository _hotelRepository;
        private readonly IIdentityMapper _identityMapper;

        public DeleteHotelCommandHandler(IObjectContext objectContext, IHotelRepository hotelRepository, IIdentityMapper identityMapper)
        {
            _objectContext = objectContext;
            _hotelRepository = hotelRepository;
            _identityMapper = identityMapper;
        }

        public void Handle(DeleteHotelCommand message, IDictionary<string, object> metaData)
        {
            var modelId = _identityMapper.GetModelId<Hotel>(message.HotelAggregateId);
            var hotel = _hotelRepository.Get(h => h.Id == modelId);
            if (hotel == null)
                throw new InvalidOperationException(string.Format("No hotel found with id {0}", modelId));

            _hotelRepository.Delete(hotel);

            _objectContext.SaveChanges();
        }
    }
}