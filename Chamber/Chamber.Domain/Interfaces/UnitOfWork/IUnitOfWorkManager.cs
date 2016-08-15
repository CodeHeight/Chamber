using System;

namespace Chamber.Domain.Interfaces.UnitOfWork
{
    public partial interface IUnitOfWorkManager : IDisposable
    {
        IUnitOfWork NewUnitOfWork();
    }
}