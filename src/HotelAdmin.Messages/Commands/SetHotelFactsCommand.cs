using System;

namespace HotelAdmin.Messages.Commands
{
    public class SetHotelFactsCommand : ICommand
    {
        public class Fact
        {
            public Guid FactTypeAggregateId;
            public bool Value;
            public string Details;
        }

        public Guid HotelAggregateId;

        public Fact[] Facts;
    }
}