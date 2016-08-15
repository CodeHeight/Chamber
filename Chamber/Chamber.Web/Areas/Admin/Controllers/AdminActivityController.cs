using System.Web.Mvc;
using Chamber.Domain.Constants;
using Chamber.Domain.Interfaces.Services;
using Chamber.Domain.Interfaces.UnitOfWork;
using Chamber.Web.Areas.Admin.ViewModels;
using Chamber.Utilities;

namespace Chamber.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public class AdminActivityController : BaseAdminController
    {
        private readonly IActivityService _activityService;
        private int activitiesPerPage = 7;

        public AdminActivityController(ILoggingService loggingService,
           IUnitOfWorkManager unitOfWorkManager,
           IMembershipService membershipService,
           ISettingsService settingsService,
           IRoleService roleService,
           IActivityService activityService)
           : base(loggingService, unitOfWorkManager, membershipService, settingsService, roleService)
        {
            _activityService = activityService;
        }

        public ActionResult Home(int? p)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                // Set the page index
                var pageIndex = p ?? 1;

                // Get the topics
                var activities = _activityService.GetPagedGroupedActivities(pageIndex, activitiesPerPage);

                // create the view model
                var viewModel = new AllRecentActivitiesViewModel
                {
                    Activities = activities,
                    PageIndex = pageIndex,
                    TotalCount = activities.TotalCount,
                };

                return View(viewModel);
            }
            //return RedirectToAction("Home", "Main", null);
        }
    }
}