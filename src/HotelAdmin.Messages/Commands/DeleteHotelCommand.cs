using System;

namespace HotelAdmin.Messages.Commands
{
    public class DeleteHotelCommand : ICommand
    {
        public Guid HotelAggregateId;
    }
}   