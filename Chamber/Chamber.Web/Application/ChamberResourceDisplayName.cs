using System.ComponentModel;
using Chamber.Domain.Interfaces;

namespace Chamber.Web.Application
{
    public class ChamberResourceDisplayName : DisplayNameAttribute, IModelAttribute
    {
        private string _resourceValue = string.Empty;
        //private readonly ILocalizationService _localizationService;

        public ChamberResourceDisplayName(string resourceKey)
                : base(resourceKey)
            {
            ResourceKey = resourceKey;
            //_localizationService = ServiceFactory.Get<ILocalizationService>();
        }

        public string ResourceKey { get; set; }

        public override string DisplayName
        {
            get
            {
                //_resourceValue = _localizationService.GetResourceString(ResourceKey.Trim());
                return _resourceValue;
            }
        }

        public string Name
        {
            get { return "ChamberResourceDisplayName"; }
        }

    }
}