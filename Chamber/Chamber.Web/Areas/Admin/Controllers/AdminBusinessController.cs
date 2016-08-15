using System;
using System.IO;
using System.Linq;
using System.Web.Hosting;
using System.Web.Mvc;
using Chamber.Domain.Constants;
using Chamber.Domain.DomainModel;
using Chamber.Domain.Interfaces.Services;
using Chamber.Domain.Interfaces.UnitOfWork;
using Chamber.Web.Application;
using Chamber.Web.Areas.Admin.ViewModels;
using Chamber.Web.Areas.Admin.ViewModels.Mapping;

namespace Chamber.Web.Areas.Admin.Controllers
{
    [Authorize(Roles = AppConstants.AdminRoleName)]
    public partial class AdminBusinessController : BaseAdminController
    {
        private readonly IBusinessService _businessService;
        private readonly IClassificationService _classificationService;
        private readonly IMembershipLevelService _membershipLevelService;

        public AdminBusinessController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            ISettingsService settingsService, IRoleService roleService, IBusinessService bussinessService, IClassificationService classificationService,
            IMembershipLevelService membershipLevelService)
            : base(loggingService, unitOfWorkManager, membershipService, settingsService, roleService)
        {
            _businessService = bussinessService;
            _classificationService = classificationService;
            _membershipLevelService = membershipLevelService;
        }

        [HttpGet]
        public ActionResult UpdateDues(int? p, Guid id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var pageIndex = p ?? 1;
                var business = _businessService.Get(id);
                var allDues = _businessService.GetUserBusinessBalances(business.Id, pageIndex, SiteConstants.Instance.AdminListPageSize);
                BusinessDuesViewModel viewModel = new BusinessDuesViewModel()
                {
                    BusinessId = business.Id,
                    BusinessName = business.Name,
                    AllDuesPaid = allDues,
                    PageIndex = pageIndex,
                    TotalCount = allDues.TotalCount,
                    TotalPages = allDues.TotalPages
                };

                return View(viewModel);
            }
        }

        public ActionResult Business(int? p, string search)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var pageIndex = p ?? 1;

                var allBusinesses = string.IsNullOrEmpty(search) ? _businessService.GetAll(pageIndex, SiteConstants.Instance.AdminListPageSize) :
                    _businessService.SearchBusiness(search, pageIndex, SiteConstants.Instance.AdminListPageSize);

                var listViewModel = new BusinessListViewModel
                {
                    Businesses = allBusinesses,
                    PageIndex = pageIndex,
                    TotalCount = allBusinesses.TotalCount,
                    Search = search,
                    TotalPages = allBusinesses.TotalPages
                };
                StatesViewModel statesViewModel = new StatesViewModel()
                {
                    allStates = SettingsService.ListOfStates().ToList()
                };

                var viewModel = new BusinessViewModel
                {
                    _listViewModel = listViewModel,
                    _stateViewModel = statesViewModel
                };
                return View(viewModel);
            }
        }

        [HttpGet]
        public ActionResult POC(Guid id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var business = _businessService.Get(id);
                var businessContacts = _businessService.GetAllPOCsByBusiness(id).ToList();
                var viewModel = new BusinessContactViewModel
                {
                    business = business,
                    businessContacts = businessContacts
                };

                return View(viewModel);
            }
        }

        [HttpPost]
        public ActionResult AddDues(BusinessDuesViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                if (viewModel.AmountPaid == null && viewModel.AmountDue == null)
                {
                    TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                    {
                        Message = "Enter an amount due.",
                        MessageType = GenericMessages.danger
                    };
                    return RedirectToAction("UpdateDues", "AdminBusiness", new { id = viewModel.BusinessId });
                }

                var loggedOnUserId = LoggedOnReadOnlyUser?.Id ?? Guid.Empty;
                var admin = MembershipService.Get(loggedOnUserId);
                var settings = SettingsService.GetSettings();
                var business = _businessService.Get(viewModel.BusinessId);

                var newBusinessBalance = new BusinessBalance()
                {
                    AmountDue = viewModel.AmountDue,
                    AmountPaid = viewModel.AmountPaid,
                    DueDate = viewModel.DueDate,
                    PaidDate = viewModel.PaidDate
                };

                newBusinessBalance.Business = business;
                _businessService.AddBusinessBalance(newBusinessBalance);
                _businessService.AdminBusinessBalanceAdded(newBusinessBalance, admin);
                unitOfWork.Commit();
                TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                {
                    Message = "Business Due Added.",
                    MessageType = GenericMessages.success
                };
                return RedirectToAction("UpdateDues", "AdminBusiness", new { id = business.Id });
            }
        }

        [HttpPost]
        public ActionResult AddPOC(BusinessContactViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var loggedOnUserId = LoggedOnReadOnlyUser?.Id ?? Guid.Empty;
                var admin = MembershipService.Get(loggedOnUserId);
                var settings = SettingsService.GetSettings();
                var business = _businessService.Get(viewModel.Id);

                var newContact = new BusinessContact
                {
                    FirstName = viewModel.FirstName,
                    LastName = viewModel.LastName,
                    PrimaryPhone = viewModel.PrimaryPhoneNumber,
                    Email = viewModel.Email,
                    Business = business
                };

                try
                {
                    _businessService.AddBusinessContact(newContact);
                    _businessService.AdminBusinessContactAdded(newContact, admin);
                    unitOfWork.Commit();
                    TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                    {
                        Message = "Business Contact Added.",
                        MessageType = GenericMessages.success
                    };
                    return RedirectToAction("POC", "AdminBusiness", new { id = business.Id });
                }
                catch (Exception ex)
                {
                    unitOfWork.Rollback();
                    //LoggingService.Error(ex);
                    TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                    {
                        Message = "Adding a business contact failed.",
                        MessageType = GenericMessages.danger
                    };
                }

                return RedirectToAction("POC", "AdminBusiness", new { id = business.Id });
            }
        }

        [HttpPost]
        public ActionResult AddBusiness(BusinessViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var loggedOnUserId = LoggedOnReadOnlyUser?.Id ?? Guid.Empty;
                var admin = MembershipService.Get(loggedOnUserId);
                var settings = SettingsService.GetSettings();

                var businessToAdd = new Business
                {
                    Name = viewModel.Name,
                    PhysicalCity = viewModel.PhysicalCity,
                    PhysicalState = viewModel.PhysicalState,
                    Active = false,
                    Completed = false
                };

                StatesViewModel statesViewModel = new StatesViewModel()          //Load static State dropdown
                {
                    allStates = SettingsService.ListOfStates().ToList()
                };
                viewModel._stateViewModel = statesViewModel;

                //ToDo: 
                //1)check if name is duplicate
                //2)
                //var createStatus = _businessService.AdminCreateBusiness(businessToAdd, loggedOnUserId);
                var business = _businessService.GetByName(viewModel.Name);
                if (business == null)
                {
                    try
                    {
                        business = _businessService.Add(businessToAdd);
                        _businessService.AdminBusinessAdded(businessToAdd, admin);
                        unitOfWork.Commit();
                        TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                        {
                            Message = "Business Added.",
                            MessageType = GenericMessages.success
                        };
                        return RedirectToAction("UpdateBusiness", "AdminBusiness", new { id = business.Id });
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        //LoggingService.Error(ex);
                        TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                        {
                            Message = "Adding a business failed.",
                            MessageType = GenericMessages.danger
                        };
                    }
                }
                else
                {
                    TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                    {
                        Message = "Duplicate business name.  Add failed.",
                        MessageType = GenericMessages.danger
                    };
                }
            }
            return View(viewModel);
        }

        [HttpGet]
        public ActionResult UpdateBusiness(Guid id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var business = _businessService.Get(id);
                if (business != null)
                {
                    var loggedOnUserId = LoggedOnReadOnlyUser?.Id ?? Guid.Empty;
                    var settings = SettingsService.GetSettings();
                    var allMemberships = MembershipService.GetAll();
                    var allMembersViewModel = allMemberships.Select(ViewModelMapping.UserToSingleMemberListViewModel).ToList();

                    ListBooleanViewModel booleanViewModel = new ListBooleanViewModel()
                    {
                        ListBoolean = SettingsService.ListOfBoolean().ToList()
                    };
                    StatesViewModel statesViewModel = new StatesViewModel()
                    {
                        allStates = SettingsService.ListOfStates().ToList()
                    };
                    AllClassificationsViewModel classificationViewModel = new AllClassificationsViewModel()
                    {
                        allClassifications = _classificationService.GetAllClassifications().ToList()
                    };
                    AllMembershipLevelViewModel membershipLevelViewModel = new AllMembershipLevelViewModel()
                    {
                        allMembershipLevels = _membershipLevelService.GetAllMembershipLevels().ToList()
                    };
                    AllMembersListViewModel membersListViewModel = new AllMembersListViewModel()
                    {
                        AllMembershipUsersList = allMembersViewModel
                    };
                    UpdateBusinessViewModel viewModel = new UpdateBusinessViewModel()
                    {
                        Id = business.Id,
                        Name = business.Name,
                        MailingAddress = business.MailingAddress,
                        MailingCity = business.MailingCity,
                        MailingState = business.MailingState,
                        MailingZipcode = business.MailingZipcode,
                        PhysicalAddress = business.PhysicalAddress,
                        PhysicalCity = business.PhysicalCity,
                        PhysicalState = business.PhysicalState,
                        PhysicalZipcode = business.PhysicalZipcode,
                        Active = business.Active,
                        Completed = business.Completed,
                        Description = business.Description,
                        Avatar = business.Avatar,
                        Phone = business.Phone,
                        WebAddress = business.WebAddress,
                        _booleanViewModel = booleanViewModel,
                        _stateViewModel = statesViewModel,
                        _allClassificationsViewModel = classificationViewModel,
                        _allMembershipLevelViewModel = membershipLevelViewModel,
                        _allMembersListViewModel = membersListViewModel
                    };
                    if (business.Classification != null)
                    {
                        viewModel.Classification_Id = business.Classification.Id;
                    }
                    if (business.MembershipLevel != null)
                    {
                        viewModel.MembershipLevel_Id = business.MembershipLevel.Id;
                    }
                    if (business.User != null)
                    {
                        viewModel.MembershipUser_Id = business.User.Id;
                    }
                    return View(viewModel);
                }
                TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                {
                    Message = "Loading the business information failed.  Try again or contact Administrator.",
                    MessageType = GenericMessages.success
                };
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
        }

        [HttpPost]
        public ActionResult UpdateBusiness(UpdateBusinessViewModel viewModel)
        {
            using (var unitOfWork = UnitOfWorkManager.NewUnitOfWork())
            {
                var business = _businessService.Get(viewModel.Id);  //need to use slug/or check is guid is modified
                if (business != null)
                {
                    var loggedOnUserId = LoggedOnReadOnlyUser?.Id ?? Guid.Empty;
                    var admin = MembershipService.Get(loggedOnUserId);
                    var settings = SettingsService.GetSettings();
                    var allMemberships = MembershipService.GetAll();
                    //query for User dropdown
                    var allMembersViewModel = allMemberships.Select(ViewModelMapping.UserToSingleMemberListViewModel).ToList();
                    //Load dropdowns:
                    ListBooleanViewModel booleanViewModel = new ListBooleanViewModel()
                    {
                        ListBoolean = SettingsService.ListOfBoolean().ToList()
                    };
                    StatesViewModel statesViewModel = new StatesViewModel()
                    {
                        allStates = SettingsService.ListOfStates().ToList()
                    };
                    AllClassificationsViewModel classificationViewModel = new AllClassificationsViewModel()
                    {
                        allClassifications = _classificationService.GetAllClassifications().ToList()
                    };
                    AllMembershipLevelViewModel membershipLevelViewModel = new AllMembershipLevelViewModel()
                    {
                        allMembershipLevels = _membershipLevelService.GetAllMembershipLevels().ToList()
                    };
                    AllMembersListViewModel membersListViewModel = new AllMembersListViewModel()
                    {
                        AllMembershipUsersList = allMembersViewModel
                    };

                    // Sort image out first
                    if (viewModel.Files != null)
                    {
                        // Before we save anything, check the user already has an upload folder and if not create one
                        var uploadFolderPath = HostingEnvironment.MapPath(string.Concat(SiteConstants.Instance.UploadFolderPath, business.Id));
                        if (!Directory.Exists(uploadFolderPath))
                        {
                            Directory.CreateDirectory(uploadFolderPath);
                        }

                        // Loop through each file and get the file info and save to the users folder and Db
                        var file = viewModel.Files[0];
                        if (file != null)
                        {
                            // If successful then upload the file
                            var uploadResult = AppHelpers.UploadFile(file, uploadFolderPath, true);

                            if (!uploadResult.UploadSuccessful)
                            {
                                TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                                {
                                    Message = uploadResult.ErrorMessage,
                                    MessageType = GenericMessages.danger
                                };
                                return View(viewModel);
                            }
                            // Save avatar to user
                            business.Avatar = uploadResult.UploadedFileName;
                        }
                    }
                    viewModel.Avatar = business.Avatar;
                    viewModel._booleanViewModel = booleanViewModel;
                    viewModel._stateViewModel = statesViewModel;
                    viewModel._allClassificationsViewModel = classificationViewModel;
                    viewModel._allMembershipLevelViewModel = membershipLevelViewModel;
                    viewModel._allMembersListViewModel = membersListViewModel;

                    business.Id = viewModel.Id;
                    business.Name = viewModel.Name; //Need to check for duplicates (look at edit DisplayName)
                    business.MailingAddress = viewModel.MailingAddress;
                    business.MailingCity = viewModel.MailingCity;
                    business.MailingState = viewModel.MailingState;
                    business.MailingZipcode = viewModel.MailingZipcode;
                    business.PhysicalAddress = viewModel.PhysicalAddress;
                    business.PhysicalCity = viewModel.PhysicalCity;
                    business.PhysicalState = viewModel.PhysicalState;
                    business.PhysicalZipcode = viewModel.PhysicalZipcode;
                    business.Active = viewModel.Active;
                    business.Completed = viewModel.Completed;
                    business.Description = viewModel.Description;
                    business.Phone = viewModel.Phone;
                    business.WebAddress = viewModel.WebAddress;

                    if (viewModel.Classification_Id != null)
                    {
                        var classification = _classificationService.GetById(viewModel.Classification_Id);
                        business.Classification = classification;
                    }
                    if (viewModel.MembershipLevel_Id != null)
                    {
                        var membershipLevel = _membershipLevelService.GetById(viewModel.MembershipLevel_Id);
                        business.MembershipLevel = membershipLevel;
                    }
                    if (viewModel.MembershipUser_Id != null)
                    {
                        var membershipUser = MembershipService.Get(viewModel.MembershipUser_Id);
                        business.User = membershipUser;
                    }
                    try
                    {
                        _businessService.AdminBusinessUpdated(business, admin);
                        unitOfWork.Commit();
                        ShowMessage(new AdminGenericMessageViewModel
                        {
                            Message = "Saving the business information was successful.",
                            MessageType = GenericMessages.success
                        });
                    }
                    catch (Exception ex)
                    {
                        unitOfWork.Rollback();
                        //LoggingService.Error(ex);
                        TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                        {
                            Message = "Saving the business information failed.  Try again or contact Administrator.",
                            MessageType = GenericMessages.danger
                        };
                    }
                    return View(viewModel);
                }

                TempData[AppConstants.MessageViewBagName] = new AdminGenericMessageViewModel
                {
                    Message = "Saving the business information failed.  Try again or contact Administrator.",
                    MessageType = GenericMessages.danger
                };
                return RedirectToAction("Index", "Admin", new { area = "Admin" });
            }
        }
    }
}