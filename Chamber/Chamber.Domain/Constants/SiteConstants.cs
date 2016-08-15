using System;
using System.Web.Mvc;
using Chamber.Domain.Interfaces.Services;

namespace Chamber.Domain.Constants
{
    public class SiteConstants
    {
        #region Singleton
        private static SiteConstants _instance;
        private static readonly object InstanceLock = new object();
        private static IConfigService _configService;
        private SiteConstants(IConfigService configService)
        {
            _configService = configService;
        }

        public static SiteConstants Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (InstanceLock)
                    {
                        if (_instance == null)
                        {
                            var configService = DependencyResolver.Current.GetService<IConfigService>();
                            _instance = new SiteConstants(configService);
                        }
                    }
                }

                return _instance;
            }
        }
        #endregion

        #region Generic Get

        /// <summary>
        /// This is the generic get config method, you can use this to also get custom config items out
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetConfig(string key)
        {
            var dict = _configService.GetProjectConfig();
            if (!string.IsNullOrEmpty(key) && dict.ContainsKey(key))
            {
                return dict[key];
            }
            return string.Empty;
        }

        public string GetType(string key)
        {
            var dict = _configService.GetTypes();
            if (!string.IsNullOrEmpty(key) && dict.ContainsKey(key))
            {
                return dict[key];
            }
            return string.Empty;
        }

        #endregion

        public string ChamberVersion => GetConfig("ChamberVersion");



        /// Paging options - Amount per page on different pages.
        //public int PagingGroupSize => Convert.ToInt32(GetConfig("PagingGroupSize"));
        //public int AdminListPageSize => Convert.ToInt32(GetConfig("AdminListPageSize"));
        //public int SearchListSize => Convert.ToInt32(GetConfig("SearchListSize"));
        public int PagingGroupSize = 7;
        public int AdminListPageSize = 7;
        public int SearchListSize => 7;



        public string ChosenEditor => GetConfig("EditorType");
        public string ChamberContext => GetConfig("ChamberContext");
        public string StorageProviderType => GetType("StorageProviderType");

        public string FileUploadAllowedExtensions => GetConfig("FileUploadAllowedExtensions");
        public string FileUploadMaximumFileSizeInBytes => GetConfig("FileUploadMaximumFileSizeInBytes");
        public string UploadFolderPath => GetConfig("UploadFolderPath");

        /// <summary>
        /// Social Gravatar size
        /// </summary>
        public int GravatarPostSize => Convert.ToInt32(GetConfig("GravatarPostSize"));
        public int GravatarTopicSize => Convert.ToInt32(GetConfig("GravatarTopicSize"));
        public int GravatarProfileSize => Convert.ToInt32(GetConfig("GravatarProfileSize"));
        public int GravatarLeaderboardSize => Convert.ToInt32(GetConfig("GravatarLeaderboardSize"));
        public int GravatarLikedBySize => Convert.ToInt32(GetConfig("GravatarLikedBySize"));
        public int GravatarLatestBySize => Convert.ToInt32(GetConfig("GravatarLatestBySize"));
        public int GravatarFooterSize => Convert.ToInt32(GetConfig("GravatarFooterSize"));

        /// <summary>
        /// Social Logins
        /// </summary>
        public string GooglePlusAppId => GetConfig("GooglePlusAppId");
        public string GooglePlusAppSecret => GetConfig("GooglePlusAppSecret");

        // Url names
        public string TagsUrlIdentifier => GetConfig("TagsUrlIdentifier");
        public string MemberUrlIdentifier => GetConfig("MemberUrlIdentifier");

    }
}
