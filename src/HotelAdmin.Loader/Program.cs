using System;
using HotelAdmin.DataAccess;
using HotelAdmin.Loader.TcneApi;
using HotelAdmin.Service;
using HotelAdmin.Service.CommandHandlers;
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
            var historyRep = new DummyHistoryItemRepository();

            var idMapRep = new IdentityMapRepository(objContext);
            var eventStorage = new EventStoreEventStorage();

            var idMapper = new IdentityMapper(objContext, idMapRep);
            
            var messageDispatcher = new MessageDispatcher();
            messageDispatcher.RegisterHandler(new AddHotelCommandHandler(objContext, hotelRep, idMapper, eventStorage));
            messageDispatcher.RegisterHandler(new UpdateHotelCommandHandler(objContext, hotelRep, idMapper, eventStorage));
            messageDispatcher.RegisterHandler(new SetHotelFactsCommandHandler(objContext, hotelRep, idMapper, eventStorage));
            messageDispatcher.RegisterHandler(new DeleteHotelCommandHandler(objContext, hotelRep, idMapper, eventStorage));

            messageDispatcher.RegisterHandler(new AddFactTypeCommandHandler(objContext, factTypeRep, idMapper, eventStorage));
            messageDispatcher.RegisterHandler(new UpdateFactTypeCommandHandler(objContext, factTypeRep, idMapper, eventStorage));

            FactTypeService factTypeService = new FactTypeService(factTypeRep, messageDispatcher, idMapper);
            HotelService hotelService = new HotelService(hotelRep, historyRep, messageDispatcher, idMapper);

            if (args != null && args.Length > 0 && args[0] == "world")
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

            Console.WriteLine("Usage: HotelAdmin.Loader.exe {{world}} [maxItems] ");
        }
    }
}
