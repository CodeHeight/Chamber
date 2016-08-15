using System;
using System.Collections.Generic;
using System.Linq;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.Activity;
using Chamber.Domain.DomainModel.General;
using Chamber.Domain.Interfaces;
using Chamber.Domain.Interfaces.Services;
using Chamber.Services.Data.Context;
using Chamber.Utilities;


namespace Chamber.Services
{
    public partial class ActivityService : IActivityService
    {
        private readonly ChamberContext _context;
        private readonly ILoggingService _loggingService;

        public ActivityService(ILoggingService loggingService, IChamberContext context)
        {
            _loggingService = loggingService;
            _context = context as ChamberContext;
        }


        public PagedList<ActivityBase> GetPagedGroupedActivities(int pageIndex, int pageSize)
        {
            // Read the database for all activities and convert each to a more specialised activity type

            var totalCount = _context.Activity.Count();
            var results = _context.Activity
                  .OrderByDescending(x => x.Timestamp)
                  .Skip((pageIndex - 1) * pageSize)
                  .Take(pageSize)
                  .ToList();

            // Return a paged list
            var activities = new PagedList<Activity>(results, pageIndex, pageSize, totalCount);

            // Convert
            var specificActivities = ConvertToSpecificActivities(activities, pageIndex, pageSize);

            return specificActivities;
        }


        public IEnumerable<Activity> GetDataFieldByGuid(Guid guid)
        {
            var stringGuid = guid.ToString();
            return _context.Activity.Where(x => x.Data.Contains(stringGuid));
        }


        public PagedList<ActivityBase> SearchPagedGroupedActivities(string search, int pageIndex, int pageSize)
        {
            // Read the database for all activities and convert each to a more specialised activity type
            search = StringUtils.SafePlainText(search);
            var totalCount = _context.Activity.Count(x => x.Type.ToUpper().Contains(search.ToUpper()));

            // Get the topics using an efficient
            var results = _context.Activity
                  .Where(x => x.Type.ToUpper().Contains(search.ToUpper()))
                  .OrderByDescending(x => x.Timestamp)
                  .Skip((pageIndex - 1) * pageSize)
                  .Take(pageSize)
                  .ToList();

            // Return a paged list
            var activities = new PagedList<Activity>(results, pageIndex, pageSize, totalCount);

            // Convert
            var specificActivities = ConvertToSpecificActivities(activities, pageIndex, pageSize);

            return specificActivities;
        }

        public IEnumerable<ActivityBase> GetAll(int howMany)
        {
            var activities = _context.Activity.Take(howMany);
            var specificActivities = ConvertToSpecificActivities(activities);
            return specificActivities;
        }

        public void MemberJoined(MembershipUser user)
        {
            var memberJoinedActivity = MemberJoinedActivity.GenerateMappedRecord(user);
            Add(memberJoinedActivity);
        }

        public void ProfileUpdated(MembershipUser user)
        {
            var profileUpdatedActivity = ProfileUpdatedActivity.GenerateMappedRecord(user);
            Add(profileUpdatedActivity);
        }

        public void AdminProfileUpdated(MembershipUser user, MembershipUser admin)
        {
            var adminProfileUpdatedActivity = AdminProfileUpdatedActivity.GenerateMappedRecord(user, admin);
            Add(adminProfileUpdatedActivity);
        }


        public void ClassificationAdded(MembershipUser user, Classification classification)
        {
            var classificationAddedActivity = ClassificationAddedActivity.GenerateMappedRecord(user, classification);
            Add(classificationAddedActivity);
        }

        public void ClassificationUpdated(MembershipUser user, Classification classification)
        {
            var classificationUpdatedActivity = ClassificationUpdatedActivity.GenerateMappedRecord(user, classification);
            Add(classificationUpdatedActivity);
        }

        public void ClassificationRemoved(MembershipUser user, Classification classification)
        {
            var classificationRemovedActivity = ClassificationRemovedActivity.GenerateMappedRecord(user, classification);
            Add(classificationRemovedActivity);
        }

        public void MembershipLevelAdded(MembershipUser user, MembershipLevel membershipLevel)
        {
            var membershipLevelAddedActivity = MembershipLevelAddedActivity.GenerateMappedRecord(user, membershipLevel);
            Add(membershipLevelAddedActivity);
        }

        public void MembershipLevelUpdated(MembershipUser user, MembershipLevel membershipLevel)
        {
            var membershipLevelUpdatedActivity = MembershipLevelUpdatedActivity.GenerateMappedRecord(user, membershipLevel);
            Add(membershipLevelUpdatedActivity);
        }

        public void AdminBusinessAdded(Business business, MembershipUser admin)
        {
            var adminBusinessAddedActivity = AdminBusinessAddedActivity.GenerateMappedRecord(business, admin);
            Add(adminBusinessAddedActivity);
        }

        public void AdminBusinessUpdated(Business business, MembershipUser admin)
        {
            var adminBusinessUpdatedActivity = AdminBusinessUpdatedActivity.GenerateMappedRecord(business, admin);
            Add(adminBusinessUpdatedActivity);
        }

        public void AdminRegisterUser(MembershipUser user, MembershipUser admin)
        {
            var adminRegisterUserAddedActivity = AdminRegisterUserAddedActivity.GenerateMappedRecord(user, admin);
            Add(adminRegisterUserAddedActivity);
        }

        public void AdminBusinessContactAdded(BusinessContact businessContact, MembershipUser admin)
        {
            var adminBusinessContactAddedActivity = AdminBusinessContactAddedActivity.GenerateMappedRecord(businessContact, admin);
            Add(adminBusinessContactAddedActivity);
        }

        public void AdminBusinessBalanceAdded(BusinessBalance businessBalance, MembershipUser admin)
        {
            var adminBusinessBalanceAddedActivity = AdminBusinessBalanceAddedActivity.GenerateMappedRecord(businessBalance, admin);
            Add(adminBusinessBalanceAddedActivity);
        }

        public void Delete(IList<Activity> activities)
        {
            foreach (var activity in activities)
            {
                Delete(activity);
            }
        }

        public Activity Add(Activity newActivity)
        {
            return _context.Activity.Add(newActivity);
        }

        public Activity Get(Guid id)
        {
            return _context.Activity.FirstOrDefault(x => x.Id == id);
        }

        public void Delete(Activity item)
        {
            _context.Activity.Remove(item);
        }

        //Private functions for Activity Log *******************

        private ProfileUpdatedActivity GenerateProfileUpdatedActivity(Activity activity)
        {
            var dataPairs = ActivityBase.UnpackData(activity);

            if (!dataPairs.ContainsKey(ProfileUpdatedActivity.KeyUserId))
            {
                // Log the problem then skip
                _loggingService.Error(
                    $"A profile updated activity record with id '{activity.Id}' has no user id in its data.");
                return null;
            }

            var userId = dataPairs[ProfileUpdatedActivity.KeyUserId];
            var user = _context.MembershipUser.FirstOrDefault(x => x.Id == new Guid(userId));

            if (user == null)
            {
                // Log the problem then skip
                _loggingService.Error(
                    $"A profile updated activity record with id '{activity.Id}' has a user id '{userId}' that is not found in the user table.");
                return null;
            }

            return new ProfileUpdatedActivity(activity, user);
        }

        private AdminProfileUpdatedActivity GenerateAdminProfileUpdatedActivity(Activity activity)
        {
            var dataPairs = ActivityBase.UnpackData(activity);

            if (!dataPairs.ContainsKey(AdminProfileUpdatedActivity.KeyUserId))
            {
                // Log the problem then skip
                _loggingService.Error(
                    $"A profile updated activity record with id '{activity.Id}' has no user id in its data.");
                return null;
            }

            var userId = dataPairs[AdminProfileUpdatedActivity.KeyUserId];
            var adminUserId = dataPairs[AdminProfileUpdatedActivity.KeyAdminId];
            var user = _context.MembershipUser.FirstOrDefault(x => x.Id == new Guid(userId));
            var admin = _context.MembershipUser.FirstOrDefault(x => x.Id == new Guid(adminUserId));
            if (user == null)
            {
                // Log the problem then skip
                _loggingService.Error(
                    $"A profile updated activity record with id '{activity.Id}' has a user id '{userId}' that is not found in the user table.");
                return null;
            }

            return new AdminProfileUpdatedActivity(activity, user, admin);
        }

        /// <summary>
        /// Make a member joined activity object from the more generic database activity object
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        private MemberJoinedActivity GenerateMemberJoinedActivity(Activity activity)
        {
            var dataPairs = ActivityBase.UnpackData(activity);

            if (!dataPairs.ContainsKey(MemberJoinedActivity.KeyUserId))
            {
                // Log the problem then skip
                _loggingService.Error(
                    $"A member joined activity record with id '{activity.Id}' has no user id in its data.");
                return null;
            }

            var userId = dataPairs[MemberJoinedActivity.KeyUserId];
            var user = _context.MembershipUser.FirstOrDefault(x => x.Id == new Guid(userId));

            if (user == null)
            {
                // Log the problem then skip
                _loggingService.Error(
                    $"A member joined activity record with id '{activity.Id}' has a user id '{userId}' that is not found in the user table.");
                return null;
            }

            return new MemberJoinedActivity(activity, user);
        }

        /// <summary>
        /// Converts a paged list of generic activities into a paged list of more specific activity instances
        /// </summary>
        /// <param name="activities">Paged list of activities where each member may be a specific activity instance e.g. a profile updated activity</param>
        /// <param name="pageIndex"> </param>
        /// <param name="pageSize"> </param>
        /// <returns></returns>
        private PagedList<ActivityBase> ConvertToSpecificActivities(PagedList<Activity> activities, int pageIndex, int pageSize)
        {
            var listedResults = ConvertToSpecificActivities(activities);

            return new PagedList<ActivityBase>(listedResults, pageIndex, pageSize, activities.Count);
        }


        private IEnumerable<ActivityBase> ConvertToSpecificActivities(IEnumerable<Activity> activities)
        {
            var listedResults = new List<ActivityBase>();
            foreach (var activity in activities)
            {
                if (activity.Type == ActivityType.MemberJoined.ToString())
                {
                    var memberJoinedActivity = GenerateMemberJoinedActivity(activity);
                    if (memberJoinedActivity != null)
                    {
                        listedResults.Add(memberJoinedActivity);
                    }
                }
                else if (activity.Type == ActivityType.ProfileUpdated.ToString())
                {
                    var profileUpdatedActivity = GenerateProfileUpdatedActivity(activity);
                    if (profileUpdatedActivity != null)
                    {
                        listedResults.Add(profileUpdatedActivity);
                    }
                }
                else if (activity.Type == ActivityType.AdminProfileUpdated.ToString())
                {
                    var adminProfileUpdatedActivity = GenerateAdminProfileUpdatedActivity(activity);
                    if (adminProfileUpdatedActivity != null)
                    {
                        listedResults.Add(adminProfileUpdatedActivity);
                    }
                }
            }
            return listedResults;
        }
    }
}