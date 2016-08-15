using System;
using System.Collections.Generic;
using System.Reflection;

namespace Chamber.Domain.Interfaces.Services
{
    public partial interface IReflectionService
    {
        List<Assembly> GetAssemblies();
        bool InterfaceFilter(Type typeObj, Object criteriaObj);
    }
}
