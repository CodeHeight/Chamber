using System.Web.Mvc;
using Chamber.Domain.Constants;
using Chamber.Domain.Interfaces.Services;
using Chamber.Domain.Interfaces.UnitOfWork;
using Chamber.Utilities;

namespace Chamber.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public partial class AdminController : BaseAdminController
    {
        public AdminController(ILoggingService loggingService,
           IUnitOfWorkManager unitOfWorkManager,
           IMembershipService membershipService,
           ISettingsService settingsService,
           IRoleService roleService)
           : base(loggingService, unitOfWorkManager, membershipService, settingsService, roleService)
        {
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UnderDevelopment()
        {
            return View();
        }
    }
}