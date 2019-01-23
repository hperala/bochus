using Autofac;
using Autofac.Integration.WebApi;
using Core;
using Infrastructure;
using System.Reflection;
using System.Web.Configuration;
using System.Web.Http;

namespace Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var settings = WebConfigurationManager.AppSettings;
            var interval = int.Parse(settings["GoodreadsCooldownMillis"]);

            // CORS
            config.EnableCors();

            // Autofac setup
            var builder = new ContainerBuilder();
            builder.RegisterApiControllers(Assembly.GetExecutingAssembly());
            builder.RegisterType<EFRepository>().As<IRepository>();
            builder.RegisterType<Goodreads>().As<IGoodreads>();
            builder.Register(c => new Throttler() { MinIntervalMillis = interval })
                .As<IThrottler>()
                .SingleInstance();
            var container = builder.Build();
            config.DependencyResolver = new AutofacWebApiDependencyResolver(container);

            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
