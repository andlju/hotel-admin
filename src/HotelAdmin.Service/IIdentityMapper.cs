using System;

namespace HotelAdmin.Service
{
    public interface IIdentityMapper
    {
        void Map<TDomain>(long modelId, Guid aggregateId);
        Guid GetAggregateId<TDomain>(long modelId);
        long GetModelId<TDomain>(Guid aggregateId);
    }
}