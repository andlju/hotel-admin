using System;
using System.Collections.Concurrent;
using HotelAdmin.Domain;
using Petite;

namespace HotelAdmin.Service
{
    public class IdentityMapper : IIdentityMapper
    {
        private readonly IIdentityMapRepository _identityMapRepository;
        private readonly IObjectContext _objectContext;

        private ConcurrentDictionary<Guid, Tuple<string, long>> _modelCache = new ConcurrentDictionary<Guid, Tuple<string, long>>();
        private ConcurrentDictionary<Tuple<string, long>, Guid> _aggregateCache = new ConcurrentDictionary<Tuple<string, long>, Guid>();

        public IdentityMapper(IObjectContext objectContext, IIdentityMapRepository identityMapRepository)
        {
            _objectContext = objectContext;
            _identityMapRepository = identityMapRepository;
        }

        public void Map<TDomain>(long modelId, Guid aggregateId)
        {
            var map = _identityMapRepository.Get(im => im.AggregateId == aggregateId) ??
                      new IdentityMap() {AggregateId = aggregateId};

            var typeName = typeof(TDomain).Name;
            map.TypeName = typeName;
            map.ModelId = modelId;

            _identityMapRepository.Add(map);
            SaveToCache(modelId, aggregateId, typeName);

            _objectContext.SaveChanges();
        }

        private void SaveToCache(long modelId, Guid aggregateId, string typeName)
        {
            _modelCache[aggregateId] = new Tuple<string, long>(typeName, modelId);
            _aggregateCache[new Tuple<string, long>(typeName, modelId)] = aggregateId;
        }

        public Guid GetAggregateId<TDomain>(long modelId)
        {
            var typeName = typeof (TDomain).Name;
            Guid aggregateId;
            if (_aggregateCache.TryGetValue(new Tuple<string, long>(typeName, modelId), out aggregateId))
                return aggregateId;

            var map = _identityMapRepository.Get(im => im.ModelId == modelId && im.TypeName == typeName);
            if (map != null)
            {
                SaveToCache(modelId, map.AggregateId, typeName);
                return map.AggregateId;
            }
            
            return Guid.Empty;
        }

        public long GetModelId<TDomain>(Guid aggregateId)
        {
            var typeName = typeof(TDomain).Name;
            Tuple<string, long> model;
            if (_modelCache.TryGetValue(aggregateId, out model) && model.Item1 == typeName)
                return model.Item2;

            var map = _identityMapRepository.Get(im => im.AggregateId == aggregateId && im.TypeName == typeName);
            if (map != null)
            {
                SaveToCache(map.ModelId, aggregateId, typeName);
                return map.ModelId;
            }

            return 0;
        }
    }
}