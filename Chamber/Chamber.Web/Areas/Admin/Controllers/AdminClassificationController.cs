using System;
using System.Linq;
using System.Web.Mvc;
using Chamber.Domain.Constants;
using Chamber.Domain.DomainModel;
using Chamber.Domain.Interfaces.Services;
using Chamber.Domain.Interfaces.UnitOfWork;
using Chamber.Utilities;
using Chamber.Web.Application;
using Chamber.Web.Areas.Admin.ViewModels;
using ClassificationCreateStatus = Chamber.Domain.DomainModel.ClassificationCreateStatus;

namespace Chamber.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public partial class AdminClassificationController : BaseAdminController
    {
        private readonly IBusinessService _businessService;
        private readonly IClassificationService _classificationService;

        public AdminClassificationController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            ISettingsService settingsService, IRoleService roleService, IBusinessService businessService, IClassificationService classificationService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, roleService)
        {
            _businessService = businessService;
            _classificationService = classificationService;
        }

        public ActionResult Classification(int? p, string search)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var pageIndex = p ?? 1;

                var allClassifications = string.IsNullOrEmpty(search) ? _classificationService.GetAll(pageIndex, SiteConstants.Instance.AdminListPageSize) :
                    _classificationService.Search(search, pageIndex, SiteConstants.Instance.AdminListPageSize);

                var listViewModel = new ClassificationListViewModel
                {
                    Classifications = allClassifications,
                    PageIndex = pageIndex,
                    TotalCount = allClassifications.TotalCount,
                    Search = search,
                    TotalPages = allClassifications.TotalPages
                };
                var viewModel = new ClassificationViewModel
                {
                    _listViewModel = listViewModel
                };
                return View(viewModel);
            }
        }

        [Authorize(Roles = AppConstants.AdminRoleName)]
        [HttpGet]
        public ActionResult RemoveClassification(Guid id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var classification = _classificationService.GetById(id);
                var classificationList = _classificationService.GetAllClassifications();
                var listViewModel = new ClassificationListViewModel
                {
                    NonPagedClassifications = classificationList.ToList()
                };

                var viewModel = new ClassificationViewModel
                {
                    ClassificationId = classification.Id,
                    EditName = classification.Name,
                    EditDescription = classification.Description,
                    _listViewModel = listViewModel
                };
                return View(viewModel);
            }
        }

        [Authorize(Roles = AppConstants.AdminRoleName)]
        [HttpGet]
        public ActionResult EditClassification(Guid id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var classification = _classificationService.GetById(id);
                var classificationList = _classificationService.GetAllClassifications();
                var listViewModel = new ClassificationListViewModel
                {
                    NonPagedClassifications = classificationList.ToList()
                };

                var viewModel = new ClassificationViewModel
                {
                    ClassificationId = classification.Id,
                    EditName = classification.Name,
                    EditDescription = classification.Description,
                    _listViewModel = listViewModel
                };
                return View(viewModel);
            }
        }

        [Authorize(Roles = AppConstants.AdminRoleName)]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditClassification(ClassificationViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                try
                {
                    var loggedOnUserId = LoggedOnReadOnlyUser?.Id ?? Guid.Empty;
                    var user = MembershipService.Get(loggedOnUserId);
                    var classificationNameCheck = _classificationService.GetByName(viewModel.EditName);
                    var classification = _classificationService.GetById(viewModel.ClassificationId);


                    //Updating classification name and the name is not taken
                    if (classificationNameCheck == null)
                    {
                        classification.Name = viewModel.EditName;
                        classification.Description = viewModel.EditDescription;
                        //Fire Event
                        _classificationService.ClassificationUpdated(user, classification);
                        unitOfWork.Commit();
                        TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                        {
                            Message = "Classification was successfully updated.",
                            MessageType = GenericMessages.success
                        };
                        return RedirectToAction("Classification", "AdminClassification");
                    }
                    else
                    {
                        //Prevent updating a classification by using a name already taken.  
                        //Compare UniqueKeys by names
                        if (classificationNameCheck.Id == classification.Id)
                        {
                            classification.Name = viewModel.EditName;
                            classification.Description = viewModel.EditDescription;
                            //Fire Event
                            _classificationService.ClassificationUpdated(user, classification);
                            unitOfWork.Commit();
                            TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                            {
                                Message = "Classification was successfully updated.",
                                MessageType = GenericMessages.success
                            };
                            return RedirectToAction("Classification", "AdminClassification");
                        }
                        TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                        {
                            Message = "There is already a classification with the name " + viewModel.EditName + ".  Update Failed.",
                            MessageType = GenericMessages.danger
                        };
                    }
                    return RedirectToAction("EditClassification", "AdminClassification", new { id = viewModel.ClassificationId });
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                    {
                        Message = "Updating the classification failed.  Error was logged for administrator.",
                        MessageType = GenericMessages.danger
                    };
                    LoggingService.Error(ex);
                }
                return RedirectToAction("Classification", "AdminClassification");
            }
        }


        [Authorize(Roles = AppConstants.AdminRoleName)]
        [ValidateAntiForgeryToken]
        public ActionResult AddClassification(ClassificationViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var newClassification = new Classification
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description
                };
                //try to insert and log activity
                var createStatus = _classificationService.CreateClassification(LoggedOnReadOnlyUser, newClassification);
                if (createStatus != ClassificationCreateStatus.Success)
                {
                    //possibly log error here or service level.  FAILED
                    TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                    {
                        Message = _classificationService.ErrorCodeToString(createStatus),
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
                            Message = "Classification successfully added.",
                            MessageType = GenericMessages.success
                        };
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        LoggingService.Error(ex);
                        TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                        {
                            Message = "Classification was not successfully added.",
                            MessageType = GenericMessages.danger
                        };
                    }
                }
                return RedirectToAction("Classification", "AdminClassification");
            }
        }
    }
}