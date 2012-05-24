using System;
using System.Collections.Generic;
using System.Linq;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Service.Infrastructure;
using Petite;

namespace HotelAdmin.Service
{
    public class HotelService : IHotelService
    {
        private readonly IHotelRepository _hotelRepository;
        private readonly IHistoryItemRepository _historyItemRepository;

        private readonly IMessageDispatcher _messageDispatcher;
        private readonly IIdentityMapper _identityMapper;

        private const int _PageSize = 15;

        public HotelService(IHotelRepository hotelRepository, IHistoryItemRepository historyItemRepository, IMessageDispatcher messageDispatcher, IIdentityMapper identityMapper)
        {
            _hotelRepository = hotelRepository;
            _messageDispatcher = messageDispatcher;
            _identityMapper = identityMapper;
            _historyItemRepository = historyItemRepository;
        }

        public HotelModel GetHotel(int hotelId)
        {
            var hotel = _hotelRepository.Get(h => h.Id == hotelId);
            return new HotelModel(hotel.Id, hotel.Name, hotel.ResortName, hotel.Description, hotel.Image,
                                  hotel.Latitude, hotel.Longitude,
                                  from f in hotel.Facts
                                  select new FactModel(f.FactTypeId, f.FactType.Code, f.FactType.Name, f.Value, f.Details)
                );
        }

        public ItemsResponse<HotelModel> ListHotels(int pageNo)
        {
            var hotels = from h in _hotelRepository.List()
                         orderby h.Name
                         select new HotelModel(h.Id, h.Name, h.ResortName, h.Description, h.Image,
                                               h.Latitude, h.Longitude,
                                               from f in h.Facts
                                               select new FactModel(f.FactTypeId, f.FactType.Code, f.FactType.Name, f.Value, f.Details));

            var cnt = hotels.Count();

            return new ItemsResponse<HotelModel>() {Items = hotels.Skip(_PageSize*pageNo).Take(_PageSize), ItemsPerPage = _PageSize, PageNumber = pageNo, TotalNumberOfPages = cnt / _PageSize };
        }

        public ItemsResponse<HotelModel> FindHotels(string query, IEnumerable<string> facts, int pageNo)
        {
            var hotels = from h in _hotelRepository.FindByFacts(query, facts)
                         orderby h.Name
                         select new HotelModel(h.Id, h.Name, h.ResortName, h.Description, h.Image,
                                               h.Latitude, h.Longitude,
                                               from f in h.Facts
                                               select
                                                   new FactModel(f.FactTypeId, f.FactType.Code, f.FactType.Name, f.Value,
                                                                 f.Details));
            var cnt = hotels.Count();

            return new ItemsResponse<HotelModel>() { Items = hotels.Skip(_PageSize * pageNo).Take(_PageSize), ItemsPerPage = _PageSize, PageNumber = pageNo, TotalNumberOfPages = cnt / _PageSize };

        }

        public ItemsResponse<HistoryDayModel> GetHistory(int pageNo)
        {
            var items = _historyItemRepository.List().Skip(_PageSize*pageNo).Take(_PageSize).ToArray();

            var dayItems = from i in items
                           orderby i.Timestamp descending 
                           group i by i.Timestamp.Date
                           into d
                           select new HistoryDayModel()
                                      {
                                          Date = d.Key,
                                          Items =
                                              d.Select(
                                                  it =>
                                                  new HistoryItemModel()
                                                      {
                                                          ActionType = (ActionTypeModel) it.ActionType,
                                                          HotelId = it.HotelId,
                                                          HotelName = it.HotelName,
                                                          Timestamp = it.Timestamp
                                                      }).ToArray()
                                      };
           

            return new ItemsResponse<HistoryDayModel>() { Items = dayItems, ItemsPerPage = _PageSize, PageNumber = 1, TotalNumberOfPages = 1 };
        }

        public int AddHotel(string name, string description, string resortName, string image, double latitude, double longitude)
        {
            var cmd = new AddHotelCommand()
                          {
                              HotelAggregateId = Guid.NewGuid(),
                              Name = name,
                              Description = description,
                              ResortName = resortName,
                              ImageUrl = image,
                              Latitude = latitude,
                              Longitude = longitude
                          };
            _messageDispatcher.Dispatch(cmd);
            var modelId = (int)_identityMapper.GetModelId<Hotel>(cmd.HotelAggregateId);
            return modelId;
        }

        public void UpdateHotel(int hotelId, string name, string description, string image)
        {
            var aggregateId = _identityMapper.GetAggregateId<Hotel>(hotelId);
            var cmd = new UpdateHotelCommand()
            {
                HotelAggregateId = aggregateId,
                Name = name,
                Description = description,
                ImageUrl = image,
            };
            _messageDispatcher.Dispatch(cmd);
        }

        public void DeleteHotel(int hotelId)
        {
            var aggregateId = _identityMapper.GetAggregateId<Hotel>(hotelId);
            var cmd = new DeleteHotelCommand()
            {
                HotelAggregateId = aggregateId,
            };
            _messageDispatcher.Dispatch(cmd);
        }

        public void SetHotelFacts(int hotelId, IEnumerable<FactModel> facts)
        {
            var aggregateId = _identityMapper.GetAggregateId<Hotel>(hotelId);
            var cmd = new SetHotelFactsCommand()
            {
                HotelAggregateId = aggregateId,
                Facts = facts.Select(f => new SetHotelFactsCommand.Fact()
                                              {
                                                  FactTypeAggregateId = _identityMapper.GetAggregateId<FactType>(f.Id),
                                                  Details = f.Details,
                                                  Value = f.Value
                                              }).ToArray()
            };
            _messageDispatcher.Dispatch(cmd);
        }
    }
}