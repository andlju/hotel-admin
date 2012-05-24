using System;

namespace HotelAdmin.Domain
{
    public class HistoryItem
    {
        public int Id { get; set; }

        public Hotel Hotel { get; set; }
        public int? HotelId { get; set; }

        public string HotelName { get; set; }
        public int ActionType { get; set; }
        public DateTime Timestamp { get; set; }
    }
}