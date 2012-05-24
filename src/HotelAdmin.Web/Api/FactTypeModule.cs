using System;
using HotelAdmin.Service;
using Nancy;
using Nancy.ModelBinding;

namespace HotelAdmin.Web.Api
{
    public class FactTypeApiModule : NancyModule
    {
        public FactTypeApiModule(IFactTypeService factTypeService) :
            base("/api/facttype")
        {
            Get["/"] = _ =>
                           {
                               if (Request.Query["q"] == null)
                               {
                                   return Response.AsJson(factTypeService.ListFactTypes());
                               }
                               return FormatterExtensions.AsJson(Response, factTypeService.FindFactTypes(Request.Query["q"]));
                           };


            Post["/"] =
            Post["/{factTypeId}"] = _ =>
                                        {
                                            FactTypeModel model = this.Bind();
                                            int factTypeId = model.Id;
                                            if (factTypeId == 0 && Context.Parameters["factTypeId"] != null)
                                            {
                                                factTypeId = Context.Parameters["factTypeId"] ?? 0;
                                            }
                                            try
                                            {
                                                if (factTypeId == 0)
                                                    factTypeId = factTypeService.AddFactType(model);
                                                else 
                                                    factTypeService.UpdateFactType(model);
                                                return Response.AsJson(new { FactTypeId = factTypeId });
                                            }
                                            catch (InvalidOperationException ex)
                                            {
                                                var response = Response.AsJson(new { ex.Message });
                                                return response.StatusCode = HttpStatusCode.NotFound;
                                            }
                                        };

        }
    }
}