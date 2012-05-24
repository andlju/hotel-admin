using System;

namespace HotelAdmin.Messages.Commands
{
    public class UpdateHotelCommand : ICommand
    {
        public Guid HotelAggregateId;

        public string Name;
        public string Description;
        public string ImageUrl;
    }
}