using System;
using System.Collections.Generic;
using System.Linq;
using HotelAdmin.Domain;
using Petite;

namespace HotelAdmin.Service
{
    public class HotelService : IHotelService
    {
        private readonly IObjectContext _objectContext;
        private readonly IHotelRepository _hotelRepository;
        private readonly IHistoryItemRepository _historyItemRepository;

        private const int _PageSize = 15;

        public HotelService(IObjectContext objectContext, IHotelRepository hotelRepository, IHistoryItemRepository historyItemRepository)
        {
            _objectContext = objectContext;
            _hotelRepository = hotelRepository;
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
            var hotel = new Hotel() { Name = name, ResortName = resortName, Description = description, Image = image, Latitude = latitude, Longitude = longitude};
            _hotelRepository.Add(hotel);
            _objectContext.SaveChanges();
            return hotel.Id;
        }

        public void UpdateHotel(int hotelId, string name, string description, string image)
        {
            var hotel = _hotelRepository.Get(h => h.Id == hotelId);
            if (hotel == null)
                throw new InvalidOperationException(string.Format("No hotel found with id {0}", hotelId));

            hotel.Name = name;
            hotel.Description = description;
            hotel.Image = image;

            _objectContext.SaveChanges();
        }

        public void DeleteHotel(int hotelId)
        {
            var hotel = _hotelRepository.Get(h => h.Id == hotelId);
            if (hotel == null)
                throw new InvalidOperationException(string.Format("No hotel found with id {0}", hotelId));
       
            _hotelRepository.Delete(hotel);

            _objectContext.SaveChanges();
        }

        public void SetHotelFacts(int hotelId, IEnumerable<FactModel> facts)
        {
            var hotel = _hotelRepository.Get(h => h.Id == hotelId);
            if (hotel == null)
                throw new InvalidOperationException(string.Format("No hotel found with id {0}", hotelId));

            hotel.Facts.Clear();
            foreach (var fact in facts)
            {
                hotel.Facts.Add(new Fact() { FactTypeId = fact.Id, Value = fact.Value, Details = fact.Details });
            }

            _objectContext.SaveChanges();
        }
    }
}