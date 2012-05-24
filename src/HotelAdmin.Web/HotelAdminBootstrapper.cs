using HotelAdmin.DataAccess;
using HotelAdmin.Domain;
using HotelAdmin.Service;
using Nancy;
using Nancy.Conventions;
using Petite;

namespace HotelAdmin.Web
{
    public class HotelAdminBootstrapper : DefaultNancyBootstrapper 
    {
        protected override void ConfigureConventions(NancyConventions nancyConventions)
        {
            base.ConfigureConventions(nancyConventions);

            Conventions.StaticContentsConventions.Add(StaticContentConventionBuilder.AddDirectory("Scripts"));

            Conventions.ViewLocationConventions.Add((viewName, model, context) => string.Concat(context.ModuleName, "/Views/", viewName));
        }

        protected override void ConfigureApplicationContainer(TinyIoC.TinyIoCContainer container)
        {
            base.ConfigureApplicationContainer(container);
            container.Register<IDbContextFactory, DbContextFactory<HotelAdminContext>>();

            container.Register<IDbSetProvider, AspNetObjectContextAdapter>().AsMultiInstance();
            container.Register<IObjectContext, AspNetObjectContextAdapter>().AsMultiInstance();

            container.Register<IHotelRepository, HotelRepository>().AsMultiInstance();
            container.Register<IFactTypeRepository, FactTypeRepository>().AsMultiInstance();
            container.Register<IHistoryItemRepository, DummyHistoryItemRepository>().AsMultiInstance();
            container.Register<IHotelService, HotelService>().AsMultiInstance();
            container.Register<IFactTypeService, FactTypeService>().AsMultiInstance();
        }
    }
}