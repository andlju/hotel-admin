using System;
using System.Collections.Generic;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Events;
using Petite;

namespace HotelAdmin.Service.EventHandlers
{
    public class FactTypeEventHandlers : 
        IMessageHandler<FactTypeAddedEvent>, 
        IMessageHandler<FactTypeUpdatedEvent>
    {
        private readonly IObjectContext _objectContext;
        private readonly IFactTypeRepository _factTypeRepository;
        private readonly IIdentityMapper _identityMapper;

        public FactTypeEventHandlers(IObjectContext objectContext, IFactTypeRepository factTypeRepository, IIdentityMapper identityMapper)
        {
            _objectContext = objectContext;
            _factTypeRepository = factTypeRepository;
            _identityMapper = identityMapper;
        }

        public void Handle(FactTypeAddedEvent message, IDictionary<string, object> metaData)
        {
            var factType = new FactType() { Code = message.Code, Name = message.Name };
            _factTypeRepository.Add(factType);

            _objectContext.SaveChanges();

            _identityMapper.Map<FactType>(factType.Id, message.FactTypeAggregateId);
        }

        public void Handle(FactTypeUpdatedEvent message, IDictionary<string, object> metaData)
        {
            var modelId = _identityMapper.GetModelId<FactType>(message.FactTypeAggregateId);
            var factType = _factTypeRepository.Get(ft => ft.Id == modelId);
            
            // TODO If FactType wasn't found, log warning and (possibly) create a new one
            factType.Code = message.Code;
            factType.Name = message.Name;

            _objectContext.SaveChanges();

        }
    }
}