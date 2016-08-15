using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Chamber.Domain.Interfaces.Services;
using Chamber.Domain.Interfaces.UnitOfWork;
using Chamber.IOC;
using Chamber.Web.Application;
using Chamber.Web.Application.ViewEngine;

namespace Chamber.Web
{
    public class MvcApplication : HttpApplication
    {
        public IUnitOfWorkManager UnitOfWorkManager => ServiceFactory.Get<IUnitOfWorkManager>();
        //public ILocalizationService LocalizationService => ServiceFactory.Get<ILocalizationService>();
        public ISettingsService SettingsService => ServiceFactory.Get<ISettingsService>();
        public IReflectionService ReflectionService => ServiceFactory.Get<IReflectionService>();

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            //GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            // Start unity
            var unityContainer = UnityHelper.Start();

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Store the value for use in the app
            Application["Version"] = AppHelpers.GetCurrentVersionNo();

            var loadedAssemblies = ReflectionService.GetAssemblies();

            //using (UnitOfWorkManager.NewUnitOfWork())
            //{
            //    var setting = SettingsService.GetSettings();
            //}


            ViewEngines.Engines.Clear();
            ViewEngines.Engines.Add(new ChamberViewEngine(SettingsService.GetSettings().Theme));
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            //It's important to check whether session object is ready
            if (HttpContext.Current.Session != null)
            {
                // Set the culture per request
                var ci = new CultureInfo("en-us");
                Thread.CurrentThread.CurrentUICulture = ci;
                Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(ci.Name);
            }
        }
    }
}
