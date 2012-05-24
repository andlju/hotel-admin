using System;
using HotelAdmin.DataAccess;
using HotelAdmin.Loader.Events;
using HotelAdmin.Messages.Events;
using HotelAdmin.Loader.TcneApi;
using HotelAdmin.Service;
using HotelAdmin.Service.CommandHandlers;
using HotelAdmin.Service.EventHandlers;
using HotelAdmin.Service.Infrastructure;
using Petite;

namespace HotelAdmin.Loader
{

    class Program
    {

        static void Main(string[] args)
        {
            var objContext = new SingleUsageObjectContextAdapter(new DbContextFactory<HotelAdminContext>());
            var hotelRep = new HotelRepository(objContext);
            var factTypeRep = new FactTypeRepository(objContext);
            var historyRep = new HistoryItemRepository(objContext);

            var idMapRep = new IdentityMapRepository(objContext);
            var idMapper = new IdentityMapper(objContext, idMapRep);

            var eventDispatcher = new MessageDispatcher();
            var factTypeEventHandlers = new FactTypeEventHandlers(objContext, factTypeRep, idMapper);
            var hotelEventHandlers = new HotelEventHandlers(objContext, hotelRep, idMapper);
            var historyItemEventHandlers = new HistoryItemEventHandlers(objContext, historyRep, idMapper);

            eventDispatcher.RegisterHandler<HotelAddedEvent>(hotelEventHandlers);

            eventDispatcher.RegisterHandler<HotelAddedEvent>(historyItemEventHandlers);
            eventDispatcher.RegisterHandler<HotelUpdatedEvent>(historyItemEventHandlers);
            eventDispatcher.RegisterHandler<HotelDeletedEvent>(historyItemEventHandlers);

            eventDispatcher.RegisterHandler<HotelUpdatedEvent>(hotelEventHandlers);
            eventDispatcher.RegisterHandler<HotelDeletedEvent>(hotelEventHandlers);
            eventDispatcher.RegisterHandler<HotelFactsSetEvent>(hotelEventHandlers);

            eventDispatcher.RegisterHandler<FactTypeAddedEvent>(factTypeEventHandlers);
            eventDispatcher.RegisterHandler<FactTypeUpdatedEvent>(factTypeEventHandlers);

            var eventStorage = new EventStoreEventStorage(eventDispatcher);
            
            var messageDispatcher = new MessageDispatcher();
            messageDispatcher.RegisterHandler(new AddHotelCommandHandler(hotelRep, idMapper, eventStorage));
            messageDispatcher.RegisterHandler(new UpdateHotelCommandHandler(hotelRep, idMapper, eventStorage));
            messageDispatcher.RegisterHandler(new SetHotelFactsCommandHandler(hotelRep, idMapper, eventStorage));
            messageDispatcher.RegisterHandler(new DeleteHotelCommandHandler(hotelRep, idMapper, eventStorage));

            messageDispatcher.RegisterHandler(new AddFactTypeCommandHandler(eventStorage));
            messageDispatcher.RegisterHandler(new UpdateFactTypeCommandHandler(factTypeRep, idMapper, eventStorage));

            FactTypeService factTypeService = new FactTypeService(factTypeRep, messageDispatcher, idMapper);
            HotelService hotelService = new HotelService(hotelRep, historyRep, messageDispatcher, idMapper);

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("Usage: HotelAdmin.Loader.exe {{world|reload}} [maxItems] ");
                return;
            }
            if (args[0] == "world")
            {
                int numberOfHotels = 100;
                if (args.Length > 1)
                {
                    if (!int.TryParse(args[1], out numberOfHotels))
                        numberOfHotels = 100; // Default to 100
                }

                WorldLoader.LoadWorld(hotelService, factTypeService, numberOfHotels);
                return;
            }

            if (args[0] == "reload")
            {
                EventReloader.ReloadEvents(eventStorage, eventDispatcher);
                return;
            }

            Console.WriteLine("Unknown argument '{0}'", args[0]);
            Console.WriteLine("Usage: HotelAdmin.Loader.exe {{world}} [maxItems] ");
            return;

        }
    }
}
