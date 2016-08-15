using System;

namespace Chamber.Domain.DomainModel.Activity
{
    // Seal this class to avoid "virtual member call in constructor" problem

    public sealed class ClassificationRemovedActivity : ActivityBase
    {
        public const string KeyUserId = @"UserID";
        public const string KeyClassificationId = @"ClassificationID";

        public MembershipUser User { get; set; }
        public Classification Classification { get; set; }

        public ClassificationRemovedActivity(Activity activity, MembershipUser user, Classification classification)
        {
            ActivityMapped = activity;
            User = user;
            Classification = classification;
        }

        public static Activity GenerateMappedRecord(MembershipUser user, Classification classification)
        {
            return new Activity
            {
                Data = KeyUserId + Equality + user.Id + Separator +
                    KeyClassificationId + Equality + classification.Id,
                Timestamp = DateTime.Now,
                Type = ActivityType.ClassificationRemoved.ToString()
            };
        }
    }
}