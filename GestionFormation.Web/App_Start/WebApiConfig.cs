using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Autofac;
using Autofac.Integration.WebApi;
using GestionFormation.CoreDomain.Formations.Queries;
using GestionFormation.EventStore;
using GestionFormation.Kernel;
using GestionFormation.Web.Controllers;


namespace GestionFormation.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterWebApiFilterProvider(config);

            var dispatcher = new EventDispatcher();
            dispatcher.AutoRegisterAllEventHandler();

            builder.Register(a => new EventBus(dispatcher, new SqlEventStore(new DomainEventJsonEventSerializer(), null))).SingleInstance();
            builder.Register(a => new FormationSqlQueries()).As<IFormationQueries>().SingleInstance();

            config.DependencyResolver = new AutofacWebApiDependencyResolver(builder.Build());

            config.MapHttpAttributeRoutes();
            
        }
    }
}
