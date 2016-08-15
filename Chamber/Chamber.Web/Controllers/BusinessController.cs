using System;
using System.Linq;
using System.Web.Mvc;
using Chamber.Domain.Constants;
using Chamber.Domain.Interfaces.Services;
using Chamber.Domain.Interfaces.UnitOfWork;
using Chamber.Web.ViewModels;

namespace Chamber.Web.Controllers
{
    public class BusinessController : BaseController
    {
        private readonly IBusinessService _businessService;
        private readonly IClassificationService _classificationService;

        public BusinessController(ILoggingService loggingService, IUnitOfWorkManager unitOfWorkManager, IMembershipService membershipService,
            IRoleService roleService, ISettingsService settingsService, IBusinessService businessService, IClassificationService classificationService)
            : base(loggingService, unitOfWorkManager, membershipService, roleService, settingsService)
        {
            _businessService = businessService;
            _classificationService = classificationService;
        }

        public ActionResult Directory(int? p, string search)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var pageIndex = p ?? 1;

                var allBusinesses = string.IsNullOrEmpty(search) ? _businessService.GetAll(pageIndex, SiteConstants.Instance.AdminListPageSize) :
                    _businessService.SearchBusiness(search, pageIndex, SiteConstants.Instance.AdminListPageSize);

                var businessList = _businessService.GetAllBusiness();

                var viewModel = new BusinessListViewModel
                {
                    Businesses = allBusinesses,
                    PageIndex = pageIndex,
                    TotalCount = allBusinesses.TotalCount,
                    Search = search,
                    TotalPages = allBusinesses.TotalPages,
                    NonPagedBusinesses = businessList.ToList()
                };

                return View(viewModel);
            }
        }


        public ActionResult Classification(int? p, string search)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var pageIndex = p ?? 1;

                var allClassifications = string.IsNullOrEmpty(search) ? _classificationService.GetAll(pageIndex, SiteConstants.Instance.AdminListPageSize) :
                    _classificationService.Search(search, pageIndex, SiteConstants.Instance.AdminListPageSize);

                var allBusinesses = _businessService.GetAllBusiness();
                ClassificationListViewModel viewModel = new ClassificationListViewModel
                {
                    AllClassifications = allClassifications,
                    AllBusinesses = allBusinesses.ToList(),
                    PageIndex = pageIndex,
                    TotalCount = allClassifications.TotalCount,
                    Search = search,
                    TotalPages = allClassifications.TotalPages
                };
                return View(viewModel);
            }
        }

        public ActionResult ClassificationDirectory(Guid id, int? p)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var pageIndex = p ?? 1;
                var classification = _classificationService.GetById(id);
                var businesses = _businessService.GetAllByClassificationId(classification, pageIndex, SiteConstants.Instance.AdminListPageSize);
                var allNonPagedBusinesses = _businessService.GetAllBusiness();
                ClassificationChildListViewModel viewModel = new ClassificationChildListViewModel
                {
                    allBusinesses = businesses,
                    AllBusinesses = allNonPagedBusinesses.ToList(),
                    PageIndex = pageIndex,
                    TotalCount = businesses.TotalCount,
                    ClassificationId = classification.Id,
                    ClassificationName = classification.Name,
                    TotalPages = businesses.TotalPages
                };
                return View(viewModel);
            }
        }

        public ActionResult Individual(int? p, string search)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var pageIndex = p ?? 1;
                var allBusinesses = string.IsNullOrEmpty(search) ? _businessService.GetAllIndividuals(pageIndex, SiteConstants.Instance.AdminListPageSize) :
                _businessService.SearchIndividuals(search, pageIndex, SiteConstants.Instance.AdminListPageSize);

                var businessList = _businessService.GetAllBusiness();

                var viewModel = new BusinessContactListViewModel
                {
                    BusinessContacts = allBusinesses,
                    PageIndex = pageIndex,
                    TotalCount = allBusinesses.TotalCount,
                    Search = search,
                    TotalPages = allBusinesses.TotalPages,
                    NonPagedBusinesses = businessList.ToList()
                };

                return View(viewModel);
            }
        }

        [HttpGet]
        public ActionResult View(Guid id)
        {
            using (UnitOfWorkManager.NewUnitOfWork())
            {
                var business = _businessService.Get(id);
                if (business != null)
                {
                    var viewModel = new BusinessViewModel
                    {
                        Id = business.Id,
                        Name = business.Name,
                        Description = business.Description,
                        PhysicalAddress = business.PhysicalAddress,
                        PhysicalCity = business.PhysicalCity,
                        PhyscialState = business.PhysicalState,
                        PhysicalZipcode = business.PhysicalZipcode,
                        Avatar = business.Avatar,
                        Phone = business.Phone,
                        WebAddress = business.WebAddress,
                        businessContacts = business.BusinessContacts.ToList()
                    };
                    if (business.Classification != null)
                    {
                        viewModel.Classification = business.Classification.Name;
                    }
                    if (business.MembershipLevel != null)
                    {
                        viewModel.MembershipLevel = business.MembershipLevel.Name;
                    }
                    return View(viewModel);
                }
                //Message
                return RedirectToAction("Index", "Business");
            }
        }
    }
}