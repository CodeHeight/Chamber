using System;
using System.Collections.Generic;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.General;

namespace Chamber.Domain.Interfaces.Services
{
    public partial interface IClassificationService
    {
        Classification Add(Classification classification);
        void Delete(Classification classification);
        IList<Classification> GetAllClassifications();
        PagedList<Classification> GetAll(int pageIndex, int pageSize);
        PagedList<Classification> Search(string search, int pageIndex, int pageSize);
        Classification GetByName(string classificationName, bool tracking = false);
        Classification GetById(Guid id);
        ClassificationCreateStatus CreateClassification(MembershipUser user, Classification newClassification);
        Classification SanitizeClassification(Classification classification);
        string ErrorCodeToString(ClassificationCreateStatus createStatus);
        void ClassificationUpdated(MembershipUser user, Classification classification);
    }
}