using System;
using System.Collections.Generic;
using Chamber.Domain.DomainModel;

namespace Chamber.Domain.Interfaces.Services
{
    public partial interface ISettingsService
    {
        Settings GetSettings(bool useCache = true);
        Settings Add(Settings settings);
        Settings Get(Guid id);
        IEnumerable<State> ListOfStates();
        IEnumerable<Bit> ListOfBoolean();
    }
}