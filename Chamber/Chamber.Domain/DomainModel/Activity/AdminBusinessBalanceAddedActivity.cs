namespace Chamber.Domain.DomainModel.Activity
{
    public class AdminBusinessBalanceAddedActivity : ActivityBase
    {
        public const string KeyBusinessBalanceId = @"BusinessBalanceID";
        public const string KeyAdminId = @"AdminID";

        public BusinessBalance BusinessBalance { get; set; }
        public MembershipUser Admin { get; set; }

        public AdminBusinessBalanceAddedActivity(Activity activity, BusinessBalance businessBalance, MembershipUser admin)
        {
            ActivityMapped = activity;
            BusinessBalance = businessBalance;
            Admin = admin;
        }

        public static Activity GenerateMappedRecord(BusinessBalance businessBalance, MembershipUser admin)
        {
            return new Activity
            {
                Data = KeyBusinessBalanceId + Equality + businessBalance.Id + Separator +
                    KeyAdminId + Equality + admin.Id,
                Timestamp = businessBalance.CreateDate,
                Type = ActivityType.AdminBusinessBalanceAdded.ToString()
            };
        }
    }
}