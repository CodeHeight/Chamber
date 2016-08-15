using System;

namespace Chamber.Domain.Interfaces.Services
{
    public partial interface ILoggingService
    {
        void Error(Exception ex);
        void Error(string error);
    }
}