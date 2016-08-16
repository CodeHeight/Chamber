using System;

namespace Chamber.Domain.DomainModel.Activity
{
    public class AdminBusinessUpdatedActivity : ActivityBase
    {
        public const string KeyBusinessId = @"BusinessID";
        public const string KeyAdminId = @"AdminID";

        public Business Business { get; set; }
        public MembershipUser Admin { get; set; }

        public AdminBusinessUpdatedActivity(Activity activity, Business business, MembershipUser admin)
        {
            ActivityMapped = activity;
            Business = business;
            Admin = admin;
        }

        public static Activity GenerateMappedRecord(Business business, MembershipUser admin)
        {
            return new Activity
            {
                Data = KeyBusinessId + Equality + business.Id + Separator +
                    KeyAdminId + Equality + admin.Id,
                Timestamp = DateTime.UtcNow,
                Type = ActivityType.AdminBusinessUpdated.ToString()
            };
        }
    }
}