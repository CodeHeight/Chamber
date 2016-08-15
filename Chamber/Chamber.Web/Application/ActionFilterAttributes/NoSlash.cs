using System.Web.Mvc;

namespace Chamber.Web.Application.ActionFilterAttributes
{
    public class NoSlash : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (filterContext.HttpContext.Request.Url != null)
            {
                var originalUrl = filterContext.HttpContext.Request.Url.ToString();
                var newUrl = originalUrl.TrimEnd('/');
                if (originalUrl.Length != newUrl.Length)
                {
                    filterContext.HttpContext.Response.Redirect(newUrl);
                }
            }
        }
    }
}