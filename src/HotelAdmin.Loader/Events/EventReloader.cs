using System;
using HotelAdmin.Service.Infrastructure;

namespace HotelAdmin.Loader.Events
{
    public class EventReloader
    {
         public static void ReloadEvents(IEventStorage eventStorage,IMessageDispatcher dispatcher)
         {
             Console.WriteLine("Getting all events");
             var events = eventStorage.GetAllEvents();
             foreach(var evt in events)
             {
                 Console.WriteLine("Dispatching Event Type {0}, AggregateId {1}", evt.GetType(), evt.AggregateId);
                 dispatcher.DispatchNonGeneric(evt);
             }
         }
    }
}