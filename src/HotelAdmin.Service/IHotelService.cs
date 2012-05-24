using System.Collections.Generic;
using System.ServiceModel;

namespace HotelAdmin.Service
{
    [ServiceContract]
    public interface IHotelService
    {
        [OperationContract]
        HotelModel GetHotel(int hotelId);

        [OperationContract]
        ItemsResponse<HotelModel> ListHotels(int pageNo);

        [OperationContract]
        ItemsResponse<HotelModel> FindHotels(string query, IEnumerable<string> facts, int pageNo);

        [OperationContract]
        ItemsResponse<HistoryDayModel> GetHistory(int pageNo);

        [OperationContract]
        int AddHotel(string name, string description, string resortName, string image, double latitude, double longitude);

        [OperationContract]
        void UpdateHotel(int hotelId, string name, string description, string image);

        [OperationContract]
        void DeleteHotel(int hotelId);

        [OperationContract]
        void SetHotelFacts(int hotelId, IEnumerable<FactModel> facts);
    }
}