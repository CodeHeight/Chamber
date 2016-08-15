using System;
using System.Linq;
using System.Web.Mvc;
using Chamber.Domain.Constants;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.Enums;
using Chamber.Domain.Events;
using Chamber.Domain.Interfaces.Services;
using Chamber.Domain.Interfaces.UnitOfWork;
using Chamber.Utilities;
using Chamber.Web.Application;
using Chamber.Web.Areas.Admin.ViewModels;
using MemberhipLevelCreateStatus = Chamber.Domain.DomainModel.MemberhipLevelCreateStatus;

namespace Chamber.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public partial class AdminMembershipLevelController : BaseAdminController
    {
        private readonly IMembershipLevelService _membershipLevelService;

        public AdminMembershipLevelController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            ISettingsService settingsService, IRoleService roleService, IBusinessService bussinessService, IMembershipLevelService membershipLevelService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, roleService)
        {
            _membershipLevelService = membershipLevelService;
        }

        public ActionResult MembershipLevel(int? p, string search)
        {
            var pageIndex = p ?? 1;

            var allMembershipLevels = string.IsNullOrEmpty(search) ? _membershipLevelService.GetAll(pageIndex, SiteConstants.Instance.AdminListPageSize) :
                _membershipLevelService.Search(search, pageIndex, SiteConstants.Instance.AdminListPageSize);

            var listViewModel = new MembershipLevelListViewModel
            {
                MembershipLevels = allMembershipLevels,
                PageIndex = pageIndex,
                TotalCount = allMembershipLevels.TotalCount,
                Search = search,
                TotalPages = allMembershipLevels.TotalPages
            };
            var viewModel = new MembershipLevelViewModel
            {
                _listViewModel = listViewModel
            };
            return View(viewModel);
        }

        [Authorize(Roles = AppConstants.AdminRoleName)]
        [ValidateAntiForgeryToken]
        public ActionResult AddMembershipLevel(MembershipLevelViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var newMembershipLevel = new MembershipLevel
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    EmployeeMax = viewModel.EmployeeMax,
                    AnnualPrice = viewModel.AnnualPrice,
                    SemiAnnualPrice = viewModel.SemiAnnualPrice
                };
                //try to insert and log activity
                var createStatus = _membershipLevelService.CreateMembershipLevel(LoggedOnReadOnlyUser, newMembershipLevel);
                if (createStatus != MemberhipLevelCreateStatus.Success)
                {
                    //possibly log error here or service level.  FAILED
                    TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                    {
                        Message = _membershipLevelService.ErrorCodeToString(createStatus),
                        MessageType = GenericMessages.danger
                    };
                }
                else
                {
                    try
                    {
                        unitOfWork.Commit();
                        TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                        {
                            Message = "Membership Level successfully added.",
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                        {
                            Message = "Membership Level was not successfully added.",
                            MessageType = GenericMessages.danger
                        };
                    }
                }
                return RedirectToAction("MembershipLevel", "AdminMembershipLevel");
            }
        }

        [Authorize(Roles = AppConstants.AdminRoleName)]
        [HttpGet]
        public ActionResult EditMembershipLevel(Guid id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var membershipLevel = _membershipLevelService.GetById(id);
                var membershipLevelList = _membershipLevelService.GetAllMembershipLevels();
                var listViewModel = new MembershipLevelListViewModel
                {
                    NonPagedMembershipLevels = membershipLevelList.ToList()
                };

                var viewModel = new MembershipLevelViewModel
                {
                    MembershipLevelId = membershipLevel.Id,
                    EditName = membershipLevel.Name,
                    EditDescription = membershipLevel.Description,
                    EditEmployeeMax = membershipLevel.EmployeeMax,
                    EditAnnualPrice = membershipLevel.AnnualPrice,
                    EditSemiAnnualPrice = membershipLevel.SemiAnnualPrice,
                    _listViewModel = listViewModel
                };
                return View(viewModel);
            }
        }

        [Authorize(Roles = AppConstants.AdminRoleName)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMembershipLevel(MembershipLevelViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var loggedOnUserId = LoggedOnReadOnlyUser?.Id ?? Guid.Empty;
                    var user = MembershipService.Get(loggedOnUserId);
                    var membershipLevelNameCheck = _membershipLevelService.GetByName(viewModel.EditName);
                    var membershipLevel = _membershipLevelService.GetById(viewModel.MembershipLevelId);

                    //Update the membership level name and the name is not taken
                    if (membershipLevelNameCheck == null)
                    {
                        membershipLevel.Name = viewModel.EditName;
                        membershipLevel.Description = viewModel.EditDescription;
                        membershipLevel.EmployeeMax = viewModel.EditEmployeeMax;
                        membershipLevel.AnnualPrice = viewModel.EditAnnualPrice;
                        membershipLevel.SemiAnnualPrice = viewModel.EditSemiAnnualPrice;
                        //Fire event
                        _membershipLevelService.MembershipLevelUpdated(user, membershipLevel);
                        unitOfWork.Commit();
                        TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                        {
                            Message = "Membership Level was successfully updated.",
                            MessageType = GenericMessages.success
                        };
                        return RedirectToAction("MembershipLevel", "AdminMembershipLevel");
                    }
                    else
                    {
                        //Prevent updating a membership level by using a name already taken.  
                        //Compare UniqueKeys by names
                        if (membershipLevelNameCheck.Id == membershipLevel.Id)
                        {
                            membershipLevel.Name = viewModel.EditName;
                            membershipLevel.Description = viewModel.EditDescription;
                            membershipLevel.EmployeeMax = viewModel.EditEmployeeMax;
                            membershipLevel.AnnualPrice = viewModel.EditAnnualPrice;
                            membershipLevel.SemiAnnualPrice = viewModel.EditSemiAnnualPrice;
                            //Fire Event
                            _membershipLevelService.MembershipLevelUpdated(user, membershipLevel);
                            unitOfWork.Commit();
                            TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                            {
                                Message = "Membership Level was successfully updated.",
                                MessageType = GenericMessages.success
                            };
                            return RedirectToAction("MembershipLevel", "AdminMembershipLevel");
                        }
                        TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                        {
                            Message = "There is already a membership level with the name " + viewModel.EditName + ".  Update Failed.",
                            MessageType = GenericMessages.danger
                        };
                    }
                    return RedirectToAction("EditMembershipLevel", "AdminMembershipLevel", new { id = viewModel.MembershipLevelId });
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                    {
                        Message = "Updating the membership level failed.  Error was logged for administrator.",
                        MessageType = GenericMessages.danger
                    };
                    LoggingService.Error(ex);
                }
                return RedirectToAction("MembershipLevel", "AdminMembershipLevel");
            }
        }
    }
}