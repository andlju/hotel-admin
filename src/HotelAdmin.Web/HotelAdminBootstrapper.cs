using HotelAdmin.DataAccess;
using HotelAdmin.Domain;
using HotelAdmin.Messages.Commands;
using HotelAdmin.Service;
using HotelAdmin.Service.CommandHandlers;
using HotelAdmin.Service.Infrastructure;
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

            // Repositories
            container.Register<IIdentityMapRepository, IdentityMapRepository>().AsMultiInstance();
            container.Register<IHotelRepository, HotelRepository>().AsMultiInstance();
            container.Register<IFactTypeRepository, FactTypeRepository>().AsMultiInstance();
            container.Register<IHistoryItemRepository, DummyHistoryItemRepository>().AsMultiInstance();

            // CQRS Infrastructure
            container.Register<IMessageDispatcher>((c, o) =>
                                                       {
                                                           var messageDispatcher = new MessageDispatcher();
                                                           
                                                           messageDispatcher.RegisterHandler(c.Resolve<IMessageHandler<AddHotelCommand>>());
                                                           messageDispatcher.RegisterHandler(c.Resolve<IMessageHandler<UpdateHotelCommand>>());
                                                           messageDispatcher.RegisterHandler(c.Resolve<IMessageHandler<DeleteHotelCommand>>());
                                                           messageDispatcher.RegisterHandler(c.Resolve<IMessageHandler<SetHotelFactsCommand>>());
                                                           messageDispatcher.RegisterHandler(c.Resolve<IMessageHandler<AddFactTypeCommand>>());
                                                           messageDispatcher.RegisterHandler(c.Resolve<IMessageHandler<UpdateFactTypeCommand>>());

                                                           return messageDispatcher;
                                                       });

            // CommandHandlers
            container.Register<IMessageHandler<AddHotelCommand>, AddHotelCommandHandler>().AsMultiInstance();
            container.Register<IMessageHandler<UpdateHotelCommand>, UpdateHotelCommandHandler>().AsMultiInstance();
            container.Register<IMessageHandler<DeleteHotelCommand>, DeleteHotelCommandHandler>().AsMultiInstance();
            container.Register<IMessageHandler<SetHotelFactsCommand>, SetHotelFactsCommandHandler>().AsMultiInstance();

            container.Register<IMessageHandler<AddFactTypeCommand>, AddFactTypeCommandHandler>().AsMultiInstance();
            container.Register<IMessageHandler<UpdateFactTypeCommand>, UpdateFactTypeCommandHandler>().AsMultiInstance();

            // Services
            container.Register<IIdentityMapper, IdentityMapper>().AsMultiInstance();
            container.Register<IHotelService, HotelService>().AsMultiInstance();
            container.Register<IFactTypeService, FactTypeService>().AsMultiInstance();
            container.Register<IEventStorage, EventStoreEventStorage>().AsMultiInstance();
        }
    }
}