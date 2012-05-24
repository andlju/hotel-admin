using System.Collections.Generic;
using System.Linq;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Events;
using Petite;

namespace HotelAdmin.Service.EventHandlers
{
    public class HotelEventHandlers :
        IMessageHandler<HotelAddedEvent>,
        IMessageHandler<HotelUpdatedEvent>,
        IMessageHandler<HotelFactsSetEvent>,
        IMessageHandler<HotelDeletedEvent>
    {
        private readonly IObjectContext _objectContext;
        private readonly IHotelRepository _hotelRepository;
        private readonly IIdentityMapper _identityMapper;

        public HotelEventHandlers(IObjectContext objectContext, IHotelRepository hotelRepository, IIdentityMapper identityMapper)
        {
            _objectContext = objectContext;
            _hotelRepository = hotelRepository;
            _identityMapper = identityMapper;
        }

        public void Handle(HotelAddedEvent message, IDictionary<string, object> metaData)
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

        public void Handle(HotelUpdatedEvent message, IDictionary<string, object> metaData)
        {
            var modelId = _identityMapper.GetModelId<Hotel>(message.HotelAggregateId);

            var hotel = _hotelRepository.Get(h => h.Id == modelId);

            // TODO If hotel doesn't exist, log warning and (possibly) create a new one

            hotel.Name = message.Name;
            hotel.Description = message.Description;
            hotel.Image = message.ImageUrl;

            _objectContext.SaveChanges();
        }

        public void Handle(HotelFactsSetEvent message, IDictionary<string, object> metaData)
        {
            var modelId = _identityMapper.GetModelId<Hotel>(message.HotelAggregateId);
            var hotel = _hotelRepository.Get(h => h.Id == modelId);

            // TODO Log warning if no hotel was found (or maybe create new dummy hotel?)

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

        public void Handle(HotelDeletedEvent message, IDictionary<string, object> metaData)
        {
            var modelId = _identityMapper.GetModelId<Hotel>(message.HotelAggregateId);
            var hotel = _hotelRepository.Get(h => h.Id == modelId);

            // TODO Log warning if no hotel was found

            _hotelRepository.Delete(hotel);

            _objectContext.SaveChanges();
        }
    }
}