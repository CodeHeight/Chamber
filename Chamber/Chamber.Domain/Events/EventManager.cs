using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using Chamber.Domain.Interfaces.Events;
using Chamber.Domain.Interfaces.Services;
using Chamber.Utilities;

namespace Chamber.Domain.Events
{
    public sealed class EventManager : IEventManager
    {
        private readonly IReflectionService _reflectionService;
        private const string InterfaceTargetName = @"Chamber.Domain.Interfaces.Events.IEventHandler";

        public EventHandler<AdminUpdateProfileEventArgs> BeforeAdminUpdateProfile;
        public EventHandler<AdminUpdateProfileEventArgs> AfterAdminUpdateProfile;

        public EventHandler<UpdateProfileEventArgs> BeforeUpdateProfile;
        public EventHandler<UpdateProfileEventArgs> AfterUpdateProfile;

        public EventHandler<RegisterUserEventArgs> BeforeRegisterUser;
        public EventHandler<RegisterUserEventArgs> AfterRegisterUser;

        public EventHandler<NewClassificationEventArgs> BeforeNewClassification;
        public EventHandler<NewClassificationEventArgs> AfterNewClassification;

        public EventHandler<UpdateClassificationEventArgs> BeforeUpdateClassification;
        public EventHandler<UpdateClassificationEventArgs> AfterUpdateClassification;

        public EventHandler<NewMembershipLevelEventArgs> BeforeNewMembershipLevel;
        public EventHandler<NewMembershipLevelEventArgs> AfterNewMembershipLevel;

        public EventHandler<UpdateMembershipLevelEventArgs> BeforeUpdateMembershipLevel;
        public EventHandler<UpdateMembershipLevelEventArgs> AfterUpdateMembershipLevel;

        public EventHandler<LoginEventArgs> BeforeLogin;
        public EventHandler<LoginEventArgs> AfterLogin;

        public EventHandler<AdminAddBusinessEventArgs> BeforeAdminBusinessAdd;
        public EventHandler<AdminAddBusinessEventArgs> AfterAdminBusinessAdd;

        public EventHandler<AdminUpdateBusinessEventArgs> BeforeAdminBusinessUpdate;
        public EventHandler<AdminUpdateBusinessEventArgs> AfterAdminBusinessUpdate;

        public EventHandler<AdminRegisterUserEventArgs> BeforeAdminRegisterUser;
        public EventHandler<AdminRegisterUserEventArgs> AfterAdminRegisterUser;

        public EventHandler<AdminAddBusinessContactEventArgs> BeforeAdminBusinessContactAdd;
        public EventHandler<AdminAddBusinessContactEventArgs> AfterAdminBusinessContactAdd;

        public EventHandler<AdminAddBusinessBalanceEventArgs> BeforeAdminBusinessBalanceAdd;
        public EventHandler<AdminAddBusinessBalanceEventArgs> AfterAdminBusinessBalanceAdd;

        private static volatile EventManager _instance;
        private static readonly object SyncRoot = new Object();
        public ILoggingService Logger { get; set; }

        private EventManager()
        {
            _reflectionService = DependencyResolver.Current.GetService<IReflectionService>();
        }

        public static EventManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (SyncRoot)
                    {
                        if (_instance == null)
                        {
                            _instance = new EventManager();
                        }
                    }
                }

                return _instance;
            }
        }

        /// <summary>
        /// Log errors
        /// </summary>
        /// <param name="msg"></param>
        public void LogError(string msg)
        {
            if (Logger != null)
            {
                Logger.Error(msg);
            }
        }



        #region Initialise Code
        /// <summary>
        /// Use reflection to get all event handling classes. Call this ONCE.
        /// </summary>
        public void Initialize(ILoggingService loggingService, List<Assembly> assemblies)
        {
            Logger = loggingService;

            var interfaceFilter = new TypeFilter(_reflectionService.InterfaceFilter);

            foreach (var nextAssembly in assemblies)
            {
                try
                {
                    foreach (var type in nextAssembly.GetTypes())
                    {
                        if (type.IsInterface)
                        {
                            continue;
                        }

                        var myInterfaces = type.FindInterfaces(interfaceFilter, InterfaceTargetName);
                        if (myInterfaces.Length <= 0)
                        {
                            // Not a match
                            continue;
                        }

                        var ctor = type.GetConstructors().First();
                        var createdActivator = ReflectionUtilities.GetActivator<IEventHandler>(ctor);

                        // Create an instance:
                        var instance = createdActivator();

                        instance.RegisterHandlers(this);
                    }
                }
                catch (ReflectionTypeLoadException rtle)
                {
                    var msg =
                        string.Format(
                            "Unable to load assembly. Probably not an event assembly, loader exception was: '{0}':'{1}'.",
                             rtle.LoaderExceptions[0].GetType(), rtle.LoaderExceptions[0].Message);
                    LogError(msg);
                }
                catch (Exception ex)
                {
                    LogError(string.Format("Error reflecting over event handlers: {0}", ex.Message));
                }
            }
        }
        #endregion


        /// <summary>
        /// Singleton instance
        /// </summary>
        //public static EventManager Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //        {
        //            lock (SyncRoot)
        //            {
        //                if (_instance == null)
        //                {
        //                    _instance = new EventManager();
        //                }
        //            }
        //        }

        //        return _instance;
        //    }
        //}

        #region UserProfileUpdated

        public void FireBeforeAdminProfileUpdated(object sender, AdminUpdateProfileEventArgs eventArgs)
        {
            var handler = BeforeAdminUpdateProfile;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }
        public void FireAfterAdminProfileUpdated(object sender, AdminUpdateProfileEventArgs eventArgs)
        {
            var handler = AfterAdminUpdateProfile;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireBeforeProfileUpdated(object sender, UpdateProfileEventArgs eventArgs)
        {
            var handler = BeforeUpdateProfile;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }
        public void FireAfterProfileUpdated(object sender, UpdateProfileEventArgs eventArgs)
        {
            var handler = AfterUpdateProfile;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }
        #endregion

        #region RegisterUser
        public void FireBeforeRegisterUser(object sender, RegisterUserEventArgs eventArgs)
        {
            var handler = BeforeRegisterUser;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireAfterRegisterUser(object sender, RegisterUserEventArgs eventArgs)
        {
            var handler = AfterRegisterUser;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireBeforeAdminRegisterUser(object sender, AdminRegisterUserEventArgs eventArgs)
        {
            var handler = BeforeAdminRegisterUser;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireAfterAdminRegisterUser(object sender, AdminRegisterUserEventArgs eventArgs)
        {
            var handler = AfterAdminRegisterUser;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        #endregion

        #region Classification
        public void FireBeforeNewClassification(object sender, NewClassificationEventArgs eventArgs)
        {
            var handler = BeforeNewClassification;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireAfterNewClassification(object sender, NewClassificationEventArgs eventArgs)
        {
            var handler = AfterNewClassification;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireBeforeUpdateClassification(object sender, UpdateClassificationEventArgs eventArgs)
        {
            var handler = BeforeUpdateClassification;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireAfterUpdateClassification(object sender, UpdateClassificationEventArgs eventArgs)
        {
            var handler = AfterUpdateClassification;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        #endregion

        #region MembershipLevel
        public void FireBeforeNewMembershipLevel(object sender, NewMembershipLevelEventArgs eventArgs)
        {
            var handler = BeforeNewMembershipLevel;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireAfterNewMembershipLevel(object sender, NewMembershipLevelEventArgs eventArgs)
        {
            var handler = AfterNewMembershipLevel;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireBeforeUpdateMembershipLevel(object sender, UpdateMembershipLevelEventArgs eventArgs)
        {
            var handler = BeforeUpdateMembershipLevel;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireAfterUpdateMembershipLevel(object sender, UpdateMembershipLevelEventArgs eventArgs)
        {
            var handler = AfterUpdateMembershipLevel;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        #endregion


        #region Business

        public void FireBeforeAdminBusinessAdd(object sender, AdminAddBusinessEventArgs eventArgs)
        {
            var handler = BeforeAdminBusinessAdd;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireAfterAdminBusinessAdd(object sender, AdminAddBusinessEventArgs eventArgs)
        {
            var handler = AfterAdminBusinessAdd;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireBeforeAdminBusinessUpdate(object sender, AdminUpdateBusinessEventArgs eventArgs)
        {
            var handler = BeforeAdminBusinessUpdate;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireAfterAdminBusinessUpdate(object sender, AdminUpdateBusinessEventArgs eventArgs)
        {
            var handler = AfterAdminBusinessUpdate;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireBeforeAdminBusinessContactAdd(object sender, AdminAddBusinessContactEventArgs eventArgs)
        {
            var handler = BeforeAdminBusinessContactAdd;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireAfterAdminBusinessContactAdd(object sender, AdminAddBusinessContactEventArgs eventArgs)
        {
            var handler = AfterAdminBusinessContactAdd;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireBeforeAdminBusinessBalanceAdd(object sender, AdminAddBusinessBalanceEventArgs eventArgs)
        {
            var handler = BeforeAdminBusinessBalanceAdd;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireAfterAdminBusinessBalanceAdd(object sender, AdminAddBusinessBalanceEventArgs eventArgs)
        {
            var handler = AfterAdminBusinessBalanceAdd;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }


        #endregion

        #region Login
        public void FireBeforeLogin(object sender, LoginEventArgs eventArgs)
        {
            var handler = BeforeLogin;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }

        public void FireAfterLogin(object sender, LoginEventArgs eventArgs)
        {
            var handler = AfterLogin;

            if (handler != null)
            {
                handler(this, eventArgs);
            }
        }
        #endregion
    }
}
