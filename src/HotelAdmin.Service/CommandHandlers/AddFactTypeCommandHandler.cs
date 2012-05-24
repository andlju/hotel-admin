using System.Collections.Generic;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Messages.Events;
using HotelAdmin.Service.Infrastructure;
using Petite;

namespace HotelAdmin.Service.CommandHandlers
{
    public class AddFactTypeCommandHandler : IMessageHandler<AddFactTypeCommand>
    {
        private readonly IEventStorage _eventStorage;

        public AddFactTypeCommandHandler(IEventStorage eventStorage)
        {
            _eventStorage = eventStorage;
        }

        public void Handle(AddFactTypeCommand message, IDictionary<string, object> metaData)
        {
            _eventStorage.Store(new FactTypeAddedEvent()
                                    {
                                        FactTypeAggregateId = message.FactTypeAggregateId,
                                        Code = message.Code,
                                        Name = message.Name
                                    });
        }
    }
}  