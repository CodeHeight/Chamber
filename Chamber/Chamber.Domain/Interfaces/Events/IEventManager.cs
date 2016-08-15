using System.Collections.Generic;
using System.Reflection;
using Chamber.Domain.Interfaces.Services;

namespace Chamber.Domain.Interfaces.Events
{
    public interface IEventManager
    {
        /// <summary>
        /// Use reflection to get all event handling classes. Call this ONCE.
        /// </summary>
        void Initialize(ILoggingService loggingService, List<Assembly> assemblies);

    }
}