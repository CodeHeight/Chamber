using System;
using System.Collections.Generic;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.General;

namespace Chamber.Domain.Interfaces.Services
{
    public partial interface IBusinessService
    {
        Business Add(Business business);
        void Delete(Business business);
        IList<Business> GetAllBusiness();
        PagedList<Business> GetAll(int pageIndex, int pageSize);
        PagedList<Business> GetAllByClassificationId(Classification classification, int pageIndex, int pageSize);
        PagedList<BusinessContact> GetAllIndividuals(int pageIndex, int pageSize);
        PagedList<Business> SearchBusiness(string search, int pageIndex, int pageSize);
        PagedList<BusinessContact> SearchIndividuals(string search, int pageIndex, int pageSize);
        IList<Business> GetLatestBusinesses(int amountToTake);
        //BusinessCreateStatus AdminCreateBusiness(Business business);
        string ErrorCodeToString(BusinessCreateStatus createStatus);
        Business SanitizeBusiness(Business business);
        void AdminBusinessAdded(Business businesss, MembershipUser admin);
        Business GetByName(string name, bool removeTracking = false);
        Business Get(Guid id);
        void AdminBusinessUpdated(Business business, MembershipUser admin);
        IList<BusinessContact> GetAllPOCsByBusiness(Guid businessId);
        void AddBusinessContact(BusinessContact businessContact);
        void AdminBusinessContactAdded(BusinessContact businessContact, MembershipUser admin);
        IList<Business> GetUserBusinesses(Guid id);
        void AddBusinessBalance(BusinessBalance businessBalance);
        void AdminBusinessBalanceAdded(BusinessBalance businessBalance, MembershipUser admin);
        PagedList<BusinessBalance> GetUserBusinessBalances(Guid businessId, int pageIndex, int pageSize);
        BusinessBalance GetTopBusinessBalance(Guid businessId);
    }
}
