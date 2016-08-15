using System.Web.Mvc;
using Chamber.Domain.Constants;
using Chamber.Domain.DomainModel;
using Chamber.Domain.Interfaces.Services;
using Chamber.Domain.Interfaces.UnitOfWork;
using Chamber.Web.Areas.Admin.ViewModels;

namespace Chamber.Web.Areas.Admin.Controllers
{
    public partial class BaseAdminController : Controller
    {
        protected readonly IMembershipService MembershipService;
        protected readonly ISettingsService SettingsService;
        protected readonly IUnitOfWorkManager UnitOfWorkManager;
        protected readonly ILoggingService LoggingService;
        protected readonly IRoleService RoleService;

        protected MembershipUser LoggedOnReadOnlyUser;

        public BaseAdminController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            ISettingsService settingsService, IRoleService roleService)
        {
            UnitOfWorkManager = unitOfWorkManager;
            MembershipService = membershipService;
            SettingsService = settingsService;
            LoggingService = loggingService;
            RoleService = roleService;

            LoggedOnReadOnlyUser = MembershipService.GetUserByEmail(System.Web.HttpContext.Current.User.Identity.Name, true);
        }
        protected void ShowMessage(AdminGenericMessageViewModel messageViewModel)
        {
            TempData[AppConstants.MessageViewBagName] = messageViewModel;
        }

    }

    public class UserNotLoggedOnException : System.Exception
    {
    }
}