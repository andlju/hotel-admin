using System;
using System.Collections.Generic;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Events;
using Petite;

namespace HotelAdmin.Service.EventHandlers
{
    public class HistoryItemEventHandlers :
        IMessageHandler<HotelAddedEvent>,
        IMessageHandler<HotelUpdatedEvent>,
        IMessageHandler<HotelDeletedEvent>
    {
                
        private readonly IObjectContext _objectContext;
        private readonly IHistoryItemRepository _historyItemRepository;
        private readonly IIdentityMapper _identityMapper;


        public HistoryItemEventHandlers(IObjectContext objectContext, IHistoryItemRepository historyItemRepository, IIdentityMapper identityMapper)
        {
            _objectContext = objectContext;
            _historyItemRepository = historyItemRepository;
            _identityMapper = identityMapper;
        }

        public void Handle(HotelAddedEvent message, IDictionary<string, object> metaData = null)
        {
            var timestamp = GetTimestamp(metaData);
            var id = _identityMapper.GetModelId<Hotel>(message.AggregateId);

            _historyItemRepository.Add(new HistoryItem()
                                           {
                                               HotelId = (int)id,
                                               ActionType = (int)ActionTypeModel.New,
                                               HotelName = message.Name,
                                               Timestamp = timestamp
                                           });
            _objectContext.SaveChanges();
        }

        private static DateTime GetTimestamp(IDictionary<string, object> metaData)
        {
            var timestamp = DateTime.Now;
            object timestampObj;
            if (metaData != null && metaData.TryGetValue("Timestamp", out timestampObj))
                timestamp = (DateTime) timestampObj;
            return timestamp;
        }

        public void Handle(HotelUpdatedEvent message, IDictionary<string, object> metaData = null)
        {
            var timestamp = GetTimestamp(metaData);

            var id = _identityMapper.GetModelId<Hotel>(message.AggregateId);

            _historyItemRepository.Add(new HistoryItem()
                                           {
                                               HotelId = (int)id,
                                               ActionType = (int)ActionTypeModel.Updated,
                                               HotelName = message.Name,
                                               Timestamp = timestamp
                                           });

            _objectContext.SaveChanges();
        }

        public void Handle(HotelDeletedEvent message, IDictionary<string, object> metaData = null)
        {
            var timestamp = GetTimestamp(metaData);
            var id = _identityMapper.GetModelId<Hotel>(message.AggregateId);
            
            var name = "Unknown hotel";
            var lastItem = _historyItemRepository.GetLastItem((int)id);
            if (lastItem != null)
                name = lastItem.HotelName;

            _historyItemRepository.Add(new HistoryItem()
                                           {
                                               ActionType = (int)ActionTypeModel.Deleted,
                                               HotelName = name,
                                               Timestamp = timestamp
                                           });

            _objectContext.SaveChanges();
        }
    }
}