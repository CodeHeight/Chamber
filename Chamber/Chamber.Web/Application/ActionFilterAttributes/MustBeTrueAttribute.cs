using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Chamber.Domain.Interfaces.Services;

namespace Chamber.Web.Application.ActionFilterAttributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class MustBeTrueAttribute : ValidationAttribute, IClientValidatable
    {
        //private readonly ILocalizationService _localizationService;

        public MustBeTrueAttribute()
        {
            //_localizationService = ServiceFactory.Get<ILocalizationService>();
        }
        public override bool IsValid(object value)
        {
            return value != null && value is bool && (bool)value;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {
            yield return new ModelClientValidationRule
            {
                //ErrorMessage = _localizationService.GetResourceString(this.ErrorMessage.Trim()),
                ErrorMessage = "Must be true",
                ValidationType = "mustbetrue"
            };
        }
    }
}