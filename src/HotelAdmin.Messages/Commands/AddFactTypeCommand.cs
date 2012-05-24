using System;

namespace HotelAdmin.Messages.Commands
{
    public class AddFactTypeCommand : ICommand
    {
        public Guid FactTypeAggregateId;

        public string Code;
        public string Name;
    }
}