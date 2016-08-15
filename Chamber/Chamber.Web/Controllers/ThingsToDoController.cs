using System.Web.Mvc;
using Chamber.Domain.Interfaces.Services;
using Chamber.Domain.Interfaces.UnitOfWork;

namespace Chamber.Web.Controllers
{
    public class ThingsToDoController : BaseController
    {
        public ThingsToDoController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
       IRoleService roleService, ISettingsService settingsService, IBusinessService businessService)
            : base(loggingService, unitOfWorkManager, membershipService, roleService, settingsService)
        {
        }

        public ActionResult Piedmont()
        {
            return View();
        }
    }
}