using System.Data.Entity;
using Chamber.Domain.Constants;
using Chamber.Domain.Interfaces;
using Chamber.Domain.Interfaces.UnitOfWork;
using Chamber.Services.Data.Context;
using Chamber.Services.Migrations;

namespace Chamber.Services.Data.UnitOfWork
{
    public class UnitOfWorkManager : IUnitOfWorkManager
    {
        private bool _isDisposed;
        private readonly ChamberContext _context;

        public UnitOfWorkManager(IChamberContext context)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ChamberContext, Configuration>(SiteConstants.Instance.ChamberContext));
            _context = context as ChamberContext;
        }

        /// <summary>
        /// Provides an instance of a unit of work. This wrapping in the manager
        /// class helps keep concerns separated
        /// </summary>
        /// <returns></returns>
        public IUnitOfWork NewUnitOfWork()
        {
            return new UnitOfWork(_context);
        }

        /// <summary>
        /// Make sure there are no open sessions.
        /// In the web app this will be called when the injected UnitOfWork manager
        /// is disposed at the end of a request.
        /// </summary>
        public void Dispose()
        {
            if (!_isDisposed)
            {
                _context.Dispose();
                _isDisposed = true;
            }
        }
    }
}