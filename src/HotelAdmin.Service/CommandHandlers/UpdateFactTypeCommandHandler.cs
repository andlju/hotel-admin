using System;
using System.Collections.Generic;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Messages.Events;
using HotelAdmin.Service.Infrastructure;
using Petite;

namespace HotelAdmin.Service.CommandHandlers
{
    public class UpdateFactTypeCommandHandler : IMessageHandler<UpdateFactTypeCommand>
    {
        private readonly IFactTypeRepository _factTypeRepository;
        private readonly IIdentityMapper _identityMapper;
        private readonly IEventStorage _eventStorage;

        public UpdateFactTypeCommandHandler(IFactTypeRepository factTypeRepository, IIdentityMapper identityMapper, IEventStorage eventStorage)
        {
            _factTypeRepository = factTypeRepository;
            _identityMapper = identityMapper;
            _eventStorage = eventStorage;
        }

        public void Handle(UpdateFactTypeCommand message, IDictionary<string, object> metaData)
        {
            var modelId = _identityMapper.GetModelId<FactType>(message.FactTypeAggregateId);
            var factType = _factTypeRepository.Get(ft => ft.Id == modelId);
            if (factType == null)
                throw new InvalidOperationException(string.Format("No FactType found with Id {0}", message.FactTypeAggregateId));

            _eventStorage.Store(new FactTypeUpdatedEvent()
                                    {
                                        FactTypeAggregateId = message.FactTypeAggregateId,
                                        Code = message.Code,
                                        Name = message.Name
                                    });
        }
    }
}