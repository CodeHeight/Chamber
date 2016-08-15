using System.Web.Mvc;
using Microsoft.Practices.Unity;
using Chamber.Domain.Interfaces;
using Chamber.Domain.Interfaces.Services;
using Chamber.Domain.Interfaces.UnitOfWork;
using Chamber.Services.Data.Context;
using Chamber.Services.Data.UnitOfWork;
using Chamber.Services;

namespace Chamber.IOC
{
    /// <summary>
    /// Bind the given interface in request scope
    /// </summary>
    public static class IocExtensions
    {
        public static void BindInRequestScope<T1, T2>(this IUnityContainer container) where T2 : T1
        {
            container.RegisterType<T1, T2>(new HierarchicalLifetimeManager());
        }

    }

    /// <summary>
    /// The injection for Unity
    /// </summary>
    public static partial class UnityHelper
    {

        public static IUnityContainer Start()
        {
            var container = new UnityContainer();
            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
            var buildUnity = BuildUnityContainer(container);
            return buildUnity;
        }

        /// <summary>
        /// Inject
        /// </summary>
        /// <returns></returns>
        private static IUnityContainer BuildUnityContainer(UnityContainer container)
        {
            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // Database context, one per request, ensure it is disposed
            container.BindInRequestScope<IChamberContext, ChamberContext>();
            container.BindInRequestScope<IUnitOfWorkManager, UnitOfWorkManager>();

            // Quartz
            //container.AddNewExtension<QuartzUnityExtension>();

            //Bind the various domain model services and repositories that e.g. our controllers require  
            container.BindInRequestScope<IActivityService, ActivityService>();
            container.BindInRequestScope<IBusinessService, BusinessService>();
            container.BindInRequestScope<ICacheService, CacheService>();
            container.BindInRequestScope<IClassificationService, ClassificationService>();
            container.BindInRequestScope<IConfigService, ConfigService>();
            container.BindInRequestScope<IEmailService, EmailService>();
            container.BindInRequestScope<ILoggingService, LoggingService>();
            container.BindInRequestScope<IMembershipLevelService, MembershipLevelService>();
            container.BindInRequestScope<IMembershipService, MembershipService>();
            container.BindInRequestScope<IReflectionService, ReflectionService>();
            container.BindInRequestScope<IRoleService, RoleService>();
            container.BindInRequestScope<ISettingsService, SettingsService>();

            CustomBindings(container);

            return container;
        }

        static partial void CustomBindings(UnityContainer container);
    }

    // Example of adding your own bindings, just create a partial class and implement
    // the CustomBindings method and add your bindings as shown below
    //public static partial class UnityHelper
    //{
    //    static partial void CustomBindings(UnityContainer container)
    //    {
    //        container.BindInRequestScope<IBlockRepository, BlockRepository>();
    //        container.BindInRequestScope<IBlockService, BlockService>();
    //    }
    //}
}