using System.Collections.Generic;

namespace Chamber.Domain.DomainModel.General
{
    public partial class BusinessWithBusinessContacts
    {
        public Business Business { get; set; }
        public IEnumerable<BusinessContact> BusinessContacts { get; set; }
    }
}
