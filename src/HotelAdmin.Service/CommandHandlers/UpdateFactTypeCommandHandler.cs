using System;
using System.Collections.Generic;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using Petite;

namespace HotelAdmin.Service.CommandHandlers
{
    public class UpdateFactTypeCommandHandler : IMessageHandler<UpdateFactTypeCommand>
    {
        private readonly IObjectContext _objectContext;
        private readonly IFactTypeRepository _factTypeRepository;
        private readonly IIdentityMapper _identityMapper;

        public UpdateFactTypeCommandHandler(IObjectContext objectContext, IFactTypeRepository factTypeRepository, IIdentityMapper identityMapper)
        {
            _objectContext = objectContext;
            _factTypeRepository = factTypeRepository;
            _identityMapper = identityMapper;
        }

        public void Handle(UpdateFactTypeCommand message, IDictionary<string, object> metaData)
        {
            var modelId = _identityMapper.GetModelId<FactType>(message.FactTypeAggregateId);
            var factType = _factTypeRepository.Get(ft => ft.Id == modelId);
            if (factType == null)
                throw new InvalidOperationException(string.Format("No FactType found with Id {0}", message.FactTypeAggregateId));
            factType.Code = message.Code;
            factType.Name = message.Name;
            
            _objectContext.SaveChanges();
        }
    }
}