using System;

namespace Chamber.Domain.DomainModel.Activity
{
    public class ProfileUpdatedActivity : ActivityBase
    {
        public const string KeyUserId = @"UserID";

        public MembershipUser User { get; set; }

        /// <summary>
        /// Constructor - useful when constructing a badge activity after reading database
        /// </summary>
        public ProfileUpdatedActivity(Activity activity, MembershipUser user)
        {
            ActivityMapped = activity;
            User = user;
        }

        public static Activity GenerateMappedRecord(MembershipUser user)
        {
            return new Activity
            {
                Data = KeyUserId + Equality + user.Id,
                Timestamp = DateTime.UtcNow,
                Type = ActivityType.ProfileUpdated.ToString()
            };
        }
    }
}