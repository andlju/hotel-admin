using System;
using HotelAdmin.Service;
using Nancy;
using Nancy.ModelBinding;

namespace HotelAdmin.Web.Api
{
    public class HotelApiModule : NancyModule
    {
        public HotelApiModule(IHotelService hotelService)
            : base("/api/hotel")
        {
            Get["/"] = _ =>
                           {
                               string q = null;
                               int p = 0;
                               string f = null;
                               if (Request.Query["q"] != null)
                               {
                                   q = Request.Query["q"];
                               }
                               if (Request.Query["p"] != null)
                               {
                                   p = Request.Query["p"];
                               }
                               if (Request.Query["f"])
                               {
                                   f = Request.Query["f"];
                               }
                               return Response.AsJson(hotelService.FindHotels(q, f!=null ? f.Split(',') : new string[0], p));
                           };

            Get["/{hotelId}"] = _ =>
                                    {
                                        int hotelId = Context.Parameters["hotelId"];
                                        
                                        return Response.AsJson(hotelService.GetHotel(hotelId));
                                    };
            Get["/history"] = _ =>
                                  {
                                      return Response.AsJson(hotelService.GetHistory(0));
                                  };

            Post["/"] =
            Post["/{hotelId}"] = _ =>
                                     {
                                         HotelModel model = this.Bind();
                                         int hotelId = model.Id;
                                         if (hotelId == 0 && Context.Parameters["hotelId"] != null)
                                         {
                                             hotelId = Context.Parameters["hotelId"] ?? 0;
                                         }
                                         try
                                         {
                                             if (hotelId == 0)
                                                 hotelId = hotelService.AddHotel(model.Name, model.Description, model.ResortName, model.Image, model.Latitude, model.Longitude);
                                             else
                                                 hotelService.UpdateHotel(hotelId, model.Name, model.Description, model.Image);

                                             if (model.Facts != null)
                                             {
                                                 hotelService.SetHotelFacts(hotelId, model.Facts);
                                             }

                                             return Response.AsJson(new { HotelId = hotelId });
                                         }
                                         catch (InvalidOperationException ex)
                                         {
                                             var response = Response.AsJson(new { ex.Message });
                                             return response.StatusCode = HttpStatusCode.NotFound;
                                         }
                                     };


            Delete["/{hotelId}"] = _ =>
                                       {
                                           int hotelId = Context.Parameters["hotelId"];
                                           hotelService.DeleteHotel(hotelId);

                                           var response = new Response();
                                           response.StatusCode = HttpStatusCode.Accepted;

                                           return new Response();
                                       };
        }
    }
}