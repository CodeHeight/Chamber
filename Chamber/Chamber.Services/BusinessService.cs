using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Hosting;
using Chamber.Domain.Events;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.General;
using Chamber.Domain.Interfaces;
using Chamber.Domain.Interfaces.Services;
using Chamber.Services.Data.Context;
using Chamber.Utilities;


namespace Chamber.Services
{
    public partial class BusinessService : IBusinessService
    {
        private readonly ChamberContext _context;
        private readonly IActivityService _activityService;
        private readonly ISettingsService _settingsService;

        public BusinessService(IChamberContext context, IActivityService activityService, ISettingsService settingsService)
        {
            _context = context as ChamberContext;
            _activityService = activityService;
            _settingsService = settingsService;
        }

        public Business Add(Business business)
        {
            SanitizeBusiness(business);
            business.CreateDate = DateTime.UtcNow;
            return _context.Business.Add(business);
        }

        public void Delete(Business business)
        {
            _context.Business.Remove(business);
        }

        public IList<Business> GetAllBusiness()
        {
            return _context.Business.ToList();
        }

        public PagedList<Business> GetAll(int pageIndex, int pageSize)
        {
            var totalCount = _context.Business.Count();
            var results = _context.Business
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new PagedList<Business>(results, pageIndex, pageSize, totalCount);
        }

        public PagedList<Business> GetAllByClassificationId(Classification classification, int pageIndex, int pageSize)
        {
            var businesses = _context.Business
                .Where(x => x.Classification.Id == classification.Id);

            var results = businesses
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new PagedList<Business>(results, pageIndex, pageSize, businesses.Count());
        }

        public PagedList<BusinessContact> GetAllIndividuals(int pageIndex, int pageSize)
        {
            var totalCount = _context.BusinessContact.Count();
            var results = _context.BusinessContact
                .OrderBy(x => x.LastName)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new PagedList<BusinessContact>(results, pageIndex, pageSize, totalCount);
        }

        public PagedList<Business> SearchBusiness(string search, int pageIndex, int pageSize)
        {
            search = StringUtils.SafePlainText(search);
            var query = _context.Business
                .Where(x => x.Name.ToUpper().Contains(search.ToUpper()));

            var results = query
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedList<Business>(results, pageIndex, pageSize, query.Count());
        }

        public PagedList<BusinessContact> SearchIndividuals(string search, int pageIndex, int pageSize)
        {
            search = StringUtils.SafePlainText(search);

            var query = _context.BusinessContact
                .Where(x => x.LastName.Contains(search.ToUpper()) || x.FirstName.Contains(search.ToUpper()));

            var results = query
                .OrderBy(x => x.LastName)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new PagedList<BusinessContact>(results, pageIndex, pageSize, query.Count());
        }

        public IList<Business> GetLatestBusinesses(int amountToTake)
        {
            return _context.Business.AsNoTracking()
                .OrderByDescending(x => x.CreateDate)
                .Take(amountToTake)
                .ToList();
        }

        //decided not to use, need more control. (look in controller)
        //public BusinessCreateStatus AdminCreateBusiness(Business business)
        //{
        //    business = SanitizeBusiness(business);
        //    var settings = _settingsService.GetSettings(false);
        //    var status = BusinessCreateStatus.Incomplete;

        //    var e = new AdminAddBusinessEventArgs { Business = business, Admin = business }; //userId, not business
        //    EventManager.Instance.FireBeforeAdminBusinessAdd(this, e);
        //    if (!e.Cancel)
        //    {
        //        status = e.CreateStatus;
        //    }
        //    else
        //    {
        //        if (string.IsNullOrEmpty(business.Name))
        //        {
        //            status = BusinessCreateStatus.Rejected;
        //        }
        //        //if (GetByName(business, true) != null)
        //        //{
        //        //    status = BusinessCreateStatus.DuplicateName;
        //        //}
        //        if (status == BusinessCreateStatus.Incomplete)
        //        {
        //            try
        //            {
        //                Add(business);
        //            }
        //            catch (Exception)
        //            {
        //                status = BusinessCreateStatus.Rejected;
        //                //log error;
        //            }
        //        }
        //    }
        //    return status;
        //}

        public string ErrorCodeToString(BusinessCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case BusinessCreateStatus.DuplicateName:
                    return "Duplicate Name";
                case BusinessCreateStatus.Incomplete:
                    return "Incomplete Status";
                case BusinessCreateStatus.Rejected:
                    return "Rejected Status";
                default:
                    return "Unknown Error";
            }
        }

        public Business SanitizeBusiness(Business business)
        {
            business.Name = StringUtils.SafePlainText(business.Name);
            business.PhysicalCity = StringUtils.SafePlainText(business.PhysicalCity);
            return business;
        }

        public void AdminBusinessAdded(Business business, MembershipUser admin)
        {
            var e = new AdminAddBusinessEventArgs { Business = business, Admin = admin };
            EventManager.Instance.FireBeforeAdminBusinessAdd(this, e);
            if (!e.Cancel)
            {
                EventManager.Instance.FireAfterAdminBusinessAdd(this, new AdminAddBusinessEventArgs { Business = business, Admin = admin });
                _activityService.AdminBusinessAdded(business, admin);
            }
        }

        public Business GetByName(string businessName, bool removeTracking = false)
        {
            Business business;

            if (removeTracking)
            {
                business = _context.Business
                    .AsNoTracking()
                    .FirstOrDefault(name => name.Name.Equals(businessName, StringComparison.CurrentCultureIgnoreCase));
            }
            else
            {
                business = _context.Business
                    .FirstOrDefault(name => name.Name.Equals(businessName, StringComparison.CurrentCultureIgnoreCase));
            }
            return business;
        }

        public Business Get(Guid id)
        {
            var results = _context.Business
                .FirstOrDefault(x => x.Id == id);

            return results;
        }

        public void AdminBusinessUpdated(Business business, MembershipUser admin)
        {
            var e = new AdminUpdateBusinessEventArgs { Business = business, Admin = admin };
            EventManager.Instance.FireBeforeAdminBusinessUpdate(this, e);
            if (!e.Cancel)
            {
                EventManager.Instance.FireAfterAdminBusinessUpdate(this, new AdminUpdateBusinessEventArgs { Business = business, Admin = admin });
                _activityService.AdminBusinessUpdated(business, admin);
            }
        }

        public IList<BusinessContact> GetAllPOCsByBusiness(Guid businessId)
        {
            return _context.BusinessContact.Where(x => x.Business.Id == businessId).ToList();
        }

        public void AddBusinessContact(BusinessContact businessContact)
        {
            businessContact.CreateDate = DateTime.UtcNow;
            _context.BusinessContact.Add(businessContact);
        }

        public void AdminBusinessContactAdded(BusinessContact businessContact, MembershipUser admin)
        {
            var e = new AdminAddBusinessContactEventArgs { BusinessContact = businessContact, Admin = admin };
            EventManager.Instance.FireBeforeAdminBusinessContactAdd(this, e);
            if (!e.Cancel)
            {
                EventManager.Instance.FireAfterAdminBusinessContactAdd(this, new AdminAddBusinessContactEventArgs { BusinessContact = businessContact, Admin = admin });
                _activityService.AdminBusinessContactAdded(businessContact, admin);
            }
        }

        public IList<Business> GetUserBusinesses(Guid id)
        {
            return _context.Business.Where(x => x.User.Id == id).ToList();
        }

        public void AddBusinessBalance(BusinessBalance businessBalance)
        {
            businessBalance.CreateDate = DateTime.UtcNow;
            _context.BusinessBalance.Add(businessBalance);
        }

        public void AdminBusinessBalanceAdded(BusinessBalance businessBalance, MembershipUser admin)
        {
            var e = new AdminAddBusinessBalanceEventArgs { BusinessBalance = businessBalance, Admin = admin };
            EventManager.Instance.FireAfterAdminBusinessBalanceAdd(this, e);
            if (!e.Cancel)
            {
                EventManager.Instance.FireAfterAdminBusinessBalanceAdd(this, new AdminAddBusinessBalanceEventArgs { BusinessBalance = businessBalance, Admin = admin });
                _activityService.AdminBusinessBalanceAdded(businessBalance, admin);
            }
        }

        public PagedList<BusinessBalance> GetUserBusinessBalances(Guid businessId, int pageIndex, int pageSize)
        {
            var totalCount = _context.BusinessBalance.Where(x => x.Business.Id == businessId).Count();
            var results = _context.BusinessBalance
                .Where(x => x.Business.Id == businessId)
                .OrderByDescending(x => x.CreateDate)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new PagedList<BusinessBalance>(results, pageIndex, pageSize, totalCount);
        }

        public BusinessBalance GetTopBusinessBalance(Guid businessId)
        {
            return _context.BusinessBalance.FirstOrDefault(u => u.Business.Id == businessId);
        }
    }
}