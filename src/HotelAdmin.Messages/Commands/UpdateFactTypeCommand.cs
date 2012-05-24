using System;

namespace HotelAdmin.Messages.Commands
{
    public class UpdateFactTypeCommand : ICommand
    {
        public Guid FactTypeAggregateId;

        public string Code;
        public string Name;
    }
}