using System.Linq;
using Chamber.Domain.DomainModel;

namespace Chamber.Web.Areas.Admin.ViewModels.Mapping
{
    public static class ViewModelMapping
    {
        public static SingleMemberListViewModel UserToSingleMemberListViewModel(MembershipUser user)
        {
            var viewModel = new SingleMemberListViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                FullName = user.LastName + ", " + user.FirstName,
                City = user.City,
                State = user.State,
                Roles = user.Roles.Select(x => x.RoleName).ToArray()
            };
            return viewModel;
        }
    }
}