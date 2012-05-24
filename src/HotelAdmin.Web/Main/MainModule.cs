using HotelAdmin.Service;
using Nancy;

namespace HotelAdmin.Web.Main
{
    public class MainModule : NancyModule
    {
        public MainModule(IHotelService hotelService)
        {
            Get["/"] = _ =>
                           {
                               Response resp = View["Index"];
                               resp.ContentType = "text/html; charset=utf-8";
                               return resp;
                           };

            Get["/hotels/"] = _ =>
                                 {
                                     Response resp = View["Hotels"];
                                     resp.ContentType = "text/html; charset=utf-8";
                                     return resp;
                                 };
        }
    }
}