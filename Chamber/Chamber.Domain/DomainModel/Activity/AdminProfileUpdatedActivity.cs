using System;

namespace Chamber.Domain.DomainModel.Activity
{
    public class AdminProfileUpdatedActivity : ActivityBase
    {
        public const string KeyUserId = @"UserID";
        public const string KeyAdminId = @"AdminId";

        public MembershipUser User { get; set; }
        public MembershipUser Admin { get; set; }

        public AdminProfileUpdatedActivity(Activity activity, MembershipUser user, MembershipUser admin)
        {
            ActivityMapped = activity;
            User = user;
            Admin = admin;
        }

        public static Activity GenerateMappedRecord(MembershipUser user, MembershipUser admin)
        {
            return new Activity
            {
                Data = KeyUserId + Equality + user.Id + Separator +
                    KeyAdminId + Equality + admin.Id,
                Timestamp = DateTime.Now,
                Type = ActivityType.AdminProfileUpdated.ToString()
            };
        }
    }
}