using System;
using System.Linq;
using Chamber.Domain.Constants;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.Enums;
using Chamber.Domain.Interfaces;
using Chamber.Domain.Interfaces.Services;
using Chamber.Services.Data.Context;
using System.Collections.Generic;

namespace Chamber.Services
{
    public partial class SettingsService : ISettingsService
    {
        private readonly ChamberContext _context;
        private readonly ICacheService _cacheService;

        public SettingsService(IChamberContext context, ICacheService cacheService)
        {
            _cacheService = cacheService;
            _context = context as ChamberContext;
        }

        /// <summary>
        /// Get the site settings from cache, if not in cache gets from database and adds into the cache
        /// </summary>
        /// <returns></returns>
        public Settings GetSettings(bool useCache = true)
        {
            if (useCache)
            {
                var cachedSettings = _cacheService.Get<Settings>(AppConstants.SettingsCacheName);
                if (cachedSettings == null)
                {
                    cachedSettings = GetSettingsLocal(false);
                    _cacheService.Set(AppConstants.SettingsCacheName, cachedSettings, CacheTimes.OneDay);
                }
                return cachedSettings;
            }
            return GetSettingsLocal(true);
        }

        private Settings GetSettingsLocal(bool addTracking)
        {
            var theSettings = _context.Setting;
            //return theSettings;

            return addTracking ? theSettings.FirstOrDefault() : theSettings.AsNoTracking().FirstOrDefault();
        }

        public Settings Add(Settings settings)
        {
            return _context.Setting.Add(settings);
        }

        public Settings Get(Guid id)
        {
            return _context.Setting.FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<State> ListOfStates()
        {
            var states = ServiceHelpers.CreateStateList();
            return states.ToList();
        }
        public IEnumerable<Bit> ListOfBoolean()
        {
            var bits = ServiceHelpers.CreateBitList();
            return bits.ToList();
        }
    }
}