using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Chamber.Domain.Constants;
using Chamber.Domain.DomainModel;
using Chamber.Domain.Interfaces.Services;
using Chamber.Domain.Interfaces.UnitOfWork;
using Chamber.Web.ViewModels;
using MembershipUser = Chamber.Domain.DomainModel.MembershipUser;


namespace Chamber.Web.Controllers
{
    public class BaseController : Controller
    {
        protected readonly IUnitOfWorkManager UnitOfWorkManager;
        protected readonly IMembershipService MembershipService;
        protected readonly IRoleService RoleService;
        protected readonly ISettingsService SettingsService;
        protected readonly ILoggingService LoggingService;

        protected MembershipUser LoggedOnReadOnlyUser;
        protected MembershipRole UsersRole;

        public BaseController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService, IRoleService roleService, ISettingsService settingsService)
        {
            UnitOfWorkManager = unitOfWorkManager;
            MembershipService = membershipService;
            RoleService = roleService;
            SettingsService = settingsService;
            LoggingService = loggingService;

            using (UnitOfWorkManager.NewUnitOfWork())
            {
                LoggedOnReadOnlyUser = UserIsAuthenticated ? MembershipService.GetUserByEmail(Email, true) : null;
                UsersRole = LoggedOnReadOnlyUser == null ? RoleService.GetRole(AppConstants.GuestRoleName, true) : LoggedOnReadOnlyUser.Roles.FirstOrDefault();
            }
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var controller = filterContext.RouteData.Values["controller"];
            var action = filterContext.RouteData.Values["action"];
            var area = filterContext.RouteData.DataTokens["area"] ?? string.Empty;
            var settings = SettingsService.GetSettings();

            //##### we can play with settings here.  Example:
            //If the user is banned - Log them out.
            //if (LoggedOnReadOnlyUser != null && LoggedOnReadOnlyUser.IsBanned)
            //{
            //    FormsAuthentication.SignOut();
            //    TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            //    {
            //        Message = "You are banned from this site!",
            //        MessageType = GenericMessages.danger
            //    };
            //    filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "controller", "Home" }, { "action", "Index" } });
            //}
        }

        protected bool UserIsAuthenticated => System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

        protected bool UserIsAdmin => User.IsInRole(AppConstants.AdminRoleName);

        protected void ShowMessage(GenericMessageViewModel messageViewModel)
        {
            //ViewData[AppConstants.MessageViewBagName] = messageViewModel;
            TempData[AppConstants.MessageViewBagName] = messageViewModel;
        }
        protected string Email => UserIsAuthenticated ? System.Web.HttpContext.Current.User.Identity.Name : null;

        internal ActionResult ErrorToHomePage(string errorMessage)
        {
            // Use temp data as its a redirect
            TempData[AppConstants.MessageViewBagName] = new GenericMessageViewModel
            {
                Message = errorMessage,
                MessageType = GenericMessages.danger
            };
            // Not allowed in here so
            return RedirectToAction("Index", "Home");
        }
    }

    public class UserNotLoggedOnException : System.Exception
    {

    }
}