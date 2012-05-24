using System;

namespace HotelAdmin.Messages.Commands
{
    public class AddHotelCommand : ICommand
    {
        public Guid HotelAggregateId;

        public string Name;
        public string Description;
        public string ResortName;
        public string ImageUrl;
        public double Latitude;
        public double Longitude;
    }
}