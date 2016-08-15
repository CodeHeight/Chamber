using System.Collections.Generic;
using System.Collections.Specialized;

namespace Chamber.Domain.Interfaces.Services
{
    public partial interface IConfigService
    {
        #region Emojies

        string Emotify(string inputText);
        OrderedDictionary GetEmoticonHashTable();
        Dictionary<string, string> GetProjectConfig();
        Dictionary<string, string> GetTypes();

        #endregion
    }
}