using System.Collections.Generic;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Messages.Events;
using HotelAdmin.Service.Infrastructure;
using Petite;

namespace HotelAdmin.Service.CommandHandlers
{
    public class AddFactTypeCommandHandler : IMessageHandler<AddFactTypeCommand>
    {
        private readonly IObjectContext _objectContext;
        private readonly IFactTypeRepository _factTypeRepository;
        private readonly IIdentityMapper _identityMapper;
        private readonly IEventStorage _eventStorage;

        public AddFactTypeCommandHandler(IObjectContext objectContext, IFactTypeRepository factTypeRepository, IIdentityMapper identityMapper, IEventStorage eventStorage)
        {
            _objectContext = objectContext;
            _factTypeRepository = factTypeRepository;
            _identityMapper = identityMapper;
            _eventStorage = eventStorage;
        }

        public void Handle(AddFactTypeCommand message, IDictionary<string, object> metaData)
        {
            var factType = new FactType() {Code = message.Code, Name = message.Name};
            _factTypeRepository.Add(factType);
            
            _objectContext.SaveChanges();

            _identityMapper.Map<FactType>(factType.Id, message.FactTypeAggregateId);
            _eventStorage.Store(new FactTypeAddedEvent()
                                    {
                                        FactTypeAggregateId = message.FactTypeAggregateId,
                                        Code = message.Code,
                                        Name = message.Name
                                    });
        }
    }
}  