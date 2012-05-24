using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace HotelAdmin.Service
{
    [DataContract]
    public class ItemsResponse<T>
    {
        [DataMember]
        public IEnumerable<T> Items { get; set; }

        [DataMember]
        public int PageNumber { get; set; }

        [DataMember]
        public int TotalNumberOfPages { get; set; }

        [DataMember]
        public int ItemsPerPage { get; set; }
    }

    [DataContract]
    public class HotelModel
    {
        public HotelModel(int id, string name, string resortName, string description, string image, double latitude, double longitude, IEnumerable<FactModel> facts)
        {
            Id = id;
            Name = name;
            ResortName = resortName;
            Description = description;

            Image = image;
            Latitude = latitude;
            Longitude = longitude;

            Facts = facts.ToArray();
        }

        public HotelModel()
        {
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string ResortName { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Image { get; set; }

        [DataMember]
        public double Latitude { get; set; }

        [DataMember]
        public double Longitude { get; set; }

        [DataMember]
        public IEnumerable<FactModel> Facts { get; set; }
    }

    [DataContract]
    public class FactModel
    {
        public FactModel(int id, string code, string name, bool value, string details)
        {
            Id = id;
            Code = code;
            Name = name;
            Value = value;
            Details = details;
        }

        public FactModel()
        {
        }

        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public bool Value { get; set; }

        [DataMember]
        public string Details { get; set; }
    }

    [DataContract]
    public enum ActionTypeModel
    {
        New,
        Updated,
        Deleted
    }

    [DataContract]
    public class HistoryDayModel
    {
        public DateTime Date { get; set; }

        public HistoryItemModel[] Items { get; set; }
    }

    [DataContract]
    public class HistoryItemModel
    {
        [DataMember]
        public int? HotelId { get; set; }

        [DataMember]
        public string HotelName { get; set; }

        [DataMember]
        public ActionTypeModel ActionType { get; set; }

        [DataMember]
        public DateTime Timestamp { get; set; }
    }
}