using Chamber.Domain.DomainModel.General;
using Chamber.Domain.DomainModel.Activity;

namespace Chamber.Web.Areas.Admin.ViewModels
{
    public class AllRecentActivitiesViewModel
    {
        public PagedList<ActivityBase> Activities { get; set; }

        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
    }
}