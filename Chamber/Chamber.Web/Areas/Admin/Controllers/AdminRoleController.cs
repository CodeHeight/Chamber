using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Chamber.Domain.Constants;
using Chamber.Domain.DomainModel;
using Chamber.Domain.Interfaces.Services;
using Chamber.Domain.Interfaces.UnitOfWork;
using Chamber.Web.Areas.Admin.ViewModels;
using Chamber.Web.Areas.Admin.ViewModels.Mapping;
using MembershipUser = Chamber.Domain.DomainModel.MembershipUser;

namespace Chamber.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public partial class AdminRoleController : BaseAdminController
    {
        public AdminRoleController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
          ISettingsService settingsService, IRoleService roleService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, roleService)
        {

        }

        public ActionResult RoleIndex(int? p, string search)
        {
            var pageIndex = p ?? 1;

            var allMemberships = string.IsNullOrEmpty(search) ? MembershipService.GetAll(pageIndex, SiteConstants.Instance.AdminListPageSize) :
                MembershipService.SearchMembersByLastName(search, pageIndex, SiteConstants.Instance.AdminListPageSize);

            var allViewModelUsers = allMemberships.Select(ViewModelMapping.UserToSingleMemberListViewModel).ToList();
            var viewModel = new MemberRoleListViewModel
            {
                MembershipUsers = allViewModelUsers,
                AllRoles = RoleService.AllRoles(),
                PageIndex = pageIndex,
                TotalCount = allMemberships.TotalCount,
                Search = search,
                TotalPages = allMemberships.TotalPages
            };
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = AppConstants.AdminRoleName)]
        public void UpdateUserRoles(AjaxRoleUpdateViewModel ajaxRoleUpdateViewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (Request.IsAjaxRequest())
                {
                    var user = MembershipService.Get(ajaxRoleUpdateViewModel.Id);

                    UpdateUserRoles(user, ajaxRoleUpdateViewModel.Roles);

                    try
                    {
                        unitOfWork.Commit();
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        //LoggingService.Error(ex);
                        throw new Exception("Error updating user roles");
                    }
                }
            }
        }

        private void UpdateUserRoles(MembershipUser user, IEnumerable<string> updatedRoles)
        {
            // ---------------------------------------------------------------------
            // IMPORTANT - If you call this it MUST be within a unit of work
            // ---------------------------------------------------------------------

            // Not done in automapper to avoid handling services in the mapper
            var updatedRolesSet = new List<MembershipRole>();
            foreach (var roleStr in updatedRoles)
            {
                var alreadyIsRoleForUser = false;
                foreach (var role in user.Roles)
                {
                    if (roleStr == role.RoleName)
                    {
                        // This role for this user is UNchanged
                        updatedRolesSet.Add(role);
                        alreadyIsRoleForUser = true;
                        break;
                    }
                }

                if (!alreadyIsRoleForUser)
                {
                    // This is a new role for this user
                    updatedRolesSet.Add(RoleService.GetRole(roleStr));
                }
            }

            // Replace the roles in the user's collection
            user.Roles.Clear();
            foreach (var role in updatedRolesSet)
            {
                user.Roles.Add(role);
            }
        }
    }
}