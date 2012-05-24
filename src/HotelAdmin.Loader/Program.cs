using System;
using HotelAdmin.DataAccess;
using HotelAdmin.Loader.TcneApi;
using HotelAdmin.Service;
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

            FactTypeService factTypeService = new FactTypeService(objContext, factTypeRep);
            HotelService hotelService = new HotelService(objContext, hotelRep, historyRep);

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
