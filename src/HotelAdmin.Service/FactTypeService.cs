using System;
using System.Collections.Generic;
using System.Linq;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Service.Infrastructure;
using Petite;

namespace HotelAdmin.Service
{
    public class FactTypeService : IFactTypeService
    {
        private readonly IFactTypeRepository _factTypeRepository;
        private readonly IMessageDispatcher _messageDispatcher;
        private readonly IIdentityMapper _identityMapper;

        public FactTypeService(IFactTypeRepository factTypeRepository, IMessageDispatcher messageDispatcher, IIdentityMapper identityMapper)
        {
            _factTypeRepository = factTypeRepository;
            _messageDispatcher = messageDispatcher;
            _identityMapper = identityMapper;
        }

        public IEnumerable<FactTypeModel> ListFactTypes()
        {
            return _factTypeRepository.List().
                Select(ft => new FactTypeModel()
                                 {
                                     Id = ft.Id,
                                     Code = ft.Code,
                                     Name = ft.Name
                                 }).ToArray();
        }

        public FactTypeModel GetFactType(string code)
        {
            
            return _factTypeRepository.Find(ft => ft.Code == code).
                Select(ft => new FactTypeModel() { Id = ft.Id, Code = ft.Code, Name = ft.Name }).SingleOrDefault();
        }

        public IEnumerable<FactTypeModel> FindFactTypes(string query)
        {
            return _factTypeRepository.Find(ft => ft.Name.Contains(query)).
                Select(ft => new FactTypeModel()
                                 {
                                     Id = ft.Id,
                                     Code = ft.Code,
                                     Name = ft.Name
                                 }).ToArray();
        }

        public int AddFactType(FactTypeModel factType)
        {
            var cmd = new AddFactTypeCommand()
                          {
                              FactTypeAggregateId = Guid.NewGuid(),
                              Code = factType.Code, 
                              Name = factType.Name
                          };
            _messageDispatcher.Dispatch(cmd);

            var modelId = _identityMapper.GetModelId<FactType>(cmd.FactTypeAggregateId);

            return (int)modelId;
        }

        public void UpdateFactType(FactTypeModel factType)
        {
            var aggregateId = _identityMapper.GetAggregateId<FactType>(factType.Id);

            var cmd = new UpdateFactTypeCommand()
            {
                FactTypeAggregateId = aggregateId,
                Code = factType.Code,
                Name = factType.Name
            };
            _messageDispatcher.Dispatch(cmd);
        }
    }
}