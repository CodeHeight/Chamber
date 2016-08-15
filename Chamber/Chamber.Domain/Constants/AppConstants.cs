using System.Collections.Generic;

namespace Chamber.Domain.Constants
{
    public static class AppConstants
    {
        public const int SaltSize = 24;

        // Cache names
        public const string SettingsCacheName = "MainSettings";
        public const string LocalisationCacheName = "Localization-";
        public static string LanguageStrings = string.Concat(LocalisationCacheName, "LangStrings-");
        public const string MemberCacheName = "#member#-{0}";

        // View Bag / Temp Data Constants
        public const string MessageViewBagName = "Nothing";
        public const string GlobalClass = "GlobalClass";
        public const string CurrentAction = "CurrentAction";
        public const string CurrentController = "CurrentController";
        public const string MemberRegisterViewModel = "MemberRegisterViewModel";

        //Don't not delete this account [This should never be changed]
        public const string SuperAdmin = "A8A7E677-FC67-4554-B337-A5D00122EE25";

        // Main admin role [This should never be changed]
        public const string AdminRoleName = "Admin";

        // Main guest role [This should never be changed]
        // This is the role a non logged in user defaults to
        public const string GuestRoleName = "Guest";

        // Paging options
        public const string PagingUrlFormat = "{0}?p={1}";

        // How long 
        public const int TimeSpanInMinutesToShowMembers = 12;

        /// <summary>
        /// Last Activity Time Check. 
        /// </summary>
        public const int TimeSpanInMinutesToDoCheck = 3;

        //Mobile Check Name
        public const string IsMobileDevice = "IsMobileDevice";

        /// <summary>
        /// Cache lengths
        /// </summary>
        public static List<string> ReflectionDllsToAvoid = new List<string>
        {
            "EcmaScript.NET.dll",
            "Unity.WebApi.dll",
            "Skybrud.Social.dll",
            "Antlr3.Runtime.dll",
            "WebGrease.dll",
            "System.Web.Optimization.dll",
            "Common.Logging.Core.dll",
            "Common.Logging.dll",
            "EntityFramework.dll",
            "EntityFramework.SqlServer.dll",
            "EntityFramework.SqlServerCompact.dll",
            "System.Data.SqlServerCe.dll",
            "Newtonsoft.Json.dll",
            "Quartz.dll",
            "SquishIt.Framework.dll",
            "SquishIt.Mvc.dll",
            "ImageProcessor.Web.dll",
            "ImageProcessor.dll",
            "AntiXssLibrary.dll",
            "HtmlSanitizationLibrary.dll",
            "System.Web.Http.dll",
            "System.Web.Http.WebHost.dll",
            "System.Net.Http.Formatting.dll",
            "System.Web.Helpers.dll",
            "System.Web.Mvc.dll",
            "System.Web.WebPages.Deployment.dll",
            "System.Web.WebPages.dll",
            "System.Web.WebPages.Razor.dll",
            "System.Web.Razor.dll",
            "Quartz.Unity.45.dll",
            "EFCache.dll",
            "HtmlAgilityPack.dll",
            "Microsoft.Practices.Unity.Configuration.dll",
            "Microsoft.Practices.Unity.dll",
            "Microsoft.Practices.Unity.RegistrationByConvention.dll",
            "Microsoft.Practices.ServiceLocation.dll",
            "Unity.WebApi.dll",
            "Unity.Mvc4.dll",
            "WebActivator.dll",
            "System.Net.Http.dll",
            "System.Net.Http.WebRequest.dll",
            "AjaxMin.dll",
            "Iesi.Collections.dll",
            "Yahoo.Yui.Compressor.dll",
            "Microsoft.Web.Services3.dll",
            "Microsoft.Web.Infrastructure.dll",
            "DotNetOpenAuth",
            "Microsoft",
        };
    }
}
