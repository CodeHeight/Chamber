using System;
using System.Collections.Generic;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.General;
using Chamber.Domain.DomainModel.Activity;

namespace Chamber.Domain.Interfaces.Services
{
    public partial interface IActivityService
    {

        PagedList<ActivityBase> GetPagedGroupedActivities(int pageIndex, int pageSize);

        /// Gets all activities by search data field for a Guid
        IEnumerable<Activity> GetDataFieldByGuid(Guid guid);

        /// Get a paged list of activities by search string
        PagedList<ActivityBase> SearchPagedGroupedActivities(string search, int pageIndex, int pageSize);

        IEnumerable<ActivityBase> GetAll(int howMany);

        /// Delete a number of activities
        void Delete(IList<Activity> activities);
        Activity Add(Activity newActivity);
        Activity Get(Guid id);
        void Delete(Activity item);


        void MemberJoined(MembershipUser user);
        void ProfileUpdated(MembershipUser user);
        void AdminProfileUpdated(MembershipUser user, MembershipUser admin);
        void ClassificationAdded(MembershipUser user, Classification classification);
        void ClassificationUpdated(MembershipUser user, Classification classification);
        void ClassificationRemoved(MembershipUser user, Classification classification);
        void MembershipLevelAdded(MembershipUser user, MembershipLevel membershipLevel);
        void MembershipLevelUpdated(MembershipUser user, MembershipLevel membershipLevel);
        void AdminBusinessAdded(Business business, MembershipUser admin);
        void AdminBusinessUpdated(Business business, MembershipUser admin);
        void AdminRegisterUser(MembershipUser user, MembershipUser admin);
        void AdminBusinessContactAdded(BusinessContact businessContact, MembershipUser admin);
        void AdminBusinessBalanceAdded(BusinessBalance businessBalance, MembershipUser admin);
    }
}