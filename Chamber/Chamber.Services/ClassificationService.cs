using System;
using System.Collections.Generic;
using System.Linq;
using Chamber.Domain.Events;
using Chamber.Domain.DomainModel;
using Chamber.Domain.DomainModel.General;
using Chamber.Domain.Interfaces;
using Chamber.Domain.Interfaces.Services;
using Chamber.Services.Data.Context;
using Chamber.Utilities;

namespace Chamber.Services
{
    public partial class ClassificationService : IClassificationService
    {
        private readonly ChamberContext _context;
        private readonly IActivityService _activityService;

        public ClassificationService(IChamberContext context, IActivityService activityService)
        {
            _context = context as ChamberContext;
            _activityService = activityService;
        }

        public Classification Add(Classification classification)
        {
            classification.CreateDate = DateTime.UtcNow;
            classification.Active = true;
            return _context.Classification.Add(classification);
        }

        public void Delete(Classification classification)
        {
            _context.Classification.Remove(classification);
        }

        public IList<Classification> GetAllClassifications()
        {
            return _context.Classification.ToList();
        }

        public PagedList<Classification> GetAll(int pageIndex, int pageSize)
        {
            var totalCount = _context.Classification.Count();
            var results = _context.Classification
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new PagedList<Classification>(results, pageIndex, pageSize, totalCount);
        }

        public PagedList<Classification> Search(string search, int pageIndex, int pageSize)
        {
            search = StringUtils.SafePlainText(search);
            var query = _context.Classification
                .Where(x => x.Name.ToUpper().Contains(search.ToUpper())
                || (x.Description.ToUpper().Contains(search.ToUpper())));

            var results = query
                .OrderBy(x => x.Name)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedList<Classification>(results, pageIndex, pageSize, query.Count());
        }

        public Classification GetByName(string classificationName, bool tracking = false)
        {
            classificationName = StringUtils.SafePlainText(classificationName);
            if (tracking == false)
            {
                return _context.Classification.AsNoTracking().FirstOrDefault(x => x.Name.ToUpper() == classificationName.ToUpper());
            }
            return _context.Classification.FirstOrDefault(x => x.Name.ToUpper() == classificationName.ToUpper());
        }

        public Classification GetById(Guid id)
        {
            return _context.Classification.FirstOrDefault(x => x.Id == id);
        }

        public ClassificationCreateStatus CreateClassification(MembershipUser user, Classification newClassification)
        {
            newClassification = SanitizeClassification(newClassification);
            var status = ClassificationCreateStatus.Success;

            var e = new NewClassificationEventArgs { User = user, Classification = newClassification };
            EventManager.Instance.FireBeforeNewClassification(this, e);
            if (e.Cancel)
            {
                status = e.CreateStatus;
            }
            else
            {
                if (string.IsNullOrEmpty(newClassification.Name))
                {
                    status = ClassificationCreateStatus.InvalidName;
                }
                if (GetByName(newClassification.Name, true) != null)
                {
                    status = ClassificationCreateStatus.DuplicateName;
                }
                if (status == ClassificationCreateStatus.Success)
                {
                    try
                    {
                        Add(newClassification);
                        _activityService.ClassificationAdded(user, newClassification);
                        EventManager.Instance.FireAfterNewClassification(this,
                            new NewClassificationEventArgs
                            {
                                User = user,
                                Classification = newClassification
                            });
                    }
                    catch (Exception)
                    {
                        status = ClassificationCreateStatus.NameRejected;
                        //log error;
                    }
                }
            }
            return status;
        }



        public Classification SanitizeClassification(Classification classification)
        {
            classification.Name = StringUtils.SafePlainText(classification.Name);
            classification.Description = StringUtils.SafePlainText(classification.Description);
            return classification;
        }

        public string ErrorCodeToString(ClassificationCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case ClassificationCreateStatus.DuplicateName:
                    return "Classification name is already in use.";
                case ClassificationCreateStatus.InvalidName:
                    return "Invalid classification name";
                case ClassificationCreateStatus.NameRejected:
                    return "Classification name was rejected.";
                default:
                    return "Classification Unknown error";
            }
        }

        public void ClassificationUpdated(MembershipUser user, Classification classification)
        {
            var e = new UpdateClassificationEventArgs { User = user };
            EventManager.Instance.FireBeforeUpdateClassification(this, e);

            if (!e.Cancel)
            {
                EventManager.Instance.FireAfterUpdateClassification(this, new UpdateClassificationEventArgs { User = user, Classification = classification });
                _activityService.ClassificationUpdated(user, classification);
            }
        }
    }
}