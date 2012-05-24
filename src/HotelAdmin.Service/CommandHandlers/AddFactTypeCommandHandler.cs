using System.Collections.Generic;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using Petite;

namespace HotelAdmin.Service.CommandHandlers
{
    public class AddFactTypeCommandHandler : IMessageHandler<AddFactTypeCommand>
    {
        private readonly IObjectContext _objectContext;
        private readonly IFactTypeRepository _factTypeRepository;
        private readonly IIdentityMapper _identityMapper;

        public AddFactTypeCommandHandler(IObjectContext objectContext, IFactTypeRepository factTypeRepository, IIdentityMapper identityMapper)
        {
            _objectContext = objectContext;
            _factTypeRepository = factTypeRepository;
            _identityMapper = identityMapper;
        }

        public void Handle(AddFactTypeCommand message, IDictionary<string, object> metaData)
        {
            var factType = new FactType() {Code = message.Code, Name = message.Name};
            _factTypeRepository.Add(factType);
            
            _objectContext.SaveChanges();

            _identityMapper.Map<FactType>(factType.Id, message.FactTypeAggregateId);
        }
    }
}  