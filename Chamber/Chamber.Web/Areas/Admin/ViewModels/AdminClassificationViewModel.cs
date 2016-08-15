using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.General;

namespace Chamber.Web.Areas.Admin.ViewModels
{
    public class ClassificationViewModel
    {
        public Guid ClassificationId { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 4)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 4)]
        public string EditName { get; set; }

        [StringLength(1000)]
        public string EditDescription { get; set; }

        public string Search { get; set; }
        //public int? MostClassifications { get; set; }
        //public int? LeastClassifications { get; set; }
        public ClassificationListViewModel _listViewModel { get; set; }
    }

    public class ClassificationListViewModel
    {
        public PagedList<Classification> Classifications { get; set; }
        public int? PageIndex { get; set; }
        public int? TotalCount { get; set; }
        public int TotalPages { get; set; }
        public string Search { get; set; }
        public List<Classification> NonPagedClassifications { get; set; }
    }

    public class AllClassificationsViewModel
    {
        public List<Classification> allClassifications { get; set; }
    }

    //public class ClassificationAddViewModel
    //{
    //    [Required]
    //    [StringLength(200, MinimumLength = 4)]
    //    public string Name { get; set; }

    //    [Required]
    //    [StringLength(1000)]
    //    public string Description { get; set; }
    //}
}