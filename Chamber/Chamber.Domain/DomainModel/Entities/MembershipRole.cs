using System;
using System.Collections.Generic;
using Chamber.Utilities;

namespace Chamber.Domain.DomainModel
{
    public partial class MembershipRole : Entity
    {
        public MembershipRole()
        {
            Id = GuidComb.GenerateComb();
        }

        public Guid Id { get; set; }
        public string RoleName { get; set; }

        public virtual IList<MembershipUser> Users { get; set; }
        public virtual Settings Settings { get; set; }

    }
}
